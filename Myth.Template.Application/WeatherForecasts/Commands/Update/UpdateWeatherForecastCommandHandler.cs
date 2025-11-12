using Microsoft.Extensions.DependencyInjection;
using Myth.Interfaces;
using Myth.Interfaces.Repositories.EntityFramework;
using Myth.Models;
using Myth.Specifications;
using Myth.Template.Domain.Interfaces;
using Myth.Template.Domain.Models;
using Myth.Template.Domain.Specifications;

namespace Myth.Template.Application.WeatherForecasts.Commands.Update;

/// <summary>
/// Command handler responsible for processing UpdateWeatherForecastCommand requests.
/// Retrieves the existing weather forecast, applies the updates, and persists the changes.
/// Uses scoped services to ensure proper transaction handling and data consistency.
/// </summary>
public class UpdateWeatherForecastCommandHandler : ICommandHandler<UpdateWeatherForecastCommand> {
	/// <summary>
	/// Factory for creating service scopes to manage the lifetime of scoped dependencies.
	/// </summary>
	private readonly IServiceScopeFactory _scopeFactory;

	/// <summary>
	/// Initializes a new instance of the UpdateWeatherForecastCommandHandler class.
	/// </summary>
	/// <param name="scopeFactory">Factory for creating service scopes to resolve scoped dependencies.</param>
	public UpdateWeatherForecastCommandHandler( IServiceScopeFactory scopeFactory ) {
		_scopeFactory = scopeFactory;
	}

	/// <summary>
	/// Handles the execution of an UpdateWeatherForecastCommand by retrieving the existing weather forecast,
	/// applying the specified changes to temperature and summary, and persisting the updates.
	/// </summary>
	/// <param name="command">The command containing the weather forecast identifier and the data to update.</param>
	/// <param name="cancellationToken">Token to monitor for cancellation requests during the operation.</param>
	/// <returns>
	/// A task that represents the asynchronous operation. The task result contains a CommandResult
	/// indicating the success or failure of the update operation.
	/// </returns>
	public async Task<CommandResult> HandleAsync( UpdateWeatherForecastCommand command, CancellationToken cancellationToken = default ) {
		var serviceProvider = _scopeFactory.CreateScope( ).ServiceProvider;

		var repository = serviceProvider.GetRequiredService<IWeatherForecastRepository>( );
		var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWorkRepository>( );

		var spec = SpecBuilder<WeatherForecast>
			.Create( )
			.WithId( command.WeatherForecastId );

		var weatherForecast = await repository.FirstAsync( spec, cancellationToken );

		weatherForecast
			.ChangeSummary( command.Summary )
			.ChangeTemperatureC( command.TemperatureC );

		await repository.UpdateAsync( weatherForecast, cancellationToken );

		await unitOfWork.SaveChangesAsync( cancellationToken );

		return CommandResult.Success( );
	}
}
