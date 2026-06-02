using Myth.Interfaces;
using Myth.Template.Domain.Models;

namespace Myth.Template.Domain.Specifications;

/// <summary>
/// Extension methods for building specifications to filter WeatherStation entities.
/// Provides fluent API methods for composing complex query specifications.
/// </summary>
public static class WeatherStationSpecification {

	/// <summary>Filters stations by their unique identifier.</summary>
	public static ISpec<WeatherStation> WithId( this ISpec<WeatherStation> spec, Guid id ) =>
		spec.And( x => x.WeatherStationId == id );

	/// <summary>
	/// Ensures no station with the given name exists.
	/// Used for uniqueness validation on creation.
	/// </summary>
	public static ISpec<WeatherStation> WithNameNotInUse( this ISpec<WeatherStation> spec, string name ) =>
		spec.And( x => x.Name == name );

	/// <summary>Filters to only return active stations.</summary>
	public static ISpec<WeatherStation> IsActiveOnly( this ISpec<WeatherStation> spec ) =>
		spec.And( x => x.IsActive );

	/// <summary>
	/// Filters stations whose name contains the given substring.
	/// Only applies the filter when the name parameter is not null or empty.
	/// </summary>
	public static ISpec<WeatherStation> WithNameContains( this ISpec<WeatherStation> spec, string? name ) =>
		spec.AndIf( !string.IsNullOrEmpty( name ), x => x.Name.Contains( name! ) );
}
