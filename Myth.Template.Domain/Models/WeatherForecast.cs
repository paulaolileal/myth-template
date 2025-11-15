namespace Myth.Template.Domain.Models;

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
	/// Gets the UTC date and time when this weather forecast was created.
	/// </summary>
	/// <value>The creation timestamp in UTC.</value>
	public DateTime CreatedAt { get; private set; }

	/// <summary>
	/// Gets the UTC date and time when this weather forecast was last updated.
	/// Returns null if the forecast has never been updated since creation.
	/// </summary>
	/// <value>The last update timestamp in UTC, or null if never updated.</value>
	public DateTime? UpdatedAt { get; private set; }

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
		CreatedAt = DateTime.UtcNow;
	}

	/// <summary>
	/// Updates the UpdatedAt property to the current UTC time.
	/// This method is called internally whenever the entity is modified.
	/// </summary>
	private void Update( ) => UpdatedAt = DateTime.UtcNow;

	/// <summary>
	/// Changes the temperature in Celsius for this weather forecast.
	/// This method updates the temperature and automatically sets the UpdatedAt timestamp.
	/// </summary>
	/// <param name="temperatureC">The new temperature value in Celsius.</param>
	/// <returns>The current WeatherForecast instance for method chaining.</returns>
	public WeatherForecast ChangeTemperatureC( int temperatureC ) {
		TemperatureC = temperatureC;
		Update( );

		return this;
	}

	/// <summary>
	/// Changes the weather summary for this weather forecast.
	/// This method updates the summary and automatically sets the UpdatedAt timestamp.
	/// </summary>
	/// <param name="summary">The new weather summary enumeration value.</param>
	/// <returns>The current WeatherForecast instance for method chaining.</returns>
	public WeatherForecast ChangeSummary( Summary summary ) {
		Summary = summary;
		Update( );

		return this;
	}

	public static IEnumerable<WeatherForecast> GenerateDataAsync( int amount, CancellationToken cancellationToken ) {
		var random = new Random( );

		var weatherForecasts = Enumerable.Range( 0, 1000 ).Select( i => {
			var date = DateOnly.FromDateTime( DateTime.Now ).AddDays( ( i + 1 ) * -1 );
			var temperatureC = random.Next( -20, 55 );
			var summaries = Summary.All.ToArray( );
			var summary = summaries[ random.Next( summaries.Length ) ];
			return new WeatherForecast( date, temperatureC, summary );
		} );

		return weatherForecasts;
	}
}
