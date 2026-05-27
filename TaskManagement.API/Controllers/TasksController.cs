using Microsoft.AspNetCore.Mvc; // Importa as classes necessárias para criar um Controller e definir rotas HTTP da API
using TaskManagement.API.DTOs; // Importa os DTOs porque o controller vai receber e devolver contratos da API
using TaskManagement.API.Entities; // Importa TaskItem porque o endpoint de criação de tarefas precisa receber um objeto desse tipo no corpo da requisição
using TaskManagement.API.Services; // Importa TaskService porque o Controller vai depender dele para centralizar a lógica de negócio das tarefas e manter o Controller mais limpo

namespace TaskManagement.API.Controllers; // Organiza o controller na camada responsável pelas rotas HTTP

[ApiController] // Ativa comportamentos automáticos de API como binding de modelos e validação de entrada
[Route("api/v1/tasks")] // Define a rota base para os endpoints de tarefas
public class TasksController : ControllerBase // Herda de ControllerBase para ter acesso a métodos de resposta HTTP, porque essa classe fornece suporte a APIs sem view
{
    private readonly TaskService _taskService; // Guarda a dependência para reutilizar o service dentro dos métodos do controller

    public TasksController(TaskService taskService) // Recebe o service por injeção de dependência em vez de criar manualmente
    {
        _taskService = taskService; // Atribui a dependência recebida ao campo privado do controller para uso posterior
    }

    [HttpPost] // Mapeia este método para requisições HTTP POST na rota api/v1/tasks
    [ProducesResponseType(typeof(TaskResponseDto), StatusCodes.Status201Created)] // Documenta no Swagger que o retorno de sucesso cria um recurso e devolve um DTO com os dados da tarefa criada
    [ProducesResponseType(StatusCodes.Status400BadRequest)] // Documenta um possível retorno de erro de validação/regra de negócio
    public ActionResult<TaskResponseDto> Create(CreateTaskDto dto) // Recebe um DTO para evitar expor a entidade do banco de dados como contrato de entrada
    {
        var task = new TaskItem // Converte o DTO de entrada em entidade para manter a persistência separada do contrato HTTP
        {
            Title = dto.Title, // Copia o título validado pelo DTO para a entidade que será salva no banco de dados
            Description = dto.Description, // Copia a descrição informada para a entidade preservando o contrato de entrada
        };

        _taskService.Create(task); // Delega a criação para a camada de serviço para manter a regra de negócio centralizada e o controller focado em lidar com HTTP

        var response = new TaskResponseDto // Converte a entidade persistida em DTO para devolver apenas os dados necessários ao cliente
        {
            Id = task.Id, // Retorna o identificador gerado para permitir referência futura ao recurso criado
            Title = task.Title, // Retorna o título persistido para confirmar os dados salvos
            Description = task.Description, // Retorna a descrição persistida para manter a consistêncua da resposta
            IsCompleted = task.IsCompleted, // Retorna o estado atual da tarefa para o cliente conhecer a situação inicial do recurso
            CreatedAt = task.CreatedAt // Retorna a data de criação para dar contexto temporal ao registro criado
        };

        return CreatedAtAction(nameof(GetById), new { id = task.Id }, response); // Retorna 201 com o link do recurso recém-criado seguindo a convenção REST
    }
    
    [HttpGet] // Expõe este método como GET /api/v1/tasks para retornar todas as tarefas
    [ProducesResponseType(typeof(List<TaskItem>), StatusCodes.Status200OK)] // Documenta no Swagger o retorno de sucesso da listagem
    
    public ActionResult<List<TaskItem>> GetAll() // Retorna a coleção de tarefas cadastradas
    {
        var tasks = _taskService.GetAll(); // Delega a consulta ao service para manter o controller limpo e focado em lidar com requisições e respostas
        return Ok(tasks); // Retorna HTTP 200 com a lista de tarefas serializada em JSON no corpo da resposta
    }

    [HttpGet("{id}")] // Expõe este método como GET /api/v1/tasks/{id} para retornar uma tarefa específica por ID
    [ProducesResponseType(typeof(TaskItem), StatusCodes.Status200OK)] // Documenta no Swagger o retorno de sucesso da busca individual
    [ProducesResponseType(StatusCodes.Status404NotFound)] // Documenta o retorno quando a tarefa não existe
    public ActionResult<TaskItem> GetById(int id) // Recebe o id pela rota e retorna a tarefa correspondente
    {
        var task = _taskService.GetById(id); // Delega a busca ao service para manter o controller focado em HTTP

        if (task is null) // Verifica se nenhuma tarefa foi encontrada para o id informado
        {
            return NotFound(); // Retorna HTTP 404 porque o recurso solicitado não existe
        }
        return Ok(task); // Retorna 200 com a tarefa encontrada no corpo da resposta, usando Ok para seguir a convenção REST de indicar que a requisição foi bem-sucedida e o recurso está sendo retornado
    }

    [HttpPut("{id}")] // Expõe este método como PUT /api/v1/tasks/{id} no Swagger para atualizar uma tarefa existente
    [ProducesResponseType(StatusCodes.Status204NoContent)] // Documenta no Swagger o sucesso da atualização sem corpo de resposta
    [ProducesResponseType(StatusCodes.Status404NotFound)] // Documenta o retorno quando a tarefa não existe
    [ProducesResponseType(StatusCodes.Status400BadRequest)] // Documenta um possível retorno de erro de validação/regra de negócio
    public IActionResult Update(int id, TaskItem task) // Recebe o id pela rota e o estado atualizado no corpo da requisição para atualizar a tarefa correspondente
    {
        var updated = _taskService.Update(id, task); // Delega a atualização ao service para manter o controller focado em HTTP

        if (!updated) // Verifica se o service informou que o recurso não foi encontrado
        {
            return NotFound(); // Retorna 404 porque não existe tarefa com o id informado
        }

        return NoContent(); // Retorna 204 porque a atualização deu certo e não precisa devolver corpo de resposta, seguindo a convenção REST de indicar que a requisição foi bem-sucedida mas não há conteúdo para retornar
    }

    [HttpDelete("{id}")] // Expõe no Swagger este método como DELETE /api/v1/tasks/{id} para remover uma tarefa existente
    [ProducesResponseType(StatusCodes.Status204NoContent)] // Documenta no Swagger o retorno de sucesso da exclusão sem corpo de resposta
    [ProducesResponseType(StatusCodes.Status404NotFound)] // Documenta o retorno quando a tarefa não existe
    public IActionResult Delete(int id) // Recebe o id pela rota e remove a tarefa correspondente
    {
        var deleted = _taskService.Delete(id); // Delega a exclusão ao service para manter o controller focado em HTTP

        if (!deleted) // Verifica se o service informou que o recurso não existia
        {
            return NotFound(); // Retorna 404 porque não há tarefa com esse id
        }

        return NoContent(); // Retorna 204 porque a exclusão foi concluída com sucesso
    }
}