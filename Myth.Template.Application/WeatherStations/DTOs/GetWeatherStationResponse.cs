using Myth.Interfaces;
using Myth.Morph;
using Myth.Template.Application.WeatherForecasts.DTOs;
using Myth.Template.Domain.Models;

namespace Myth.Template.Application.WeatherStations.DTOs;

/// <summary>
/// Data transfer object for weather station responses.
/// Implements <see cref="IMorphableFrom{WeatherStation}"/> so that <c>.To&lt;GetWeatherStationResponse&gt;()</c>
/// can be called directly on a <see cref="WeatherStation"/> instance.
/// All matching properties are auto-mapped by name; the optional <see cref="Forecasts"/> collection
/// is populated separately when <c>withForecasts=true</c> is requested.
/// </summary>
public record GetWeatherStationResponse : IMorphableFrom<WeatherStation> {

	/// <summary>Gets the unique identifier of the weather station.</summary>
	public Guid WeatherStationId { get; set; }

	/// <summary>Gets the display name of the station.</summary>
	public string Name { get; set; } = null!;

	/// <summary>Gets the geographic location of the station.</summary>
	public string Location { get; set; } = null!;

	/// <summary>Gets whether the station is currently active.</summary>
	public bool IsActive { get; set; }

	/// <summary>Gets the UTC creation timestamp.</summary>
	public DateTime CreatedAt { get; set; }

	/// <summary>Gets the UTC last-update timestamp, or null if never updated.</summary>
	public DateTime? UpdatedAt { get; set; }

	/// <summary>
	/// Gets the forecasts recorded by this station.
	/// Populated only when explicitly loaded by the query — otherwise <c>null</c>.
	/// </summary>
	public IReadOnlyCollection<GetWeatherForecastResponse>? Forecasts { get; set; }

	/// <summary>
	/// All standard properties are auto-mapped by name matching.
	/// <see cref="Forecasts"/> is intentionally left as <c>null</c> here since it requires
	/// a separate load step based on request parameters.
	/// </summary>
	public void MorphFrom( Schema<WeatherStation> schema ) { }
}
