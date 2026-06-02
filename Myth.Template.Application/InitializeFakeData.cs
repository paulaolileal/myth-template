using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Hosting;
using Myth.ServiceProvider;
using Myth.Template.Data.Contexts;
using Myth.Template.Domain.Models;

namespace Myth.Template.Application;

/// <summary>
/// Hosted service responsible for seeding the database with mock weather forecast data on startup.
///
/// <para>
/// Demonstrates the use of <see cref="IScopedService{T}"/> from Myth.Commons to safely resolve
/// a scoped <see cref="ForecastContext"/> inside a singleton hosted service — without manually
/// managing <c>IServiceScopeFactory</c> or <c>IServiceScope</c> lifecycle.
/// <c>IScopedService&lt;T&gt;.ExecuteAsync()</c> creates its own scope, executes the operation,
/// and disposes the scope automatically.
/// </para>
/// </summary>
/// <param name="context">
/// Scoped DbContext wrapped by <see cref="IScopedService{T}"/> for safe resolution
/// inside this singleton hosted service.
/// </param>
[ExcludeFromCodeCoverage]
public class InitializeFakeData( IScopedService<ForecastContext> context ) : IHostedService {

	/// <summary>
	/// Seeds 1000 weather forecasts and 20 weather stations on application startup.
	/// All <c>AddRangeAsync</c> and <c>SaveChangesAsync</c> calls share the same
	/// scoped <see cref="ForecastContext"/> instance via <see cref="IScopedService{T}.ExecuteAsync"/>.
	/// Stations are inserted before forecasts so the foreign-key relationship is already
	/// resolvable if a relational provider is in use.
	/// </summary>
	public async Task StartAsync( CancellationToken cancellationToken ) {
		var forecasts = WeatherForecast.GenerateDataAsync( 1000, cancellationToken );
		var stations = WeatherStation.GenerateDataAsync( 20, cancellationToken );

		await context.ExecuteAsync( async ctx => {
			await ctx.Set<WeatherStation>( ).AddRangeAsync( stations, cancellationToken );
			await ctx.Set<WeatherForecast>( ).AddRangeAsync( forecasts, cancellationToken );
			await ctx.SaveChangesAsync( cancellationToken );
		} );
	}

	/// <summary>No cleanup required — seeding is a one-time startup operation.</summary>
	public Task StopAsync( CancellationToken cancellationToken ) => Task.CompletedTask;
}
