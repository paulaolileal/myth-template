using Myth.Extensions;
using Myth.Interfaces;
using Myth.Models;
using Myth.ServiceProvider;
using Myth.Specifications;
using Myth.Template.Application.WeatherForecasts.DTOs;
using Myth.Template.Domain.Interfaces;
using Myth.Template.Domain.Models;
using Myth.Template.Domain.Specifications;

namespace Myth.Template.Application.WeatherForecasts.Queries.GetById;

/// <summary>
/// Query handler responsible for processing GetWeatherForecastQuery requests.
/// Retrieves a single weather forecast by its identifier and transforms it into a response DTO.
/// Uses the repository pattern with specifications to query the data store efficiently.
/// </summary>
public class GetWeatherForecastsByIdQueryHandler : IQueryHandler<GetWeatherForecastByIdQuery, GetWeatherForecastResponse> {
	/// <summary>
	/// Scoped service wrapper for the weather forecast repository to ensure proper dependency management.
	/// </summary>
	private readonly IScopedService<IWeatherForecastRepository> _weatherForecastRepository;

	/// <summary>
	/// Initializes a new instance of the GetWeatherForecastsQueryHandler class.
	/// </summary>
	/// <param name="weatherForecastRepository">Scoped service for accessing the weather forecast repository.</param>
	public GetWeatherForecastsByIdQueryHandler( IScopedService<IWeatherForecastRepository> weatherForecastRepository ) {
		_weatherForecastRepository = weatherForecastRepository;
	}

	/// <summary>
	/// Handles the execution of a GetWeatherForecastQuery by retrieving the specified weather forecast
	/// from the repository and transforming it into a GetWeatherForecastResponse.
	/// </summary>
	/// <param name="query">The query containing the weather forecast identifier to retrieve.</param>
	/// <param name="cancellationToken">Token to monitor for cancellation requests during the operation.</param>
	/// <returns>
	/// A task that represents the asynchronous operation. The task result contains a QueryResult
	/// with the weather forecast response DTO if the forecast is found and retrieved successfully.
	/// </returns>
	public async Task<QueryResult<GetWeatherForecastResponse>> HandleAsync( GetWeatherForecastByIdQuery query, CancellationToken cancellationToken = default ) {
		var spec = SpecBuilder<WeatherForecast>
			.Create( )
			.WithId( query.WeatherForecastId );

		var result = await _weatherForecastRepository.ExecuteAsync( x => x.FirstOrDefaultAsync( spec, cancellationToken ) );

		var response = result!.To<GetWeatherForecastResponse>( );

		return QueryResult<GetWeatherForecastResponse>.Success( response );
	}
}
