using Microsoft.AspNetCore.Mvc;
using Myth.Extensions;
using Myth.Flow.Actions.Extensions;
using Myth.Guard;
using Myth.Interfaces;
using Myth.Interfaces.Results;
using Myth.Template.Application.WeatherStations.Commands.Activate;
using Myth.Template.Application.WeatherStations.Commands.Create;
using Myth.Template.Application.WeatherStations.Commands.CreateWithForecasts;
using Myth.Template.Application.WeatherStations.Commands.Decommission;
using Myth.Template.Application.WeatherStations.Commands.Delete;
using Myth.Template.Application.WeatherStations.Commands.Update;
using Myth.Template.Application.WeatherStations.DTOs;
using Myth.Template.Application.WeatherStations.Queries.GetAll;
using Myth.Template.Application.WeatherStations.Queries.GetById;
using Myth.Validation;
using Myth.ValueObjects;

namespace Myth.Template.API.Controllers;

/// <summary>
/// RESTful API controller for managing weather station data in the Myth Template application.
/// Extends the patterns shown in <see cref="WeatherForecastController"/> with advanced Myth features
/// including parallel validation, conditional pipeline branches, lifecycle-scoped validation contexts,
/// and transactional savepoints.
/// </summary>
/// <remarks>
/// ## Myth Features introduced beyond WeatherForecastController
/// - <c>Validate.All()</c>: concurrent independent field validation with aggregated errors
/// - <c>Sentry.For()</c>: standalone field validation builder with custom predicates
/// - <c>.When()</c>: conditional pipeline branch based on runtime state
/// - <c>ValidationContextKey.Activate</c>: predefined non-CRUD lifecycle context
/// - <c>ValidationContextKey.Custom("Decommission")</c>: domain-specific custom context key
/// - <c>IUnitOfWorkRepository</c>: transaction management with named savepoints
/// </remarks>
[Tags( "Weather station" )]
[ApiController]
[Route( "api/v1/[controller]" )]
public class WeatherStationController( IValidator validator, ILogger<WeatherStationController> logger ) : ControllerBase {

	/// <summary>
	/// Retrieves a paginated list of weather stations with optional filtering and conditional forecast inclusion.
	/// </summary>
	/// <param name="name">Optional name substring filter.</param>
	/// <param name="isActive">Optional active-status filter.</param>
	/// <param name="withForecasts">
	/// When <c>true</c>, logs that forecast data would be included.
	/// Drives the <c>.When()</c> conditional step in the pipeline.
	/// </param>
	/// <param name="pagination">Page number and page size.</param>
	/// <param name="cancellationToken">Token to monitor for cancellation requests during the asynchronous operation.</param>
	/// <response code="200">Successfully retrieved the paginated station collection.</response>
	/// <response code="400">Invalid pagination parameters.</response>
	/// <response code="500">Internal server error occurred during processing.</response>
	/// <remarks>
	/// ## Description
	/// This endpoint demonstrates parallel pagination validation and conditional pipeline branching,
	/// extending the basic pipeline pattern shown in <see cref="WeatherForecastController.GetAsync"/>.
	///
	/// ## Myth Features used here
	/// - `Myth.Guard`: `Validate.All()` — runs multiple independent validations concurrently and aggregates all errors before throwing
	/// - `Myth.Guard`: `Sentry.For()` — creates a standalone validation builder scoped to a single field with custom `Respect()` predicates
	/// - `Myth.Flow`: `.When()` — executes a conditional pipeline branch only when `withForecasts` is `true`, without interrupting the main flow
	/// - `Myth.Flow.Actions`: `.Query&lt;T, R&gt;()` with `UseCache()` — dispatches the query and activates response caching
	///
	/// ## Notes
	/// `Validate.All()` validates `pageNumber` and `pageSize` in parallel — both rules run concurrently
	/// and all errors are aggregated before throwing, providing better feedback than sequential validation.
	///
	/// ## Example
	/// ```http
	/// GET /api/v1/weatherstation?name=North&amp;isActive=true&amp;withForecasts=false&amp;pageNumber=1&amp;pageSize=20
	/// ```
	/// </remarks>
	[HttpGet]
	public async Task<IActionResult> GetAsync(
		[FromQuery] string? name,
		[FromQuery] bool? isActive,
		[FromQuery] Pagination pagination,
		[FromQuery] bool withForecasts = false,
		CancellationToken cancellationToken = default ) {

		var result = await PipelineExtensions
			.Start( new GetAllWeatherStationsQuery( name, isActive, withForecasts, pagination ) )
			.TapAsync( async pipeline => {
				// Validate.All(): run pagination validations in parallel — independent rules
				// execute concurrently and all errors are aggregated before throwing.
				// Uses Sentry.For() which returns IStandaloneValidationBuilder<T> with Respect() for custom predicates.
				var pageNumber = pipeline.CurrentRequest!.PageNumber;
				var pageSize = pipeline.CurrentRequest!.PageSize;
				await Validate.All( )
					.Add( ( ) => Sentry.For( pageNumber, nameof( Pagination.PageNumber ) )
						.Respect( v => v > 0 )
						.WithMessage( $"'{nameof( Pagination.PageNumber )}' must be greater than 0." ) )
					.Add( ( ) => Sentry.For( pageSize, nameof( Pagination.PageSize ) )
						.Respect( v => v is > 0 and <= 100 )
						.WithMessage( $"'{nameof( Pagination.PageSize )}' must be between 1 and 100." ) )
					.ValidateAndThrowAsync( cancellationToken: cancellationToken );
			} )
			.TapAsync( pipeline => validator.ValidateAsync( pipeline.CurrentRequest! ) )
			.Tap( _ => logger.LogDebug( "Filters validated with success!" ) )
			// .When(): the branch executes only when withForecasts is true — demonstrates
			// conditional pipeline steps without breaking the main execution flow.
			.When(
				pipeline => pipeline.CurrentRequest!.WithForecasts,
				branch => branch.Tap( _ => logger.LogDebug( "Including forecasts in station response" ) ) )
			.Query<GetAllWeatherStationsQuery, IPaginated<GetWeatherStationResponse>>( ( _, conf ) => conf.UseCache( ) )
			.Tap( pipeline => logger.LogInformation( "Weather stations queried with `{Amount}` results", pipeline.CurrentRequest!.Items.Count( ) ) )
			.ExecuteAsync( cancellationToken );

		return Ok( result.Value );
	}

