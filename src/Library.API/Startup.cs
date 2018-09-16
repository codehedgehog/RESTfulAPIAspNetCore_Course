﻿namespace Library.API
{
	using Library.API.DbContexts;
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
			services.AddMvc(setupAction: options =>
			{
				options.ReturnHttpNotAcceptable = true;
				options.OutputFormatters.Add(new XmlDataContractSerializerOutputFormatter());
				options.InputFormatters.Add(new XmlDataContractSerializerInputFormatter(options));
			})
			.SetCompatibilityVersion(version: CompatibilityVersion.Version_2_1);
			services.AddDbContext<LibraryContext>(o => o.UseSqlServer(Configuration.GetConnectionString("libraryDbContext")), contextLifetime: ServiceLifetime.Scoped, optionsLifetime: ServiceLifetime.Scoped);
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
				cfg.CreateMap<Entities.Author, Models.AuthorDto>()
						.ForMember(dest => dest.Name, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"))
						.ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.DateOfBirth.GetCurrentAge()));
				cfg.CreateMap<Entities.Book, Models.BookDto>();
				cfg.CreateMap<Models.AuthorForCreationDto, Entities.Author>();

				cfg.CreateMap<Models.BookForCreationDto, Entities.Book>();
				cfg.CreateMap<Models.BookForUpdateDto, Entities.Book>();
				cfg.CreateMap<Entities.Book, Models.BookForUpdateDto>();
			});
			libraryContext.EnsureSeedDataForContext();
			app.UseMvc();
		}
	}
}