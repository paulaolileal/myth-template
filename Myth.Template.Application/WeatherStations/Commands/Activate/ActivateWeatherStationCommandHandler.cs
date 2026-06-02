using Myth.Guard;
using Myth.Interfaces;
using Myth.Interfaces.Repositories.EntityFramework;
using Myth.Models;
using Myth.Specifications;
using Myth.Template.Domain.Interfaces;
using Myth.Template.Domain.Models;
using Myth.Template.Domain.Specifications;

namespace Myth.Template.Application.WeatherStations.Commands.Activate;

/// <summary>
/// Handles activation of a weather station.
/// Validates against <see cref="ValidationContextKey.Activate"/> to ensure the station
/// is currently inactive before setting it active — demonstrating a predefined non-CRUD context.
/// </summary>
/// <param name="stationRepository">Repository for weather station data access.</param>
/// <param name="unitOfWork">Unit of Work for transaction management.</param>
/// <param name="validator">Validator used to apply the Activate context rules on the entity.</param>
public class ActivateWeatherStationCommandHandler(
	IWeatherStationRepository stationRepository,
	IUnitOfWorkRepository unitOfWork,
	IValidator validator ) : ICommandHandler<ActivateWeatherStationCommand> {

	public async Task<CommandResult> HandleAsync( ActivateWeatherStationCommand command, CancellationToken cancellationToken = default ) {
		var spec = SpecBuilder<WeatherStation>.Create( ).WithId( command.WeatherStationId );
		var station = await stationRepository.FirstAsync( spec, cancellationToken );

		await validator.ValidateAsync( station, ValidationContextKey.Activate, cancellationToken );

		station.Activate( );

		await stationRepository.UpdateAsync( station, cancellationToken );
		await unitOfWork.SaveChangesAsync( cancellationToken );

		return CommandResult.Success( );
	}
}
