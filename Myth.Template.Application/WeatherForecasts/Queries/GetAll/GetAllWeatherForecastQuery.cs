using Myth.Builder;
using Myth.Extensions;
using Myth.Guard;
using Myth.Interfaces;
using Myth.Interfaces.Results;
using Myth.Template.Application.WeatherForecasts.DTOs;
using Myth.Template.Domain.Models;
using Myth.ValueObjects;

namespace Myth.Template.Application.WeatherForecasts.Queries.GetAll {

	/// <summary>
	/// Query for retrieving a paginated list of weather forecasts with optional filtering criteria.
	/// Supports filtering by summary type, date ranges, and temperature ranges.
	/// Inherits from Pagination to provide page-based result sets.
	/// </summary>
	public class GetAllWeatherForecastQuery : Pagination, IValidatable<GetAllWeatherForecastQuery>, IQuery<IPaginated<GetWeatherForecastResponse>> {
		/// <summary>
		/// Gets or sets the optional weather summary filter.
		/// When specified, only forecasts with this summary type will be returned.
		/// </summary>
		/// <value>An optional Summary enumeration value for filtering results.</value>
		public Summary? Summary { get; set; }

		/// <summary>
		/// Gets the optional minimum date filter.
		/// When specified, only forecasts on or after this date will be returned.
		/// </summary>
		/// <value>An optional minimum date for filtering results.</value>
		public DateOnly? MinimumDate { get; }

		/// <summary>
		/// Gets the optional maximum date filter.
		/// When specified, only forecasts on or before this date will be returned.
		/// </summary>
		/// <value>An optional maximum date for filtering results.</value>
		public DateOnly? MaximumDate { get; }

		/// <summary>
		/// Gets the optional minimum temperature filter in Celsius.
		/// When specified, only forecasts with temperatures at or above this value will be returned.
		/// </summary>
		/// <value>An optional minimum temperature in Celsius for filtering results.</value>
		public int? MinimumTemperature { get; }

		/// <summary>
		/// Gets the optional maximum temperature filter in Celsius.
		/// When specified, only forecasts with temperatures at or below this value will be returned.
		/// </summary>
		/// <value>An optional maximum temperature in Celsius for filtering results.</value>
		public int? MaximumTemperature { get; }

		/// <summary>
		/// Initializes a new instance of the GetAllWeatherForecastQuery class with the specified filtering and pagination parameters.
		/// </summary>
		/// <param name="summary">Optional weather summary filter.</param>
		/// <param name="minimumDate">Optional minimum date filter.</param>
		/// <param name="maximumDate">Optional maximum date filter.</param>
		/// <param name="minimumTemperature">Optional minimum temperature filter in Celsius.</param>
		/// <param name="maximumTemperature">Optional maximum temperature filter in Celsius.</param>
		/// <param name="pagination">Pagination settings including page number and page size.</param>
		public GetAllWeatherForecastQuery(
			Summary? summary,
			DateOnly? minimumDate,
			DateOnly? maximumDate,
			int? minimumTemperature,
			int? maximumTemperature,
			Pagination pagination ) {
			Summary = summary;
			MinimumDate = minimumDate;
			MaximumDate = maximumDate;
			MinimumTemperature = minimumTemperature;
			MaximumTemperature = maximumTemperature;
			PageNumber = pagination.PageNumber;
			PageSize = pagination.PageSize;
		}

		/// <summary>
		/// Validates the query parameters to ensure pagination values are valid.
		/// Checks that both page number and page size are greater than zero.
		/// </summary>
		/// <param name="builder">The validation builder used to define validation rules.</param>
		/// <param name="context">Optional validation context for additional validation scenarios.</param>
		public void Validate( ValidationBuilder<GetAllWeatherForecastQuery> builder, ValidationContextKey? context = null ) {
			builder.For( PageNumber, rules => rules
				.GreaterThan( 0 ) );

			builder.For( PageSize, rules => rules
				.GreaterThan( 0 ) );

			builder.For( Summary, rules => rules
				.IsValidNullableEnumValue( ) );

			builder.For( MinimumTemperature, rules => rules
				.GreaterOrEquals( -100 )
				.When( t => t.HasValue ) );

			builder.For( MaximumTemperature, rules => rules
				.LessOrEquals( 100 )
				.When( t => t.HasValue ) );

			builder.For( MinimumDate, rules => rules
				.GreaterThan( DateOnly.MinValue )
				.When( t => t.HasValue ) );

			builder.For( MinimumDate, rules => rules
				.LessOrEquals( DateOnly.MaxValue )
				.When( t => t.HasValue ) );
		}
	}
}