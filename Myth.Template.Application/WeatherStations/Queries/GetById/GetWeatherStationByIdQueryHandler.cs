using Myth.Extensions;
using Myth.Interfaces;
using Myth.Models;
using Myth.Specifications;
using Myth.Template.Application.WeatherStations.DTOs;
using Myth.Template.Domain.Interfaces;
using Myth.Template.Domain.Models;
using Myth.Template.Domain.Specifications;

namespace Myth.Template.Application.WeatherStations.Queries.GetById;

/// <summary>
/// Handles retrieval of a single weather station by its identifier.
/// </summary>
/// <param name="stationRepository">Repository for weather station data access.</param>
public class GetWeatherStationByIdQueryHandler( IWeatherStationRepository stationRepository )
	: IQueryHandler<GetWeatherStationByIdQuery, GetWeatherStationResponse> {

	public async Task<QueryResult<GetWeatherStationResponse>> HandleAsync(
		GetWeatherStationByIdQuery query,
		CancellationToken cancellationToken = default ) {

		var spec = SpecBuilder<WeatherStation>.Create( ).WithId( query.WeatherStationId );
		var station = await stationRepository.FirstAsync( spec, cancellationToken );

		return QueryResult<GetWeatherStationResponse>.Success( station.To<GetWeatherStationResponse>( ) );
	}
}
