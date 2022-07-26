using System.Text.Json;
using Kadmium_Dmx_Shared.Models;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Options;
using MongoDB.Driver;
using MQTTnet.Client;
using MQTTnet.Extensions.ManagedClient;
using MQTTnet.Formatter;
using Webapi.Models;
using Webapi.Services;

namespace Kadmium_Dmx_Shared
{
	public class Program
	{
		public static async Task Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			// Add services to the container.

			var camelCasePack = new ConventionPack { new CamelCaseElementNameConvention() };
			ConventionRegistry.Register("camelCase", camelCasePack, t => true);

			var dictionaryPack = new ConventionPack { new DictionaryRepresentationConvention(DictionaryRepresentation.ArrayOfArrays) };
			ConventionRegistry.Register("dictionaryRep", dictionaryPack, t => true);

			builder.Services.AddControllers().AddJsonOptions((options) => options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase);
			// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
			builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddSwaggerGen();

			builder.Services.AddSingleton<IVenueProvider, VenueProvider>();
			builder.Services.AddSingleton<IFixtureDefinitionProvider, FixtureDefinitionProvider>();
			builder.Services.AddSingleton<IMidiMapProvider, MidiMapProvider>();
			builder.Services.AddSingleton<IDefaultsProvider, MongoDefaultsProvider>();
			builder.Services.AddSingleton<IMqttConnectionProvider, MqttConnectionProvider>();

			builder.Services.AddSingleton<ManagedMqttClientOptions>((serviceProvider) =>
					{
						return new ManagedMqttClientOptionsBuilder()
						.WithAutoReconnectDelay(TimeSpan.FromSeconds(1))
						.WithClientOptions(new MqttClientOptionsBuilder()
							.WithTcpServer("mqtt")
							.WithProtocolVersion(MqttProtocolVersion.V500)
							.Build()
						)
						.Build();
					});

			IKadmiumDmxConfigurationProvider configProvider = new KadmiumDmxEnvironmentVariableConfigurationProvider();
			builder.Services.AddSingleton<IKadmiumDmxConfigurationProvider>(configProvider);
			var client = new MongoClient(configProvider.MongoDbConnectionString);
			var db = client.GetDatabase("kadmium-dmx-2");
			builder.Services.AddSingleton<IMongoDatabase>(db);

			builder.Services.AddSpaStaticFiles(configuration =>
			{
				configuration.RootPath = "htdocs";
			});

			var app = builder.Build();

			app.UseSwagger();
			app.UseSwaggerUI();

			//app.UseHttpsRedirection();

			app.UseCors(policy =>
			{
				policy.AllowAnyHeader();
				policy.AllowAnyMethod();
				policy.AllowAnyOrigin();
			});

			app.UseAuthorization();
			app.MapControllers();
			app.UseSpaStaticFiles(new StaticFileOptions { });
			app.UseSpa(spa =>
			{
				spa.Options.SourcePath = "htdocs";
			});

			var defaultsProvider = app.Services.GetRequiredService<IDefaultsProvider>();
			var defaults = await defaultsProvider.GetDefaults();
			var mqtt = app.Services.GetRequiredService<IMqttConnectionProvider>();
			await mqtt.Begin();

			if (defaults.VenueId != null)
			{
				var venueProvider = app.Services.GetRequiredService<IVenueProvider>();
				await venueProvider.Activate(defaults.VenueId);
			}
			if (defaults.MidiMapId != null)
			{
				var midiMapProvider = app.Services.GetRequiredService<IMidiMapProvider>();
				await midiMapProvider.Activate(defaults.MidiMapId);
			}

			await app.RunAsync();
		}

	}
}