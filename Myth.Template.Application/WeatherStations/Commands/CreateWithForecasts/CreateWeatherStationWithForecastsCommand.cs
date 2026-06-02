using System.Net;
using Microsoft.Extensions.DependencyInjection;
using Myth.Builder;
using Myth.Guard;
using Myth.Interfaces;
using Myth.ServiceProvider;
using Myth.Specifications;
using Myth.Template.Data.Resources;
using Myth.Template.Domain.Interfaces;
using Myth.Template.Domain.Models;
using Myth.Template.Domain.Specifications;

namespace Myth.Template.Application.WeatherStations.Commands.CreateWithForecasts;

/// <summary>
/// Command for creating a weather station together with its initial set of forecasts
/// in a single transaction. Demonstrates the <c>IUnitOfWorkRepository</c> savepoint pattern.
/// </summary>
public record CreateWeatherStationWithForecastsCommand : ICommand<Guid>, IValidatable<CreateWeatherStationWithForecastsCommand> {

	/// <summary>Gets the display name for the new station.</summary>
	public string Name { get; init; } = null!;

	/// <summary>Gets the geographic location for the new station.</summary>
	public string Location { get; init; } = null!;

	/// <summary>Gets the initial forecasts to associate with the station upon creation.</summary>
	public IReadOnlyCollection<InitialForecastItem> InitialForecasts { get; init; } = [ ];

	public void Validate( ValidationBuilder<CreateWeatherStationWithForecastsCommand> builder, ValidationContextKey? context = null ) {
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
				.WithMessage( _ => string.Format( Messages.Conflict, "Name" ) ) );
		} );
	}
}

/// <summary>A single forecast entry to be created alongside a new weather station.</summary>
public record InitialForecastItem {

	/// <summary>Gets the date for the initial forecast.</summary>
	public DateOnly Date { get; init; }

	/// <summary>Gets the temperature in Celsius.</summary>
	public int TemperatureC { get; init; }

	/// <summary>Gets the name of the weather summary (e.g., "Sunny").</summary>
	public string Summary { get; init; } = null!;
}
