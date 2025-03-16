using IdeaFrame.Infrastructure.DbContextCustom;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTests
{
    public class InMemoryWebApplicationFactory:WebApplicationFactory<Program>
    {
        private string _dbName;
        public IConfiguration Configuration { get; private set; }

        public InMemoryWebApplicationFactory()
        {
            _dbName = Guid.NewGuid().ToString();

            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true) 
                .AddUserSecrets<InMemoryWebApplicationFactory>(); 

            Configuration = builder.Build();
        }
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            base.ConfigureWebHost(builder);
            builder.ConfigureServices(RemoveOldDbAndAddInMemoryDb);
        }


        private void RemoveOldDbAndAddInMemoryDb(IServiceCollection services)
        {
            var dbContextOptions = this.GetDbContext(services);
            services.Remove(dbContextOptions);

            services.AddDbContext<MyDbContexSqlServer>(optionsForDBcOntext);
        }

        private void optionsForDBcOntext(DbContextOptionsBuilder builder)
        {
            builder.UseInMemoryDatabase(_dbName);
        }


        public MyDbContexSqlServer GetDbContextInstance()
        {
            var scope = Services.CreateScope();
            return scope.ServiceProvider.GetRequiredService<MyDbContexSqlServer>();
        }



        private ServiceDescriptor GetDbContext(IServiceCollection services)
        {
            return services
                        .SingleOrDefault(service => service.ServiceType == typeof(DbContextOptions<MyDbContexSqlServer>));
        }

    }
}
