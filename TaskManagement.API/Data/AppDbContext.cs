using Microsoft.EntityFrameworkCore; // Biblioteca principal do EF Core para trabalhar com o banco de dados
using TaskManagement.API.Entities; // Importa sua Entity TaskItem para ser usada no DbContext

namespace TaskManagement.API.Data; // Organiza essa class dentro do namespace da camada de dados da API
public class AppDbContext : DbContext // Herda de DbContext, porque o EF Core precisa dessa class para gerenciar o banco de dados
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) // Recebe as configurações do DbContext via injeção de dependência
    {
    }
    public DbSet<TaskItem> Tasks { get; set; } // Representa a tabela de tarefas no banco de dados, onde cada TaskItem é uma linha nessa tabela
}