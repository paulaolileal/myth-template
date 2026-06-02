using Myth.Interfaces;
using Myth.Morph;
using Myth.Template.Application.WeatherStations.Commands.Create;

namespace Myth.Template.Application.WeatherStations.DTOs;

/// <summary>
/// Request DTO for creating a new weather station.
/// Uses <see cref="IMorphableTo{T}"/> to transform into the corresponding command.
/// </summary>
public record CreateWeatherStationRequest : IMorphableTo<CreateWeatherStationCommand> {

	/// <summary>Gets or sets the display name of the station.</summary>
	public string Name { get; set; } = null!;

	/// <summary>Gets or sets the geographic location of the station.</summary>
	public string Location { get; set; } = null!;

	/// <summary>Auto-mapping via property name convention — no custom bindings needed.</summary>
	public void MorphTo( Schema<CreateWeatherStationCommand> schema ) { }
}
