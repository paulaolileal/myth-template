using Myth.Repositories.EntityFramework;
using Myth.Template.Data.Contexts;
using Myth.Template.Domain.Interfaces;
using Myth.Template.Domain.Models;

namespace Myth.Template.Data.Repositories;

/// <summary>
/// Entity Framework Core implementation of the weather station repository.
/// Inherits standard CRUD operations from <see cref="ReadWriteRepositoryAsync{WeatherStation}"/>.
/// </summary>
/// <param name="context">The database context for weather data operations.</param>
public class WeatherStationRepository( ForecastContext context )
	: ReadWriteRepositoryAsync<WeatherStation>( context ), IWeatherStationRepository {
}
