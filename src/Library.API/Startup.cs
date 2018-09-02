namespace Library.API
{
	using Library.API.Services;
	using Microsoft.AspNetCore.Builder;
	using Microsoft.AspNetCore.Hosting;
	using Microsoft.AspNetCore.Mvc;
	using Microsoft.Extensions.Configuration;
	using Microsoft.Extensions.DependencyInjection;

	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
			//var connectionString = Configuration["connectionStrings:libraryDBConnectionString"];
			//services.AddDbContext<LibraryContext>(o => o.UseSqlServer(connectionString));
			// register the repository
			services.AddScoped<ILibraryRepository, LibraryRepository>();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			else
			{
				app.UseExceptionHandler();
				app.UseHsts();
			}

			app.UseHttpsRedirection();
			//libraryContext.EnsureSeedDataForContext();
			app.UseMvc();
		}
	}
}