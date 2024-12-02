using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Repository;
using Serilog;
using Service;
using Sunlit.Dispatch.Api.Middlewares;
using Sunlit.PointToPoint.Api.Middelwares;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
// 加入appsettings.json的配置
var configuration = builder.Configuration;
// Add services to the container.
builder.Services.AddRepositories();
builder.Services.AddServices();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddDistributedMemoryCache();

var audienceConfig = configuration.GetSection("Audience");
var symmetricKeyAsBase64 = audienceConfig["Secret"];
var keyByteArray = Encoding.ASCII.GetBytes(symmetricKeyAsBase64);
var signingKey = new SymmetricSecurityKey(keyByteArray);

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
   .AddJwtBearer(option =>
   {
       option.TokenValidationParameters = new TokenValidationParameters
       {
           ValidateLifetime = true,
           ValidateAudience = true,
           ValidateIssuer = true,
           ValidAudience = audienceConfig["Audience"],
           ValidIssuer = audienceConfig["Issuer"],//發行人
           ValidateIssuerSigningKey = true,
           ClockSkew = TimeSpan.Zero,
           RequireExpirationTime = true,
           IssuerSigningKey = signingKey,
       };
   });


#region  設定Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1",
        new OpenApiInfo
        {
            Title = "滿好ERP 維護運專案 API",
            Version = "v1",
            Description = "",
            TermsOfService = new Uri("https://www.sunlit.com.tw/abouts.php")
        }
    );

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"本服務採用JWT 進行驗證，因此請透過Login或RefershToken取得JWT Token，並在下面輸入
                                    'Bearer 您取得的Token' ",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
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
                            } ,
                            Scheme = "oaith2",
                            Name="Bearer",
                            In = ParameterLocation.Header,
                        },
                        new List<string>()
                    }
                });

    var dir = new DirectoryInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory));
    foreach (var fi in dir.EnumerateFiles("*.xml"))
    {
        c.IncludeXmlComments(fi.FullName);
    }
    c.EnableAnnotations();
});
#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
var _mb = 1000000;
#region 設定log檔
#if DEBUG
var logConfig = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.File(@"Logs\log-.txt", rollingInterval: RollingInterval.Day, fileSizeLimitBytes: _mb * 5, retainedFileCountLimit: 3);

#else
var logConfig = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.File(@"Logs\log-.txt", rollingInterval: RollingInterval.Hour, fileSizeLimitBytes: _mb * 100, retainedFileCountLimit: 720);
#endif

Log.Logger = logConfig.CreateLogger();
#endregion


#region Cors設定
app.UseCors(policy =>
    policy
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials()
        .WithExposedHeaders("Content-Disposition")
        .SetIsOriginAllowed(origin => true)
);
#endregion

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<JwtMiddleware>();
app.UseMiddleware<LogMiddleware>();
app.UseMiddleware<ExceptionMiddleware>(); ////中介層

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

