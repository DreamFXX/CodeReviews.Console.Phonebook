using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PhoneGallery.DreamFXX.Data;
using PhoneGallery.DreamFXX.Services;

var builder = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.AddDbContext<PhoneGalleryContext>(options =>
            options.UseSqlServer(hostContext.Configuration.GetConnectionString("DefaultConnectionString")));

        services.AddScoped<PhoneGalleryService>();
    });

var host = builder.Build();

using (var scope = host.Services.CreateScope())
{

    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<PhoneGalleryContext>();
        context.Database.Migrate();

        var phoneGalleryService = services.GetRequiredService<PhoneGalleryService>();
        phoneGalleryService.Start();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"An error has occured while configuring / migrating connection to the database.\nInfo: {ex.Message}");
        throw;
    }
}