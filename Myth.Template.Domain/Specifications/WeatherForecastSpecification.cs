using Myth.Interfaces;
using Myth.Template.Domain.Models;

namespace Myth.Template.Domain.Specifications;

/// <summary>
/// Static class containing extension methods for building specifications to filter WeatherForecast entities.
/// Provides fluent API methods for creating complex query specifications with various filtering criteria.
/// </summary>
public static class WeatherForecastSpecification {

	/// <summary>
	/// Adds a specification to filter weather forecasts by summary type.
	/// Only applies the filter if the summary parameter is not null.
	/// </summary>
	/// <param name="specifications">The existing specification to extend.</param>
	/// <param name="summary">Optional summary type to filter by. If null, no filter is applied.</param>
	/// <returns>A new specification that includes the summary filter if applicable.</returns>
	public static ISpec<WeatherForecast> WithSummary( this ISpec<WeatherForecast> specifications, string? summary ) => specifications.AndIf( !string.IsNullOrEmpty( summary ), x => x.Summary.Value == Summary.FromName( summary ).Value );

	/// <summary>
	/// Adds a specification to filter weather forecasts by their unique identifier.
	/// </summary>
	/// <param name="specifications">The existing specification to extend.</param>
	/// <param name="weatherForecastId">The unique identifier to filter by.</param>
	/// <returns>A new specification that includes the ID filter.</returns>
	public static ISpec<WeatherForecast> WithId( this ISpec<WeatherForecast> specifications, Guid weatherForecastId ) => specifications.And( x => x.WeatherForecastId == weatherForecastId );

	/// <summary>
	/// Adds a specification to ensure the specified date is not already used by existing weather forecasts.
	/// This is typically used for validation to prevent duplicate forecasts for the same date.
	/// </summary>
	/// <param name="specifications">The existing specification to extend.</param>
	/// <param name="date">The date that should not be in use.</param>
	/// <returns>A new specification that excludes forecasts with the specified date.</returns>
	public static ISpec<WeatherForecast> WithDateNotInUse( this ISpec<WeatherForecast> specifications, DateOnly date ) => specifications.And( x => x.Date != date );

	/// <summary>
	/// Adds a specification to filter weather forecasts with dates greater than or equal to the specified minimum date.
	/// Only applies the filter if the date parameter is not null.
	/// </summary>
	/// <param name="specifications">The existing specification to extend.</param>
	/// <param name="date">Optional minimum date to filter by. If null, no filter is applied.</param>
	/// <returns>A new specification that includes the minimum date filter if applicable.</returns>
	public static ISpec<WeatherForecast> WithDateGreaterThan( this ISpec<WeatherForecast> specifications, DateOnly? date ) => specifications.AndIf( date != null, x => x.Date >= date );

	/// <summary>
	/// Adds a specification to filter weather forecasts with dates less than or equal to the specified maximum date.
	/// Only applies the filter if the date parameter is not null.
	/// </summary>
	/// <param name="specifications">The existing specification to extend.</param>
	/// <param name="date">Optional maximum date to filter by. If null, no filter is applied.</param>
	/// <returns>A new specification that includes the maximum date filter if applicable.</returns>
	public static ISpec<WeatherForecast> WithDateLowerThan( this ISpec<WeatherForecast> specifications, DateOnly? date ) => specifications.AndIf( date != null, x => x.Date <= date );

	/// <summary>
	/// Adds a specification to filter weather forecasts with temperatures greater than or equal to the specified minimum temperature.
	/// Only applies the filter if the temperature parameter is not null.
	/// </summary>
	/// <param name="specifications">The existing specification to extend.</param>
	/// <param name="temperature">Optional minimum temperature in Celsius to filter by. If null, no filter is applied.</param>
	/// <returns>A new specification that includes the minimum temperature filter if applicable.</returns>
	public static ISpec<WeatherForecast> WithTemparatureGreaterThan( this ISpec<WeatherForecast> specifications, int? temperature ) => specifications.AndIf( temperature != null, x => x.TemperatureC >= temperature );

	/// <summary>
	/// Adds a specification to filter weather forecasts with temperatures less than or equal to the specified maximum temperature.
	/// Only applies the filter if the temperature parameter is not null.
	/// </summary>
	/// <param name="specifications">The existing specification to extend.</param>
	/// <param name="temperature">Optional maximum temperature in Celsius to filter by. If null, no filter is applied.</param>
	/// <returns>A new specification that includes the maximum temperature filter if applicable.</returns>
	public static ISpec<WeatherForecast> WithTemparatureLowerThan( this ISpec<WeatherForecast> specifications, int? temperature ) => specifications.AndIf( temperature != null, x => x.TemperatureC <= temperature );
}
