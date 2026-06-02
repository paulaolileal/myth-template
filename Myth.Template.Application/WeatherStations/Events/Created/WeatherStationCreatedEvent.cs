using Myth.Models;

namespace Myth.Template.Application.WeatherStations.Events.Created;

/// <summary>
/// Domain event raised when a new weather station has been successfully created.
/// </summary>
public record WeatherStationCreatedEvent : DomainEvent {

	/// <summary>Gets the unique identifier of the newly created station.</summary>
	public Guid WeatherStationId { get; init; }

	public WeatherStationCreatedEvent( Guid weatherStationId ) {
		WeatherStationId = weatherStationId;
	}
}
