using System.Net;
using Microsoft.Extensions.DependencyInjection;
using Myth.Builder;
using Myth.Guard;
using Myth.Interfaces;
using Myth.ServiceProvider;
using Myth.Template.Application.WeatherForecasts.DTOs;
using Myth.Template.Data.Resources;
using Myth.Template.Domain.Constants;
using Myth.Template.Domain.Interfaces;

namespace Myth.Template.Application.WeatherForecasts.Queries.GetById;

/// <summary>
/// Query for retrieving a single weather forecast by its unique identifier.
/// Validates that the provided identifier is not empty or default value.
/// Returns a GetWeatherForecastResponse containing the forecast details.
/// </summary>
public class GetWeatherForecastByIdQuery : IValidatable<GetWeatherForecastByIdQuery>, IQuery<GetWeatherForecastResponse> {
	/// <summary>
	/// Gets the unique identifier of the weather forecast to retrieve.
	/// Must be a valid, non-default GUID value.
	/// </summary>
	/// <value>The GUID identifier of the weather forecast.</value>
	public Guid WeatherForecastId { get; private set; }

	/// <summary>
	/// Initializes a new instance of the GetWeatherForecastQuery class with the specified weather forecast identifier.
	/// </summary>
	/// <param name="weatherForecastId">The unique identifier of the weather forecast to retrieve.</param>
	public GetWeatherForecastByIdQuery( Guid weatherForecastId ) {
		WeatherForecastId = weatherForecastId;
	}

	/// <summary>
	/// Validates the query parameters to ensure the weather forecast identifier is valid.
	/// Checks that the GUID is not empty or contains the default value.
	/// </summary>
	/// <param name="builder">The validation builder used to define validation rules.</param>
	/// <param name="context">Optional validation context for additional validation scenarios.</param>
	public void Validate( ValidationBuilder<GetWeatherForecastByIdQuery> builder, ValidationContextKey? context = null ) {
		builder.For( WeatherForecastId, rules => rules
			.NotDefault( )
			.SetStopOnFailure( )

			.RespectAsync( ( value, cancellationToken, provider ) => provider.GetRequiredService<IScopedService<IWeatherForecastRepository>>( ).ExecuteAsync( x => x.AnyAsync( x => x.WeatherForecastId == value, cancellationToken ) ) )
			.WithMessage( value => string.Format( Messages.NotFound, value ) )
			.WithStatusCode( HttpStatusCode.NotFound )
			.WithCode( ValidationCodes.NotFound ) );
	}
}
