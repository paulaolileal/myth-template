using Myth.Builder;
using Myth.Guard;
using Myth.Interfaces;
using Myth.Interfaces.Results;
using Myth.Template.Application.WeatherStations.DTOs;
using Myth.ValueObjects;

namespace Myth.Template.Application.WeatherStations.Queries.GetAll;

/// <summary>
/// Query for retrieving a paginated list of weather stations with optional filtering.
/// The <c>WithForecasts</c> flag is used in the controller's <c>.When()</c> pipeline step
/// to conditionally log additional context before dispatching the query.
/// </summary>
public class GetAllWeatherStationsQuery : Pagination, IQuery<IPaginated<GetWeatherStationResponse>>, IValidatable<GetAllWeatherStationsQuery> {

	/// <summary>Gets an optional name filter (substring match).</summary>
	public string? Name { get; }

	/// <summary>Gets an optional active-status filter.</summary>
	public bool? IsActive { get; }

	/// <summary>
	/// Gets whether the response should include forecast entries for each station.
	/// When true, the controller pipeline logs a debug message via <c>.When()</c>.
	/// </summary>
	public bool WithForecasts { get; }

	public GetAllWeatherStationsQuery( string? name, bool? isActive, bool withForecasts, Pagination pagination ) {
		Name = name;
		IsActive = isActive;
		WithForecasts = withForecasts;
		PageNumber = pagination.PageNumber;
		PageSize = pagination.PageSize;
	}

	public void Validate( ValidationBuilder<GetAllWeatherStationsQuery> builder, ValidationContextKey? context = null ) {
		builder.For( PageNumber, rules => rules.GreaterThan( 0 ) );
		builder.For( PageSize, rules => rules.GreaterThan( 0 ) );
	}
}
