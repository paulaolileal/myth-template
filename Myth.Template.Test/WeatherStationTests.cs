using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Myth.Exceptions;
using Myth.Extensions;
using Myth.Flow.Actions.Extensions;
using Myth.Models.Results;
using Myth.Template.API.Controllers;
using Myth.Template.Application.WeatherStations.Commands.CreateWithForecasts;
using Myth.Template.Application.WeatherStations.DTOs;
using Myth.Template.Application.WeatherStations.Queries.GetAll;
using Myth.Template.Data.Contexts;
using Myth.Template.Domain.Models;
using Myth.Template.ExternalData.Breweries.Interfaces;
using Myth.Template.Test.Repositories;
using Myth.Repositories;
using Myth.ValueObjects;

namespace Myth.Template.Test;

/// <summary>
/// Integration test suite for the WeatherStationController API endpoints.
/// Tests all CRUD operations and lifecycle transitions including validation scenarios,
/// error handling, and business logic such as Activate, Decommission, and batch creation.
/// Inherits from BaseDatabaseTests to provide in-memory database testing capabilities.
/// </summary>
[Collection( "SequentialTests" )]
public class WeatherStationTests : BaseDatabaseTests<ForecastContext> {
	/// <summary>The controller instance under test, initialized with all required dependencies.</summary>
	private readonly WeatherStationController _controller;

