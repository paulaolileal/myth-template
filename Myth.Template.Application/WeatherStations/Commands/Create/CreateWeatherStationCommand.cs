using System.Net;
using Microsoft.Extensions.DependencyInjection;
using Myth.Builder;
using Myth.Guard;
using Myth.Interfaces;
using Myth.ServiceProvider;
using Myth.Specifications;
using Myth.Template.Data.Resources;
using Myth.Template.Domain.Interfaces;
using Myth.Template.Domain.Models;
using Myth.Template.Domain.Specifications;

namespace Myth.Template.Application.WeatherStations.Commands.Create;

/// <summary>
/// Command for creating a new weather station.
/// Uses <see cref="ValidationContextKey.Create"/> for an async uniqueness check on <c>Name</c>,
/// demonstrating context-specific async business rule validation.
/// </summary>
public record CreateWeatherStationCommand : ICommand<Guid>, IValidatable<CreateWeatherStationCommand> {

	/// <summary>Gets the display name for the new station.</summary>
	public string Name { get; init; } = null!;

	/// <summary>Gets the geographic location for the new station.</summary>
	public string Location { get; init; } = null!;

	public void Validate( ValidationBuilder<CreateWeatherStationCommand> builder, ValidationContextKey? context = null ) {
		builder.For( Name, rules => rules
			.NotEmpty( )
			.MinLength( 2 )
			.MaxLength( 100 ) );

		builder.For( Location, rules => rules
			.NotEmpty( )
			.MaxLength( 200 ) );

		builder.InContext( ValidationContextKey.Create, b => {
			b.For( Name, rules => rules
				.RespectAsync( async ( name, ct, sp ) => {
					var repository = sp.GetRequiredService<IScopedService<IWeatherStationRepository>>( );
					var spec = SpecBuilder<WeatherStation>.Create( ).WithNameNotInUse( name );
					return !await repository.ExecuteAsync( r => r.AnyAsync( spec, ct ) );
				} )
				.WithStatusCode( HttpStatusCode.Conflict )
				.WithMessage( _ => string.Format( Messages.Conflict, "Name" ) ) );
		} );
	}
}
