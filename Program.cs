using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using SchoolERP.Net.Data;
using SchoolERP.Net.Utilities;
using SchoolERP.Net.Services;
using SchoolERP.Net.Services.Clients;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddMemoryCache();
builder.Services.AddHttpContextAccessor();

// Register Custom Helpers
builder.Services.AddSingleton<SqlHelper>();
builder.Services.AddSingleton<JwtHelper>();
builder.Services.AddSingleton<EncryptionHelper>();
builder.Services.AddScoped<ILocalizationService, LocalizationService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IMenuService, MenuService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserManagementService, UserManagementService>();
builder.Services.AddScoped<IOrganisationService, OrganisationService>();
builder.Services.AddScoped<ICompanyService, CompanyService>();
builder.Services.AddScoped<ISessionService, SessionService>();
builder.Services.AddScoped<ICurrencyService, CurrencyService>();
builder.Services.AddScoped<ILanguageService, LanguageService>();
builder.Services.AddScoped<IEmailConfigService, EmailConfigService>();
builder.Services.AddScoped<ISmsConfigService, SmsConfigService>();
builder.Services.AddScoped<IPaymentMethodService, PaymentMethodService>();


// Configure API Clients
var apiBaseUrl = builder.Configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7237/"; // Default fallback

// Add AuthClient
builder.Services.AddHttpClient<IAuthClientService, AuthClientService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
});

// Add UserClient
builder.Services.AddHttpClient<IUserClientService, UserClientService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
});

// Add RoleClient
builder.Services.AddHttpClient<IRoleClientService, RoleClientService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
});

// Add UserTypeClient
builder.Services.AddHttpClient<IUserTypeClientService, UserTypeClientService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
});

// Add MenuClient
builder.Services.AddHttpClient<IMenuClientService, MenuClientService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
});

// Add SettingsClient
builder.Services.AddHttpClient<ISettingsClientService, SettingsClientService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
});

// Add UtilityClient
builder.Services.AddHttpClient<IUtilityClientService, UtilityClientService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
});

// Add OrganisationClient
builder.Services.AddHttpClient<IOrganisationClientService, OrganisationClientService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
});

builder.Services.AddHttpClient<ICompanyClientService, CompanyClientService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
});

builder.Services.AddHttpClient<ISessionClientService, SessionClientService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
});

// Add CurrencyClient
builder.Services.AddHttpClient<ICurrencyClientService, CurrencyClientService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
});

// Add LanguageClient
builder.Services.AddHttpClient<ILanguageClientService, LanguageClientService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
});

builder.Services.AddHttpClient<IEmailConfigClientService, EmailConfigClientService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
});

builder.Services.AddHttpClient<ISmsConfigClientService, SmsConfigClientService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
});

builder.Services.AddHttpClient<IPaymentMethodClientService, PaymentMethodClientService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
});


// Configure JWT Authentication
var jwtKey = builder.Configuration["Jwt:Key"] ?? "SchoolERP_Default_Key_1234567890";
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseMiddleware<LocalizationMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Login}/{id?}");

app.Run();