	/// <summary>
	/// Retrieves a specific weather station by its unique identifier.
	/// </summary>
	/// <param name="weatherStationId">The unique identifier (GUID) of the weather station to retrieve.</param>
	/// <param name="cancellationToken">Token to monitor for cancellation requests during the asynchronous operation.</param>
	/// <response code="200">Successfully retrieved the weather station.</response>
	/// <response code="400">Invalid identifier format provided.</response>
	/// <response code="404">Weather station not found.</response>
	/// <response code="500">Internal server error occurred during processing.</response>
	/// <remarks>
	/// ## Description
	/// This endpoint retrieves a single weather station by its GUID, demonstrating
	/// the standard query pipeline with validation and caching.
	///
	/// ## Myth Features used here
	/// - `Myth.Flow`: `.TapAsync()` — runs the validator as an async pipeline side effect
	/// - `Myth.Flow`: `.Tap()` — synchronous logger side effect without altering context
	/// - `Myth.Flow.Actions`: `.Query&lt;T, R&gt;()` with `UseCache()` — dispatches the query and activates response caching
	/// - `Myth.Guard`: `IValidator.ValidateAsync()` — fluent Guard validation on the query object
	///
	/// ## Notes
	/// Follows the same single-entity retrieval pattern as
	/// <see cref="WeatherForecastController.GetByIdAsync"/> but targets weather stations.
	///
	/// ## Example
	/// ```http
	/// GET /api/v1/weatherstation/123e4567-e89b-12d3-a456-426614174000
	/// ```
	/// </remarks>
	[HttpGet( "{weatherStationId}", Name = "GetStationByIdAsync" )]
	public async Task<IActionResult> GetByIdAsync(
		[FromRoute] Guid weatherStationId,
		CancellationToken cancellationToken = default ) {

		var result = await PipelineExtensions
			.Start( new GetWeatherStationByIdQuery( weatherStationId ) )
			.TapAsync( pipeline => validator.ValidateAsync( pipeline.CurrentRequest! ) )
			.Tap( _ => logger.LogDebug( "Request validated with success!" ) )
			.Query<GetWeatherStationByIdQuery, GetWeatherStationResponse>( ( _, cache ) => cache.UseCache( ) )
			.Tap( pipeline => logger.LogInformation( "Weather station queried for identifier `{Id}`", pipeline.CurrentRequest!.WeatherStationId ) )
			.ExecuteAsync( cancellationToken );

		return Ok( result.Value );
	}

