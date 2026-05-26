using Microsoft.EntityFrameworkCore; // Importa o EF Core porque o UseSQLServer vem dessa biblioteca
using TaskManagement.API.Data; // Importa o AppDbContext para registrá-lo na aplicação
using TaskManagement.API.Services; // Importa o TaskService para o Program.cs conseguir registrá-lo como um serviço e injetá-lo nos Controllers quando necessário

var builder = WebApplication.CreateBuilder(args); // Cria o builder principal que configura serviços e Pipeline da API

builder.Services.AddControllers(); // Ativa o uso de Controllers porque a API será organizada por classes e rotas
builder.Services.AddEndpointsApiExplorer(); // Permite que o Swagger descubra os endpoints criados pelos Controllers
builder.Services.AddSwaggerGen(); // Gera documentação interativa da API usando Swagger para teste no navegador
builder.Services.AddDbContext<AppDbContext>(options =>

    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")
    )); // Lê a string de conexão do appsettings.json e conecta o EF Core ao SQL Server usando o AppDbContext para gerenciar o banco de dados
builder.Services.AddScoped<TaskService>(); // Registra o TaskService por requisição para que ele possa ser injetado nos Controllers e centralizar a lógica de negócio das tarefas

var app = builder.Build(); // Constrói a aplicação com todos os serviços e configurações definidas

if (app.Environment.IsDevelopment()) // Executa esse bloco apenas se estiver em ambiente de desenvolvimento para facilitar testes e depuração
{
    app.UseSwagger(); // Ativa o endpoint do Swagger com a descrição interativa da API
    app.UseSwaggerUI(); // Ativa a interface do Swagger para testar os endpoints diretamente no navegador
}

app.UseHttpsRedirection(); // Redireciona chamadas HTTP para HTTPS para garantir segurança na comunicação

app.UseAuthorization(); // Deixa a pipeline pronta para regras de autorização quando adicionar autenticação

app.MapControllers(); // Mapeia os Controllers para que as rotas realmente funcionem e respondam às requisições

app.Run(); // Inicia a aplicação e deixa a API escutando as requisições dos clientes

// O Program.cs é onde o ASP.NET Core aprende quais serviços existem e como montar a aplicação em tempo de execução. Ele é o ponto de entrada da aplicação e é responsável por configurar os serviços, o pipeline de requisições e iniciar a aplicação.