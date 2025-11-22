using Myth.Repositories.EntityFramework;
using Myth.Template.Data.Contexts;
using Myth.Template.Domain.Interfaces;
using Myth.Template.Domain.Models;

namespace Myth.Template.Data.Repositories;

/// <summary>
/// Repository implementation for managing WeatherForecast entities using Entity Framework.
/// Provides concrete implementation of IWeatherForecastRepository interface with full CRUD operations.
/// Inherits from ReadWriteRepositoryAsync to leverage standard repository patterns and async operations.
/// </summary>
/// <remarks>
/// Initializes a new instance of the WeatherForecastRepository class.
/// </remarks>
/// <param name="context">The Entity Framework database context for weather forecast data operations.</param>
public class WeatherForecastRepository( ForecastContext context ) : ReadWriteRepositoryAsync<WeatherForecast>( context ), IWeatherForecastRepository {
}
