using Myth.Interfaces.Repositories.Base;
using Myth.Template.Domain.Models;

namespace Myth.Template.Domain.Interfaces;

/// <summary>
/// Repository interface for managing weather station entities in the data store.
/// Extends the base read-write repository to provide standard CRUD operations for WeatherStation entities.
/// </summary>
public interface IWeatherStationRepository : IReadWriteRepositoryAsync<WeatherStation> {
}
