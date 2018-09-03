namespace Library.API.Controllers

{
	using AutoMapper;
	using Library.API.Entities;
	using Library.API.Helpers;
	using Library.API.Models;
	using Library.API.Services;
	using Microsoft.AspNetCore.Mvc;
	using System;
	using System.Collections.Generic;
	using System.Linq;

	[Route("api/authorcollections")]
	public class AuthorCollectionsController : Controller
	{
		private ILibraryRepository _libraryRepository;

		public AuthorCollectionsController(ILibraryRepository libraryRepository)
		{
			_libraryRepository = libraryRepository;
		}

		[HttpPost]
		public IActionResult CreateAuthorCollection([FromBody] IEnumerable<AuthorForCreationDto> authorCollection)
		{
			if (authorCollection == null) return BadRequest();
			IEnumerable<Author> authorEntities = Mapper.Map<IEnumerable<Author>>(authorCollection);
			foreach (Author author in authorEntities) { _libraryRepository.AddAuthor(author); }
			if (!_libraryRepository.Save()) { throw new Exception("Creating an author collection failed on save."); }
			IEnumerable<AuthorDto> authorCollectionToReturn = Mapper.Map<IEnumerable<AuthorDto>>(authorEntities);
			string idsAsString = string.Join(",", authorCollectionToReturn.Select(a => a.Id));
			return CreatedAtRoute(routeName: "GetAuthorCollection", routeValues: new { ids = idsAsString }, value: authorCollectionToReturn);
		}

		// (key1,key2, ...)
		[HttpGet("({ids})", Name = "GetAuthorCollection")]
		public IActionResult GetAuthorCollection([ModelBinder(BinderType = typeof(ArrayModelBinder))] IEnumerable<Guid> ids)
		{
			if (ids == null) { return BadRequest(); }
			IEnumerable<Author> authorEntities = _libraryRepository.GetAuthors(ids);
			if (ids.Count() != authorEntities.Count()) { return NotFound(); }
			IEnumerable<AuthorDto> authorsToReturn = Mapper.Map<IEnumerable<AuthorDto>>(authorEntities);
			return Ok(authorsToReturn);
		}
	}
}