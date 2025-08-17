using Microsoft.EntityFrameworkCore;
using DataAccessLayer.DataContext;
using BusinessObject.Models;
using Repository.Interfaces;
using Repository;
using Microsoft.IdentityModel.Tokens;
using Service.Interfaces;
using Service;
using Repository.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Microsoft.OpenApi.Models;
using SafeCityBackEnd.Helpers;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Builder.Extensions;
using FirebaseAdmin;
using Microsoft.Extensions.FileProviders;
using MediatR;
using BusinessObject.Events;
using Service.EventHandlers;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IGenericRepository<Commune>, GenericRepository<Commune>>();
builder.Services.AddScoped<IProvinceRepository, ProvinceRepository>();
builder.Services.AddScoped<ICommuneRepository, CommuneRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<IIdentityCardRepository, IdentityCardRepository>();
builder.Services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<IPayosTransactionRepository, PayosTransactionRepository>();
builder.Services.AddScoped<ISubscriptionService, SubscriptionService>();
builder.Services.AddScoped<IBlogRepository, BlogRepository>();
builder.Services.AddScoped<IBlogLikeRepository, BlogLikeRepository>();
builder.Services.AddScoped<IBlogMediaRepository, BlogMediaRepository>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();
builder.Services.AddScoped<IBlogModerationRepository, BlogModerationRepository>();
builder.Services.AddScoped<IEscortGroupRepository, EscortGroupRepository>();
builder.Services.AddScoped<IEscortGroupJoinRequestRepository, EscortGroupJoinRequestRepository>();
builder.Services.AddScoped<IEscortJourneyGroupService, EscortJourneyGroupService>();
builder.Services.AddScoped<IProvinceService, ProvinceService>();
builder.Services.AddScoped<ICommentService, CommentService>();
builder.Services.AddScoped<IBlogService, BlogService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<ICommuneService, CommuneService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IMailService, MailService>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IAchievementService, AchievementService>(); 
builder.Services.AddScoped<IAchievementRepository, AchievementRepository>();
builder.Services.AddScoped<IPackageService, PackageService>();
builder.Services.AddScoped<IPackageRepository, PackageRepository>();
builder.Services.AddScoped<IAssignOfficerHistoryRepository, AssignOfficerHistoryRepository>();
builder.Services.AddScoped<IPackageChangeHistoryRepository, PackageChangeHistoryRepository>();
builder.Services.AddScoped<IIncidentReportService, IncidentReportService>();
builder.Services.AddScoped<IIncidentReportRepository, IncidentReportRepository>();
builder.Services.AddScoped<INoteRepository, NoteRepository>();
builder.Services.AddScoped<IMapService, MapService>();
builder.Services.AddHostedService<AchievementCronJob>();
builder.Services.AddHostedService<ReputationResetCronJob>();
builder.Services.AddScoped<IPointHistoryRepository, PointHistoryRepository>();
builder.Services.AddScoped<IPointHistoryService, PointHistoryService>();



builder.Services.AddScoped<IScanningCardService, ScanningCardService>();

builder.Services.AddScoped<IFirebaseStorageService, FirebaseStorageService>();
var firebasePath = builder.Configuration["Firebase:CredentialPath"];
FirebaseApp.Create(new AppOptions()
{
    Credential = GoogleCredential.FromFile(firebasePath)
});

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(PointChangedEvent).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(AchievementCheckerHandler).Assembly);
});

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddSingleton<BlogModerationService>(provider =>
{
    var logger = provider.GetRequiredService<ILogger<BlogModerationService>>();
    var config = provider.GetRequiredService<IConfiguration>();

    var apiKey = config["OpenAI:ApiKey"] ?? Environment.GetEnvironmentVariable("OPENAI_API_KEY");

    if (string.IsNullOrEmpty(apiKey))
    {
        throw new InvalidOperationException("OpenAI API key not found in configuration or environment variables");
    }

    return new BlogModerationService(apiKey, logger);
});


builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var errors = context.ModelState
                .Where(e => e.Value.Errors.Count > 0)
                .ToDictionary(
                    e => e.Key,
                    e => e.Value.Errors.Select(error => error.ErrorMessage).ToArray()
                );

            var response = new
            {
                http_status = StatusCodes.Status400BadRequest,
                time_stamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"),
                message = "Validation Error",
                errors
            };

            return new BadRequestObjectResult(response);
        };
    })
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.DefaultIgnoreCondition =
            System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var key = Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddHttpContextAccessor();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Safe City API", Version = "v1" });
    c.TagActionsBy(api => new[] { api.GroupName });
    c.UseInlineDefinitionsForEnums();
    c.DocInclusionPredicate((name, api) => true);
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme.",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new List<string>()
        }
    });

    c.OperationFilter<AddAuthHeaderOperationFilter>();
});

var app = builder.Build();

app.UseExceptionHandler(_ => { });

app.UseRouting();

if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "app/.well-known")),
    RequestPath = "/.well-known"
});

app.MapControllers();

app.Run();

