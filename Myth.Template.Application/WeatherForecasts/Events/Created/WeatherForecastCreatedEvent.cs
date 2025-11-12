using Myth.Models;

namespace Myth.Template.Application.WeatherForecasts.Events.Created;

/// <summary>
/// Domain event that is raised when a new weather forecast has been successfully created.
/// This event can be used to trigger additional business processes or notifications
/// in response to the creation of a weather forecast.
/// </summary>
public record WeatherForecastCreatedEvent : DomainEvent {
	/// <summary>
	/// Gets the unique identifier of the newly created weather forecast.
	/// </summary>
	/// <value>The GUID identifier of the created weather forecast.</value>
	public Guid WeatherForecastId { get; init; }

	/// <summary>
	/// Initializes a new instance of the WeatherForecastCreatedEvent record with the specified weather forecast identifier.
	/// </summary>
	/// <param name="weatherForecastId">The unique identifier of the created weather forecast.</param>
	public WeatherForecastCreatedEvent( Guid weatherForecastId ) {
		WeatherForecastId = weatherForecastId;
	}
}
