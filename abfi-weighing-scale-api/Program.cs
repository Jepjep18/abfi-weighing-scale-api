using abfi_weighing_scale_api.DependencyInjection;
using abfi_weighing_scale_api.Utilities;
using Microsoft.AspNetCore.Http.Features;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();

// Add CORS policy for Angular app
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp",
        builder =>
        {
            builder.WithOrigins(
                    "http://localhost:4200",          // Angular dev server
                    "https://localhost:4200"          // Angular dev server with HTTPS
                )
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        });
});

// Enable Swagger + configure IFormFile support
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "ABFI Weighing Scale API", Version = "v1" });
    c.OperationFilter<SwaggerFileOperationFilter>();
});

// Optional: allow large file uploads
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 1073741824; // 1 GB max
});

// Register Application Services (DbContext + Repositories + Services)
builder.Services.AddApplicationServices(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Add CORS middleware - Must be before UseAuthorization() and MapControllers()
app.UseCors("AllowAngularApp");

app.UseAuthorization();
app.MapControllers();
app.Run();