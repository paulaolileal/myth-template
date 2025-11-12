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
public class WeatherForecastRepository : ReadWriteRepositoryAsync<WeatherForecast>, IWeatherForecastRepository {

	/// <summary>
	/// Initializes a new instance of the WeatherForecastRepository class.
	/// </summary>
	/// <param name="context">The Entity Framework database context for weather forecast data operations.</param>
	public WeatherForecastRepository( ForecastContext context ) : base( context ) {
	}
}
