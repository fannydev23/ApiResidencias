using ApiResidencias.Models.Entities;
using ApiResidencias.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Globalization;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
string? dbResidencias = builder.Configuration.GetConnectionString("ResidenciasConnection");

builder.Services.AddDbContext<residenciasContext>(optionsBuilder =>
optionsBuilder.UseMySql(dbResidencias, ServerVersion.AutoDetect(dbResidencias)), ServiceLifetime.Transient);

//JWT

string issuer = "residenciasTEC.sistemas19.com";
string audience = "residenciatec";
var secret = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("TuMiChiquitita83_"));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(jwt =>
{
    jwt.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = issuer,
        ValidAudience = audience,
        IssuerSigningKey = secret,
        ValidateAudience = true,
        ValidateIssuer = true
    };

});


var app = builder.Build();

//RequestLocalizationOptions localizationOptions = new RequestLocalizationOptions
//{
//    SupportedCultures = new List<CultureInfo> { new CultureInfo("es-MX") },
//    SupportedUICultures = new List<CultureInfo> { new CultureInfo("es-MX") },
//    DefaultRequestCulture = new RequestCulture("es-MX"),
//    FallBackToParentCultures = true,
//    FallBackToParentUICultures = true,
//    RequestCultureProviders = null
//};

//CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("es-MX");
//CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("es-MX");
//CultureInfo.CurrentCulture = new CultureInfo("es-MX");
//CultureInfo.CurrentUICulture = new CultureInfo("es-MX");

app.UseAuthorization();

//app.UseRequestLocalization(localizationOptions);

app.UseStaticFiles();
app.MapControllers();

app.UseCors(options =>
{
    options.AllowAnyOrigin()
           .AllowAnyMethod()
           .AllowAnyHeader();
});

app.Run();