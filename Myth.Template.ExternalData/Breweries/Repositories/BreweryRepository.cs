using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Myth.Exceptions;
using Myth.Guard;
using Myth.Interfaces;
using Myth.Template.ExternalData.Breweries.DTOs;
using Myth.Template.ExternalData.Breweries.Exceptions;
using Myth.Template.ExternalData.Breweries.Interfaces;

namespace Myth.Template.ExternalData.Breweries.Repositories;

/// <summary>
/// Repository implementation for accessing brewery data from external REST APIs.
/// Uses the Myth REST client framework to communicate with brewery data providers.
/// Implements IBreweryRepository interface for brewery data access operations.
/// </summary>
/// <remarks>
/// Initializes a new instance of the BreweryRepository class.
/// </remarks>
/// <param name="restFactory">Factory for creating REST client instances configured with brewery API settings.</param>
[ExcludeFromCodeCoverage]
public class BreweryRepository( IRestFactory restFactory, ILogger<BreweryRepository> logger ) : IBreweryRepository {
	/// <summary>
	/// REST client instance configured for brewery API communications.
	/// </summary>
	private readonly IRestRequest _client = restFactory.Create( "brewery" );

	/// <summary>
	/// Retrieves a random brewery from the external brewery API.
	/// Makes a GET request to the breweries/random endpoint and returns the first brewery from the response.
	/// </summary>
	/// <param name="cancellationToken">Token to monitor for cancellation requests during the operation.</param>
	/// <returns>
	/// A task that represents the asynchronous operation. The task result contains
	/// a BreweryResponseDto with information about a randomly selected brewery.
	/// </returns>
	/// <exception cref="HttpRequestException">Thrown when the API request fails or returns a non-success status code.</exception>
	public async Task<BreweryResponseDto> GetRandomBreweryAsync( CancellationToken cancellationToken ) {
		try {
			var request = await _client
				.DoGet( "breweries/random" )
				.OnResult( res => res.UseTypeForSuccess<IEnumerable<BreweryResponseDto>>( ) )
				.OnError( err => err.ThrowForNonSuccess( ) )
				.BuildAsync( cancellationToken );

			var breweries = request.GetAs<IEnumerable<BreweryResponseDto>>( );

			var validation = await Sentry.For( breweries )
				.NotNull( )
				.NotEmpty( )
				.WithMessage( "No exists available brewery for this weather!" )
				.WithCode( "BAD_WEATHER" )
				.WithStatusCode( StatusCodes.Status424FailedDependency )
				.ValidateAsync( cancellationToken: cancellationToken );

			if ( validation.IsValid )
				logger.LogInformation( "`{BreweriesCount}` breweries found!", breweries.Count( ) );
			else
				logger.LogWarning( "No breweries found, with `{CODE}`: {MESSAGE}", validation.FirstError!.Code, validation.FirstError.Message );

			return breweries.First( );
		} catch ( NonSuccessException exception ) {
			throw new BreweryException( exception.Message, exception );
		}
	}
}
