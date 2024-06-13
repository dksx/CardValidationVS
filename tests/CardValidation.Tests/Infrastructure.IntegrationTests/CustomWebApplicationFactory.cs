using System.Reflection;
using Microsoft.AspNetCore.Mvc.Testing;

namespace CardValidation.Tests.Infrastructure.IntegrationTests;

public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            /*services.AddEndpointsApiExplorer();
            Assembly assembly = Assembly.GetExecutingAssembly();

            services.AddMediatR(config => config.RegisterServicesFromAssembly(assembly));
            services.AddValidatorsFromAssembly(assembly);*/
        });

        builder.UseEnvironment("Development");
    }
}
