using System.Net;
using Microsoft.Extensions.DependencyInjection;
using Myth.Builder;
using Myth.Guard;
using Myth.Interfaces;
using Myth.ServiceProvider;
using Myth.Template.Data.Resources;
using Myth.Template.Domain.Interfaces;

namespace Myth.Template.Application.WeatherStations.Commands.Update;

/// <summary>
/// Command for updating an existing weather station's name and location.
/// </summary>
public record UpdateWeatherStationCommand : ICommand, IValidatable<UpdateWeatherStationCommand> {

	/// <summary>Gets the identifier of the station to update.</summary>
	public Guid WeatherStationId { get; init; }

	/// <summary>Gets the new display name for the station.</summary>
	public string Name { get; init; } = null!;

	/// <summary>Gets the new geographic location for the station.</summary>
	public string Location { get; init; } = null!;

	public UpdateWeatherStationCommand( Guid weatherStationId, string name, string location ) {
		WeatherStationId = weatherStationId;
		Name = name;
		Location = location;
	}

	public void Validate( ValidationBuilder<UpdateWeatherStationCommand> builder, ValidationContextKey? context = null ) {
		builder.For( WeatherStationId, rules => rules
			.NotDefault( )
			.SetStopOnFailure( )
			.RespectAsync( ( id, ct, sp ) => sp
				.GetRequiredService<IScopedService<IWeatherStationRepository>>( )
				.ExecuteAsync( repo => repo.AnyAsync( x => x.WeatherStationId == id, ct ) ) )
			.WithMessage( value => string.Format( Messages.NotFound, value ) )
			.WithStatusCode( HttpStatusCode.NotFound ) );

		builder.For( Name, rules => rules
			.NotEmpty( )
			.MinLength( 2 )
			.MaxLength( 100 ) );

		builder.For( Location, rules => rules
			.NotEmpty( )
			.MaxLength( 200 ) );
	}
}
