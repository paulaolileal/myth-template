using Myth.Interfaces;
using Myth.Morph;

namespace Myth.Template.Domain.Models {

	/// <summary>
	/// Domain model representing a weather forecast entity.
	/// Contains weather information for a specific date including temperature and summary conditions.
	/// </summary>
	public class WeatherForecast {
		/// <summary>
		/// Gets the unique identifier for the weather forecast.
		/// This value is automatically generated during construction.
		/// </summary>
		/// <value>A GUID that uniquely identifies this weather forecast instance.</value>
		public Guid WeatherForecastId { get; private set; }

		/// <summary>
		/// Gets the date for which this weather forecast applies.
		/// </summary>
		/// <value>The date of the weather forecast.</value>
		public DateOnly Date { get; private set; }

		/// <summary>
		/// Gets the temperature in Celsius for this forecast.
		/// </summary>
		/// <value>The temperature measurement in Celsius degrees.</value>
		public int TemperatureC { get; private set; }

		/// <summary>
		/// Gets the calculated temperature in Fahrenheit.
		/// This property automatically converts the Celsius temperature to Fahrenheit.
		/// </summary>
		/// <value>The temperature measurement in Fahrenheit degrees, calculated from TemperatureC.</value>
		public int TemperatureF => 32 + ( int )( TemperatureC / 0.5556 );

		/// <summary>
		/// Gets the weather summary describing the conditions for this forecast.
		/// </summary>
		/// <value>An enumeration value representing the weather conditions.</value>
		public Summary Summary { get; private set; }

		/// <summary>
		/// Initializes a new instance of the WeatherForecast class with the specified parameters.
		/// Automatically generates a new unique identifier for the instance.
		/// </summary>
		/// <param name="date">The date for which the forecast applies.</param>
		/// <param name="temperatureC">The temperature in Celsius.</param>
		/// <param name="summary">The weather summary describing the conditions.</param>
		public WeatherForecast( DateOnly date, int temperatureC, Summary summary ) {
			WeatherForecastId = Guid.NewGuid( );
			Date = date;
			TemperatureC = temperatureC;
			Summary = summary;
		}


	}
}