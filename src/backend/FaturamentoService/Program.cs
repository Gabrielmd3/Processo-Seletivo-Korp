using FaturamentoService.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// 1. Adicionar os serviços dos Controladores com suporte a enums como strings.
builder.Services.AddControllers().AddJsonOptions(options =>
    {
        // converte enums para strings no JSON
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });;

// 2. Adicionar os serviços do Swagger para documentação da API.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Pega a string de conexão
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Adiciona o DbContext específico para o serviço de faturamento.
builder.Services.AddDbContext<FaturamentoDbContext>(options =>
    options.UseNpgsql(connectionString));

// Configura o HttpClient para se comunicar com o EstoqueService.
builder.Services.AddHttpClient("EstoqueService", client =>
{
    var estoqueApiUrl = builder.Configuration["ServiceUrls:EstoqueApi"];
    if (string.IsNullOrEmpty(estoqueApiUrl))
    {
        throw new InvalidOperationException("URL do EstoqueService não configurada.");
    }
    client.BaseAddress = new Uri(estoqueApiUrl);
});

// Cria a aplicação.
var app = builder.Build();

// Aplica as migrações pendentes ao iniciar a aplicação.
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<FaturamentoDbContext>();
    dbContext.Database.Migrate();
}

// Configura o middleware para o Swagger.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Habilita o roteamento para os controladores.
app.MapControllers();

app.Run();