	/// <summary>
	/// Creates a new weather station in the system.
	/// </summary>
	/// <param name="request">The station creation payload containing name and location.</param>
	/// <param name="cancellationToken">Token to monitor for cancellation requests during the asynchronous operation.</param>
	/// <response code="201">Weather station successfully created.</response>
	/// <response code="400">Invalid request data or validation errors.</response>
	/// <response code="409">A station with this name already exists.</response>
	/// <response code="500">Internal server error occurred during processing.</response>
	/// <remarks>
	/// ## Description
	/// This endpoint creates a new weather station, demonstrating context-specific validation
	/// using <c>ValidationContextKey.Create</c> to enforce uniqueness rules only at creation time.
	///
	/// ## Myth Features used here
	/// - `Myth.Morph`: `.To&lt;T&gt;()` — transforms the request DTO into a command object using the morphing pipeline
	/// - `Myth.Guard`: `ValidationContextKey.Create` — activates Create-scoped rules such as the async name uniqueness check
	/// - `Myth.Flow.Actions`: `.Process&lt;TCommand, TResponse&gt;()` — dispatches the command and returns the new station's GUID
	///
	/// ## Notes
	/// The uniqueness check on `Name` runs only in the `Create` context — it is defined with
	/// `builder.InContext(ValidationContextKey.Create, ...)` inside the `WeatherStation` domain entity.
	///
	/// ## Example
	/// ```json
	/// POST /api/v1/weatherstation
	/// {
	///   "name": "North Ridge Station",
	///   "location": "Lat 45.1234, Lon -93.5678"
	/// }
	/// ```
	/// </remarks>
	[HttpPost]
	public async Task<IActionResult> PostAsync( [FromBody] CreateWeatherStationRequest request, CancellationToken cancellationToken = default ) {
		var result = await PipelineExtensions
			.Start( request.To<CreateWeatherStationCommand>( ) )
			.TapAsync( pipeline => validator.ValidateAsync( pipeline.CurrentRequest!, ValidationContextKey.Create ) )
			.Tap( _ => logger.LogDebug( "Request validated with success!" ) )
			.Process<CreateWeatherStationCommand, Guid>( )
			.Tap( _ => logger.LogInformation( "Weather station created with success" ) )
			.ExecuteAsync( cancellationToken );

		return CreatedAtRoute(
			"GetStationByIdAsync",
			new { weatherStationId = result.Value },
			result.Value );
	}

	/// <summary>
	/// Updates an existing weather station's name and location.
	/// </summary>
	/// <param name="weatherStationId">The identifier of the station to update.</param>
	/// <param name="request">The update payload containing the new name and location.</param>
	/// <param name="cancellationToken">Token to monitor for cancellation requests during the asynchronous operation.</param>
	/// <response code="204">Weather station successfully updated.</response>
	/// <response code="400">Invalid request data or validation errors.</response>
	/// <response code="404">Weather station not found.</response>
	/// <response code="500">Internal server error occurred during processing.</response>
	/// <remarks>
	/// ## Description
	/// This endpoint updates the display name and geographic location of an existing weather station,
	/// demonstrating the standard command pipeline pattern with validation.
	///
	/// ## Myth Features used here
	/// - `Myth.Flow`: `.TapAsync()` — runs the validator as an async pipeline side effect
	/// - `Myth.Flow`: `.Tap()` — synchronous logger side effects without altering context
	/// - `Myth.Flow.Actions`: `.Process()` — dispatches the update command without a typed response
	/// - `Myth.Guard`: `IValidator.ValidateAsync()` — fluent validation on the command object
	///
	/// ## Notes
	/// Uses the default validation context (no specific context key), applying only
	/// global rules defined in the `WeatherStation` domain entity.
	///
	/// ## Example
	/// ```json
	/// PUT /api/v1/weatherstation/123e4567-e89b-12d3-a456-426614174000
	/// {
	///   "name": "Updated Station Name",
	///   "location": "Lat 46.0000, Lon -94.0000"
	/// }
	/// ```
	/// </remarks>
	[HttpPut( "{weatherStationId}" )]
	public async Task<IActionResult> PutAsync( [FromRoute] Guid weatherStationId, [FromBody] UpdateWeatherStationRequest request, CancellationToken cancellationToken = default ) {
		var result = await PipelineExtensions
			.Start( new UpdateWeatherStationCommand( weatherStationId, request.Name, request.Location ) )
			.TapAsync( pipeline => validator.ValidateAsync( pipeline.CurrentRequest! ) )
			.Tap( _ => logger.LogDebug( "Request validated with success!" ) )
			.Process( )
			.Tap( _ => logger.LogInformation( "Weather station updated with success" ) )
			.ExecuteAsync( cancellationToken );

		return NoContent( );
	}

