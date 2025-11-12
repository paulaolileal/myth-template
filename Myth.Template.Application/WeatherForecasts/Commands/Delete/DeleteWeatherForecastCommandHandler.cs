using Microsoft.Extensions.DependencyInjection;
using Myth.Interfaces;
using Myth.Interfaces.Repositories.EntityFramework;
using Myth.Models;
using Myth.Specifications;
using Myth.Template.Domain.Interfaces;
using Myth.Template.Domain.Models;
using Myth.Template.Domain.Specifications;

namespace Myth.Template.Application.WeatherForecasts.Commands.Delete;

/// <summary>
/// Command handler responsible for processing DeleteWeatherForecastCommand requests.
/// Retrieves the existing weather forecast and permanently removes it from the system.
/// Uses scoped services to ensure proper transaction handling and data consistency.
/// </summary>
public class DeleteWeatherForecastCommandHandler : ICommandHandler<DeleteWeatherForecastCommand> {
	/// <summary>
	/// Factory for creating service scopes to manage the lifetime of scoped dependencies.
	/// </summary>
	private readonly IServiceScopeFactory _scopeFactory;

	/// <summary>
	/// Initializes a new instance of the DeleteWeatherForecastCommandHandler class.
	/// </summary>
	/// <param name="scopeFactory">Factory for creating service scopes to resolve scoped dependencies.</param>
	public DeleteWeatherForecastCommandHandler( IServiceScopeFactory scopeFactory ) {
		_scopeFactory = scopeFactory;
	}

	/// <summary>
	/// Handles the execution of a DeleteWeatherForecastCommand by retrieving the existing weather forecast
	/// and permanently removing it from the repository. Changes are persisted through the unit of work pattern.
	/// </summary>
	/// <param name="command">The command containing the identifier of the weather forecast to delete.</param>
	/// <param name="cancellationToken">Token to monitor for cancellation requests during the operation.</param>
	/// <returns>
	/// A task that represents the asynchronous operation. The task result contains a CommandResult
	/// indicating the success or failure of the deletion operation.
	/// </returns>
	public async Task<CommandResult> HandleAsync( DeleteWeatherForecastCommand command, CancellationToken cancellationToken = default ) {
		var serviceProvider = _scopeFactory.CreateScope( ).ServiceProvider;

		var repository = serviceProvider.GetRequiredService<IWeatherForecastRepository>( );
		var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWorkRepository>( );

		var spec = SpecBuilder<WeatherForecast>
			.Create( )
			.WithId( command.WeatherForecastId );

		var weatherForecast = await repository.FirstAsync( spec, cancellationToken );

		await repository.RemoveAsync( weatherForecast, cancellationToken );

		await unitOfWork.SaveChangesAsync( cancellationToken );

		return CommandResult.Success( );
	}
}
