namespace Library.API.Helpers
{
	using System;

	public static class DateTimeOffsetExtensions
	{
		public static int GetCurrentAge(this DateTimeOffset dateTimeOffset)
		{
			DateTime currentDate = DateTime.UtcNow;
			int age = currentDate.Year - dateTimeOffset.Year;

			if (currentDate < dateTimeOffset.AddYears(age))
			{
				age--;
			}

			return age;
		}
	}
}