	/// <summary>
	/// Permanently removes a weather station from the system.
	/// </summary>
	/// <param name="weatherStationId">The GUID identifier of the weather station to remove.</param>
	/// <param name="cancellationToken">Token to monitor for cancellation requests during the asynchronous operation.</param>
	/// <response code="204">Weather station successfully deleted.</response>
	/// <response code="400">Invalid identifier format provided.</response>
	/// <response code="404">Weather station not found.</response>
	/// <response code="500">Internal server error occurred during processing.</response>
	/// <remarks>
	/// ## Description
	/// This endpoint permanently removes a weather station identified by its GUID,
	/// demonstrating the standard delete command pipeline.
	///
	/// ## Myth Features used here
	/// - `Myth.Flow`: `.TapAsync()` — runs the validator as an async pipeline side effect
	/// - `Myth.Flow`: `.Tap()` — synchronous logger side effects
	/// - `Myth.Flow.Actions`: `.Process()` — dispatches the delete command
	/// - `Myth.Guard`: `IValidator.ValidateAsync()` — fluent validation on the command object
	///
	/// ## Notes
	/// Associated forecasts remain in the database with their `WeatherStationId` set to `null`
	/// due to the `SetNull` delete behavior configured in `WeatherStationMap`.
	///
	/// ## Example
	/// ```http
	/// DELETE /api/v1/weatherstation/123e4567-e89b-12d3-a456-426614174000
	/// ```
	/// </remarks>
	[HttpDelete( "{weatherStationId}" )]
	public async Task<IActionResult> DeleteAsync( [FromRoute] Guid weatherStationId, CancellationToken cancellationToken = default ) {
		var result = await PipelineExtensions
			.Start( new DeleteWeatherStationCommand( weatherStationId ) )
			.TapAsync( pipeline => validator.ValidateAsync( pipeline.CurrentRequest! ) )
			.Tap( _ => logger.LogDebug( "Request validated with success!" ) )
			.Process( )
			.Tap( _ => logger.LogInformation( "Weather station deleted with success" ) )
			.ExecuteAsync( cancellationToken );

		return NoContent( );
	}

	/// <summary>
	/// Activates an inactive weather station, allowing it to accept new forecast data.
	/// </summary>
	/// <param name="weatherStationId">The identifier of the station to activate.</param>
	/// <param name="cancellationToken">Token to monitor for cancellation requests during the asynchronous operation.</param>
	/// <response code="204">Weather station successfully activated.</response>
	/// <response code="400">Station is already active.</response>
	/// <response code="404">Weather station not found.</response>
	/// <response code="500">Internal server error occurred during processing.</response>
	/// <remarks>
	/// ## Description
	/// This endpoint transitions a station to the active state using a predefined, non-CRUD validation
	/// context to enforce lifecycle rules that must not apply to other operations.
	///
	/// ## Myth Features used here
	/// - `Myth.Guard`: `ValidationContextKey.Activate` — predefined non-CRUD context key that activates lifecycle-specific rules (e.g., station must currently be inactive)
	/// - `Myth.Flow`: `.TapAsync()` — runs the validator as an async pipeline side effect
	/// - `Myth.Flow`: `.Tap()` — synchronous logger side effects
	/// - `Myth.Flow.Actions`: `.Process()` — dispatches the activate command
	///
	/// ## Notes
	/// The rule `IsActive.IsFalse()` is scoped exclusively to `ValidationContextKey.Activate`
	/// in the `WeatherStation` domain entity, ensuring it never interferes with other operations.
	///
	/// ## Example
	/// ```http
	/// POST /api/v1/weatherstation/123e4567-e89b-12d3-a456-426614174000/activate
	/// ```
	/// </remarks>
	[HttpPost( "{weatherStationId}/activate" )]
	public async Task<IActionResult> ActivateAsync( [FromRoute] Guid weatherStationId, CancellationToken cancellationToken = default ) {
		var result = await PipelineExtensions
			.Start( new ActivateWeatherStationCommand( weatherStationId ) )
			.TapAsync( pipeline => validator.ValidateAsync( pipeline.CurrentRequest! ) )
			.Tap( _ => logger.LogDebug( "Request validated with success!" ) )
			.Process( )
			.Tap( _ => logger.LogInformation( "Weather station activated with success" ) )
			.ExecuteAsync( cancellationToken );

		return NoContent( );
	}

