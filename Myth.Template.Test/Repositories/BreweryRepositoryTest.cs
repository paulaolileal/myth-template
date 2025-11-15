using System;
using System.Collections.Generic;
using System.Text;
using Bogus;
using Myth.Template.ExternalData.Breweries.DTOs;
using Myth.Template.ExternalData.Breweries.Interfaces;

namespace Myth.Template.Test.Repositories;

internal class BreweryRepositoryTest : IBreweryRepository {
	private readonly Faker<BreweryResponseDto> _faker;

	public BreweryRepositoryTest( ) {
		_faker = new Faker<BreweryResponseDto>( )
			.RuleFor( b => b.Id, f => f.Random.Guid( ) )
			.RuleFor( b => b.Name, f => f.Company.CompanyName( ) );
	}

	public Task<BreweryResponseDto> GetRandomBreweryAsync( CancellationToken cancellationToken ) {
		var result = _faker.Generate( );

		return Task.FromResult( result );
	}
}
