namespace Library.API
{
	using Library.API.Entities;
	using Library.API.Helpers;
	using Library.API.Services;
	using Microsoft.AspNetCore.Builder;
	using Microsoft.AspNetCore.Hosting;
	using Microsoft.AspNetCore.Http;
	using Microsoft.AspNetCore.Mvc;
	using Microsoft.AspNetCore.Mvc.Formatters;
	using Microsoft.EntityFrameworkCore;
	using Microsoft.Extensions.Configuration;
	using Microsoft.Extensions.DependencyInjection;
	using Microsoft.Extensions.Logging;

	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		public void ConfigureServices(IServiceCollection services)
		{
			services.AddMvc(setupAction: setupAction =>
			{
				setupAction.ReturnHttpNotAcceptable = true;
				setupAction.OutputFormatters.Add(new XmlDataContractSerializerOutputFormatter());
			})
				.SetCompatibilityVersion(version: CompatibilityVersion.Version_2_1);
			services.AddDbContext<LibraryContext>(o => o.UseSqlServer(Configuration.GetConnectionString("libraryDbContext")), contextLifetime: ServiceLifetime.Scoped, optionsLifetime: ServiceLifetime.Scoped);
			// register the repository
			services.AddScoped<ILibraryRepository, LibraryRepository>();
		}

		public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, LibraryContext libraryContext)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			else
			{
				app.UseExceptionHandler(appBuilder =>
				{
					appBuilder.Run(async context =>
					{
						context.Response.StatusCode = 500;
						await context.Response.WriteAsync("An unexpected fault happened. Try again later.");
					});
				});
				app.UseHsts();
			}

			app.UseHttpsRedirection();

			AutoMapper.Mapper.Initialize(cfg =>
			{
				cfg.CreateMap<Author, Models.AuthorDto>()
						.ForMember(dest => dest.Name, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"))
						.ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.DateOfBirth.GetCurrentAge()));
				cfg.CreateMap<Book, Models.BookDto>();
			});
			libraryContext.EnsureSeedDataForContext();
			app.UseMvc();
		}
	}
}