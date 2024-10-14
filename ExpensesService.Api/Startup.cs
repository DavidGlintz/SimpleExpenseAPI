using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using ExpensesService.Api.Data;
using ExpensesService.Service.Services;
using ExpensesService.Gateway.Gateway;
using ExpensesService.Gateway.EntityGateway;
using ExpensesService.Gateway.Interfaces;

namespace ExpensesService.Api
{
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
            var settingsSection = Configuration.GetSection("ConnectionStrings");
            var remoteConnectionString = settingsSection.GetValue<string>("DefaultConnection");

            // Add framework services.
            var connecion = services.AddDbContext<ApplicationDbContext>(options =>
                options.UseMySql(remoteConnectionString, ServerVersion.AutoDetect(remoteConnectionString)));

            Console.WriteLine(connecion);

            services.AddControllers();

            services.AddTransient<ServiceManager>();
            services.AddTransient<IExpenseRepository, ExpenseRepository>();
            services.AddTransient<IUserRepository, UserRepository>();

            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "ExpensesService.Api" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ExpensesService.Api v1"));
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers(); // Maps attribute-routed controllers
            });
        }
    }
}
