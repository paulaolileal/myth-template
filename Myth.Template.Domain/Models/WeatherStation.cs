using System.Net;
using Microsoft.Extensions.DependencyInjection;
using Myth.Builder;
using Myth.Guard;
using Myth.Interfaces;
using Myth.ServiceProvider;
using Myth.Specifications;
using Myth.Template.Domain.Interfaces;
using Myth.Template.Domain.Specifications;

namespace Myth.Template.Domain.Models;

/// <summary>
/// Represents a weather monitoring station that records meteorological data.
/// Serves as the aggregate root for weather station management, demonstrating
/// domain entity design with multiple validation contexts and lifecycle methods.
/// </summary>
/// <remarks>
/// Initializes a new WeatherStation with the provided name and location.
/// The station is active by default upon creation.
/// </remarks>
/// <param name="name">The display name of the weather station.</param>
/// <param name="location">The geographic location of the station.</param>
public class WeatherStation( string name, string location ) : IValidatable<WeatherStation> {

	/// <summary>Gets the unique identifier for this weather station.</summary>
	public Guid WeatherStationId { get; private set; } = Guid.NewGuid( );

	/// <summary>Gets the display name of the weather station.</summary>
	public string Name { get; private set; } = name;

	/// <summary>Gets the geographic location of the station.</summary>
	public string Location { get; private set; } = location;

	/// <summary>Gets whether the station is currently active and recording data.</summary>
	public bool IsActive { get; private set; } = true;

	/// <summary>Gets the UTC timestamp when this station was created.</summary>
	public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

	/// <summary>Gets the UTC timestamp of the last update, or null if never updated.</summary>
	public DateTime? UpdatedAt { get; private set; }

	/// <summary>
	/// Gets the collection of weather forecasts recorded by this station.
	/// Declared as <c>virtual</c> so that EF Core lazy-loading proxies can override it.
	/// </summary>
	public virtual ICollection<WeatherForecast> Forecasts { get; private set; } = [ ];

	/// <summary>
	/// Custom validation context for decommissioning (permanently deactivating) a station.
	/// Demonstrates the use of <see cref="ValidationContextKey.Custom"/> for domain-specific contexts
	/// beyond the predefined CRUD set.
	/// </summary>
	public static readonly ValidationContextKey Decommission = ValidationContextKey.Custom( "Decommission" );

	private void Update( ) => UpdatedAt = DateTime.UtcNow;

	/// <summary>Activates the station so it starts accepting new forecast data.</summary>
	public WeatherStation Activate( ) {
		IsActive = true;
		Update( );
		return this;
	}

	/// <summary>Deactivates the station, preventing new forecast entries.</summary>
	public WeatherStation Deactivate( ) {
		IsActive = false;
		Update( );
		return this;
	}

	/// <summary>Updates the station's display name and location.</summary>
	public WeatherStation UpdateInfo( string name, string location ) {
		Name = name;
		Location = location;
		Update( );
		return this;
	}

	/// <summary>
	/// Defines validation rules for the weather station entity.
	/// Demonstrates three distinct validation contexts:
	/// <list type="bullet">
	///   <item>Global rules applied in all contexts.</item>
	///   <item><see cref="ValidationContextKey.Create"/> — async uniqueness check via <c>IScopedService&lt;T&gt;</c>.</item>
	///   <item><see cref="ValidationContextKey.Activate"/> — predefined non-CRUD context key.</item>
	///   <item><see cref="Decommission"/> — custom context key via <see cref="ValidationContextKey.Custom"/>.</item>
	/// </list>
	/// </summary>
	/// <summary>
	/// Generates a collection of randomized weather station entities for use in seeding or testing.
	/// Each station receives a unique name combining directional and terrain tokens with a sequential
	/// suffix to guarantee uniqueness across the generated set.
	/// </summary>
	/// <param name="amount">The number of weather station instances to generate.</param>
	/// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
	/// <returns>An enumerable sequence of unsaved <see cref="WeatherStation"/> instances.</returns>
	public static IEnumerable<WeatherStation> GenerateDataAsync( int amount, CancellationToken cancellationToken ) {
		var random = new Random( );

		var prefixes = new[ ] { "North", "South", "East", "West", "Central", "Upper", "Lower", "Mountain", "Coastal", "Valley" };
		var suffixes = new[ ] { "Ridge", "Peak", "Bay", "Plains", "Delta", "Plateau", "Canyon", "Coast", "Highlands", "Basin" };

		return Enumerable.Range( 0, amount ).Select( i => {
			var prefix = prefixes[ random.Next( prefixes.Length ) ];
			var suffix = suffixes[ random.Next( suffixes.Length ) ];
			var name = $"{prefix} {suffix} Station #{i + 1}";
			var lat = Math.Round( -90.0 + random.NextDouble( ) * 180.0, 4 );
			var lon = Math.Round( -180.0 + random.NextDouble( ) * 360.0, 4 );
			var location = $"Lat {lat}, Lon {lon}";
			return new WeatherStation( name, location );
		} );
	}

	public void Validate( ValidationBuilder<WeatherStation> builder, ValidationContextKey? context = null ) {
		builder.For( Name, rules => rules
			.NotEmpty( )
			.MinLength( 2 )
			.MaxLength( 100 ) );

		builder.For( Location, rules => rules
			.NotEmpty( )
			.MaxLength( 200 ) );

		builder.InContext( ValidationContextKey.Create, b => {
			b.For( Name, rules => rules
				.RespectAsync( async ( name, ct, sp ) => {
					var repository = sp.GetRequiredService<IScopedService<IWeatherStationRepository>>( );
					var spec = SpecBuilder<WeatherStation>.Create( ).WithNameNotInUse( name );
					return !await repository.ExecuteAsync( r => r.AnyAsync( spec, ct ) );
				} )
				.WithStatusCode( HttpStatusCode.Conflict )
				.WithMessage( _ => "A weather station with this name already exists." ) );
		} );

		builder.InContext( ValidationContextKey.Activate, b => {
			b.For( IsActive, rules => rules
				.IsFalse( )
				.WithMessage( _ => "Station is already active." ) );
		} );

		builder.InContext( Decommission, b => {
			b.For( IsActive, rules => rules
				.IsTrue( )
				.WithMessage( _ => "Station is already inactive." ) );
		} );
	}
}
