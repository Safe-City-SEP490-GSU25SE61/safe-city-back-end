using Microsoft.EntityFrameworkCore;
using DataAccessLayer.DataContext;
using BusinessObject.Models;
using Repository.Interfaces;
using Repository;
using Service.Interfaces;
using Service;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IGenericRepository<District>, GenericRepository<District>>();
builder.Services.AddScoped<IGenericRepository<Ward>, GenericRepository<Ward>>();
builder.Services.AddScoped<IDistrictRepository, DistrictRepository>();
builder.Services.AddScoped<IWardRepository, WardRepository>();
builder.Services.AddScoped<IDistrictService, DistrictService>();
builder.Services.AddScoped<IWardService, WardService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

