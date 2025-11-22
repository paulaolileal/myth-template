using System;
using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Myth.Exceptions;
using Myth.Extensions;
using Myth.Flow.Actions.Extensions;
using Myth.Models.Results;
using Myth.Template.API.Controllers;
using Myth.Template.Application.WeatherForecasts.DTOs;
using Myth.Template.Application.WeatherForecasts.Events.Created;
using Myth.Template.Application.WeatherForecasts.Queries.GetAll;
using Myth.Template.Data.Contexts;
using Myth.Template.Domain.Models;
using Myth.Template.ExternalData.Breweries.Interfaces;
using Myth.Template.Test.Repositories;
using Myth.Testing.Extensions;
using Myth.Testing.Repositories;
using Myth.ValueObjects;

namespace Myth.Template.Test;

/// <summary>
/// Integration test suite for the WeatherForecastController API endpoints.
/// Tests all CRUD operations including validation scenarios, error handling, and business logic.
/// Inherits from BaseDatabaseTests to provide in-memory database testing capabilities.
/// </summary>
public class WeatherForecastTests : BaseDatabaseTests<ForecastContext> {
	/// <summary>
	/// The controller instance under test, initialized with all required dependencies.
	/// </summary>
	private readonly WeatherForecastController _controller;

	/// <summary>
	/// Initializes a new instance of the WeatherForecastTests class.
	/// Sets up the dependency injection container with all required services for testing.
	/// </summary>
	public WeatherForecastTests( ) : base( ) {
		AddServices( ( services ) => {
			services.AddLogging( );
			services.AddRepositories( );
			services.AddScoped<IBreweryRepository, BreweryRepositoryTest>( );
			services.AddUnitOfWorkForContext<ForecastContext>( );
			services.AddScopedServiceProvider( );
			services.AddMorph( );
			services.AddGuard( );
			services.AddFlow( conf => conf
				.UseExceptionFilter<ValidationException>( )
				.UseActions( x => x
					.UseInMemory( )
					.ScanAssemblies(
					typeof( GetAllWeatherForecastsQueryHandler ).Assembly ) )
				);
		} );

		_controller = CreateInstance<WeatherForecastController>( );
	}

	/// <summary>
	/// Creates mock weather forecast data in the test database.
	/// Generates weather forecasts with random data within the specified date range.
	/// </summary>
	/// <param name="minDate">The minimum date for generated forecasts. Defaults to 1000 days before maxDate if null.</param>
	/// <param name="maxDate">The maximum date for generated forecasts. Defaults to current date if null.</param>
	/// <returns>A task that represents the asynchronous mock data creation operation.</returns>
	private async Task MockData( DateOnly? minDate = null, DateOnly? maxDate = null ) {
		maxDate ??= DateOnly.FromDateTime( DateTime.UtcNow );
		minDate ??= maxDate.Value.AddDays( -1000 );
		var amount = maxDate.Value.DayNumber - minDate.Value.DayNumber;

		var data = WeatherForecast.GenerateDataAsync( amount, _cancellationToken );

		await GetContext( )
			.AddRangeAsync( data, CancellationToken.None );

		await SaveChangesAsync( CancellationToken.None );
	}

