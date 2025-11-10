using Microsoft.EntityFrameworkCore;
using Myth.Exceptions;
using Myth.Extensions;
using Myth.Flow.Actions.Extensions;
using Myth.Template.Application;
using Myth.Template.Application.WeatherForecasts.Queries.GetAll;
using Myth.Template.Data.Contexts;
using System.Diagnostics.CodeAnalysis;

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

		builder.Services.AddGuard( );

		builder.Services.AddFlow( config => config
			.UseLogging( )
			.UseExceptionFilter<ValidationException>( )
			.UseTelemetry( )
			.UseRetry( 3 )
			.UseActions( x => x
				.UseInMemory( )
				.ScanAssemblies(
					typeof( GetAllWeatherForecastsQueryHandler ).Assembly ) ) );

		builder.Services.AddControllers( );

		builder.Services.AddEndpointsApiExplorer( );

		builder.Services.AddSwaggerGen( );

		builder.Services.AddHealthChecks( );

		builder.Services.AddHostedService<InitializeMockedData>( );

		var app = builder.BuildApp( );

		// Configure the HTTP request pipeline.
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