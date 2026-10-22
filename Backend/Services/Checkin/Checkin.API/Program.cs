using Checkin.API.Data;
using Microsoft.EntityFrameworkCore;
using Shared.Common.Auth;
using Shared.Common.Swagger;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<CheckinDbContext>(o => o.UseInMemoryDatabase("CheckinDb"));
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddAuthorization();
builder.Services.AddSwaggerWithJwt("Check-In API");
builder.Services.AddControllers();

var app = builder.Build();

if (app.Environment.IsDevelopment()) { app.UseSwagger(); app.UseSwaggerUI(); }

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