	/// <summary>
	/// Tests the GetAsync endpoint with valid parameters to ensure it returns paginated weather forecast data.
	/// Verifies proper pagination, data integrity, and response structure.
	/// </summary>
	/// <param name="summary">Optional summary filter ID for testing specific weather conditions.</param>
	/// <param name="minimumDate">Optional minimum date filter in string format.</param>
	/// <param name="maximumDate">Optional maximum date filter in string format.</param>
	/// <param name="minimumTemperature">Optional minimum temperature filter.</param>
	/// <param name="maximumTemperature">Optional maximum temperature filter.</param>
	/// <param name="pageNumber">The page number for pagination testing.</param>
	/// <param name="pageSize">The page size for pagination testing.</param>
	[Theory( )]
	[InlineData( null, null, null, null, null, 1, 100 )]
	[InlineData( null, "2000-01-01", "2025-01-30", -100, 100, 1, 100 )]
	public async Task GetAsync_WithValidData_ShouldReturnValue( string? summary, string? minimumDate, string? maximumDate, int? minimumTemperature, int? maximumTemperature, int pageNumber, int pageSize ) {
		// Arrange
		Pagination pagination = new( pageNumber, pageSize );
		DateOnly? minDate = minimumDate != null
			? DateOnly.Parse( minimumDate )
			: null;
		DateOnly? maxDate = maximumDate != null
			? DateOnly.Parse( maximumDate )
			: null;

		await MockData( minDate, maxDate );

		// Act
		var response = await _controller.GetAsync(
			summary,
			minDate,
			maxDate,
			minimumTemperature,
			maximumTemperature,
			pagination );

		// Assert

		response.Should( ).BeStatusCodeOk( );

		var result = response.GetAs<Paginated<GetWeatherForecastResponse>>( );

		result!.TotalItems.Should( ).BeGreaterThan( 0 );
		result.TotalPages.Should( ).BeGreaterThan( 0 );
		result.PageNumber.Should( ).Be( pageNumber );
		result.PageSize.Should( ).BeLessThanOrEqualTo( pageSize );
		result.Items.Should( ).HaveCountGreaterThan( 0 );
		result.Items.Should( ).AllSatisfy( item => {
			item.TemperatureC.Should( ).BeInRange( -100, 100 );
			item.Date.Should( ).BeOnOrAfter( DateOnly.MinValue );
			item.Date.Should( ).BeOnOrBefore( DateOnly.MaxValue );
		} );
	}

	/// <summary>
	/// Tests the GetAsync endpoint with invalid parameters to ensure proper validation and error handling.
	/// Verifies that validation exceptions are thrown for out-of-range values and invalid inputs.
	/// </summary>
	/// <param name="summary">Invalid summary ID for testing validation.</param>
	/// <param name="minimumDate">Invalid minimum date in string format.</param>
	/// <param name="maximumDate">Invalid maximum date in string format.</param>
	/// <param name="minimumTemperature">Invalid minimum temperature value.</param>
	/// <param name="maximumTemperature">Invalid maximum temperature value.</param>
	/// <param name="pageNumber">Invalid page number for testing pagination validation.</param>
	/// <param name="pageSize">Invalid page size for testing pagination validation.</param>
	[Theory( )]
	[InlineData( null, null, null, null, null, -1, 10 )]
	[InlineData( null, null, null, null, null, 1, -1 )]
	[InlineData( null, null, null, null, 1000, 1, 10 )]
	[InlineData( null, null, null, -1000, null, 1, 10 )]
	[InlineData( null, "0001-01-01", null, null, null, 1, 10 )]
	[InlineData( "xxxx", null, null, null, null, 1, 10 )]
	public async Task GetAsync_WithInvalidData_ShouldThrowException( string? summary, string? minimumDate, string? maximumDate, int? minimumTemperature, int? maximumTemperature, int pageNumber, int pageSize ) {
		// Arrange
		DateOnly? minDate = minimumDate != null ? DateOnly.Parse( minimumDate ) : null;
		DateOnly? maxDate = maximumDate != null ? DateOnly.Parse( maximumDate ) : null;
		Pagination pagination = new( pageNumber, pageSize );

		// Act
		var action = async ( ) => await _controller.GetAsync(
			summary,
			minDate,
			maxDate,
			minimumTemperature,
			maximumTemperature,
			pagination );

		// Assert
		var result = await action.Should( ).ThrowAsync<ValidationException>( );

		var response = result.Which;

		response.Message.Should( ).NotBeEmpty( );
		response.ValidationResult.StatusCode.Should( ).Be( HttpStatusCode.BadRequest );
		response.ValidationResult.IsValid.Should( ).BeFalse( );
		response.ValidationResult.Errors.Should( ).HaveCount( 1 );

	}

