using com.tweetapp.Middleware;
using com.tweetapp.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System.Configuration;


//Configure Logging here
//Add below code in try catch later

var builder = WebApplication.CreateBuilder(args);
//log configuration

// Add services to the container.
builder.Services.ConfigureCors();
builder.Services.ConfigureMongoDatabase();
builder.Services.ConfigureDependencyInjection();
builder.Services.ConfigureSwagger();
builder.Services.ConfigureAuthentication();
builder.Services.AddControllers().AddNewtonsoftJson();
builder.Services.AddApiVersioning(o => {
    o.ReportApiVersions = true;
    o.AssumeDefaultVersionWhenUnspecified = true;
    o.DefaultApiVersion = new ApiVersion(1, 0);
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddRouting(options => options.LowercaseUrls = true);
builder.Services.ConfigureAppSettings();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});
app.MapControllers();
app.Run();
