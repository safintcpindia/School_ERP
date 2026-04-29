using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.IdentityModel.Tokens;
using SchoolERP.Net.Filters;
using SchoolERP.Net.Data;
using SchoolERP.Net.Utilities;
using SchoolERP.Net.Services;
using SchoolERP.Net.Services.Clients;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<MenuPermissionAuthorizationFilter>();
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.AddService<MenuPermissionAuthorizationFilter>();
});
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
builder.Services.AddScoped<IUserMenuPermissionService, UserMenuPermissionService>();
builder.Services.AddScoped<IOrganisationService, OrganisationService>();
builder.Services.AddScoped<ICompanyService, CompanyService>();
builder.Services.AddScoped<ISessionService, SessionService>();
builder.Services.AddScoped<ICurrencyService, CurrencyService>();
builder.Services.AddScoped<ILanguageService, LanguageService>();
builder.Services.AddScoped<IEmailConfigService, EmailConfigService>();
builder.Services.AddScoped<ISmsConfigService, SmsConfigService>();
builder.Services.AddScoped<IPaymentMethodService, PaymentMethodService>();
builder.Services.AddScoped<ISectionService, SectionService>();
builder.Services.AddScoped<IClassService, ClassService>();
builder.Services.AddScoped<ISubjectService, SubjectService>();
builder.Services.AddScoped<ISubjectGroupService, SubjectGroupService>();
builder.Services.AddScoped<IFrontOfficeService, FrontOfficeService>();
builder.Services.AddScoped<IHostelService, HostelService>();
builder.Services.AddScoped<IFieldService, FieldService>();
builder.Services.AddScoped<IAccountHeadService, AccountHeadService>();
builder.Services.AddScoped<IAccountEntryService, AccountEntryService>();
builder.Services.AddScoped<IPickupPointService, PickupPointService>();
builder.Services.AddScoped<IRouteService, RouteService>();
builder.Services.AddScoped<IVehicleService, VehicleService>();
builder.Services.AddScoped<IVehicleAssignService, VehicleAssignService>();
builder.Services.AddScoped<IRoutePickupPointService, RoutePickupPointService>();
builder.Services.AddScoped<IHumanResourceService, HumanResourceService>();
builder.Services.AddScoped<SchoolERP.Net.Helpers.PermissionHelper>();


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

builder.Services.AddHttpClient<ISectionClientService, SectionClientService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
});

builder.Services.AddHttpClient<IClassClientService, ClassClientService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
});

builder.Services.AddHttpClient<ISubjectClientService, SubjectClientService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
});

builder.Services.AddHttpClient<ISubjectGroupClientService, SubjectGroupClientService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
});

builder.Services.AddHttpClient<IFrontOfficeClientService, FrontOfficeClientService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
});

builder.Services.AddHttpClient<IHostelClientService, HostelClientService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
});

builder.Services.AddHttpClient<IAccountHeadClientService, AccountHeadClientService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
});

builder.Services.AddHttpClient<IAccountEntryClientService, AccountEntryClientService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
});

builder.Services.AddHttpClient<IPickupPointClientService, PickupPointClientService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
});

builder.Services.AddHttpClient<IRouteClientService, RouteClientService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
});

builder.Services.AddHttpClient<IVehicleClientService, VehicleClientService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
});

builder.Services.AddHttpClient<IVehicleAssignClientService, VehicleAssignClientService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
});

builder.Services.AddHttpClient<IRoutePickupPointClientService, RoutePickupPointClientService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
});

builder.Services.AddHttpClient<IHumanResourceClientService, HumanResourceClientService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
});


// Configure Global Authorization Policy
builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
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

    // Allow JWT auth for normal MVC page loads by reading token from cookie.
    // (Your login page stores the JWT client-side.)
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            if (string.IsNullOrEmpty(context.Token))
            {
                var cookieToken = context.Request.Cookies["token"];
                if (!string.IsNullOrEmpty(cookieToken))
                {
                    context.Token = cookieToken;
                }
            }
            return Task.CompletedTask;
        },
        OnChallenge = context =>
        {
            // If it's not an API request and we're not already heading to Login, redirect to Login page
            var path = context.Request.Path;
            if (!path.StartsWithSegments("/api") && !path.StartsWithSegments("/Auth"))
            {
                context.HandleResponse();
                context.Response.Redirect("/Auth/Login");
            }
            return Task.CompletedTask;
        }
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
app.Use(async (context, next) =>
{
    var path = context.Request.Path;
    var isApiRoute = path.StartsWithSegments("/api", StringComparison.OrdinalIgnoreCase);
    var isAnonymousApiRoute = path.StartsWithSegments("/api/auth/login", StringComparison.OrdinalIgnoreCase);

    if (isApiRoute && !isAnonymousApiRoute && context.User?.Identity?.IsAuthenticated != true)
    {
        await context.ChallengeAsync(JwtBearerDefaults.AuthenticationScheme);
        return;
    }

    await next();
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Login}/{id?}");

app.Run();
