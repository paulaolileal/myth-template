using Microsoft.AspNetCore.Mvc;
using Myth.Extensions;
using Myth.Flow.Actions.Extensions;
using Myth.Interfaces;
using Myth.Interfaces.Results;
using Myth.Template.Application.WeatherForecasts.Commands.Create;
using Myth.Template.Application.WeatherForecasts.Commands.Delete;
using Myth.Template.Application.WeatherForecasts.Commands.Update;
using Myth.Template.Application.WeatherForecasts.DTOs;
using Myth.Template.Application.WeatherForecasts.Events.Created;
using Myth.Template.Application.WeatherForecasts.Queries.GetAll;
using Myth.Template.Application.WeatherForecasts.Queries.GetById;
using Myth.Template.Domain.Models;
using Myth.ValueObjects;

namespace Myth.Template.API.Controllers;

/// <summary>
/// RESTful API controller for managing weather forecast data in the Myth Template application.
/// This controller serves as a comprehensive example of implementing Clean Architecture principles,
/// CQRS pattern, and advanced pipeline processing using the Myth framework.
/// 
/// Features demonstrated in this controller:
/// - Pipeline-based request processing with validation, caching, and logging
/// - CQRS implementation with separate command and query handlers
/// - Event-driven architecture with domain event publishing
/// - Advanced filtering and pagination capabilities
/// - Comprehensive error handling and validation
/// - Async/await patterns with cancellation token support
/// </summary>
/// <remarks>
/// This controller is designed as a template for building robust, scalable APIs using the Myth framework.
/// It demonstrates best practices for:
/// - Request validation using the built-in validation pipeline
/// - Response caching for improved performance
/// - Structured logging with contextual information
/// - Event publishing for loose coupling between bounded contexts
/// - RESTful API design with proper HTTP status codes
/// </remarks>
[Tags( "Weather forecast" )]
[ApiController]
[Route( "api/v1/[controller]" )]
public class WeatherForecastController( IValidator validator, ILogger<WeatherForecastController> logger ) : ControllerBase {

	/// <summary>
	/// Retrieves a paginated collection of weather forecasts with optional filtering capabilities.
	/// </summary>
	/// <param name="summary">Optional filter to retrieve forecasts with a specific weather summary type.</param>
	/// <param name="minimumDate">Optional filter to retrieve forecasts on or after the specified date.</param>
	/// <param name="maximumDate">Optional filter to retrieve forecasts on or before the specified date.</param>
	/// <param name="minimumTemperature">Optional filter to retrieve forecasts with temperature at or above this value (in Celsius).</param>
	/// <param name="maximumTemperature">Optional filter to retrieve forecasts with temperature at or below this value (in Celsius).</param>
	/// <param name="pagination">Pagination parameters including page number and page size for result limiting.</param>
	/// <param name="cancellationToken">Token to monitor for cancellation requests during the asynchronous operation.</param>
	/// <response code="200">Successfully retrieved the paginated weather forecast collection</response>
	/// <response code="400">Invalid request parameters or validation errors</response>
	/// <response code="500">Internal server error occurred during processing</response>
	/// <remarks>
	/// ## Description
	/// This endpoint demonstrates advanced querying with multiple filter parameters, caching,
	/// and comprehensive logging throughout the pipeline execution.
	/// 
	/// ## Notes
	/// This endpoint demonstrates the complete query processing pipeline including validation,
	/// caching capabilities, and structured filtering for weather forecasts.
	///
	/// ## Example
	/// ```http
	/// GET /api/v1/weatherforecast?summary=Sunny&amp;minimumDate=2024-01-01&amp;maximumDate=2024-12-31&amp;pageNumber=1&amp;pageSize=10
	/// ```
	/// </remarks>
	[HttpGet]
	public async Task<IActionResult> GetAsync(
		[FromQuery] Summary? summary,
		[FromQuery] DateOnly? minimumDate,
		[FromQuery] DateOnly? maximumDate,
		[FromQuery] int? minimumTemperature,
		[FromQuery] int? maximumTemperature,
		[FromQuery] Pagination pagination,
		CancellationToken cancellationToken = default ) {
		var result = await PipelineExtensions
			.Start( new GetAllWeatherForecastQuery(
				summary,
				minimumDate,
				maximumDate,
				minimumTemperature,
				maximumTemperature,
				pagination ) )
			.TapAsync( pipeline => validator.ValidateAsync( pipeline.CurrentRequest! ) )
			.Tap( pipeline => logger.LogDebug( "Filters validated with success!" ) )
			.Query<GetAllWeatherForecastQuery, IPaginated<GetWeatherForecastResponse>>( ( query, conf ) => conf.UseCache( ) )
			.Tap( pipeline => logger.LogInformation( "Weather forecast queried with `{Amount}` results", pipeline.CurrentRequest!.Items.Count( ) ) )
			.ExecuteAsync( cancellationToken );


		return Ok( result.Value );

	}

