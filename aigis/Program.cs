using aigis.DAL;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Data;

var builder = WebApplication.CreateBuilder(args);

// Configuración de CORS para permitir solicitudes desde todos los orígenes
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder =>
        {
            builder
                .AllowAnyOrigin()  // Considera restringir los orígenes en producción
                .AllowAnyMethod()
                .AllowAnyHeader();
        });
});

builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();

// Agregar servicios de Swagger para la documentación de la API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<AyudaUsuariosDAL>();



// Registrar el servicio de logging
builder.Services.AddLogging();

var app = builder.Build();

// Configuración del middleware de la aplicación
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1"));
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Aplicar la política de CORS antes de los demás middlewares que necesiten CORS
app.UseCors("AllowAllOrigins");

app.UseAuthentication(); // Si estás usando autenticación
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
