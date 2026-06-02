using System.Net;
using Microsoft.Extensions.DependencyInjection;
using Myth.Builder;
using Myth.Guard;
using Myth.Interfaces;
using Myth.ServiceProvider;
using Myth.Template.Application.WeatherStations.DTOs;
using Myth.Template.Data.Resources;
using Myth.Template.Domain.Interfaces;

namespace Myth.Template.Application.WeatherStations.Queries.GetById;

/// <summary>
/// Query for retrieving a single weather station by its identifier.
/// </summary>
public record GetWeatherStationByIdQuery : IQuery<GetWeatherStationResponse>, IValidatable<GetWeatherStationByIdQuery> {

	/// <summary>Gets the identifier of the station to retrieve.</summary>
	public Guid WeatherStationId { get; private set; }

	public GetWeatherStationByIdQuery( Guid weatherStationId ) => WeatherStationId = weatherStationId;

	public void Validate( ValidationBuilder<GetWeatherStationByIdQuery> builder, ValidationContextKey? context = null ) {
		builder.For( WeatherStationId, rules => rules
			.NotDefault( )
			.SetStopOnFailure( )
			.RespectAsync( ( id, ct, sp ) => sp
				.GetRequiredService<IScopedService<IWeatherStationRepository>>( )
				.ExecuteAsync( repo => repo.AnyAsync( x => x.WeatherStationId == id, ct ) ) )
			.WithMessage( value => string.Format( Messages.NotFound, value ) )
			.WithStatusCode( HttpStatusCode.NotFound ) );
	}
}