	/// <summary>
	/// Tests the GetByIdAsync endpoint with a valid weather forecast ID.
	/// Verifies that the correct weather forecast is retrieved and all properties are properly mapped.
	/// </summary>
	[Fact( )]
	public async Task GetByIdAsync_WithValidId_ShouldReturnValue( ) {
		// Arrange
		await MockData( );


		var existingForecast =
			GetContext( )
			.Set<WeatherForecast>( )
			.First( );

		// Act
		var response = await _controller.GetByIdAsync( existingForecast.WeatherForecastId );

		// Assert
		response.Should( ).BeStatusCodeOk( );

		var result = response.GetAs<GetWeatherForecastResponse>( );

		result!.WeatherForecastId.Should( ).Be( existingForecast.WeatherForecastId );
		result.Date.Should( ).Be( existingForecast.Date );
		result.TemperatureC.Should( ).Be( existingForecast.TemperatureC );
		result.TemperatureF.Should( ).Be( existingForecast.TemperatureF );
		result.SummaryId.Should( ).Be( ( int )existingForecast.Summary );
		result.SummaryDescription.Should( ).Be( existingForecast.Summary.ToString( ) );
	}

	/// <summary>
	/// Tests the GetByIdAsync endpoint with a default (empty) GUID.
	/// Verifies that proper validation occurs and a BadRequest response is returned.
	/// </summary>
	[Fact]
	public async Task GetByIdAsync_WithDefaultId_ShouldReturnBadRequest( ) {
		// Arrange
		var nonExistentId = Guid.Empty;

		// Act
		var action = async ( ) => await _controller.GetByIdAsync( nonExistentId );

		// Assert
		var result = await action.Should( ).ThrowAsync<ValidationException>( );

		var response = result.Which;

		response.Message.Should( ).NotBeEmpty( );
		response.ValidationResult.StatusCode.Should( ).Be( HttpStatusCode.BadRequest );
		response.ValidationResult.IsValid.Should( ).BeFalse( );
		response.ValidationResult.Errors.Should( ).HaveCount( 1 );
	}

	/// <summary>
	/// Tests the GetByIdAsync endpoint with a non-existent weather forecast ID.
	/// Verifies that a NotFound response is returned when the requested resource doesn't exist.
	/// </summary>
	[Fact]
	public async Task GetByIdAsync_WithInvalidId_ShouldReturnBadRequest( ) {
		// Arrange
		var nonExistentId = Guid.NewGuid( );

		// Act
		var action = async ( ) => await _controller.GetByIdAsync( nonExistentId );

		// Assert
		var result = await action.Should( ).ThrowAsync<ValidationException>( );

		var response = result.Which;

		response.Message.Should( ).NotBeEmpty( );
		response.ValidationResult.StatusCode.Should( ).Be( HttpStatusCode.NotFound );
		response.ValidationResult.IsValid.Should( ).BeFalse( );
		response.ValidationResult.Errors.Should( ).HaveCount( 1 );
	}

	/// <summary>
	/// Tests the PostAsync endpoint with valid weather forecast data.
	/// Verifies successful creation, proper response format, and data persistence in the database.
	/// </summary>
	/// <param name="date">Valid date string for the weather forecast.</param>
	/// <param name="temperatureC">Valid temperature in Celsius.</param>
	/// <param name="summary">Valid summary enumeration ID.</param>
	[Theory( )]
	[InlineData( "2025-01-01", 25, "Hot" )]
	[InlineData( "2024-12-25", -5, "Chilly" )]
	[InlineData( "2024-06-15", 35, "Sweltering" )]
	public async Task PostAsync_WithValidData_ShouldReturnCreated( string date, int temperatureC, string summary ) {
		// Arrange
		var request = new CreateWeatherForecastRequest {
			Date = DateOnly.Parse( date ),
			TemperatureC = temperatureC,
			Summary = summary
		};

		// Act
		var response = await _controller.PostAsync( request );

		// Assert
		var result = response.Should( ).BeOfType<CreatedAtRouteResult>( ).Which;

		result.Should( ).NotBeNull( );
		result.StatusCode.Should( ).Be( ( int )HttpStatusCode.Created );
		var resultBody = result.Value as WeatherForecastCreatedEvent;

		var context = await GetContextAsync( );
		var item = context
			.Set<WeatherForecast>( )
			.First( x => x.WeatherForecastId == resultBody!.WeatherForecastId );

		item!.Date.Should( ).Be( request.Date );
		item.TemperatureC.Should( ).Be( request.TemperatureC );
		item.Summary.Should( ).Be( Summary.FromName( request.Summary ) );
	}

