namespace Library.API.Controllers
{
	using AutoMapper;
	using Library.API.Entities;
	using Library.API.Models;
	using Library.API.Services;
	using Microsoft.AspNetCore.Mvc;
	using System;
	using System.Collections.Generic;
	using System.Threading.Tasks;

	[Route("api/authors")]
	public class AuthorsController : Controller
	{
		private ILibraryRepository _libraryRepository;

		public AuthorsController(ILibraryRepository libraryRepository)
		{
			_libraryRepository = libraryRepository;
		}

		[HttpGet]
		public async Task<IActionResult> GetAuthors()
		{
			// throw new Exception("Random exception for testing purpose");
			IEnumerable<Author> authorsFromRepo = await Task.FromResult(_libraryRepository.GetAuthors());
			IEnumerable<AuthorDto> authors = Mapper.Map<IEnumerable<AuthorDto>>(authorsFromRepo);
			return Ok(authors);
		}

		[HttpGet(template: "{id}", Name = "GetAuthor")]
		public async Task<IActionResult> GetAuthor(Guid id)
		{
			Author authorFromRepo = await Task.FromResult(_libraryRepository.GetAuthor(id));
			if (authorFromRepo == null) { return NotFound(); }
			AuthorDto author = Mapper.Map<AuthorDto>(authorFromRepo);
			return Ok(author);
		}

	}
}