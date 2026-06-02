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

namespace Myth.Template.Application.WeatherStations.Commands.Activate;

/// <summary>
/// Command for activating an inactive weather station.
/// Demonstrates the use of the predefined <see cref="ValidationContextKey.Activate"/> context key,
/// which enforces that the station must currently be inactive before it can be activated.
/// </summary>
public record ActivateWeatherStationCommand : ICommand, IValidatable<ActivateWeatherStationCommand> {

	/// <summary>Gets the identifier of the station to activate.</summary>
	public Guid WeatherStationId { get; private set; }

	public ActivateWeatherStationCommand( Guid weatherStationId ) => WeatherStationId = weatherStationId;

	public void Validate( ValidationBuilder<ActivateWeatherStationCommand> builder, ValidationContextKey? context = null ) {
		builder.For( WeatherStationId, rules => rules
			.NotDefault( )
			.SetStopOnFailure( )
			.RespectAsync( ( id, ct, sp ) => sp
				.GetRequiredService<IScopedService<IWeatherStationRepository>>( )
				.ExecuteAsync( repo => repo.AnyAsync( x => x.WeatherStationId == id, ct ) ) )
			.WithMessage( value => string.Format( Messages.NotFound, value ) )
			.WithStatusCode( HttpStatusCode.NotFound ) );
	}
}
