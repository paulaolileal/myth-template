using System.Net;
using Microsoft.Extensions.DependencyInjection;
using Myth.Builder;
using Myth.Guard;
using Myth.Interfaces;
using Myth.ServiceProvider;
using Myth.Template.Data.Resources;
using Myth.Template.Domain.Interfaces;

namespace Myth.Template.Application.WeatherStations.Commands.Delete;

/// <summary>
/// Command for permanently removing a weather station from the system.
/// </summary>
public record DeleteWeatherStationCommand : ICommand, IValidatable<DeleteWeatherStationCommand> {

	/// <summary>Gets the identifier of the station to delete.</summary>
	public Guid WeatherStationId { get; private set; }

	public DeleteWeatherStationCommand( Guid weatherStationId ) => WeatherStationId = weatherStationId;

	public void Validate( ValidationBuilder<DeleteWeatherStationCommand> builder, ValidationContextKey? context = null ) {
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
