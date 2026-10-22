using Search.API.Services;
using Shared.Common.Auth;
using Shared.Common.Swagger;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddAuthorization();
builder.Services.AddSingleton<IFlightDataService, FlightDataService>();
builder.Services.AddSwaggerWithJwt("Search API");
builder.Services.AddControllers();

var app = builder.Build();

if (app.Environment.IsDevelopment()) { app.UseSwagger(); app.UseSwaggerUI(); }

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
