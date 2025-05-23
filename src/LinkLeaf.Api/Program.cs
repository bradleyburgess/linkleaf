using LinkLeaf.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAppRepositoriesAndServices(builder.Configuration);
builder.Services.AddJwtMiddleware(builder.Configuration);

builder.WebHost.ConfigureKestrel(serverOptions =>
{
    var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";
    serverOptions.ListenAnyIP(int.Parse(port));
});

var app = builder.Build();

app.UseServices();
app.MigrateDatabase();

app.Run();
