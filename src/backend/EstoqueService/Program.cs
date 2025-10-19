using EstoqueService.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// --- CONFIGURAÇÃO DOS SERVIÇOS ---

// 1. Adicionar o serviço que registra e habilita o uso de Controladores na aplicação.
builder.Services.AddControllers();

// 2. Adicionar os serviços padrão do Swagger que funcionam com Controladores.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Pega a string de conexão do appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Registra o DbContext com o provedor do PostgreSQL.
builder.Services.AddDbContext<EstoqueDbContext>(options =>
    options.UseNpgsql(connectionString));

// --- CONSTRUÇÃO DA APLICAÇÃO ---

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<EstoqueDbContext>();
    dbContext.Database.Migrate();
}

// --- CONFIGURAÇÃO DO PIPELINE HTTP ---

// Configura o pipeline de requisições HTTP.
if (app.Environment.IsDevelopment())
{
    // 3. Adicionar o middleware que gera a página do Swagger UI.
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Linha que comentamos para desabilitar o redirecionamento HTTPS
// app.UseHttpsRedirection();

// 4. Adicionar o middleware que mapeia as rotas para os Controladores.
app.MapControllers();

app.Run();