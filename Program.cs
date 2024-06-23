using Microsoft.OpenApi.Models;
using WebAPIVini.Estudantes;
using WebAPIVini.Estudantes.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc(
        "v1",
        new OpenApiInfo { Title = "API criada para elaboração do PDI", Version = "v1" }
    );
    c.EnableAnnotations();
});
builder.Services.AddScoped<AppDbContext>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1"));
}

app.UseHttpsRedirection();

app.AddRotasEstudantes();

app.Run();
