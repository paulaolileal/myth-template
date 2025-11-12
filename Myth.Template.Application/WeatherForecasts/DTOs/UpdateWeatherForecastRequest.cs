using Myth.Interfaces;
using Myth.Morph;
using Myth.Template.Application.WeatherForecasts.Commands.Create;
using Myth.Template.Application.WeatherForecasts.Commands.Update;
using Myth.Template.Domain.Models;

namespace Myth.Template.Application.WeatherForecasts.DTOs;

/// <summary>
/// Data transfer object representing a request to update an existing weather forecast.
/// This record implements IMorphable to enable transformation to UpdateWeatherForecastCommand.
/// </summary>
public record UpdateWeatherForecastRequest : IMorphableTo<UpdateWeatherForecastCommand> {
	/// <summary>
	/// Gets or sets the unique identifier of the weather forecast to update.
	/// </summary>
	/// <value>The GUID identifier of the weather forecast to modify.</value>
	public Guid WeatherForecastId { get; set; }

	/// <summary>
	/// Gets or sets the temperature in Celsius.
	/// </summary>
	/// <value>The temperature measurement in Celsius degrees.</value>
	public int TemperatureC { get; set; }

	/// <summary>
	/// Gets or sets the weather summary for the forecast.
	/// </summary>
	/// <value>An enumeration value describing the weather conditions.</value>
	public Summary Summary { get; set; }

	/// <summary>
	/// Defines the morphing rules for transforming this DTO to an UpdateWeatherForecastCommand.
	/// This method is called during the transformation process to map properties between objects.
	/// </summary>
	/// <param name="schema">The schema builder used to define property mappings.</param>
	public void MorphTo( Schema<UpdateWeatherForecastCommand> schema ) { }
}
