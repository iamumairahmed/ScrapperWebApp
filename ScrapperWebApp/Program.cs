using Microsoft.EntityFrameworkCore;
using ScrapperWebApp;
using ScrapperWebApp.Components;
using ScrapperWebApp.Data;
using ScrapperWebApp.Repository;
using ScrapperWebApp.Services;
using ScrapperWebApp.Services.Interfaces;
using ScrapperWebApp.UnitOfWork;
using System;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();


builder.Services.AddDbContext<ScrapperWebApp.Models.ScrapperDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
}, ServiceLifetime.Scoped);
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IFiltroService, FiltroService>();
builder.Services.AddScoped<IAtividadeService, AtividadeService>();
builder.Services.AddScoped<ICepService, CepService>();
builder.Services.AddScoped<IEmpresaService, EmpresaService>();
builder.Services.AddScoped<IScrapperService, ScrapperService>();
builder.Services.AddScoped<IImportService ,ImportService>();
builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
