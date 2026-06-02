using Myth.Interfaces;
using Myth.Interfaces.Repositories.EntityFramework;
using Myth.Models;
using Myth.Specifications;
using Myth.Template.Domain.Interfaces;
using Myth.Template.Domain.Models;
using Myth.Template.Domain.Specifications;

namespace Myth.Template.Application.WeatherStations.Commands.Delete;

/// <summary>
/// Handles permanent removal of a weather station.
/// </summary>
/// <param name="stationRepository">Repository for weather station data access.</param>
/// <param name="unitOfWork">Unit of Work for transaction management.</param>
public class DeleteWeatherStationCommandHandler(
	IWeatherStationRepository stationRepository,
	IUnitOfWorkRepository unitOfWork ) : ICommandHandler<DeleteWeatherStationCommand> {

	public async Task<CommandResult> HandleAsync( DeleteWeatherStationCommand command, CancellationToken cancellationToken = default ) {
		var spec = SpecBuilder<WeatherStation>.Create( ).WithId( command.WeatherStationId );
		var station = await stationRepository.FirstAsync( spec, cancellationToken );

		await stationRepository.RemoveAsync( station, cancellationToken );
		await unitOfWork.SaveChangesAsync( cancellationToken );

		return CommandResult.Success( );
	}
}
