using Microsoft.EntityFrameworkCore;
using Myth.Contexts;

namespace Myth.Template.Data.Contexts;

/// <summary>
/// Entity Framework database context for weather forecast data operations.
/// Inherits from BaseContext to leverage common database functionality and configurations.
/// Manages the connection and mapping for all weather forecast related entities.
/// </summary>
/// <remarks>
/// Initializes a new instance of the ForecastContext class.
/// </remarks>
/// <param name="options">The database context options containing connection string and provider configuration.</param>
public class ForecastContext( DbContextOptions options ) : BaseContext( options ) {
}
