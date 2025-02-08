using SimpleInjector;
using MongoDB.Driver;
using Web;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson.Serialization.Conventions;
using Web.Models.Spell;
using Web.Services;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;

services.AddControllers(options =>
{
    options.Filters.Add(new ProducesAttribute("application/json"));
    options.Filters.Add(new ConsumesAttribute("application/json"));
});

services.AddEndpointsApiExplorer();
services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Spell card API",
        Version = "v1"
    });
});

services.AddAutoMapper(typeof(SpellProfile));

services.Configure<RouteOptions>(options => options.LowercaseUrls = true);

var container = new Container();
services.AddSimpleInjector(container, options =>
{
    options.AddAspNetCore()
        .AddControllerActivation();
});

var databaseSettings = builder.Configuration.GetRequiredSection(DatabaseSettings.Name).Get<DatabaseSettings>();

container.RegisterInstance<DatabaseSettings>(databaseSettings);

container.RegisterInstance(new MongoClient(databaseSettings.ConnectionString));
container.Register<SpellService>();

var camelCaseConvention = new ConventionPack { new CamelCaseElementNameConvention() };
ConventionRegistry.Register("CamelCase", camelCaseConvention, type => true);

var app = builder.Build();

app.Services.UseSimpleInjector(container);

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options => {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

container.Verify();

app.Run();