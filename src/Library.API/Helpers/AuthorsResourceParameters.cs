namespace Library.API.Helpers
{
	public class AuthorsResourceParameters
	{
		private const int maxPageSize = 20;
		private int _pageSize = 10;

		public int PageNumber { get; set; } = 1;

		public int PageSize
		{
			get => _pageSize;
			set => _pageSize = (value > maxPageSize) ? maxPageSize : value;
		}

		public string Genre { get; set; }

		public string SearchQuery { get; set; }
	}
}