using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Myth.Constants;
using Myth.DependencyInjection;
using Myth.Exceptions;
using Myth.Extensions;
using Myth.Flow.Actions.Extensions;
using Myth.Template.Application.WeatherForecasts.Queries.GetAll;
using Myth.Template.Data.Contexts;
using Myth.Template.ExternalData.Breweries.Interfaces;
using Myth.Template.ExternalData.Breweries.Repositories;

namespace Myth.Template.API.Extensions;

/// <summary>
/// Utils extensions for configuring services in the web application builder.
/// </summary>
[ExcludeFromCodeCoverage]
public static class ServiceCollectionExtensions {
	/// <summary>
	/// Configures and adds Myth framework services to the web application builder.
	/// </summary>
	/// <param name="builder">The web application builder to configure.</param>
	/// <returns>The configured web application builder.</returns>
	public static WebApplicationBuilder AddMyth( this WebApplicationBuilder builder ) {
		builder.Services.AddScopedServiceProvider( );

		builder.Services.AddMorph( );

		builder.Services.AddGuard( config => config.AutoGuardCommonExceptions( ) );

		builder.Services.AddFlow( config => config
			.UseLogging( )
			.UseExceptionFilter<ValidationException>( )
			.UseTelemetry( )
			.UseRetry( 3 )
			.UseActions( x => x
				.UseInMemory( )
				.ScanAssemblies(
					typeof( GetAllWeatherForecastsQueryHandler ).Assembly ) ) );

		builder.Services.AddRestFactory( )
			.AddRestConfiguration( "brewery", conf => conf
				.WithBaseUrl( builder.Configuration
					.GetRequiredSection( "OpenBreweryDbAPI" )
					.GetValue<string>( "BaseUrl" )! )
				.WithBodyDeserialization( CaseStrategy.SnakeCase ) );

		return builder;
	}

	/// <summary>
	/// Configures API documentation services including Swagger and XML comments.
	/// </summary>
	/// <param name="builder">The web application builder to configure.</param>
	/// <returns>The configured web application builder.</returns>
	public static WebApplicationBuilder AddDocs( this WebApplicationBuilder builder ) {
		builder.Services.AddEndpointsApiExplorer( );

		builder.Services.AddSwaggerGen( c => {
			var xmlFile = $"{Assembly.GetExecutingAssembly( ).GetName( ).Name}.xml";
			var xmlPath = Path.Combine( AppContext.BaseDirectory, xmlFile );
			c.IncludeXmlComments( xmlPath );
		} );

		return builder;
	}

	/// <summary>
	/// Configures database services including Entity Framework context, repositories, and unit of work.
	/// </summary>
	/// <param name="builder">The web application builder to configure.</param>
	/// <returns>The configured web application builder.</returns>
	public static WebApplicationBuilder AddDatabase( this WebApplicationBuilder builder ) {
		builder.Services.AddDbContext<ForecastContext>( options => options
			.UseInMemoryDatabase( "database" ), ServiceLifetime.Scoped, ServiceLifetime.Singleton );

		builder.Services.AddRepositories( );

		builder.Services.AddScoped<IBreweryRepository, BreweryRepository>( );

		builder.Services.AddUnitOfWorkForContext<ForecastContext>( );

		return builder;
	}
}
