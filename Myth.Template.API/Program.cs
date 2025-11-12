using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using Myth.Constants;
using Myth.DependencyInjection;
using Myth.Exceptions;
using Myth.Extensions;
using Myth.Flow.Actions.Extensions;
using Myth.Template.Application;
using Myth.Template.Application.WeatherForecasts.Queries.GetAll;
using Myth.Template.Data.Contexts;
using Myth.Template.ExternalData.Breweries.Interfaces;
using Myth.Template.ExternalData.Breweries.Repositories;

namespace Myth.Template.API;

[ExcludeFromCodeCoverage]
internal class Program {

	private static void Main( string[ ] args ) {
		var builder = WebApplication.CreateBuilder( args );

		// Add services to the container
		builder.Services.AddDbContext<ForecastContext>( options => options
			.UseInMemoryDatabase( "database" ), ServiceLifetime.Scoped, ServiceLifetime.Singleton );

		builder.Services.AddRepositories( );

		builder.Services.AddUnitOfWorkForContext<ForecastContext>( );

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

		builder.Services.AddScoped<IBreweryRepository, BreweryRepository>( );

		builder.Services.AddControllers( );

		builder.Services.AddEndpointsApiExplorer( );

		builder.Services.AddSwaggerGen( );

		builder.Services.AddHealthChecks( );

		builder.Services.AddHostedService<InitializeFakeData>( );

		var app = builder.BuildApp( );

		if ( app.Environment.IsDevelopment( ) ) {
			app.UseSwagger( );
			app.UseSwaggerUI( );
		}

		app.UseGuard( );

		app.UseHttpsRedirection( );

		app.UseAuthorization( );

		app.MapControllers( );

		app.Run( );
	}
}
