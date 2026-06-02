using System.Diagnostics.CodeAnalysis;
using Myth.Extensions;
using Myth.Template.API.Extensions;
using Myth.Template.Application;
using Myth.ValueProviders;

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

		// TypeProvider (Myth.DependencyInjection): after the app is built, all assemblies are loaded.
		// Log the total number of application types discovered — this is the same set
		// that AddServiceFromType<T>() and ScanAssemblies() use internally.
		var startupLogger = app.Services.GetRequiredService<ILogger<Program>>( );
		startupLogger.LogInformation(
			"[Myth.DI] TypeProvider discovered {TypeCount} types across {AssemblyCount} application assemblies",
			TypeProvider.ApplicationTypes.Count( ),
			TypeProvider.ApplicationAssemblies.Count( ) );

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
