using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Myth.Template.Domain.Models;

namespace Myth.Template.Data.Mappings;

public class WeatherStationMap : IEntityTypeConfiguration<WeatherStation> {

	public void Configure( EntityTypeBuilder<WeatherStation> builder ) {
		builder
			.ToTable( "weather_station" )
			.HasKey( x => x.WeatherStationId )
			.HasName( "weather_station_id" );

		builder
			.Property( x => x.WeatherStationId )
			.HasColumnName( "weather_station_id" )
			.IsRequired( );

		builder
			.Property( x => x.Name )
			.HasColumnName( "name" )
			.HasMaxLength( 100 )
			.IsRequired( );

		builder
			.Property( x => x.Location )
			.HasColumnName( "location" )
			.HasMaxLength( 200 )
			.IsRequired( );

		builder
			.Property( x => x.IsActive )
			.HasColumnName( "is_active" )
			.IsRequired( );

		builder
			.Property( x => x.CreatedAt )
			.HasColumnName( "created_at" )
			.IsRequired( );

		builder
			.Property( x => x.UpdatedAt )
			.HasColumnName( "updated_at" )
			.IsRequired( false );

		builder
			.HasMany( x => x.Forecasts )
			.WithOne( )
			.HasForeignKey( f => f.WeatherStationId )
			.IsRequired( false )
			.OnDelete( DeleteBehavior.SetNull );
	}
}
