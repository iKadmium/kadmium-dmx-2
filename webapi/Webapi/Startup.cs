using GraphQL;
using GraphQL.MicrosoftDI;
using GraphQL.Server;
using GraphQL.SystemTextJson;
using GraphQL.Types;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Webapi.Schema;

namespace Webapi
{
	public class Startup
	{
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddGraphQL(b => b
				.AddHttpMiddleware<ISchema>()
				.AddUserContextBuilder(httpContext => new GraphQLUserContext(httpContext.User))
				.AddSystemTextJson()
				.AddErrorInfoProvider(opt => opt.ExposeExceptionStackTrace = true)
				.AddSchema<DmxSchema>()
				.AddGraphTypes(typeof(DmxSchema).Assembly));

			//services.AddSingleton<StarWarsData>();
			services.AddLogging(builder => builder.AddConsole());
			services.AddHttpContextAccessor();
		}

		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
				app.UseDeveloperExceptionPage();

			app.UseGraphQL<ISchema>();
			app.UseGraphQLAltair();
			app.UseHttpsRedirection();
		}
	}
}