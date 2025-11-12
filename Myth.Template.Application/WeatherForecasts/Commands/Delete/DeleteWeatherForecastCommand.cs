using System.Net;
using Microsoft.Extensions.DependencyInjection;
using Myth.Builder;
using Myth.Guard;
using Myth.Interfaces;
using Myth.ServiceProvider;
using Myth.Template.Domain.Interfaces;

namespace Myth.Template.Application.WeatherForecasts.Commands.Delete;

/// <summary>
/// Command for deleting an existing weather forecast from the system.
/// Implements validation rules to ensure the target forecast exists before attempting deletion.
/// Uses a record type for immutability and value-based equality.
/// </summary>
public record DeleteWeatherForecastCommand : ICommand, IValidatable<DeleteWeatherForecastCommand> {
	/// <summary>
	/// Gets the unique identifier of the weather forecast to delete.
	/// Must be a valid, non-default GUID that corresponds to an existing weather forecast.
	/// </summary>
	/// <value>The GUID identifier of the weather forecast to remove.</value>
	public Guid WeatherForecastId { get; private set; }

	/// <summary>
	/// Initializes a new instance of the DeleteWeatherForecastCommand record with the specified weather forecast identifier.
	/// </summary>
	/// <param name="weatherForecastId">The unique identifier of the weather forecast to delete.</param>
	public DeleteWeatherForecastCommand( Guid weatherForecastId ) {
		WeatherForecastId = weatherForecastId;
	}

	/// <summary>
	/// Validates the command properties according to business rules and data constraints.
	/// Ensures the weather forecast identifier is not empty and corresponds to an existing forecast.
	/// </summary>
	/// <param name="builder">The validation builder used to define validation rules.</param>
	/// <param name="context">Optional validation context for additional validation scenarios.</param>
	public void Validate( ValidationBuilder<DeleteWeatherForecastCommand> builder, ValidationContextKey? context = null ) {
		builder.For( WeatherForecastId, rules => rules
			.NotDefault( )

			.RespectAsync( ( weatherForecastId, cancellationToken, provider ) => provider
				.GetRequiredService<IScopedService<IWeatherForecastRepository>>( )
				.ExecuteAsync( service => service.AnyAsync( x => x.WeatherForecastId == weatherForecastId, cancellationToken ) ) )
			.WithMessage( "Weather forecast with the specified ID does not exist." )
			.WithCode( "NOT_FOUND" )
			.WithStatusCode( HttpStatusCode.NotFound ) );
	}
}
