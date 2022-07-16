using System.Text.Json;
using Kadmium_Dmx_Shared.Models;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
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

			var dictionaryPack = new ConventionPack { new DictionaryRepresentationConvention() };
			ConventionRegistry.Register("dictionaryRep", dictionaryPack, t => t == typeof(FixtureDefinition));

			builder.Services.AddControllers().AddJsonOptions((options) => options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase);
			// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
			builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddSwaggerGen();

			builder.Services.AddSingleton<ICrudProvider<VenueKey, Venue>, VenueProvider>();
			builder.Services.AddSingleton<ICrudProvider<FixtureDefinitionKey, FixtureDefinition>, FixtureDefinitionProvider>();

			IKadmiumDmxConfigurationProvider configProvider = new KadmiumDmxEnvironmentVariableConfigurationProvider();
			builder.Services.AddSingleton<IKadmiumDmxConfigurationProvider>(configProvider);
			var client = new MongoClient(configProvider.MongoDbConnectionString);
			var db = client.GetDatabase("kadmium-dmx-2");
			builder.Services.AddSingleton<IMongoDatabase>(db);

			var app = builder.Build();

			// Configure the HTTP request pipeline.
			if (app.Environment.IsDevelopment())
			{
				app.UseSwagger();
				app.UseSwaggerUI();
			}

			app.UseHttpsRedirection();

			app.UseAuthorization();

			app.MapControllers();

			await app.RunAsync();
		}
	}
}