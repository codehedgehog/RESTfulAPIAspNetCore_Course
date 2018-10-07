namespace Library.API.Services
{
	using System.Collections.Generic;

	public class PropertyMapping<TSource, TDestination> : IPropertyMapping
	{
		public Dictionary<string, PropertyMappingValue> _mappingDictionary { get; private set; }

		public PropertyMapping(Dictionary<string, PropertyMappingValue> mappingDictionary)
		{
			_mappingDictionary = mappingDictionary;
		}
	}
}