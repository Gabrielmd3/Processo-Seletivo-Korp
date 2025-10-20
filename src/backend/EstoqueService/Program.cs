using EstoqueService.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. Adicionar os serviços dos Controladores.
builder.Services.AddControllers();

// 2. Adicionar os serviços do Swagger para documentação da API.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configura a string de conexão com o banco de dados PostgreSQL.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Adiciona o DbContext específico para o serviço de estoque.
builder.Services.AddDbContext<EstoqueDbContext>(options =>
    options.UseNpgsql(connectionString));

// Cria a aplicação.
var app = builder.Build();

// Aplica as migrações pendentes ao iniciar a aplicação.
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<EstoqueDbContext>();
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