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
using Myth.Testing.Extensions;
using Myth.Testing.Repositories;
using Myth.ValueObjects;
using System;
using System.Net;

namespace Myth.Template.Test {
	public class WeatherForecastTests : BaseDatabaseTests<ForecastContext> {
		private readonly WeatherForecastController _controller;
		public WeatherForecastTests( ) : base( ) {
			AddServices( ( services ) => {
				services.AddLogging( );
				services.AddRepositories( );
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

		private async Task MockData( DateOnly? minDate = null, DateOnly? maxDate = null ) {
			var random = new Random( );
			maxDate ??= DateOnly.FromDateTime( DateTime.UtcNow );
			minDate ??= maxDate.Value.AddDays( -1000 );
			var amount = maxDate.Value.DayNumber - minDate.Value.DayNumber;

			var weatherForecasts = Enumerable.Range( 0, amount ).Select( i => {
				var date = maxDate.Value.AddDays( i - 1 );
				var temperatureC = random.Next( -20, 55 );
				var summaries = Enum
					.GetValues( typeof( Summary ) )
					.Cast<Summary>( )
					.ToArray( );
				var summary = summaries[ random.Next( summaries.Length ) ];

				return new WeatherForecast( date, temperatureC, summary );
			} );

			await GetContext( ).AddRangeAsync( weatherForecasts, CancellationToken.None );

			await SaveChangesAsync( CancellationToken.None );
		}

		[Theory( )]
		[InlineData( null, null, null, null, null, 1, 100 )]
		[InlineData( null, "2000-01-01", "2025-01-30", -100, 100, 1, 100 )]
		public async Task GetAsync_WithValidData_ShouldReturnValue( int? summaryId, string? minimumDate, string? maximumDate, int? minimumTemperature, int? maximumTemperature, int pageNumber, int pageSize ) {
			// Arrange
			Pagination pagination = new( pageNumber, pageSize );
			Summary? summary = summaryId != null
				? ( Summary )summaryId
				: null;
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

		[Theory( )]
		[InlineData( null, null, null, null, null, -1, 10 )]
		[InlineData( null, null, null, null, null, 1, -1 )]
		[InlineData( null, null, null, null, 1000, 1, 10 )]
		[InlineData( null, null, null, -1000, null, 1, 10 )]
		[InlineData( null, "0001-01-01", null, null, null, 1, 10 )]
		[InlineData( 99, null, null, null, null, 1, 10 )]
		public async Task GetAsync_WithInvalidData_ShouldThrowException( int? summaryId, string? minimumDate, string? maximumDate, int? minimumTemperature, int? maximumTemperature, int pageNumber, int pageSize ) {
			// Arrange
			Summary? summary = summaryId != null ? ( Summary )summaryId : null;
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

		[Fact( )]
		public async Task GetByIdAsync_WithValidId_ShouldReturnValue( ) {
			// Arrange
			await MockData( );

			var context = GetContext( );

			var existingForecast = context
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

		[Theory( )]
		[InlineData( "2025-01-01", 25, 4 )]
		[InlineData( "2024-12-25", -5, 0 )]
		[InlineData( "2024-06-15", 35, 7 )]
		public async Task PostAsync_WithValidData_ShouldReturnCreated( string date, int temperatureC, int summaryId ) {
			// Arrange
			var request = new CreateWeatherForecastRequest {
				Date = DateOnly.Parse( date ),
				TemperatureC = temperatureC,
				Summary = ( Summary )summaryId
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
			item.Summary.Should( ).Be( request.Summary );
		}

		[Theory( )]
		[InlineData( "0001-01-01", 25, 4 )]
		[InlineData( "2025-01-01", -1000, 4 )]
		[InlineData( "2025-01-01", 1000, 4 )]
		[InlineData( "2025-01-01", 25, 99 )]
		public async Task PostAsync_WithInvalidData_ShouldThrowException( string date, int temperatureC, int summaryId ) {
			// Arrange
			var request = new CreateWeatherForecastRequest {
				Date = DateOnly.Parse( date ),
				TemperatureC = temperatureC,
				Summary = ( Summary )summaryId
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
	}
}
