using Myth.Interfaces;
using Myth.Interfaces.Repositories.EntityFramework;
using Myth.Models;
using Myth.Template.Application.WeatherStations.Events.Created;
using Myth.Template.Domain.Interfaces;
using Myth.Template.Domain.Models;

namespace Myth.Template.Application.WeatherStations.Commands.Create;

/// <summary>
/// Handles the creation of a new weather station.
/// Persists the station, saves via Unit of Work, and publishes a domain event.
/// </summary>
/// <param name="stationRepository">Repository for weather station data access.</param>
/// <param name="unitOfWork">Unit of Work for transaction management.</param>
/// <param name="dispatcher">Dispatcher for publishing domain events.</param>
public class CreateWeatherStationCommandHandler(
	IWeatherStationRepository stationRepository,
	IUnitOfWorkRepository unitOfWork,
	IDispatcher dispatcher ) : ICommandHandler<CreateWeatherStationCommand, Guid> {

	public async Task<CommandResult<Guid>> HandleAsync( CreateWeatherStationCommand command, CancellationToken cancellationToken = default ) {
		var station = new WeatherStation( command.Name, command.Location );

		await stationRepository.AddAsync( station, cancellationToken );
		await unitOfWork.SaveChangesAsync( cancellationToken );

		await dispatcher.PublishEventAsync( new WeatherStationCreatedEvent( station.WeatherStationId ), cancellationToken );

		return CommandResult<Guid>.Success( station.WeatherStationId );
	}
}
