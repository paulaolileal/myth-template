namespace Myth.Template.Domain.Models {

	/// <summary>
	/// Enumeration representing different weather condition summaries.
	/// Provides a range of weather descriptions from freezing cold to scorching hot.
	/// </summary>
	public enum Summary {
		/// <summary>
		/// Extremely cold conditions, typically below freezing point.
		/// </summary>
		Freezing,
		
		/// <summary>
		/// Cold and invigorating weather conditions.
		/// </summary>
		Bracing,
		
		/// <summary>
		/// Noticeably cold but not extremely cold conditions.
		/// </summary>
		Chilly,
		
		/// <summary>
		/// Pleasantly cold weather conditions.
		/// </summary>
		Cool,
		
		/// <summary>
		/// Moderate and comfortable temperature conditions.
		/// </summary>
		Mild,
		
		/// <summary>
		/// Comfortably hot weather conditions.
		/// </summary>
		Warm,
		
		/// <summary>
		/// Pleasant and soothing warm weather conditions.
		/// </summary>
		Balmy,
		
		/// <summary>
		/// High temperature conditions.
		/// </summary>
		Hot,
		
		/// <summary>
		/// Extremely hot and humid conditions.
		/// </summary>
		Sweltering,
		
		/// <summary>
		/// Intensely hot conditions, typically the highest temperature range.
		/// </summary>
		Scorching
	}
}