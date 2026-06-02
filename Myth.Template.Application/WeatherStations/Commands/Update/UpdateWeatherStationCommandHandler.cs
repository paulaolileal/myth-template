using Myth.Interfaces;
using Myth.Interfaces.Repositories.EntityFramework;
using Myth.Models;
using Myth.Specifications;
using Myth.Template.Domain.Interfaces;
using Myth.Template.Domain.Models;
using Myth.Template.Domain.Specifications;

namespace Myth.Template.Application.WeatherStations.Commands.Update;

/// <summary>
/// Handles updates to an existing weather station's name and location.
/// </summary>
/// <param name="stationRepository">Repository for weather station data access.</param>
/// <param name="unitOfWork">Unit of Work for transaction management.</param>
public class UpdateWeatherStationCommandHandler(
	IWeatherStationRepository stationRepository,
	IUnitOfWorkRepository unitOfWork ) : ICommandHandler<UpdateWeatherStationCommand> {

	public async Task<CommandResult> HandleAsync( UpdateWeatherStationCommand command, CancellationToken cancellationToken = default ) {
		var spec = SpecBuilder<WeatherStation>.Create( ).WithId( command.WeatherStationId );
		var station = await stationRepository.FirstAsync( spec, cancellationToken );

		station.UpdateInfo( command.Name, command.Location );

		await stationRepository.UpdateAsync( station, cancellationToken );
		await unitOfWork.SaveChangesAsync( cancellationToken );

		return CommandResult.Success( );
	}
}
