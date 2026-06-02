using Myth.Extensions;
using Myth.Interfaces;
using Myth.Interfaces.Results;
using Myth.Models;
using Myth.Specifications;
using Myth.Template.Application.WeatherStations.DTOs;
using Myth.Template.Domain.Interfaces;
using Myth.Template.Domain.Models;
using Myth.Template.Domain.Specifications;

namespace Myth.Template.Application.WeatherStations.Queries.GetAll;

/// <summary>
/// Handles retrieval of a paginated list of weather stations.
/// Ordering is by name ascending; optional filters for name substring and active status.
/// </summary>
/// <param name="stationRepository">Repository for weather station data access.</param>
public class GetAllWeatherStationsQueryHandler( IWeatherStationRepository stationRepository )
	: IQueryHandler<GetAllWeatherStationsQuery, IPaginated<GetWeatherStationResponse>> {

	public async Task<QueryResult<IPaginated<GetWeatherStationResponse>>> HandleAsync(
		GetAllWeatherStationsQuery query,
		CancellationToken cancellationToken = default ) {

		var spec = SpecBuilder<WeatherStation>
			.Create( )
			.WithNameContains( query.Name )
			.AndIf( query.IsActive.HasValue, x => x.IsActive == query.IsActive )
			.Order( x => x.Name )
			.WithPagination( query );

		var result = await stationRepository.SearchPaginatedAsync( spec, cancellationToken );

		var response = result.To<IPaginated<GetWeatherStationResponse>>( );

		return QueryResult<IPaginated<GetWeatherStationResponse>>.Success( response );
	}
}
