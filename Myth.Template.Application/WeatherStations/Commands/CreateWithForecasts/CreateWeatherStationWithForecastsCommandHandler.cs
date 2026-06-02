using Myth.Interfaces;
using Myth.Interfaces.Repositories.EntityFramework;
using Myth.Models;
using Myth.Template.Domain.Interfaces;
using Myth.Template.Domain.Models;

namespace Myth.Template.Application.WeatherStations.Commands.CreateWithForecasts;

/// <summary>
/// Handles creation of a weather station together with its initial set of forecasts
/// in a single transaction, using savepoints for fine-grained rollback control.
///
/// <para>
/// This handler demonstrates <see cref="IUnitOfWorkRepository"/> with savepoints:
/// <list type="number">
///   <item>A transaction is opened with <c>BeginTransactionAsync</c>.</item>
///   <item>The station is persisted and a savepoint <c>"after_station"</c> is created.</item>
///   <item>Each forecast is added; if a forecast fails, the handler could roll back to
///       the savepoint and commit the station-only state.</item>
///   <item>On full success the transaction is committed.</item>
/// </list>
/// </para>
///
/// <para>
/// <b>Note on the in-memory EF provider:</b> EF Core's InMemory provider silently ignores
/// transaction calls, so savepoints have no observable effect in this template environment.
/// The pattern is architecturally correct and will work as expected with a real database
/// provider such as SQL Server or PostgreSQL.
/// </para>
/// </summary>
/// <param name="stationRepository">Repository for weather station data access.</param>
/// <param name="forecastRepository">Repository for weather forecast data access.</param>
/// <param name="unitOfWork">Unit of Work for transaction and savepoint management.</param>
public class CreateWeatherStationWithForecastsCommandHandler(
	IWeatherStationRepository stationRepository,
	IWeatherForecastRepository forecastRepository,
	IUnitOfWorkRepository unitOfWork ) : ICommandHandler<CreateWeatherStationWithForecastsCommand, Guid> {

	public async Task<CommandResult<Guid>> HandleAsync( CreateWeatherStationWithForecastsCommand command, CancellationToken cancellationToken = default ) {
		await unitOfWork.BeginTransactionAsync( cancellationToken );

		var station = new WeatherStation( command.Name, command.Location );
		await stationRepository.AddAsync( station, cancellationToken );
		await unitOfWork.SaveChangesAsync( cancellationToken );

		await unitOfWork.CreateSavepointAsync( "after_station", cancellationToken );

		foreach ( var item in command.InitialForecasts ) {
			var forecast = new WeatherForecast( item.Date, item.TemperatureC, Summary.FromName( item.Summary ) )
				.AssignToStation( station.WeatherStationId );

			await forecastRepository.AddAsync( forecast, cancellationToken );
		}

		await unitOfWork.SaveChangesAsync( cancellationToken );
		await unitOfWork.CommitAsync( cancellationToken );

		return CommandResult<Guid>.Success( station.WeatherStationId );
	}
}
