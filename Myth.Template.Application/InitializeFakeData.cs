using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Myth.Template.Data.Contexts;
using Myth.Template.Domain.Models;

namespace Myth.Template.Application;

/// <summary>
/// Hosted service responsible for initializing the application with mock weather forecast data.
/// Runs during application startup to populate the database with sample data for development and testing purposes.
/// Generates 1000 weather forecast entries with random data spanning historical dates.
/// </summary>
[ExcludeFromCodeCoverage]
public class InitializeFakeData : IHostedService {
	/// <summary>
	/// Factory for creating service scopes to access scoped dependencies like the database context.
	/// </summary>
	private readonly IServiceScopeFactory _serviceScopeFactory;

	/// <summary>
	/// Initializes a new instance of the InitializeMockedData class.
	/// </summary>
	/// <param name="serviceScopeFactory">Factory for creating service scopes to resolve scoped dependencies.</param>
	public InitializeFakeData( IServiceScopeFactory serviceScopeFactory ) {
		_serviceScopeFactory = serviceScopeFactory;
	}

	/// <summary>
	/// Starts the hosted service and initializes the database with mock weather forecast data.
	/// Creates 1000 weather forecast entries with randomized data including dates (going back in time),
	/// temperatures (ranging from -20°C to 55°C), and weather summaries.
	/// This method is automatically called when the application starts.
	/// </summary>
	/// <param name="cancellationToken">Token to monitor for cancellation requests during the operation.</param>
	/// <returns>A task that represents the asynchronous initialization operation.</returns>
	public async Task StartAsync( CancellationToken cancellationToken ) {
		var data = WeatherForecast.GenerateDataAsync( 1000, cancellationToken );

		using var scope = _serviceScopeFactory.CreateScope( );
		var context = scope.ServiceProvider.GetRequiredService<ForecastContext>( );

		await context
			.Set<WeatherForecast>( )
			.AddRangeAsync( data, cancellationToken );

		await context.SaveChangesAsync( cancellationToken );
	}

	/// <summary>
	/// Stops the hosted service. Currently performs no cleanup operations as the data initialization
	/// is a one-time operation that doesn't require ongoing maintenance.
	/// This method is automatically called when the application shuts down.
	/// </summary>
	/// <param name="cancellationToken">Token to monitor for cancellation requests during the operation.</param>
	/// <returns>A completed task since no cleanup operations are required.</returns>
	public Task StopAsync( CancellationToken cancellationToken ) => Task.CompletedTask;
}
