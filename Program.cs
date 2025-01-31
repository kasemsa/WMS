using Microsoft.Extensions.FileProviders;
using Microsoft.OpenApi.Models;
using WarehouseManagementSystem.Contract;
using WarehouseManagementSystem.Helper;
using WarehouseManagementSystem.Infrastructure;
using WarehouseManagementSystem.Middlewares;
using WarehouseManagementSystem.Profiles;

var builder = WebApplication.CreateBuilder(args);

GlobalAttributes.sqlConfiguration.connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Add services to the container.
builder.Services.AddPersistenceServices(builder.Configuration);
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Warehouse Management System",
        Version = "v1"
    });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please insert JWT with Bearer into field",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
           {
               new OpenApiSecurityScheme
               {
                   Reference = new OpenApiReference
                   {
                         Type = ReferenceType.SecurityScheme,
                         Id = "Bearer"
                   }
               },
              new string[] { }
           }
        });
});
builder.Services.AddHttpContextAccessor();
AutoMapperConfig.RegisterMappings(builder.Services);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
           Path.Combine(builder.Environment.ContentRootPath, "wwwroot/UploadedFiles")),
    RequestPath = "/UploadedFiles"
});

//app.UseMiddleware<AuthenticationMiddleware>();
app.UseCors("Open");
app.UseHttpsRedirection();

//app.UseAuthorization();
app.UseAuthenticationMiddleware();
app.MapControllers();

app.Run();
