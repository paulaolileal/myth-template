using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Myth.Template.ExternalData.Breweries.DTOs;

/// <summary>
/// Data transfer object representing a brewery response from external API sources.
/// Contains basic brewery information including identifier and name.
/// </summary>
[ExcludeFromCodeCoverage]
public class BreweryResponseDto {
	/// <summary>
	/// Gets or sets the unique identifier for the brewery.
	/// </summary>
	/// <value>A GUID that uniquely identifies the brewery in the external system.</value>
	public Guid Id { get; set; }

	/// <summary>
	/// Gets or sets the name of the brewery.
	/// </summary>
	/// <value>The display name or business name of the brewery.</value>
	public string Name { get; set; } = null!;
}