	/// <summary>
	/// Decommissions (permanently deactivates) a weather station, preventing future forecast entries.
	/// </summary>
	/// <param name="weatherStationId">The identifier of the station to decommission.</param>
	/// <param name="cancellationToken">Token to monitor for cancellation requests during the asynchronous operation.</param>
	/// <response code="204">Weather station successfully decommissioned.</response>
	/// <response code="400">Station is already inactive.</response>
	/// <response code="404">Weather station not found.</response>
	/// <response code="500">Internal server error occurred during processing.</response>
	/// <remarks>
	/// ## Description
	/// This endpoint permanently deactivates a weather station using a fully custom validation context,
	/// demonstrating how to define domain-specific lifecycle contexts beyond the predefined CRUD set.
	///
	/// ## Myth Features used here
	/// - `Myth.Guard`: `ValidationContextKey.Custom("Decommission")` — domain-specific context key defined as `WeatherStation.Decommission` in the domain entity, eliminating magic strings at the call site
	/// - `Myth.Flow`: `.TapAsync()` — runs the validator as an async pipeline side effect
	/// - `Myth.Flow`: `.Tap()` — synchronous logger side effects
	/// - `Myth.Flow.Actions`: `.Process()` — dispatches the decommission command
	///
	/// ## Notes
	/// `WeatherStation.Decommission = ValidationContextKey.Custom("Decommission")` is a static field
	/// on the domain entity — the key is strongly-typed and discoverable without relying on raw strings.
	///
	/// ## Example
	/// ```http
	/// POST /api/v1/weatherstation/123e4567-e89b-12d3-a456-426614174000/decommission
	/// ```
	/// </remarks>
	[HttpPost( "{weatherStationId}/decommission" )]
	public async Task<IActionResult> DecommissionAsync( [FromRoute] Guid weatherStationId, CancellationToken cancellationToken = default ) {
		var result = await PipelineExtensions
			.Start( new DecommissionWeatherStationCommand( weatherStationId ) )
			.TapAsync( pipeline => validator.ValidateAsync( pipeline.CurrentRequest! ) )
			.Tap( _ => logger.LogDebug( "Request validated with success!" ) )
			.Process( )
			.Tap( _ => logger.LogInformation( "Weather station decommissioned with success" ) )
			.ExecuteAsync( cancellationToken );

		return NoContent( );
	}

	/// <summary>
	/// Creates a new weather station together with its initial forecast entries in a single atomic transaction.
	/// </summary>
	/// <param name="request">The station and initial forecast data.</param>
	/// <param name="cancellationToken">Token to monitor for cancellation requests during the asynchronous operation.</param>
	/// <response code="201">Weather station and forecasts successfully created.</response>
	/// <response code="400">Invalid request data or validation errors.</response>
	/// <response code="409">A station with this name already exists.</response>
	/// <response code="500">Internal server error occurred during processing.</response>
	/// <remarks>
	/// ## Description
	/// This endpoint creates a weather station and its initial forecasts atomically,
	/// demonstrating transactional control with named savepoints for fine-grained rollback.
	///
	/// ## Myth Features used here
	/// - `Myth.Repository.EntityFramework`: `IUnitOfWorkRepository` — wraps the operation in a transaction via `BeginTransactionAsync()` and `CommitAsync()`
	/// - `Myth.Repository.EntityFramework`: `CreateSavepointAsync("after_station")` — sets a named savepoint after station creation for partial rollback without aborting the full transaction
	/// - `Myth.Guard`: `ValidationContextKey.Create` — activates Create-scoped rules including the async name uniqueness check
	/// - `Myth.Flow.Actions`: `.Process&lt;TCommand, TResponse&gt;()` — dispatches the command and returns the new station's GUID
	///
	/// ## Notes
	/// The In-Memory EF provider silently ignores transactions and savepoints — this pattern is
	/// fully effective with SQL Server or PostgreSQL in production environments.
	///
	/// ## Example
	/// ```json
	/// POST /api/v1/weatherstation/with-forecasts
	/// {
	///   "name": "South Bay Station",
	///   "location": "Lat -33.8688, Lon 151.2093",
	///   "forecasts": [
	///     { "date": "2024-01-15", "temperatureC": 28, "summary": "Sunny" }
	///   ]
	/// }
	/// ```
	/// </remarks>
	[HttpPost( "with-forecasts" )]
	public async Task<IActionResult> PostWithForecastsAsync( [FromBody] CreateWeatherStationWithForecastsCommand request, CancellationToken cancellationToken = default ) {
		var result = await PipelineExtensions
			.Start( request )
			.TapAsync( pipeline => validator.ValidateAsync( pipeline.CurrentRequest!, ValidationContextKey.Create ) )
			.Tap( _ => logger.LogDebug( "Request validated with success!" ) )
			.Process<CreateWeatherStationWithForecastsCommand, Guid>( )
			.Tap( _ => logger.LogInformation( "Weather station with forecasts created with success" ) )
			.ExecuteAsync( cancellationToken );

		return CreatedAtRoute(
			"GetStationByIdAsync",
			new { weatherStationId = result.Value },
			result.Value );
	}
}