	/// <summary>
	/// Tests the PostAsync endpoint with invalid weather forecast data.
	/// Verifies that validation errors are properly caught and appropriate error responses are returned.
	/// </summary>
	/// <param name="date">Invalid date string for testing date validation.</param>
	/// <param name="temperatureC">Invalid temperature value for testing temperature range validation.</param>
	/// <param name="summary">Invalid summary ID for testing enumeration validation.</param>
	[Theory( )]
	[InlineData( "0001-01-01", 25, "Hot" )]
	[InlineData( "2025-01-01", -1000, "Hot" )]
	[InlineData( "2025-01-01", 1000, "Hot" )]
	[InlineData( "2025-01-01", 25, "Invalid" )]
	public async Task PostAsync_WithInvalidData_ShouldThrowException( string date, int temperatureC, string summary ) {
		// Arrange
		var request = new CreateWeatherForecastRequest {
			Date = DateOnly.Parse( date ),
			TemperatureC = temperatureC,
			Summary = summary
		};

		// Act
		var action = async ( ) => await _controller.PostAsync( request );

		// Assert
		var result = await action.Should( ).ThrowAsync<ValidationException>( );

		var response = result.Which;

		response.Message.Should( ).NotBeEmpty( );
		response.ValidationResult.StatusCode.Should( ).Be( HttpStatusCode.BadRequest );
		response.ValidationResult.IsValid.Should( ).BeFalse( );
		response.ValidationResult.Errors.Should( ).HaveCountGreaterThan( 0 );
	}

	/// <summary>
	/// Tests the PutAsync endpoint with valid weather forecast data.
	/// Verifies successful update, proper response format, and data persistence in the database.
	/// </summary>
	/// <param name="temperatureC">Valid temperature in Celsius for the update.</param>
	/// <param name="summary">Valid summary enumeration ID for the update.</param>
	[Theory( )]
	[InlineData( 30, "Hot" )]
	[InlineData( -10, "Freezing" )]
	[InlineData( 45, "Balmy" )]
	[InlineData( 0, "Cool" )]
	public async Task PutAsync_WithValidData_ShouldReturnNoContent( int temperatureC, string summary ) {
		// Arrange
		await MockData( );

		var existingForecast =
			GetContext( )
			.Set<WeatherForecast>( )
			.First( );

		var request = new UpdateWeatherForecastRequest {
			TemperatureC = temperatureC,
			Summary = summary
		};

		// Act
		var response = await _controller.PutAsync( existingForecast.WeatherForecastId, request );

		// Assert
		var result = response.Should( ).BeOfType<NoContentResult>( ).Which;

		result!.StatusCode.Should( ).Be( ( int )HttpStatusCode.NoContent );

		// Verify data was updated in database
		var updatedItem =
			GetContext( )
			.Set<WeatherForecast>( )
			.First( x => x.WeatherForecastId == existingForecast.WeatherForecastId );

		updatedItem.TemperatureC.Should( ).Be( temperatureC );
		updatedItem.Summary.Should( ).Be( Summary.FromName( summary ) );
		updatedItem.Date.Should( ).Be( existingForecast.Date ); // Date should not change
	}

