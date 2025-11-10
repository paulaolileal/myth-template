using Myth.Interfaces;
using Myth.Morph;
using Myth.Template.Domain.Models;

namespace Myth.Template.Application.WeatherForecasts.DTOs {

	/// <summary>
	/// Data transfer object representing the response when retrieving a weather forecast.
	/// Contains all relevant information about a weather forecast including temperature conversions and summary details.
	/// </summary>
	public record GetWeatherForecastResponse : IMorphableFrom<WeatherForecast> {
		/// <summary>
		/// Gets or sets the unique identifier for the weather forecast.
		/// </summary>
		/// <value>A GUID that uniquely identifies the weather forecast record.</value>
		public Guid WeatherForecastId { get; set; }

		/// <summary>
		/// Gets or sets the date for which the weather forecast applies.
		/// </summary>
		/// <value>The date of the weather forecast.</value>
		public DateOnly Date { get; set; }

		/// <summary>
		/// Gets or sets the temperature in Celsius.
		/// </summary>
		/// <value>The temperature measurement in Celsius degrees.</value>
		public int TemperatureC { get; set; }

		/// <summary>
		/// Gets or sets the temperature in Fahrenheit.
		/// </summary>
		/// <value>The temperature measurement in Fahrenheit degrees.</value>
		public int TemperatureF { get; set; }

		/// <summary>
		/// Gets or sets the numeric identifier for the weather summary.
		/// </summary>
		/// <value>The integer representation of the Summary enumeration value.</value>
		public int SummaryId { get; set; }

		/// <summary>
		/// Gets or sets the descriptive text for the weather summary.
		/// </summary>
		/// <value>The string representation of the weather conditions.</value>
		public string SummaryDescription { get; set; } = null!;

		/// <summary>
		/// Defines the morphing rules for transforming this domain model to a GetWeatherForecastResponse.
		/// Maps the Summary enumeration to both its string name and integer value in the response.
		/// </summary>
		/// <param name="schema">The schema builder used to define property mappings for the transformation.</param>
		public void MorphFrom( Schema<WeatherForecast> schema ) {
			schema.Bind( ( ) => SummaryDescription, src => Enum.GetName( src.Summary ) );
			schema.Bind( ( ) => SummaryId, src => ( int )src.Summary );
		}
	}
}