using System;
using System.Collections.Generic;
using System.Text;

namespace Myth.Template.ExternalData.Breweries.Exceptions;

public class BreweryException : Exception {
	public BreweryException( ) {
	}

	public BreweryException( string? message, Exception exception ) : base( $"Problem accessing OpenBreweryAPI with message: {message}", exception ) {
	}
}
