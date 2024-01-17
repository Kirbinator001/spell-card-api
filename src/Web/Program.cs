using SpellCard;
using SimpleInjector;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;

services.AddControllers();
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

var container = new Container();
services.AddSimpleInjector(container, options =>
{
    options.AddAspNetCore()
        .AddControllerActivation();
});

var databaseSettings = builder.Configuration.GetRequiredSection(DatabaseSettings.Name).Get<DatabaseSettings>();

container.RegisterInstance<DatabaseSettings>(databaseSettings);

var app = builder.Build();

app.Services.UseSimpleInjector(container);

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

container.Verify();

app.Run();