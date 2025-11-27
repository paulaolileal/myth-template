using System.Net;
using Microsoft.Extensions.DependencyInjection;
using Myth.Builder;
using Myth.Enums;
using Myth.Extensions;
using Myth.Guard;
using Myth.Interfaces;
using Myth.ServiceProvider;
using Myth.Specifications;
using Myth.Template.Data.Resources;
using Myth.Template.Domain.Constants;
using Myth.Template.Domain.Interfaces;
using Myth.Template.Domain.Models;
using Myth.Template.Domain.Specifications;

namespace Myth.Template.Application.WeatherForecasts.Commands.Create;

/// <summary>
/// Command for creating a new weather forecast.
/// Implements validation rules to ensure data integrity and business logic compliance.
/// Returns the newly created weather forecast's unique identifier upon successful execution.
/// </summary>
public class CreateWeatherForecastCommand : ICommand<Guid>, IValidatable<CreateWeatherForecastCommand> {
	/// <summary>
	/// Gets the date for which the weather forecast will be created.
	/// Must be a past date and not already exist in the system.
	/// </summary>
	/// <value>The date for the weather forecast.</value>
	public DateOnly Date { get; private set; }

	/// <summary>
	/// Gets the temperature in Celsius for the forecast.
	/// Must be within the valid range of -100 to 100 degrees Celsius.
	/// </summary>
	/// <value>The temperature measurement in Celsius degrees.</value>
	public int TemperatureC { get; private set; }

	/// <summary>
	/// Gets the weather summary describing the forecasted conditions.
	/// Must be a valid enumeration value.
	/// </summary>
	/// <value>An enumeration value representing the weather conditions.</value>
	public string Summary { get; private set; } = null!;

	/// <summary>
	/// Validates the command properties according to business rules and data constraints.
	/// Ensures the date is in the past, unique in the system, temperature is within valid range,
	/// and summary is a valid enumeration value.
	/// </summary>
	/// <param name="builder">The validation builder used to define validation rules.</param>
	/// <param name="context">Optional validation context for additional validation scenarios.</param>
	public void Validate( ValidationBuilder<CreateWeatherForecastCommand> builder, ValidationContextKey? context = null ) {
		builder.For( Date, rules => rules
			.Past( )

			.GreaterThan( DateOnly.MinValue )

			.RespectAsync( async ( date, ct, sp ) => {
				var repository = sp.GetRequiredService<IScopedService<IWeatherForecastRepository>>( );

				var spec = SpecBuilder<WeatherForecast>
					.Create( )
					.WithDateNotInUse( date );

				return await repository.ExecuteAsync( x => x.AllAsync( spec, ct ) );
			} )
			.WithStatusCode( HttpStatusCode.Conflict )
			.WithCode( ValidationCodes.Conflict )
			.WithMessage( value => string.Format( Messages.Conflict, value ) ) );

		builder.For( TemperatureC, rules => rules
			.Between( -100, 100 ) );

		builder.For( Summary, rules => rules
			.NameExistsInConstant<Summary, int>( ).WithOptions( OptionsType.OnlyName ) );
	}
}
