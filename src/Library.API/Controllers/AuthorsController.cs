namespace Library.API.Controllers
{
	using AutoMapper;
	using Library.API.Entities;
	using Library.API.Helpers;
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
		#region Private Fields

		private ILibraryRepository _libraryRepository;
		private IUrlHelper _urlHelper;
		private IPropertyMappingService _propertyMappingService;
		private ITypeHelperService _typeHelperService;

		private const int maxAuthorPageSize = 20;

		#endregion Private Fields

		#region Constructors

		public AuthorsController(ILibraryRepository libraryRepository, IUrlHelper urlHelper, IPropertyMappingService propertyMappingService, ITypeHelperService typeHelperService)
		{
			_libraryRepository = libraryRepository;
			_urlHelper = urlHelper;
			_propertyMappingService = propertyMappingService;
			_typeHelperService = typeHelperService;
		}

		#endregion Constructors

		#region Public Actions

		[HttpGet(Name = "GetAuthors")]
		public async Task<IActionResult> GetAuthors(AuthorsResourceParameters authorsResourceParameters)
		{
			// throw new Exception("Random exception for testing purpose");
			if (!_propertyMappingService.ValidMappingExistsFor<AuthorDto, Author>(authorsResourceParameters.OrderBy)) { return BadRequest(); }
			if (!_typeHelperService.TypeHasProperties<AuthorDto>(authorsResourceParameters.Fields)) { return BadRequest(); }
			PagedList<Author> authorsFromRepo = await Task.FromResult(_libraryRepository.GetAuthors(authorsResourceParameters));
			string previousPageLink = authorsFromRepo.HasPrevious ? CreateAuthorsResourceUri(authorsResourceParameters, ResourceUriType.PreviousPage) : null;
			string nextPageLink = authorsFromRepo.HasNext ? CreateAuthorsResourceUri(authorsResourceParameters, ResourceUriType.NextPage) : null;
			var paginationMetadata = new
			{
				totalCount = authorsFromRepo.TotalCount,
				pageSize = authorsFromRepo.PageSize,
				currentPage = authorsFromRepo.CurrentPage,
				totalPages = authorsFromRepo.TotalPages,
				previousPageLink,
				nextPageLink
			};
			Response.Headers.Add("X-Pagination", Newtonsoft.Json.JsonConvert.SerializeObject(paginationMetadata));
			IEnumerable<AuthorDto> authors = Mapper.Map<IEnumerable<AuthorDto>>(authorsFromRepo);
			return Ok(authors.ShapeData(authorsResourceParameters.Fields));
		}

		[HttpGet(template: "{id}", Name = "GetAuthor")]
		public async Task<IActionResult> GetAuthor(Guid id, [FromQuery] string fields)
		{
			if (!_typeHelperService.TypeHasProperties<AuthorDto>(fields)) { return BadRequest(); }
			Author authorFromRepo = await Task.FromResult(_libraryRepository.GetAuthor(id));
			if (authorFromRepo == null) { return NotFound(); }
			AuthorDto author = Mapper.Map<AuthorDto>(authorFromRepo);
			return Ok(author.ShapeData(fields));
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

		#endregion Public Actions

		#region Private Funcationss

		private string CreateAuthorsResourceUri(AuthorsResourceParameters authorsResourceParameters, ResourceUriType type)
		{
			switch (type)
			{
				case ResourceUriType.PreviousPage:
					return _urlHelper.Link("GetAuthors",
						new
						{
							fields = authorsResourceParameters.Fields,
							orderBy = authorsResourceParameters.OrderBy,
							searchQuery = authorsResourceParameters.SearchQuery,
							genre = authorsResourceParameters.Genre,
							pageNumber = authorsResourceParameters.PageNumber - 1,
							pageSize = authorsResourceParameters.PageSize
						});

				case ResourceUriType.NextPage:
					return _urlHelper.Link("GetAuthors",
						new
						{
							fields = authorsResourceParameters.Fields,
							orderBy = authorsResourceParameters.OrderBy,
							searchQuery = authorsResourceParameters.SearchQuery,
							genre = authorsResourceParameters.Genre,
							pageNumber = authorsResourceParameters.PageNumber + 1,
							pageSize = authorsResourceParameters.PageSize
						});

				default:
					return _urlHelper.Link("GetAuthors",
					new
					{
						fields = authorsResourceParameters.Fields,
						orderBy = authorsResourceParameters.OrderBy,
						searchQuery = authorsResourceParameters.SearchQuery,
						genre = authorsResourceParameters.Genre,
						pageNumber = authorsResourceParameters.PageNumber,
						pageSize = authorsResourceParameters.PageSize
					});
			}
		}

		#endregion Private Funcationss
	}
}