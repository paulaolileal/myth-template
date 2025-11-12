using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Myth.Template.ExternalData.Breweries.DTOs;

namespace Myth.Template.ExternalData.Breweries.Interfaces;

/// <summary>
/// Repository interface for accessing brewery data from external sources.
/// Provides methods for retrieving brewery information, typically from third-party APIs.
/// </summary>
public interface IBreweryRepository {
	/// <summary>
	/// Retrieves a random brewery from the external data source.
	/// This method typically calls a third-party API to get brewery information.
	/// </summary>
	/// <param name="cancellationToken">Token to monitor for cancellation requests during the operation.</param>
	/// <returns>
	/// A task that represents the asynchronous operation. The task result contains
	/// a BreweryResponseDto with information about a randomly selected brewery.
	/// </returns>
	Task<BreweryResponseDto> GetRandomBreweryAsync( CancellationToken cancellationToken );
}
