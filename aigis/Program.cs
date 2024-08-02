using aigis.DAL;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Data;

var builder = WebApplication.CreateBuilder(args);

// Configuraci�n de CORS para permitir solicitudes desde todos los or�genes
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder =>
        {
            builder
                .AllowAnyOrigin()  // Considera restringir los or�genes en producci�n
                .AllowAnyMethod()
                .AllowAnyHeader();
        });
});

builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();

// Agregar servicios de Swagger para la documentaci�n de la API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<AyudaUsuariosDAL>();



// Registrar el servicio de logging
builder.Services.AddLogging();

var app = builder.Build();

// Configuraci�n del middleware de la aplicaci�n
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

// Aplicar la pol�tica de CORS antes de los dem�s middlewares que necesiten CORS
app.UseCors("AllowAllOrigins");

app.UseAuthentication(); // Si est�s usando autenticaci�n
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