	/// <summary>
	/// Tests the PutAsync endpoint with invalid weather forecast data.
	/// Verifies that validation errors are properly caught and appropriate error responses are returned.
	/// </summary>
	/// <param name="useDefaultId">Whether to use default GUID for testing ID validation.</param>
	/// <param name="useNonExistentId">Whether to use non-existent GUID for testing existence validation.</param>
	/// <param name="temperatureC">Temperature value for testing temperature range validation.</param>
	/// <param name="summary">Summary ID for testing enumeration validation.</param>
	[Theory( )]
	[InlineData( true, false, 25, "Hot" )] // Default ID
	[InlineData( false, true, 25, "Hot" )] // Non-existent ID
	[InlineData( false, false, -1000, "Hot" )] // Temperature too low
	[InlineData( false, false, 1000, "Hot" )] // Temperature too high
	[InlineData( false, false, 25, "Invalid" )] // Invalid summary
	public async Task PutAsync_WithInvalidData_ShouldThrowException( bool useDefaultId, bool useNonExistentId, int temperatureC, string summary ) {
		// Arrange
		await MockData( );

		var context = GetContext( );
		var existingForecast = context
			.Set<WeatherForecast>( )
			.First( );

		var weatherForecastId = useDefaultId ? Guid.Empty :
			useNonExistentId ? Guid.NewGuid( ) :
			existingForecast.WeatherForecastId;

		var request = new UpdateWeatherForecastRequest {
			TemperatureC = temperatureC,
			Summary = summary
		};

		// Act
		var action = async ( ) => await _controller.PutAsync( weatherForecastId, request );

		// Assert
		var result = await action.Should( ).ThrowAsync<ValidationException>( );

		var response = result.Which;

		response.Message.Should( ).NotBeEmpty( );
		var expectedStatusCode = useNonExistentId ? HttpStatusCode.NotFound : HttpStatusCode.BadRequest;
		response.ValidationResult.StatusCode.Should( ).Be( expectedStatusCode );
		response.ValidationResult.IsValid.Should( ).BeFalse( );
		response.ValidationResult.Errors.Should( ).HaveCountGreaterThan( 0 );
	}

	/// <summary>
	/// Tests the DeleteAsync endpoint with valid weather forecast ID.
	/// Verifies successful deletion and proper response format.
	/// </summary>
	[Fact]
	public async Task DeleteAsync_WithValidId_ShouldReturnNoContent( ) {
		// Arrange
		await MockData( );

		var context = GetContext( );
		var existingForecast = context
			.Set<WeatherForecast>( )
			.First( );

		// Act
		var response = await _controller.DeleteAsync( existingForecast.WeatherForecastId );

		// Assert
		response.Should( ).BeOfType<NoContentResult>( );

		var result = response as NoContentResult;
		result!.StatusCode.Should( ).Be( ( int )HttpStatusCode.NoContent );

		// Verify data was deleted from database
		var updatedContext = await GetContextAsync( );
		var deletedItem = updatedContext
			.Set<WeatherForecast>( )
			.FirstOrDefault( x => x.WeatherForecastId == existingForecast.WeatherForecastId );

		deletedItem.Should( ).BeNull( );
	}

	/// <summary>
	/// Tests the DeleteAsync endpoint with invalid weather forecast ID.
	/// Verifies that validation errors are properly caught and appropriate error responses are returned.
	/// </summary>
	/// <param name="useDefaultId">Whether to use default GUID for testing ID validation.</param>
	/// <param name="useNonExistentId">Whether to use non-existent GUID for testing existence validation.</param>
	[Theory( )]
	[InlineData( true, false )] // Default ID
	[InlineData( false, true )] // Non-existent ID
	public async Task DeleteAsync_WithInvalidId_ShouldThrowException( bool useDefaultId, bool useNonExistentId ) {
		// Arrange
		await MockData( );

		var context = GetContext( );
		var existingForecast = context
			.Set<WeatherForecast>( )
			.First( );

		var weatherForecastId = useDefaultId ? Guid.Empty :
			useNonExistentId ? Guid.NewGuid( ) :
			existingForecast.WeatherForecastId;

		// Act
		var action = async ( ) => await _controller.DeleteAsync( weatherForecastId );

		// Assert
		var result = await action.Should( ).ThrowAsync<ValidationException>( );

		var response = result.Which;

		response.Message.Should( ).NotBeEmpty( );
		var expectedStatusCode = useNonExistentId ? HttpStatusCode.NotFound : HttpStatusCode.BadRequest;
		response.ValidationResult.StatusCode.Should( ).Be( expectedStatusCode );
		response.ValidationResult.IsValid.Should( ).BeFalse( );
		response.ValidationResult.Errors.Should( ).HaveCount( 1 );
	}
}
