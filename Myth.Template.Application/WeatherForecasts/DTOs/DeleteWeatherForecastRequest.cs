using Myth.Interfaces;
using Myth.Morph;
using Myth.Template.Application.WeatherForecasts.Commands.Delete;

namespace Myth.Template.Application.WeatherForecasts.DTOs;

/// <summary>
/// Data transfer object representing a request to delete an existing weather forecast.
/// This record implements IMorphable to enable transformation to DeleteWeatherForecastCommand.
/// </summary>
public record DeleteWeatherForecastRequest : IMorphableTo<DeleteWeatherForecastCommand> {
	/// <summary>
	/// Gets or sets the unique identifier of the weather forecast to delete.
	/// </summary>
	/// <value>The GUID identifier of the weather forecast to remove from the system.</value>
	public Guid WeatherForecastId { get; set; }

	/// <summary>
	/// Defines the morphing rules for transforming this DTO to a DeleteWeatherForecastCommand.
	/// This method is called during the transformation process to map properties between objects.
	/// </summary>
	/// <param name="schema">The schema builder used to define property mappings.</param>
	public void MorphTo( Schema<DeleteWeatherForecastCommand> schema ) { }
}
