using System.Net;
using Microsoft.Extensions.DependencyInjection;
using Myth.Builder;
using Myth.Guard;
using Myth.Interfaces;
using Myth.ServiceProvider;
using Myth.Template.Data.Resources;
using Myth.Template.Domain.Interfaces;

namespace Myth.Template.Application.WeatherStations.Commands.Decommission;

/// <summary>
/// Command for permanently decommissioning (deactivating) a weather station.
/// Demonstrates the use of <see cref="WeatherStation.Decommission"/>, a <em>custom</em>
/// <see cref="ValidationContextKey"/> created via <c>ValidationContextKey.Custom("Decommission")</c>.
/// </summary>
public record DecommissionWeatherStationCommand : ICommand, IValidatable<DecommissionWeatherStationCommand> {

	/// <summary>Gets the identifier of the station to decommission.</summary>
	public Guid WeatherStationId { get; private set; }

	public DecommissionWeatherStationCommand( Guid weatherStationId ) => WeatherStationId = weatherStationId;

	public void Validate( ValidationBuilder<DecommissionWeatherStationCommand> builder, ValidationContextKey? context = null ) {
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
