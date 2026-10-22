using Booking.API.Data;
using Microsoft.EntityFrameworkCore;
using Shared.Common.Auth;
using Shared.Common.Swagger;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<BookingDbContext>(o => o.UseInMemoryDatabase("BookingDb"));
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddAuthorization();
builder.Services.AddSwaggerWithJwt("Booking API");
builder.Services.AddControllers();

var app = builder.Build();

if (app.Environment.IsDevelopment()) { app.UseSwagger(); app.UseSwaggerUI(); }

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
