using Myth.Interfaces;
using Myth.Morph;
using Myth.Template.Application.WeatherStations.Commands.Update;

namespace Myth.Template.Application.WeatherStations.DTOs;

/// <summary>
/// Request DTO for updating an existing weather station's name and location.
/// </summary>
public record UpdateWeatherStationRequest : IMorphableTo<UpdateWeatherStationCommand> {

	/// <summary>Gets or sets the new display name for the station.</summary>
	public string Name { get; set; } = null!;

	/// <summary>Gets or sets the new geographic location for the station.</summary>
	public string Location { get; set; } = null!;

	/// <summary>Auto-mapping via property name convention — no custom bindings needed.</summary>
	public void MorphTo( Schema<UpdateWeatherStationCommand> schema ) { }
}
