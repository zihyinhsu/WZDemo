
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using WZDemo.API.Data;
using WZDemo.API.Mappings;
using WZDemo.API.repositories;
using WZDemo.API.Repositories;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.FileProviders;
using Serilog;
using WZDemo.API.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var logger = new LoggerConfiguration()
	.WriteTo.Console() // 將日誌輸出到控制台
	.WriteTo.File("Logs/WZDemo_log.txt",rollingInterval:RollingInterval.Day)// 將日誌輸出到文件
	.MinimumLevel.Information() // 設置最低日誌級別為 Information
	.CreateLogger(); // 創建日誌對象

builder.Logging.ClearProviders(); // 清除默認的日誌提供程序
builder.Logging.AddSerilog(logger); // 添加 Serilog 日誌提供程序

builder.Services.AddControllers();
// 用於訪問當前的 HttpContext。HttpContext 包含有關當前 HTTP 請求的所有信息，例如請求頭、請求路徑、用戶信息等。
builder.Services.AddHttpContextAccessor();


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{

	options.SwaggerDoc("v1", new OpenApiInfo
	{
		Title = "WZDemo API",
		Version = "v1"
	});

	// 在 Swagger 中添加 JWT 身份驗證
	options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
	{
		Name = "Authorization",
		In = ParameterLocation.Header,
		Type = SecuritySchemeType.ApiKey,
		Scheme = JwtBearerDefaults.AuthenticationScheme,
	});

	options.AddSecurityRequirement(new OpenApiSecurityRequirement
	{
		{
			new OpenApiSecurityScheme{
			Reference = new OpenApiReference{
				Type = ReferenceType.SecurityScheme,
				Id = JwtBearerDefaults.AuthenticationScheme
			},
			Scheme = "Oauth2",
			Name = JwtBearerDefaults.AuthenticationScheme,
			In = ParameterLocation.Header
		},
			new List<string>()
		}
	});
});

// 注入DBContext
builder.Services.AddDbContext<WZDemoDBContext>(Options =>
Options.UseSqlServer(builder.Configuration.GetConnectionString("ConnectionString")));

builder.Services.AddDbContext<WZAuthDBContext>(Options =>
Options.UseSqlServer(builder.Configuration.GetConnectionString("AuthConnectionString")));

// 將介面和 Repository 類型註冊到依賴注入（Dependency Injection, DI）容器中，並指定它們的生命週期為 Scoped。
builder.Services.AddScoped<IRegionRepository, SQLRegionRepository>();
builder.Services.AddScoped<IWalkRepository, SQLWalkRepository>();
builder.Services.AddScoped<ITokenRepository, TokenRepository>();
builder.Services.AddScoped<IImageRepository, LocalImageRepository>();

// Add AutoMapper
builder.Services.AddAutoMapper(typeof(AutoMapperProfiles));

// Add IdentityCore
builder.Services.AddIdentityCore<IdentityUser>() // 添加核心身份驗證服務，並指定使用 IdentityUser 作為用戶類型
	.AddRoles<IdentityRole>() // 添加角色管理服務，並指定使用 IdentityRole 作為角色類型。
	.AddTokenProvider<DataProtectorTokenProvider<IdentityUser>>("WZDemo") // 添加令牌提供程序，並指定使用 DataProtectorTokenProvider 作為令牌提供程序。"WZDemo" 是令牌提供者的名稱。
	.AddEntityFrameworkStores<WZAuthDBContext>() // 添加 Entity Framework 存儲，並指定使用 WZAuthDBContext 作為數據庫上下文。
	.AddDefaultTokenProviders(); // 添加默認令牌提供程序。

builder.Services.Configure<IdentityOptions>(options =>
{
	options.Password.RequireDigit = false; // 密碼中不需要包含數字
	options.Password.RequireLowercase = false; // 密碼中不需要包含小寫字母
	options.Password.RequireNonAlphanumeric = false; // 密碼中不需要包含非字母數字字符
	options.Password.RequireUppercase = false; // 密碼中不需要包含大寫字母
	options.Password.RequiredLength = 6; // 密碼至少需要包含 6 個字符
	options.Password.RequiredUniqueChars = 1; // 密碼中至少需要包含 1 個唯一字符
});

// Add Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
	.AddJwtBearer(options =>
	options.TokenValidationParameters = new TokenValidationParameters
	{
		ValidateIssuer = true, // 指示是否應驗證令牌的發行者
		ValidateAudience = true,// 指示是否應驗證令牌的接收方
		ValidateLifetime = true, // 指示是否應驗證令牌的有效期
		ValidateIssuerSigningKey = true, // 指示是否應驗證令牌的簽名密鑰
		ValidIssuer = builder.Configuration["Jwt:Issuer"],// 指定受信任的發行者
		ValidAudience = builder.Configuration["Jwt:Audience"],// 指定受信任的接收方
		IssuerSigningKey = new SymmetricSecurityKey(
			Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])) // 指定用於驗證令牌簽名的密鑰。
	});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

// middleware
app.UseMiddleware<ExceptionHandlerMiddleware>();

app.UseHttpsRedirection();

// Add Authentication
app.UseAuthentication();
app.UseAuthorization();

// 讀取靜態文件
app.UseStaticFiles(new StaticFileOptions { 
	FileProvider= new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(),"Images")),
	RequestPath = "/Images"
});
app.MapControllers();

app.Run();