	/// <summary>
	/// Retrieves a specific weather forecast by its unique identifier.
	/// </summary>
	/// <param name="weatherForecastId">The unique identifier (GUID) of the weather forecast to retrieve.</param>
	/// <param name="cancellationToken">Token to monitor for cancellation requests during the asynchronous operation.</param>
	/// <response code="200">Successfully retrieved the weather forecast</response>
	/// <response code="400">Invalid identifier format provided</response>
	/// <response code="404">Weather forecast not found</response>
	/// <response code="500">Internal server error occurred during processing</response>
	/// <remarks>
	/// ## Description
	/// This endpoint demonstrates simple entity retrieval with validation and caching capabilities.
	/// 
	/// ## Notes
	/// This endpoint demonstrates the query processing pipeline for single entity retrieval,
	/// including proper validation and caching mechanisms.
	///
	/// ## Example
	/// ```http
	/// GET /api/v1/weatherforecast/123e4567-e89b-12d3-a456-426614174000
	/// ```
	/// </remarks>
	[HttpGet( "{weatherForecastId}", Name = "GetByIdAsync" )]
	public async Task<IActionResult> GetByIdAsync( [FromRoute] Guid weatherForecastId, CancellationToken cancellationToken = default ) {
		var result = await PipelineExtensions
			.Start( new GetWeatherForecastByIdQuery( weatherForecastId ) )
			.TapAsync( pipeline => validator.ValidateAsync( pipeline.CurrentRequest! ) )
			.Tap( pipeline => logger.LogDebug( "Request validated with success!" ) )
			.Query<GetWeatherForecastByIdQuery, GetWeatherForecastResponse>( )
			.Tap( pipeline => logger.LogInformation( "Weather forecast queried for identifier `{WeatherForecastId}` results", pipeline.CurrentRequest!.WeatherForecastId ) )
			.ExecuteAsync( cancellationToken );

		return Ok( result.Value );

	}

	/// <summary>
	/// Creates a new weather forecast entry in the system.
	/// </summary>
	/// <param name="request">The request object containing the weather forecast data to be created.</param>
	/// <param name="cancellationToken">Token to monitor for cancellation requests during the asynchronous operation.</param>
	/// <response code="201">Weather forecast successfully created</response>
	/// <response code="400">Invalid request data or validation errors</response>
	/// <response code="409">Weather forecast for the specified date already exists</response>
	/// <response code="500">Internal server error occurred during processing</response>
	/// <remarks>
	/// ## Description
	/// This endpoint demonstrates the complete command processing pipeline including validation,
	/// business logic execution, event publishing, and proper RESTful response creation.
	/// 
	/// ## Notes
	/// This endpoint demonstrates the command processing pipeline for entity creation,
	/// including validation, business rule enforcement, event publishing, and proper location header response.
	///
	/// ## Example
	/// ```json
	/// POST /api/v1/weatherforecast
	/// {
	///   "date": "2024-01-15",
	///   "temperatureC": 25,
	///   "summary": "Sunny"
	/// }
	/// ```
	/// </remarks>
	[HttpPost]
	public async Task<IActionResult> PostAsync( [FromBody] CreateWeatherForecastRequest request, CancellationToken cancellationToken = default ) {
		var result = await PipelineExtensions
			.Start( request.To<CreateWeatherForecastCommand>( ) )
			.TapAsync( pipeline => validator.ValidateAsync( pipeline.CurrentRequest! ) )
			.Tap( pipeline => logger.LogDebug( "Request validated with success!" ) )
			.Process<CreateWeatherForecastCommand, Guid>( )
			.Tap( pipeline => logger.LogInformation( "Command runned with success" ) )
			.Transform( result => new WeatherForecastCreatedEvent( result ) )
			.Publish( )
			.Tap( pipeline => logger.LogDebug( "Event published with success!" ) )
			.ExecuteAsync( cancellationToken );


		return CreatedAtRoute(
			nameof( GetByIdAsync ),
			new {
				weatherForecastId = result.Value.WeatherForecastId
			},
			result.Value );

	}

