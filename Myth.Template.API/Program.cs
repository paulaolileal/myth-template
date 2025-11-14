using System.Diagnostics.CodeAnalysis;
using Myth.Extensions;
using Myth.Template.API.Extensions;
using Myth.Template.Application;

namespace Myth.Template.API;

[ExcludeFromCodeCoverage]
internal class Program {

	private static void Main( string[ ] args ) {
		var builder = WebApplication.CreateBuilder( args );

		// Add services to the container
		builder.AddDatabase( );

		builder.AddMyth( );

		builder.Services.AddControllers( );

		builder.AddDocs( );

		builder.Services.AddHealthChecks( );

		builder.Services.AddHostedService<InitializeFakeData>( );

		var app = builder.BuildApp( );

		if ( app.Environment.IsDevelopment( ) ) {
			app.UseSwagger( );
			app.UseSwaggerUI( );
		}

		app.UseGuard( );

		app.UseHttpsRedirection( );

		app.UseAuthorization( );

		app.MapControllers( );

		app.Run( );
	}
}
