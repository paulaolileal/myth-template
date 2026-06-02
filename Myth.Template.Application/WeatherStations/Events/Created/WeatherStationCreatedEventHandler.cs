using Microsoft.Extensions.Logging;
using Myth.Interfaces;
using Myth.Template.ExternalData.Breweries.Interfaces;

namespace Myth.Template.Application.WeatherStations.Events.Created;

/// <summary>
/// Event handler for <see cref="WeatherStationCreatedEvent"/>.
/// Logs the creation and calls the external Brewery API as a side effect,
/// demonstrating event-driven integration with third-party services.
/// </summary>
/// <param name="logger">Logger for recording event processing.</param>
/// <param name="breweryRepository">Repository for the external brewery integration.</param>
public class WeatherStationCreatedEventHandler(
	ILogger<WeatherStationCreatedEventHandler> logger,
	IBreweryRepository breweryRepository ) : IEventHandler<WeatherStationCreatedEvent> {

	public async Task HandleAsync( WeatherStationCreatedEvent @event, CancellationToken cancellationToken = default ) {
		logger.LogInformation( "Weather station created with ID `{WeatherStationId}`", @event.WeatherStationId );

		var brewery = await breweryRepository.GetRandomBreweryAsync( cancellationToken );

		logger.LogInformation( "Station {WeatherStationId} is ready — {BreweryName} has a cold one waiting!", @event.WeatherStationId, brewery.Name );
	}
}
