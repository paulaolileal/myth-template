using Myth.Interfaces;
using Myth.Morph;
using Myth.Template.Application.WeatherForecasts.Commands.Create;
using Myth.Template.Domain.Models;

namespace Myth.Template.Application.WeatherForecasts.DTOs;

/// <summary>
/// Data transfer object representing a request to create a new weather forecast.
/// This class implements IMorphable to enable transformation to CreateWeatherForecastCommand.
/// </summary>
public record CreateWeatherForecastRequest : IMorphableTo<CreateWeatherForecastCommand> {
	/// <summary>
	/// Gets or sets the date for the weather forecast.
	/// </summary>
	/// <value>The date for which the weather forecast is being created.</value>
	public DateOnly Date { get; set; }

	/// <summary>
	/// Gets or sets the temperature in Celsius.
	/// </summary>
	/// <value>The temperature measurement in Celsius degrees.</value>
	public int TemperatureC { get; set; }

	/// <summary>
	/// Gets or sets the weather summary for the forecast.
	/// </summary>
	/// <value>An enumeration value describing the weather conditions.</value>
	public string Summary { get; set; } = null!;

	/// <summary>
	/// Defines the morphing rules for transforming this DTO to a CreateWeatherForecastCommand.
	/// This method is called during the transformation process to map properties between objects.
	/// </summary>
	/// <param name="schema">The schema builder used to define property mappings.</param>
	public void MorphTo( Schema<CreateWeatherForecastCommand> schema ) { }
}
