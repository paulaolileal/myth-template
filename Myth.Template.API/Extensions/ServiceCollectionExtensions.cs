using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Myth.Constants;
using Myth.DependencyInjection;
using Myth.Exceptions;
using Myth.Extensions;
using Myth.Flow.Actions.Extensions;
using Myth.Flow.Actions.Settings;
using Myth.Template.Application.WeatherForecasts.Queries.GetAll;
using Myth.Template.Data.Contexts;
using Myth.Template.Domain.Interfaces;
using Myth.Template.ExternalData.Breweries.Exceptions;
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

		builder.Services.AddGuard( config => config
			.AutoGuardCommonExceptions( )
			.Guard<BreweryException>( )
				.WithStatusCode( StatusCodes.Status424FailedDependency )
				.WithResponse( ex => ex.Message ) );

		builder.Services.AddFlow( config => config
			.UseLogging( )
			.UseExceptionFilter<ValidationException>( )
			.UseTelemetry( )
			.UseRetry( 3 )
			.UseActions( actionsSettings => actionsSettings
				.UseInMemory( )
				.UseCaching( cacheSettings => cacheSettings.ProviderType = CacheProviderType.Memory )
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

		builder.Services.AddSwaggerGen( options => {
			// Include XML comments from code documentation
			var xmlFile = $"{Assembly.GetExecutingAssembly( ).GetName( ).Name}.xml";
			var xmlPath = Path.Combine( AppContext.BaseDirectory, xmlFile );
			options.IncludeXmlComments( xmlPath );
		} );

		return builder;
	}

	/// <summary>
	/// Configures database services including Entity Framework context, repositories, and unit of work.
	/// <para>
	/// Demonstrates <c>AddServiceFromType&lt;T&gt;()</c> from <c>Myth.DependencyInjection</c>:
	/// instead of <c>AddRepositories()</c> (which auto-discovers all repos by convention),
	/// each repository is registered explicitly with <c>AddServiceFromType</c>.
	/// Both approaches use <see cref="Myth.ValueProviders.TypeProvider"/> internally to resolve
	/// implementations by naming convention (e.g., <c>IWeatherForecastRepository</c> →
	/// <c>WeatherForecastRepository</c>).
	/// </para>
	/// </summary>
	/// <param name="builder">The web application builder to configure.</param>
	/// <returns>The configured web application builder.</returns>
	public static WebApplicationBuilder AddDatabase( this WebApplicationBuilder builder ) {
		builder.Services.AddDbContext<ForecastContext>( options => options
			.UseInMemoryDatabase( "database" ), ServiceLifetime.Scoped, ServiceLifetime.Singleton );

		// Myth.DependencyInjection: AddServiceFromType<T> registers all implementations
		// of T found by TypeProvider scanning application assemblies.
		// Convention: interface name must contain the concrete type's name
		// (e.g., IWeatherForecastRepository → WeatherForecastRepository).
		builder.Services.AddServiceFromType<IWeatherForecastRepository>( );
		builder.Services.AddServiceFromType<IWeatherStationRepository>( );

		builder.Services.AddScoped<IBreweryRepository, BreweryRepository>( );

		builder.Services.AddUnitOfWorkForContext<ForecastContext>( );

		return builder;
	}
}
