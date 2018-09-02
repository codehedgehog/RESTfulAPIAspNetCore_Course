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

	[Route("api/authors/{authorId}/books")]
	public class BooksController : Controller
	{
		private ILibraryRepository _libraryRepository;

		public BooksController(ILibraryRepository libraryRepository)
		{
			_libraryRepository = libraryRepository;
		}

		[HttpGet()]
		public async Task<IActionResult> GetBooksForAuthor(Guid authorId)
		{
			if (!_libraryRepository.AuthorExists(authorId)) return NotFound();
			IEnumerable<Book> booksForAuthorFromRepo = await Task.FromResult(_libraryRepository.GetBooksForAuthor(authorId));
			IEnumerable<BookDto> booksForAuthor = Mapper.Map<IEnumerable<BookDto>>(booksForAuthorFromRepo);
			return Ok(booksForAuthor);
		}

		[HttpGet("{id}")]
		public async Task<IActionResult> GetBookForAuthor(Guid authorId, Guid id)
		{
			if (!_libraryRepository.AuthorExists(authorId)) return NotFound();
			Book bookForAuthorFromRepo = await Task.FromResult(_libraryRepository.GetBookForAuthor(authorId, id));
			if (bookForAuthorFromRepo == null) return NotFound();
			BookDto bookForAuthor = Mapper.Map<BookDto>(bookForAuthorFromRepo);
			return Ok(bookForAuthor);
		}
	}
}