	/// <summary>
	/// Updates an existing weather forecast with new data.
	/// </summary>
	/// <param name="request">The request object containing the updated weather forecast data including the identifier.</param>
	/// <param name="cancellationToken">Token to monitor for cancellation requests during the asynchronous operation.</param>
	/// <response code="204">Weather forecast successfully updated</response>
	/// <response code="400">Invalid request data or validation errors</response>
	/// <response code="404">Weather forecast not found for update</response>
	/// <response code="500">Internal server error occurred during processing</response>
	/// <remarks>
	/// ## Description
	/// This endpoint demonstrates the command processing pipeline for entity modifications,
	/// including validation and business rule enforcement.
	/// 
	/// ## Notes
	/// This endpoint demonstrates the command processing pipeline for entity updates,
	/// including proper validation and business rule enforcement.
	///
	/// ## Example
	/// ```json
	/// PUT /api/v1/weatherforecast
	/// {
	///   "weatherForecastId": "123e4567-e89b-12d3-a456-426614174000",
	///   "temperatureC": 30,
	///   "summary": "Hot"
	/// }
	/// ```
	/// </remarks>
	[HttpPut]
	public async Task<IActionResult> PutAsync( [FromBody] UpdateWeatherForecastRequest request, CancellationToken cancellationToken = default ) {
		var result = await PipelineExtensions
			.Start( request.To<UpdateWeatherForecastCommand>( ) )
			.TapAsync( pipeline => validator.ValidateAsync( pipeline.CurrentRequest! ) )
			.Tap( pipeline => logger.LogDebug( "Request validated with success!" ) )
			.Process( )
			.Tap( pipeline => logger.LogInformation( "Command runned with success" ) )
			.ExecuteAsync( cancellationToken );

		return NoContent( );
	}

	/// <summary>
	/// Deletes an existing weather forecast from the system.
	/// </summary>
	/// <param name="request">The request object containing the identifier of the weather forecast to be deleted.</param>
	/// <param name="cancellationToken">Token to monitor for cancellation requests during the asynchronous operation.</param>
	/// <response code="204">Weather forecast successfully deleted</response>
	/// <response code="400">Invalid request data or validation errors</response>
	/// <response code="404">Weather forecast not found for deletion</response>
	/// <response code="500">Internal server error occurred during processing</response>
	/// <remarks>
	/// ## Description
	/// This endpoint handles the deletion of a weather forecast entity identified by its unique identifier.
	/// 
	/// ## Notes
	/// This endpoint demonstrates the command processing pipeline for entity removal,
	/// including proper validation and business rule enforcement.
	///
	/// ## Example
	/// ```json
	/// DELETE /api/v1/weatherforecast
	/// {
	///   "weatherForecastId": "123e4567-e89b-12d3-a456-426614174000"
	/// }
	/// ```
	/// </remarks>
	[HttpDelete]
	public async Task<IActionResult> DeleteAsync( [FromBody] DeleteWeatherForecastRequest request, CancellationToken cancellationToken = default ) {
		var result = await PipelineExtensions
			.Start( request.To<DeleteWeatherForecastCommand>( ) )
			.TapAsync( pipeline => validator.ValidateAsync( pipeline.CurrentRequest! ) )
			.Tap( pipeline => logger.LogDebug( "Request validated with success!" ) )
			.Process( )
			.Tap( pipeline => logger.LogInformation( "Command runned with success" ) )
			.ExecuteAsync( cancellationToken );

		return NoContent( );
	}
}
