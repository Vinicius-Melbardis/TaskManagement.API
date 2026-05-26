using TaskManagement.API.Data; // Importa o AppDbContext porque o TaskService pode precisar acessar o banco de dados para criar tarefas persistentes.
using TaskManagement.API.Entities; // Importa a entidade TaskItem para o arquivo conseguir usar essa classe
namespace TaskManagement.API.Services; // Organiza esse arquivo na camada de serviços da aplicação
public class TaskService // Cria a classe que vai centralizar a regra de negócio das tarefas
{
    private readonly AppDbContext _context; // Guarda o contexto injetado para reutilizar a mesma unidade de trabalho da requisição e garantir que as operações de banco de dados sejam consistentes
    public TaskService(AppDbContext context) // Recebe o contexto por injeção para evitar criar conexão manualmente
    {
        _context = context; // Atribui o contexto recebido para o service poder persistir dados
    }
    public TaskItem Create(TaskItem task) // Método para criar uma nova tarefa, recebe um objeto do tipo TaskItem como parâmetro
    {
        if (string.IsNullOrWhiteSpace(task.Title)) // Impede que uma tarefa inválida seja criada sem título
        {
            throw new Exception("Title is required."); // Interrope o fluxo quando a regra de negócio é violada, nesse caso, quando o título da tarefa é nulo ou vazio
        }
        task.CreatedAt = DateTime.UtcNow; // Define a data no servidor para não depender do relógio do cliente
        task.IsCompleted = false; // Garante que toda tarefa nova comece como não concluída

        _context.Tasks.Add(task); // Marca a entidade como nova para o EF Core gerar um INSERT no banco de dados
        _context.SaveChanges(); // Persiste a alteração e permite que o banco de dados gere o Id definitivo

        return task; // Retorna a entidade já salva com os valores finais já preenchidos pelo EF Core, como o Id gerado pelo banco de dados
    }

    public List<TaskItem> GetAll() // Retorna todas as tarefas persistidas no banco de dados, para permitir leitura básica do recurso
    {
        return _context.Tasks.ToList(); // Materializa a consulta para devolver uma lista de tarefas pronta ao controller
    }

    public TaskItem? GetById(int id) // Busca uma tarefa pela chave primária para permitir consulta individual
    {
        return _context.Tasks.Find(id); // Usa o método Find do EF Core porque a consulta é por chave primária e pode ser mais direta
    }

    public bool Update(int id, TaskItem updatedTask) // Atualiza uma tarefa existente, para permitir edição completa do recurso
    {
        var existingTask = _context.Tasks.Find(id); // Busca a entidade atual para editar o registro rastreado pelo contexto

        if(existingTask is null) // Interrompe a atualização quando o recurso solicitado não existe
        {
            return false; // Informa ao controller que o id não foi encontrado
        }

        if (string.IsNullOrWhiteSpace(updatedTask.Title)) // Impede que a atualização apague o dado mínimo obrigatório do título
        {
            throw new Exception("Title is required."); // Mantém a mesma regra de negócio da criação para garantir que o recurso continue válido mesmo após a edição
        }

        existingTask.Title = updatedTask.Title; // Atualiza o título da tarefa existente no objeto rastreado para o EF gerar o UPDATE correto
        existingTask.Description = updatedTask.Description; // Atualiza a descrição mantendo a edição no mesmo registro
        existingTask.IsCompleted = updatedTask.IsCompleted; // Permite alterar o status de conclusão da tarefa

        _context.SaveChanges(); // Persiste as alterações no banco de dados porque modificar o objeto sozinho não executa SQL, é necessário salvar as mudanças para que o EF Core gere o comando UPDATE

        return true; // Informa ao controller que a atualização foi concluida com sucesso
    }

    public bool Delete(int id) // Remove uma tarefa existente para completar o ciclo básico de manutenção do recurso
    {
        var existingTask = _context.Tasks.Find(id); // Busca a entidade pela chave primária para garantir que só apague o que existe

        if (existingTask is null) // Verifica se o recurso existe antes de tentar apagar para evitar erros e garantir que a resposta seja adequada
        {
            return false; // Informa ao controller que o id não foi encontrado para que ele possa retornar 404
        }

        _context.Tasks.Remove(existingTask); // Marca a entidade para remoção, o EF Core vai gerar um DELETE no banco de dados
        _context.SaveChanges(); // Persiste a alteração para que o banco de dados execute o comando DELETE
        
        return true; // Informa ao controller que a exclusão foi realizada com sucesso
    }
}