using System.Net;
using Microsoft.Extensions.DependencyInjection;
using Myth.Builder;
using Myth.Enums;
using Myth.Extensions;
using Myth.Guard;
using Myth.Interfaces;
using Myth.ServiceProvider;
using Myth.Template.Data.Resources;
using Myth.Template.Domain.Constants;
using Myth.Template.Domain.Interfaces;
using Myth.Template.Domain.Models;

namespace Myth.Template.Application.WeatherForecasts.Commands.Update;

/// <summary>
/// Command for updating an existing weather forecast.
/// Allows modification of temperature and summary while preserving the original date.
/// Implements validation rules to ensure data integrity and verify the target forecast exists.
/// </summary>
public class UpdateWeatherForecastCommand( Guid weatherForecastId, int temperatureC, string summary ) : ICommand, IValidatable<UpdateWeatherForecastCommand> {

	/// <summary>
	/// Gets the unique identifier of the weather forecast to update.
	/// Must be a valid, non-default GUID that corresponds to an existing weather forecast.
	/// </summary>
	/// <value>The GUID identifier of the weather forecast to modify.</value>
	public Guid WeatherForecastId { get; private set; } = weatherForecastId;

	/// <summary>
	/// Gets the temperature in Celsius for the forecast.
	/// Must be within the valid range of -100 to 100 degrees Celsius.
	/// </summary>
	/// <value>The temperature measurement in Celsius degrees.</value>
	public int TemperatureC { get; private set; } = temperatureC;

	/// <summary>
	/// Gets the weather summary describing the forecasted conditions.
	/// Must be a valid enumeration value.
	/// </summary>
	/// <value>An enumeration value representing the weather conditions.</value>
	public string Summary { get; private set; } = summary;

	/// <summary>
	/// Validates the command properties according to business rules and data constraints.
	/// Ensures the weather forecast exists, temperature is within valid range,
	/// and summary is a valid enumeration value.
	/// </summary>
	/// <param name="builder">The validation builder used to define validation rules.</param>
	/// <param name="context">Optional validation context for additional validation scenarios.</param>
	public void Validate( ValidationBuilder<UpdateWeatherForecastCommand> builder, ValidationContextKey? context = null ) {
		builder.For( WeatherForecastId, rules => rules
			.NotDefault( )
			.SetStopOnFailure( )

			.RespectAsync( ( weatherForecastId, cancellationToken, provider ) => provider
				.GetRequiredService<IScopedService<IWeatherForecastRepository>>( )
				.ExecuteAsync( service => service.AnyAsync( x => x.WeatherForecastId == weatherForecastId, cancellationToken ) ) )
			.WithMessage( value => string.Format( Messages.NotFound, value ) )
			.WithCode( ValidationCodes.NotFound )
			.WithStatusCode( HttpStatusCode.NotFound ) );

		builder.For( TemperatureC, rules => rules
			.Between( -100, 100 ) );

		builder.For( Summary, rules => rules
			.NameExistsInConstant<Summary, int>( ).WithOptions( OptionsType.OnlyName ) );
	}
}
