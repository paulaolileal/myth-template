using Myth.Extensions;
using Myth.Interfaces;
using Myth.Interfaces.Results;
using Myth.Models;
using Myth.ServiceProvider;
using Myth.Specifications;
using Myth.Template.Application.WeatherForecasts.DTOs;
using Myth.Template.Domain.Interfaces;
using Myth.Template.Domain.Models;
using Myth.Template.Domain.Specifications;

namespace Myth.Template.Application.WeatherForecasts.Queries.GetAll;

/// <summary>
/// Query handler responsible for processing GetAllWeatherForecastQuery requests.
/// Retrieves a paginated collection of weather forecasts based on various filtering criteria
/// and transforms them into response DTOs with pagination information.
/// </summary>
public class GetAllWeatherForecastsQueryHandler : IQueryHandler<GetAllWeatherForecastQuery, IPaginated<GetWeatherForecastResponse>> {
	/// <summary>
	/// Scoped service wrapper for the weather forecast repository to ensure proper dependency management.
	/// </summary>
	private readonly IScopedService<IWeatherForecastRepository> _weatherForecastRepository;

	/// <summary>
	/// Initializes a new instance of the GetAllWeatherForecastsQueryHandler class.
	/// </summary>
	/// <param name="weatherForecastRepository">Scoped service for accessing the weather forecast repository.</param>
	public GetAllWeatherForecastsQueryHandler( IScopedService<IWeatherForecastRepository> weatherForecastRepository ) {
		_weatherForecastRepository = weatherForecastRepository;
	}

	/// <summary>
	/// Handles the execution of a GetAllWeatherForecastQuery by building a specification with the provided filters,
	/// retrieving paginated results from the repository, and transforming them into response DTOs.
	/// Results are ordered by date in descending order (most recent first).
	/// </summary>
	/// <param name="query">The query containing filtering criteria, pagination settings, and ordering preferences.</param>
	/// <param name="cancellationToken">Token to monitor for cancellation requests during the operation.</param>
	/// <returns>
	/// A task that represents the asynchronous operation. The task result contains a QueryResult
	/// with a paginated collection of weather forecast response DTOs matching the specified criteria.
	/// </returns>
	public async Task<QueryResult<IPaginated<GetWeatherForecastResponse>>> HandleAsync( GetAllWeatherForecastQuery query, CancellationToken cancellationToken = default ) {
		var spec = SpecBuilder<WeatherForecast>
			.Create( )
			.WithSummary( query.Summary )
			.WithDateGreaterThan( query.MinimumDate )
			.WithDateLowerThan( query.MaximumDate )
			.WithTemparatureGreaterThan( query.MinimumTemperature )
			.WithTemparatureLowerThan( query.MaximumTemperature )
			.OrderDescending( x => x.Date )
			.WithPagination( query );

		var result = await _weatherForecastRepository.ExecuteAsync( x => x.SearchPaginatedAsync( spec, cancellationToken ) );

		var response = result.To<IPaginated<GetWeatherForecastResponse>>( );

		return QueryResult<IPaginated<GetWeatherForecastResponse>>.Success( response );
	}
}
