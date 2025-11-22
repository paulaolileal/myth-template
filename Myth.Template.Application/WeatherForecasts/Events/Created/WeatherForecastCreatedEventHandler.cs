using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Myth.Interfaces;
using Myth.Template.ExternalData.Breweries.Interfaces;

namespace Myth.Template.Application.WeatherForecasts.Events.Created;

/// <summary>
/// Event handler responsible for processing WeatherForecastCreatedEvent occurrences.
/// Logs information about newly created weather forecasts for auditing and monitoring purposes.
/// This handler demonstrates how domain events can be used to trigger cross-cutting concerns.
/// </summary>
/// <remarks>
/// Initializes a new instance of the WeatherForecastCreatedEventHandler class.
/// </remarks>
/// <param name="logger">Logger instance for recording event processing information.</param>
public class WeatherForecastCreatedEventHandler( ILogger<WeatherForecastCreatedEventHandler> logger, IServiceScopeFactory scopeFactory ) : IEventHandler<WeatherForecastCreatedEvent> {


	/// <summary>
	/// Handles the WeatherForecastCreatedEvent by logging information about the newly created weather forecast.
	/// This method is automatically invoked when a WeatherForecastCreatedEvent is published in the system.
	/// </summary>
	/// <param name="event">The domain event containing details about the created weather forecast.</param>
	/// <param name="cancellationToken">Token to monitor for cancellation requests during the operation.</param>
	/// <returns>A task that represents the asynchronous operation completion.</returns>
	public async Task HandleAsync( WeatherForecastCreatedEvent @event, CancellationToken cancellationToken = default ) {
		logger.LogInformation( "Weather forecast created with ID `{WeatherForecastId}`", @event.WeatherForecastId );

		var provider = scopeFactory.CreateScope( ).ServiceProvider;
		var breweryRepository = provider.GetRequiredService<IBreweryRepository>( );
		var brewery = await breweryRepository.GetRandomBreweryAsync( cancellationToken );

		logger.LogInformation( "And {BreweryName} has a good beer for this weather!", brewery.Name );

	}
}
