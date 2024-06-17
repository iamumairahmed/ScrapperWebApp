using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer.Infrastructure.Internal;
using MudBlazor;
using MudBlazor.Services;
using ScrapperWebApp;
using ScrapperWebApp.Components;
using ScrapperWebApp.Data;
using ScrapperWebApp.Models;
using ScrapperWebApp.Services;
using ScrapperWebApp.Services.Interfaces;
using System;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();


//builder.Services.AddDbContext<ScrapperWebApp.Models.ScrapperDbContext>(options =>
//{
//    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
//}, ServiceLifetime.Transient);

var cs = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContextFactory<ScrapperDbContext>(options => options.UseSqlServer(cs, options => options.EnableRetryOnFailure()));

builder.Services.AddScoped<IFiltroService, FiltroService>();
builder.Services.AddScoped<IAtividadeService, AtividadeService>();
builder.Services.AddScoped<ICepService, CepService>();
builder.Services.AddScoped<IEmpresaService, EmpresaService>();
builder.Services.AddScoped<IScrapperService, ScrapperService>();
builder.Services.AddScoped<IImportService ,ImportService>();
builder.Services.AddScoped<IExportService ,ExportService>();
builder.Services.AddScoped<INatJurService ,NatJurService>();
builder.Services.AddScoped<IURAService ,URAService>();
builder.Services.AddAutoMapper(typeof(AutoMapperProfile));
builder.Services.AddMudServices();
builder.Services.AddMudServices(config =>
{
    config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomLeft;
    config.SnackbarConfiguration.PreventDuplicates = false;
    config.SnackbarConfiguration.NewestOnTop = false;
    config.SnackbarConfiguration.ShowCloseIcon = true;
    config.SnackbarConfiguration.VisibleStateDuration = 10000;
    config.SnackbarConfiguration.HideTransitionDuration = 500;
    config.SnackbarConfiguration.ShowTransitionDuration = 500;
    config.SnackbarConfiguration.SnackbarVariant = Variant.Filled;
});

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
