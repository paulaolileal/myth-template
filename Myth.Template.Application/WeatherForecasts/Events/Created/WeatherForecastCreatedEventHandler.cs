using Microsoft.Extensions.Logging;
using Myth.Interfaces;

namespace Myth.Template.Application.WeatherForecasts.Events.Created {

	/// <summary>
	/// Event handler responsible for processing WeatherForecastCreatedEvent occurrences.
	/// Logs information about newly created weather forecasts for auditing and monitoring purposes.
	/// This handler demonstrates how domain events can be used to trigger cross-cutting concerns.
	/// </summary>
	public class WeatherForecastCreatedEventHandler : IEventHandler<WeatherForecastCreatedEvent> {
		/// <summary>
		/// Logger instance for recording information about weather forecast creation events.
		/// </summary>
		private readonly ILogger<WeatherForecastCreatedEventHandler> _logger;

		/// <summary>
		/// Initializes a new instance of the WeatherForecastCreatedEventHandler class.
		/// </summary>
		/// <param name="logger">Logger instance for recording event processing information.</param>
		public WeatherForecastCreatedEventHandler( ILogger<WeatherForecastCreatedEventHandler> logger ) {
			_logger = logger;
		}

		/// <summary>
		/// Handles the WeatherForecastCreatedEvent by logging information about the newly created weather forecast.
		/// This method is automatically invoked when a WeatherForecastCreatedEvent is published in the system.
		/// </summary>
		/// <param name="event">The domain event containing details about the created weather forecast.</param>
		/// <param name="cancellationToken">Token to monitor for cancellation requests during the operation.</param>
		/// <returns>A task that represents the asynchronous operation completion.</returns>
		public Task HandleAsync( WeatherForecastCreatedEvent @event, CancellationToken cancellationToken = default ) {
			_logger.LogInformation( "Weather forecast created with ID `{WeatherForecastId}`", @event.WeatherForecastId );

			return Task.CompletedTask;
		}
	}
}