	/// <summary>
	/// Initializes a new instance of the WeatherStationTests class.
	/// Sets up the dependency injection container with all required services for testing.
	/// </summary>
	public WeatherStationTests( ) : base( ) {
		AddServices( services => {
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
					.ScanAssemblies( typeof( GetAllWeatherStationsQueryHandler ).Assembly ) ) );
		} );

		_controller = CreateInstance<WeatherStationController>( );
	}

	/// <summary>
	/// Seeds a single weather station into the test database.
	/// </summary>
	/// <param name="isActive">Whether the station should be active. Defaults to true.</param>
	/// <param name="name">The display name for the station.</param>
	/// <param name="location">The geographic location for the station.</param>
	/// <returns>The persisted station entity.</returns>
	private async Task<WeatherStation> MockStation(
		bool isActive = true,
		string name = "Test Station Alpha",
		string location = "Lat 45.1234, Lon -93.5678" ) {
		var station = new WeatherStation( name, location );
		if ( !isActive ) station.Deactivate( );
		await GetContext( ).AddAsync( station );
		await SaveChangesAsync( CancellationToken.None );
		return station;
	}

	/// <summary>
	/// Seeds multiple randomly generated weather stations into the test database.
	/// </summary>
	/// <param name="amount">Number of stations to generate. Defaults to 5.</param>
	/// <returns>The list of persisted station entities.</returns>
	private async Task<IReadOnlyList<WeatherStation>> MockStations( int amount = 5 ) {
		var stations = WeatherStation.GenerateDataAsync( amount, _cancellationToken ).ToList( );
		await GetContext( ).AddRangeAsync( stations );
		await SaveChangesAsync( CancellationToken.None );
		return stations;
	}

	#region GetAsync

	/// <summary>
	/// Tests the GetAsync endpoint with valid parameters to ensure it returns paginated weather station data.
	/// Verifies proper pagination structure and item property integrity across various filter combinations.
	/// </summary>
	/// <param name="name">Optional name substring filter.</param>
	/// <param name="isActive">Optional active-status filter.</param>
	/// <param name="withForecasts">Whether to trigger the conditional .When() pipeline branch.</param>
	/// <param name="pageNumber">The page number for pagination.</param>
	/// <param name="pageSize">The page size for pagination.</param>
	[Theory]
	[InlineData( null, null, false, 1, 10 )]
	[InlineData( null, null, true, 1, 10 )]
	[InlineData( null, true, false, 1, 10 )]
	[InlineData( "Station", null, false, 1, 10 )]
	public async Task GetAsync_WithValidParameters_ShouldReturnPaginatedStations(
		string? name, bool? isActive, bool withForecasts, int pageNumber, int pageSize ) {
		// Arrange
		await MockStations( );
		var pagination = new Pagination( pageNumber, pageSize );

		// Act
		var response = await _controller.GetAsync( name, isActive, pagination, withForecasts );

		// Assert
		response.Should( ).BeStatusCodeOk( );

		var result = response.GetAs<Paginated<GetWeatherStationResponse>>( );
		result!.PageNumber.Should( ).Be( pageNumber );
		result.PageSize.Should( ).BeLessThanOrEqualTo( pageSize );
		result.Items.Should( ).AllSatisfy( item => {
			item.WeatherStationId.Should( ).NotBeEmpty( );
			item.Name.Should( ).NotBeNullOrEmpty( );
			item.Location.Should( ).NotBeNullOrEmpty( );
		} );
	}

	/// <summary>
	/// Tests the GetAsync endpoint with invalid pagination values to ensure validation fails correctly.
	/// Verifies that a BadRequest ValidationException is thrown for out-of-range page parameters.
	/// </summary>
	/// <param name="pageNumber">Invalid page number for testing.</param>
	/// <param name="pageSize">Invalid page size for testing.</param>
	[Theory]
	[InlineData( 0, 10 )]
	[InlineData( -1, 10 )]
	[InlineData( 1, 0 )]
	[InlineData( 1, 101 )]
	public async Task GetAsync_WithInvalidPagination_ShouldThrowValidationException( int pageNumber, int pageSize ) {
		// Arrange
		var pagination = new Pagination( pageNumber, pageSize );

		// Act
		var action = async ( ) => await _controller.GetAsync( null, null, pagination );

		// Assert
		var result = await action.Should( ).ThrowAsync<ValidationException>( );

		var response = result.Which;
		response.Message.Should( ).NotBeEmpty( );
		response.ValidationResult.StatusCode.Should( ).Be( HttpStatusCode.BadRequest );
		response.ValidationResult.IsValid.Should( ).BeFalse( );
		response.ValidationResult.Errors.Should( ).HaveCount( 1 );
	}

	#endregion

	#region GetByIdAsync

	/// <summary>
	/// Tests the GetByIdAsync endpoint with a valid station ID.
	/// Verifies that the correct station is retrieved with all properties properly mapped.
	/// </summary>
	[Fact]
	public async Task GetByIdAsync_WithValidId_ShouldReturnStation( ) {
		// Arrange
		var existing = await MockStation( );

		// Act
		var response = await _controller.GetByIdAsync( existing.WeatherStationId );

		// Assert
		response.Should( ).BeStatusCodeOk( );

		var result = response.GetAs<GetWeatherStationResponse>( );
		result!.WeatherStationId.Should( ).Be( existing.WeatherStationId );
		result.Name.Should( ).Be( existing.Name );
		result.Location.Should( ).Be( existing.Location );
		result.IsActive.Should( ).Be( existing.IsActive );
		result.CreatedAt.Should( ).BeCloseTo( existing.CreatedAt, TimeSpan.FromSeconds( 1 ) );
	}

	/// <summary>
	/// Tests the GetByIdAsync endpoint with an empty (default) GUID.
	/// Verifies that a BadRequest ValidationException is thrown.
	/// </summary>
	[Fact]
	public async Task GetByIdAsync_WithDefaultId_ShouldThrowBadRequest( ) {
		// Arrange
		var defaultId = Guid.Empty;

		// Act
		var action = async ( ) => await _controller.GetByIdAsync( defaultId );

		// Assert
		var result = await action.Should( ).ThrowAsync<ValidationException>( );

		var response = result.Which;
		response.Message.Should( ).NotBeEmpty( );
		response.ValidationResult.StatusCode.Should( ).Be( HttpStatusCode.BadRequest );
		response.ValidationResult.IsValid.Should( ).BeFalse( );
		response.ValidationResult.Errors.Should( ).HaveCount( 1 );
	}

	/// <summary>
	/// Tests the GetByIdAsync endpoint with a non-existent station ID.
	/// Verifies that a NotFound ValidationException is thrown.
	/// </summary>
	[Fact]
	public async Task GetByIdAsync_WithNonExistentId_ShouldThrowNotFound( ) {
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

	#endregion

	#region PostAsync

	/// <summary>
	/// Tests the PostAsync endpoint with valid station data.
	/// Verifies successful creation with 201 response and data persistence in the database.
	/// </summary>
	/// <param name="name">Valid station name.</param>
	/// <param name="location">Valid station location.</param>
	[Theory]
	[InlineData( "North Bay Station", "Lat 45.1234, Lon -93.5678" )]
	[InlineData( "South Ridge Station", "Lat -33.8688, Lon 151.2093" )]
	public async Task PostAsync_WithValidData_ShouldReturnCreated( string name, string location ) {
		// Arrange
		var request = new CreateWeatherStationRequest { Name = name, Location = location };

		// Act
		var response = await _controller.PostAsync( request );

		// Assert
		var result = response.Should( ).BeOfType<CreatedAtRouteResult>( ).Which;
		result.StatusCode.Should( ).Be( (int)HttpStatusCode.Created );

		var stationId = (Guid)result.Value!;
		var context = await GetContextAsync( );
		var item = context.Set<WeatherStation>( ).First( x => x.WeatherStationId == stationId );

		item.Name.Should( ).Be( request.Name );
		item.Location.Should( ).Be( request.Location );
		item.IsActive.Should( ).BeTrue( );
	}

	/// <summary>
	/// Tests the PostAsync endpoint with invalid station data.
	/// Verifies that a BadRequest ValidationException is thrown for invalid field values.
	/// </summary>
	/// <param name="name">Invalid station name for testing.</param>
	/// <param name="location">Invalid station location for testing.</param>
	[Theory]
	[InlineData( "", "Valid Location" )]
	[InlineData( "A", "Valid Location" )]
	[InlineData( "Valid Station Name", "" )]
	public async Task PostAsync_WithInvalidData_ShouldThrowBadRequest( string name, string location ) {
		// Arrange
		var request = new CreateWeatherStationRequest { Name = name, Location = location };

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
	/// Tests the PostAsync endpoint when a station with the same name already exists.
	/// Verifies that a Conflict ValidationException is thrown due to the uniqueness rule in the Create context.
	/// </summary>
	[Fact]
	public async Task PostAsync_WithDuplicateName_ShouldThrowConflict( ) {
		// Arrange
		var existing = await MockStation( name: "Unique Station Name" );
		var request = new CreateWeatherStationRequest { Name = existing.Name, Location = "Lat 10.0, Lon 20.0" };

		// Act
		var action = async ( ) => await _controller.PostAsync( request );

		// Assert
		var result = await action.Should( ).ThrowAsync<ValidationException>( );

		var response = result.Which;
		response.Message.Should( ).NotBeEmpty( );
		response.ValidationResult.StatusCode.Should( ).Be( HttpStatusCode.Conflict );
		response.ValidationResult.IsValid.Should( ).BeFalse( );
		response.ValidationResult.Errors.Should( ).HaveCount( 1 );
	}

	#endregion

	#region PutAsync

	/// <summary>
	/// Tests the PutAsync endpoint with valid update data.
	/// Verifies successful update with 204 response and data persistence in the database.
	/// </summary>
	/// <param name="newName">New valid station name.</param>
	/// <param name="newLocation">New valid station location.</param>
	[Theory]
	[InlineData( "Updated Station Name", "Updated Location Lat 10.0" )]
	[InlineData( "Mountain Peak Station", "Lat -15.7801, Lon -47.9292" )]
	public async Task PutAsync_WithValidData_ShouldReturnNoContent( string newName, string newLocation ) {
		// Arrange
		var existing = await MockStation( );
		var request = new UpdateWeatherStationRequest { Name = newName, Location = newLocation };

		// Act
		var response = await _controller.PutAsync( existing.WeatherStationId, request );

		// Assert
		var result = response.Should( ).BeOfType<NoContentResult>( ).Which;
		result.StatusCode.Should( ).Be( (int)HttpStatusCode.NoContent );

		var updatedContext = await GetContextAsync( );
		var updated = updatedContext.Set<WeatherStation>( ).First( x => x.WeatherStationId == existing.WeatherStationId );
		updated.Name.Should( ).Be( newName );
		updated.Location.Should( ).Be( newLocation );
	}

	/// <summary>
	/// Tests the PutAsync endpoint with invalid update data.
	/// Verifies that a ValidationException is thrown with the appropriate HTTP status code.
	/// </summary>
	/// <param name="useDefaultId">Whether to use an empty GUID as the station identifier.</param>
	/// <param name="useNonExistentId">Whether to use a non-existent GUID as the station identifier.</param>
	/// <param name="name">Station name for the update request.</param>
	/// <param name="location">Station location for the update request.</param>
	[Theory]
	[InlineData( true, false, "Valid Name", "Valid Location" )]
	[InlineData( false, true, "Valid Name", "Valid Location" )]
	[InlineData( false, false, "", "Valid Location" )]
	[InlineData( false, false, "A", "Valid Location" )]
	[InlineData( false, false, "Valid Name", "" )]
	public async Task PutAsync_WithInvalidData_ShouldThrowException(
		bool useDefaultId, bool useNonExistentId, string name, string location ) {
		// Arrange
		var existing = await MockStation( );
		var weatherStationId = useDefaultId ? Guid.Empty :
			useNonExistentId ? Guid.NewGuid( ) :
			existing.WeatherStationId;

		var request = new UpdateWeatherStationRequest { Name = name, Location = location };

		// Act
		var action = async ( ) => await _controller.PutAsync( weatherStationId, request );

		// Assert
		var result = await action.Should( ).ThrowAsync<ValidationException>( );

		var response = result.Which;
		response.Message.Should( ).NotBeEmpty( );
		var expectedStatusCode = useNonExistentId ? HttpStatusCode.NotFound : HttpStatusCode.BadRequest;
		response.ValidationResult.StatusCode.Should( ).Be( expectedStatusCode );
		response.ValidationResult.IsValid.Should( ).BeFalse( );
		response.ValidationResult.Errors.Should( ).HaveCountGreaterThan( 0 );
	}

	#endregion

	#region DeleteAsync

	/// <summary>
	/// Tests the DeleteAsync endpoint with a valid station ID.
	/// Verifies successful deletion with 204 response and removal from the database.
	/// </summary>
	[Fact]
	public async Task DeleteAsync_WithValidId_ShouldReturnNoContent( ) {
		// Arrange
		var existing = await MockStation( );

		// Act
		var response = await _controller.DeleteAsync( existing.WeatherStationId );

		// Assert
		response.Should( ).BeOfType<NoContentResult>( );
		var result = response as NoContentResult;
		result!.StatusCode.Should( ).Be( (int)HttpStatusCode.NoContent );

		var updatedContext = await GetContextAsync( );
		var deleted = updatedContext.Set<WeatherStation>( ).FirstOrDefault( x => x.WeatherStationId == existing.WeatherStationId );
		deleted.Should( ).BeNull( );
	}

	/// <summary>
	/// Tests the DeleteAsync endpoint with invalid station identifiers.
	/// Verifies that a ValidationException is thrown with the appropriate HTTP status code.
	/// </summary>
	/// <param name="useDefaultId">Whether to use an empty GUID as the station identifier.</param>
	/// <param name="useNonExistentId">Whether to use a non-existent GUID as the station identifier.</param>
	[Theory]
	[InlineData( true, false )]
	[InlineData( false, true )]
	public async Task DeleteAsync_WithInvalidId_ShouldThrowException( bool useDefaultId, bool useNonExistentId ) {
		// Arrange
		await MockStation( );
		var weatherStationId = useDefaultId ? Guid.Empty : Guid.NewGuid( );

		// Act
		var action = async ( ) => await _controller.DeleteAsync( weatherStationId );

		// Assert
		var result = await action.Should( ).ThrowAsync<ValidationException>( );

		var response = result.Which;
		response.Message.Should( ).NotBeEmpty( );
		var expectedStatusCode = useNonExistentId ? HttpStatusCode.NotFound : HttpStatusCode.BadRequest;
		response.ValidationResult.StatusCode.Should( ).Be( expectedStatusCode );
		response.ValidationResult.IsValid.Should( ).BeFalse( );
		response.ValidationResult.Errors.Should( ).HaveCount( 1 );
	}

	#endregion

	#region ActivateAsync

	/// <summary>
	/// Tests the ActivateAsync endpoint with an inactive station.
	/// Verifies successful activation with 204 response and IsActive = true in the database.
	/// </summary>
	[Fact]
	public async Task ActivateAsync_WithInactiveStation_ShouldReturnNoContent( ) {
		// Arrange
		var existing = await MockStation( isActive: false );

		// Act
		var response = await _controller.ActivateAsync( existing.WeatherStationId );

		// Assert
		response.Should( ).BeOfType<NoContentResult>( );
		var result = response as NoContentResult;
		result!.StatusCode.Should( ).Be( (int)HttpStatusCode.NoContent );

		var updatedContext = await GetContextAsync( );
		var updated = updatedContext.Set<WeatherStation>( ).First( x => x.WeatherStationId == existing.WeatherStationId );
		updated.IsActive.Should( ).BeTrue( );
	}

	/// <summary>
	/// Tests the ActivateAsync endpoint when the station is already active.
	/// Verifies that a BadRequest ValidationException is thrown, enforcing the Activate context rule.
	/// </summary>
	[Fact]
	public async Task ActivateAsync_WithAlreadyActiveStation_ShouldThrowBadRequest( ) {
		// Arrange
		var existing = await MockStation( isActive: true );

		// Act
		var action = async ( ) => await _controller.ActivateAsync( existing.WeatherStationId );

		// Assert
		var result = await action.Should( ).ThrowAsync<ValidationException>( );

		var response = result.Which;
		response.Message.Should( ).NotBeEmpty( );
		response.ValidationResult.StatusCode.Should( ).Be( HttpStatusCode.BadRequest );
		response.ValidationResult.IsValid.Should( ).BeFalse( );
		response.ValidationResult.Errors.Should( ).HaveCount( 1 );
	}

	/// <summary>
	/// Tests the ActivateAsync endpoint with a non-existent station ID.
	/// Verifies that a NotFound ValidationException is thrown.
	/// </summary>
	[Fact]
	public async Task ActivateAsync_WithNonExistentId_ShouldThrowNotFound( ) {
		// Arrange
		var nonExistentId = Guid.NewGuid( );

		// Act
		var action = async ( ) => await _controller.ActivateAsync( nonExistentId );

		// Assert
		var result = await action.Should( ).ThrowAsync<ValidationException>( );

		var response = result.Which;
		response.Message.Should( ).NotBeEmpty( );
		response.ValidationResult.StatusCode.Should( ).Be( HttpStatusCode.NotFound );
		response.ValidationResult.IsValid.Should( ).BeFalse( );
		response.ValidationResult.Errors.Should( ).HaveCount( 1 );
	}

	/// <summary>
	/// Tests the ActivateAsync endpoint with an empty (default) GUID.
	/// Verifies that a BadRequest ValidationException is thrown by the NotDefault rule.
	/// </summary>
	[Fact]
	public async Task ActivateAsync_WithDefaultId_ShouldThrowBadRequest( ) {
		// Arrange
		var defaultId = Guid.Empty;

		// Act
		var action = async ( ) => await _controller.ActivateAsync( defaultId );

		// Assert
		var result = await action.Should( ).ThrowAsync<ValidationException>( );

		var response = result.Which;
		response.Message.Should( ).NotBeEmpty( );
		response.ValidationResult.StatusCode.Should( ).Be( HttpStatusCode.BadRequest );
		response.ValidationResult.IsValid.Should( ).BeFalse( );
		response.ValidationResult.Errors.Should( ).HaveCount( 1 );
	}

	#endregion

	#region DecommissionAsync

	/// <summary>
	/// Tests the DecommissionAsync endpoint with an active station.
	/// Verifies successful decommission with 204 response and IsActive = false in the database.
	/// </summary>
	[Fact]
	public async Task DecommissionAsync_WithActiveStation_ShouldReturnNoContent( ) {
		// Arrange
		var existing = await MockStation( isActive: true );

		// Act
		var response = await _controller.DecommissionAsync( existing.WeatherStationId );

		// Assert
		response.Should( ).BeOfType<NoContentResult>( );
		var result = response as NoContentResult;
		result!.StatusCode.Should( ).Be( (int)HttpStatusCode.NoContent );

		var updatedContext = await GetContextAsync( );
		var updated = updatedContext.Set<WeatherStation>( ).First( x => x.WeatherStationId == existing.WeatherStationId );
		updated.IsActive.Should( ).BeFalse( );
	}

	/// <summary>
	/// Tests the DecommissionAsync endpoint when the station is already inactive.
	/// Verifies that a BadRequest ValidationException is thrown, enforcing the custom Decommission context rule.
	/// </summary>
	[Fact]
	public async Task DecommissionAsync_WithAlreadyInactiveStation_ShouldThrowBadRequest( ) {
		// Arrange
		var existing = await MockStation( isActive: false );

		// Act
		var action = async ( ) => await _controller.DecommissionAsync( existing.WeatherStationId );

		// Assert
		var result = await action.Should( ).ThrowAsync<ValidationException>( );

		var response = result.Which;
		response.Message.Should( ).NotBeEmpty( );
		response.ValidationResult.StatusCode.Should( ).Be( HttpStatusCode.BadRequest );
		response.ValidationResult.IsValid.Should( ).BeFalse( );
		response.ValidationResult.Errors.Should( ).HaveCount( 1 );
	}

	/// <summary>
	/// Tests the DecommissionAsync endpoint with a non-existent station ID.
	/// Verifies that a NotFound ValidationException is thrown.
	/// </summary>
	[Fact]
	public async Task DecommissionAsync_WithNonExistentId_ShouldThrowNotFound( ) {
		// Arrange
		var nonExistentId = Guid.NewGuid( );

		// Act
		var action = async ( ) => await _controller.DecommissionAsync( nonExistentId );

		// Assert
		var result = await action.Should( ).ThrowAsync<ValidationException>( );

		var response = result.Which;
		response.Message.Should( ).NotBeEmpty( );
		response.ValidationResult.StatusCode.Should( ).Be( HttpStatusCode.NotFound );
		response.ValidationResult.IsValid.Should( ).BeFalse( );
		response.ValidationResult.Errors.Should( ).HaveCount( 1 );
	}

	/// <summary>
	/// Tests the DecommissionAsync endpoint with an empty (default) GUID.
	/// Verifies that a BadRequest ValidationException is thrown by the NotDefault rule.
	/// </summary>
	[Fact]
	public async Task DecommissionAsync_WithDefaultId_ShouldThrowBadRequest( ) {
		// Arrange
		var defaultId = Guid.Empty;

		// Act
		var action = async ( ) => await _controller.DecommissionAsync( defaultId );

		// Assert
		var result = await action.Should( ).ThrowAsync<ValidationException>( );

		var response = result.Which;
		response.Message.Should( ).NotBeEmpty( );
		response.ValidationResult.StatusCode.Should( ).Be( HttpStatusCode.BadRequest );
		response.ValidationResult.IsValid.Should( ).BeFalse( );
		response.ValidationResult.Errors.Should( ).HaveCount( 1 );
	}

	#endregion

	#region PostWithForecastsAsync

	/// <summary>
	/// Tests the PostWithForecastsAsync endpoint with valid station and forecast data.
	/// Verifies atomic creation of station and forecasts with 201 response and data persistence.
	/// </summary>
	[Fact]
	public async Task PostWithForecastsAsync_WithValidData_ShouldReturnCreated( ) {
		// Arrange
		var request = new CreateWeatherStationWithForecastsCommand {
			Name = "South Bay Station",
			Location = "Lat -33.8688, Lon 151.2093",
			InitialForecasts = [
				new InitialForecastItem { Date = new DateOnly( 2024, 1, 15 ), TemperatureC = 28, Summary = "Hot" },
				new InitialForecastItem { Date = new DateOnly( 2024, 1, 16 ), TemperatureC = 22, Summary = "Chilly" }
			]
		};

		// Act
		var response = await _controller.PostWithForecastsAsync( request );

		// Assert
		var result = response.Should( ).BeOfType<CreatedAtRouteResult>( ).Which;
		result.StatusCode.Should( ).Be( (int)HttpStatusCode.Created );

		var stationId = (Guid)result.Value!;
		stationId.Should( ).NotBeEmpty( "handler should return a valid station identifier" );

		var context = await GetContextAsync( );

		var station = context.Set<WeatherStation>( ).FirstOrDefault( x => x.WeatherStationId == stationId );
		station.Should( ).NotBeNull( "created station should be persisted in the database" );
		station!.Name.Should( ).Be( request.Name );
		station.Location.Should( ).Be( request.Location );
		station.IsActive.Should( ).BeTrue( );

		var forecasts = context.Set<WeatherForecast>( ).Where( x => x.WeatherStationId == stationId ).ToList( );
		forecasts.Should( ).HaveCount( request.InitialForecasts.Count );
	}

	/// <summary>
	/// Tests the PostWithForecastsAsync endpoint with invalid station data.
	/// Verifies that a BadRequest ValidationException is thrown for invalid field values.
	/// </summary>
	/// <param name="name">Invalid station name for testing.</param>
	/// <param name="location">Invalid station location for testing.</param>
	[Theory]
	[InlineData( "", "Valid Location Lat 10.0, Lon 20.0" )]
	[InlineData( "A", "Valid Location Lat 10.0, Lon 20.0" )]
	[InlineData( "Valid Station Name", "" )]
	public async Task PostWithForecastsAsync_WithInvalidData_ShouldThrowBadRequest( string name, string location ) {
		// Arrange
		var request = new CreateWeatherStationWithForecastsCommand { Name = name, Location = location };

		// Act
		var action = async ( ) => await _controller.PostWithForecastsAsync( request );

		// Assert
		var result = await action.Should( ).ThrowAsync<ValidationException>( );

		var response = result.Which;
		response.Message.Should( ).NotBeEmpty( );
		response.ValidationResult.StatusCode.Should( ).Be( HttpStatusCode.BadRequest );
		response.ValidationResult.IsValid.Should( ).BeFalse( );
		response.ValidationResult.Errors.Should( ).HaveCountGreaterThan( 0 );
	}

	/// <summary>
	/// Tests the PostWithForecastsAsync endpoint when a station with the same name already exists.
	/// Verifies that a Conflict ValidationException is thrown due to the uniqueness rule in the Create context.
	/// </summary>
	[Fact]
	public async Task PostWithForecastsAsync_WithDuplicateName_ShouldThrowConflict( ) {
		// Arrange
		var existing = await MockStation( name: "Unique Bay Station" );
		var request = new CreateWeatherStationWithForecastsCommand {
			Name = existing.Name,
			Location = "Lat 10.0, Lon 20.0"
		};

		// Act
		var action = async ( ) => await _controller.PostWithForecastsAsync( request );

		// Assert
		var result = await action.Should( ).ThrowAsync<ValidationException>( );

		var response = result.Which;
		response.Message.Should( ).NotBeEmpty( );
		response.ValidationResult.StatusCode.Should( ).Be( HttpStatusCode.Conflict );
		response.ValidationResult.IsValid.Should( ).BeFalse( );
		response.ValidationResult.Errors.Should( ).HaveCount( 1 );
	}

	#endregion
}
