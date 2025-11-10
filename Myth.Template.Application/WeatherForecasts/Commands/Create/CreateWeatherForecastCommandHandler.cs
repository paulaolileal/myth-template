using Microsoft.Extensions.DependencyInjection;
using Myth.Interfaces;
using Myth.Interfaces.Repositories.EntityFramework;
using Myth.Models;
using Myth.Template.Domain.Interfaces;
using Myth.Template.Domain.Models;

namespace Myth.Template.Application.WeatherForecasts.Commands.Create {

	/// <summary>
	/// Command handler responsible for processing CreateWeatherForecastCommand requests.
	/// Creates a new WeatherForecast entity, persists it to the repository, and returns the generated identifier.
	/// Uses scoped services to ensure proper transaction handling and data consistency.
	/// </summary>
	public class CreateWeatherForecastCommandHandler : ICommandHandler<CreateWeatherForecastCommand, Guid> {
		/// <summary>
		/// Factory for creating service scopes to manage the lifetime of scoped dependencies.
		/// </summary>
		private readonly IServiceScopeFactory _serviceScopeFactory;

		/// <summary>
		/// Initializes a new instance of the CreateWeatherForecastCommandHandler class.
		/// </summary>
		/// <param name="serviceScopeFactory">Factory for creating service scopes to resolve scoped dependencies.</param>
		public CreateWeatherForecastCommandHandler( IServiceScopeFactory serviceScopeFactory ) {
			_serviceScopeFactory = serviceScopeFactory;
		}

		/// <summary>
		/// Handles the execution of a CreateWeatherForecastCommand by creating a new weather forecast entity,
		/// adding it to the repository, and persisting the changes through the unit of work pattern.
		/// </summary>
		/// <param name="command">The command containing the data for creating the weather forecast.</param>
		/// <param name="cancellationToken">Token to monitor for cancellation requests during the operation.</param>
		/// <returns>
		/// A task that represents the asynchronous operation. The task result contains a CommandResult
		/// with the unique identifier of the newly created weather forecast if successful.
		/// </returns>
		public async Task<CommandResult<Guid>> HandleAsync( CreateWeatherForecastCommand command, CancellationToken cancellationToken = default ) {
			var weatherForecast = new WeatherForecast(
				command.Date,
				command.TemperatureC,
				command.Summary );

			var serviceProvider = _serviceScopeFactory.CreateScope( ).ServiceProvider;

			var repository = serviceProvider.GetRequiredService<IWeatherForecastRepository>( );

			var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWorkRepository>( );

			await repository.AddAsync( weatherForecast, cancellationToken );

			await unitOfWork.SaveChangesAsync( cancellationToken );

			return CommandResult<Guid>.Success( weatherForecast.WeatherForecastId );
		}
	}
}