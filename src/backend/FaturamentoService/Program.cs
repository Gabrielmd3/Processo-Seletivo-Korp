using FaturamentoService.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// --- CONFIGURAÇÃO DOS SERVIÇOS ---

// 1. Adicionar o serviço que registra e habilita o uso de Controladores na aplicação.
builder.Services.AddControllers().AddJsonOptions(options =>
    {
        // Esta linha faz a mágica: converte enums para strings no JSON
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });;

// 2. Adicionar os serviços padrão do Swagger que funcionam com Controladores.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Pega a string de conexão
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Registra o DbContext com o provedor do PostgreSQL.
builder.Services.AddDbContext<FaturamentoDbContext>(options =>
    options.UseNpgsql(connectionString));

//Configura o IHttpClientFactory para se comunicar com o EstoqueService

builder.Services.AddHttpClient("EstoqueService", client =>
{
    var estoqueApiUrl = builder.Configuration["ServiceUrls:EstoqueApi"];
    if (string.IsNullOrEmpty(estoqueApiUrl))
    {
        throw new InvalidOperationException("URL do EstoqueService não configurada.");
    }
    client.BaseAddress = new Uri(estoqueApiUrl);
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<FaturamentoDbContext>();
    dbContext.Database.Migrate();
}

// Configura o pipeline de requisições HTTP.
if (app.Environment.IsDevelopment())
{
    // 3. Adicionar o middleware que gera a página do Swagger UI.
    app.UseSwagger();
    app.UseSwaggerUI();
}

// 4. Adicionar o middleware que mapeia as rotas para os Controladores.
app.MapControllers();

app.Run();