
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

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{

	options.SwaggerDoc("v1", new OpenApiInfo
	{
		Title = "WZDemo API",
		Version = "v1"
	});

	// �b Swagger ���K�[ JWT ��������
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

// �`�JDBContext
builder.Services.AddDbContext<WZDemoDBContext>(Options =>
Options.UseSqlServer(builder.Configuration.GetConnectionString("ConnectionString")));

builder.Services.AddDbContext<WZAuthDBContext>(Options =>
Options.UseSqlServer(builder.Configuration.GetConnectionString("AuthConnectionString")));

// �N�����M Repository �������U��̿�`�J�]Dependency Injection, DI�^�e�����A�ë��w���̪��ͩR�g���� Scoped�C
builder.Services.AddScoped<IRegionRepository, SQLRegionRepository>();
builder.Services.AddScoped<IWalkRepository, SQLWalkRepository>();
builder.Services.AddScoped<ITokenRepository, TokenRepository>();

// Add AutoMapper
builder.Services.AddAutoMapper(typeof(AutoMapperProfiles));

// Add IdentityCore
builder.Services.AddIdentityCore<IdentityUser>() // �K�[�֤ߨ������ҪA�ȡA�ë��w�ϥ� IdentityUser �@���Τ�����
	.AddRoles<IdentityRole>() // �K�[����޲z�A�ȡA�ë��w�ϥ� IdentityRole �@�����������C
	.AddTokenProvider<DataProtectorTokenProvider<IdentityUser>>("WZDemo") // �K�[�O�P���ѵ{�ǡA�ë��w�ϥ� DataProtectorTokenProvider �@���O�P���ѵ{�ǡC"WZDemo" �O�O�P���Ѫ̪��W�١C
	.AddEntityFrameworkStores<WZAuthDBContext>() // �K�[ Entity Framework �s�x�A�ë��w�ϥ� WZAuthDBContext �@���ƾڮw�W�U��C
	.AddDefaultTokenProviders(); // �K�[�q�{�O�P���ѵ{�ǡC

builder.Services.Configure<IdentityOptions>(options =>
{
	options.Password.RequireDigit = false; // �K�X�����ݭn�]�t�Ʀr
	options.Password.RequireLowercase = false; // �K�X�����ݭn�]�t�p�g�r��
	options.Password.RequireNonAlphanumeric = false; // �K�X�����ݭn�]�t�D�r���Ʀr�r��
	options.Password.RequireUppercase = false; // �K�X�����ݭn�]�t�j�g�r��
	options.Password.RequiredLength = 6; // �K�X�ܤֻݭn�]�t 6 �Ӧr��
	options.Password.RequiredUniqueChars = 1; // �K�X���ܤֻݭn�]�t 1 �Ӱߤ@�r��
});

// Add Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
	.AddJwtBearer(options =>
	options.TokenValidationParameters = new TokenValidationParameters
	{
		ValidateIssuer = true, // ���ܬO�_�����ҥO�P���o���
		ValidateAudience = true,// ���ܬO�_�����ҥO�P��������
		ValidateLifetime = true, // ���ܬO�_�����ҥO�P�����Ĵ�
		ValidateIssuerSigningKey = true, // ���ܬO�_�����ҥO�P��ñ�W�K�_
		ValidIssuer = builder.Configuration["Jwt:Issuer"],// ���w���H�����o���
		ValidAudience = builder.Configuration["Jwt:Audience"],// ���w���H����������
		IssuerSigningKey = new SymmetricSecurityKey(
			Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])) // ���w�Ω����ҥO�Pñ�W���K�_�C
	});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Add Authentication
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
