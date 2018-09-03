namespace Library.API.Controllers
{
	using AutoMapper;
	using Library.API.Entities;
	using Library.API.Models;
	using Library.API.Services;
	using Microsoft.AspNetCore.Http;
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

		[HttpPost]
		public async Task<IActionResult> CreateAuthor([FromBody] AuthorForCreationDto author)
		{
			if (author == null) return BadRequest();
			Author authorEntity = Mapper.Map<Author>(author);
			_libraryRepository.AddAuthor(authorEntity);
			if (!await _libraryRepository.SaveAsync())
			{
				throw new Exception("Creating an author failed on save.");
				// return StatusCode(500, "A problem happened with handling your request.");
			}
			AuthorDto authorToReturn = Mapper.Map<AuthorDto>(authorEntity);
			return CreatedAtRoute(routeName: "GetAuthor", routeValues: new { id = authorToReturn.Id }, value: authorToReturn);
		}

		[HttpPost("{id}")]
		public async Task<IActionResult> BlockAuthorCreation(Guid id)
		{
			if (_libraryRepository.AuthorExists(id)) { return await Task.FromResult(new StatusCodeResult(StatusCodes.Status409Conflict)); }
			return NotFound();
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteAuthor(Guid id)
		{
			Author authorFromRepo = _libraryRepository.GetAuthor(id);
			if (authorFromRepo == null) return NotFound();
			_libraryRepository.DeleteAuthor(authorFromRepo);
			if (!(await _libraryRepository.SaveAsync()))
			{
				throw new Exception($"Deleting author {id} failed on save.");
			}
			return NoContent();
		}
	}
}