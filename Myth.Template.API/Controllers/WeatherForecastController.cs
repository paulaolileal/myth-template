using Microsoft.AspNetCore.Mvc;
using Myth.Extensions;
using Myth.Flow.Actions.Extensions;
using Myth.Interfaces;
using Myth.Interfaces.Results;
using Myth.Template.Application.WeatherForecasts.Commands.Create;
using Myth.Template.Application.WeatherForecasts.DTOs;
using Myth.Template.Application.WeatherForecasts.Events.Created;
using Myth.Template.Application.WeatherForecasts.Queries.GetAll;
using Myth.Template.Application.WeatherForecasts.Queries.GetById;
using Myth.Template.Domain.Models;
using Myth.ValueObjects;

namespace Myth.Template.API.Controllers {

	[Tags( "Weather forecast" )]
	[ApiController]
	[Route( "api/v1/[controller]" )]
	public class WeatherForecastController : ControllerBase {
		private readonly IValidator _validator;

		public WeatherForecastController( IValidator validator ) {
			_validator = validator;
		}

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
				.TapAsync( pipeline => _validator.ValidateAsync( pipeline.CurrentRequest! ) )
				.Query<GetAllWeatherForecastQuery, IPaginated<GetWeatherForecastResponse>>( )
				.ExecuteAsync( cancellationToken );


			return Ok( result.Value );

		}

		[HttpGet( "{weatherForecastId}", Name = "GetByIdAsync" )]
		public async Task<IActionResult> GetByIdAsync( [FromRoute] Guid weatherForecastId, CancellationToken cancellationToken = default ) {
			var result = await PipelineExtensions
				.Start( new GetWeatherForecastByIdQuery( weatherForecastId ) )
				.TapAsync( pipeline => _validator.ValidateAsync( pipeline.CurrentRequest! ) )
				.Query<GetWeatherForecastByIdQuery, GetWeatherForecastResponse>( )
				.ExecuteAsync( cancellationToken );

			return Ok( result.Value );

		}

		[HttpPost]
		public async Task<IActionResult> PostAsync( [FromBody] CreateWeatherForecastRequest request, CancellationToken cancellationToken = default ) {
			var result = await PipelineExtensions
				.Start( request.To<CreateWeatherForecastCommand>( ) )
				.TapAsync( pipeline => _validator.ValidateAsync( pipeline.CurrentRequest! ) )
				.Process<CreateWeatherForecastCommand, Guid>( )
				.Transform( result => new WeatherForecastCreatedEvent( result ) )
				.Publish( )
				.ExecuteAsync( cancellationToken );


			return CreatedAtRoute(
				nameof( GetByIdAsync ),
				new {
					weatherForecastId = result.Value.WeatherForecastId
				},
				result.Value );

		}
	}
}