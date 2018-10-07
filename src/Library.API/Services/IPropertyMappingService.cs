namespace Library.API.Services
{
	using System.Collections.Generic;

	public interface IPropertyMappingService
	{
		bool ValidMappingExistsFor<TSource, TDestination>(string fields);

		Dictionary<string, PropertyMappingValue> GetPropertyMapping<TSource, TDestination>();
	}
}