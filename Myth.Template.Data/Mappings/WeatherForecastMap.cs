using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Myth.Template.Domain.Models;

namespace Myth.Template.Data.Mappings;

public class WeatherForecastMap : IEntityTypeConfiguration<WeatherForecast> {

	public void Configure( EntityTypeBuilder<WeatherForecast> builder ) {
		builder
			.ToTable( "weather_forecast" )
			.HasKey( x => x.WeatherForecastId )
			.HasName( "weather_forecast_id" );

		builder
			.Property( x => x.WeatherForecastId )
			.HasColumnName( "weather_forecast_id" )
			.IsRequired( );

		builder
			.Property( x => x.Date )
			.HasColumnName( "date" )
			.IsRequired( );

		builder
			.Property( x => x.Summary )
			.HasColumnName( "summary_id" )
			.HasConversion(
				p => p.Value,
				p => Summary.FromValue( p ) )
			.IsRequired( );

		builder
			.Property( x => x.TemperatureC )
			.HasColumnName( "temperature_c" )
			.IsRequired( );
	}
}
