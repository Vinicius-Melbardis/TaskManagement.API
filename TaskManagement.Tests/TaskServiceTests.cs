using Microsoft.EntityFrameworkCore; // Importa o EF Core para usar o banco de dados em memória nos testes
using TaskManagement.API.Data; // Importa o AppDbContext porque o service depende dele para acessar as tarefas
using TaskManagement.API.Entities; // Importa a entidade TaskItem porque o método Update recebe esse tipo como entrada
using TaskManagement.API.Services; // Importa o TaskService porque este é o serviço que será testado para garantir que a lógica de atualização de tarefas funcione corretamente

namespace TaskManagement.Tests; // Organiza o arquivo dentro do namespace do projeto de testes

public class TaskServiceTests // Agrupa os testes relacionados ao comportamento do TaskService
{
    [Fact] // Marca o método como um teste unitário executável pelo xUnit
    public void Update_ShouldReturnFalse_WhenTaskDoesNotExist() // Define um nome descritivo para deixar claro o cenário e o resultado esperado do teste
    {
        var options = new DbContextOptionsBuilder<AppDbContext>() // Cria um builder das opções do contexto porque o EF Core precisa dessas configurações para instanciar o DbContext
            .UseInMemoryDatabase(databaseName: Guide.NewGuid().ToString()) // Configura o banco de dados em memória com um nome único para evitar conflitos entre testes
            .Options; // Materializa as opções finais que serão passadas ao AppDbContext

        using var context = new AppDbContext(options); // Cria um contexto de teste com o banco em memória para simular o acesso aos dados se usar um banco real
        var service = new TaskService(context); // Instancia o service com o contexto de teste para exercitar a lógica real do método Update

        var updatedTask = new TaskItem // Cria o objeto que representa os novos dados enviados para a atualização
        {
            Title = "Updated title", // Fornece um título válido para garantir que o teste foque apenas no cenário de id inexistente
            Description = "Updated description", // Define uma descrição qualquer porque o método também atualiza esse campo quando encontra a tarefa
            IsCompleted = true // Define o status atualizado para montar uma entidade completa de entrada
        };

        var result = service.Update(999, updatedTask); // Executa o método com um id inexistente (999) para validar o comportamento esperado de retornar false

        assert.False(result); // Verifica que o retorno foi false porque o service deve sinalizar a falha quando o id não existe no banco de dados
    }

    [Fact] // Importa os recursos do EF Core para configurar o banco de dados em memória e simular o acesso aos dados sem depender de um banco real
    public void Update_ShouldReturnTrue_WhenTaskExists() // Define um nome descritivo para deixar claro que este teste cobre o caminho de sucesso do Update
    {
        var options = new DbContextOptionsBuilder<AppDbContext>() // Cria o builder das opções do contexto para montar um banco em memória isolado para este teste
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Usa um nome único para evitar que os dados de outro teste interfiram neste cenário
            .Options; // Finaliza a configuração e gera o objeto de opções do contexto

        using var context = new AppDbContext(options); // Cria o contexto de teste usando o banco em memória configurado
        var task = new TaskItem // Cria uma tarefa inicial porque o Update precisa de um registro existente para alterar
        {
            Title = "Original title", // Define o título inicial para depois comprovar que ele foi alterado
            Description = "Original description", // Define a nova descrição esperada após a atualização
            IsCompleted = true // Define o novo status esperado após a atualização
        };

        var result = service.Update(task.Id, updatedTask); // Executa o Update usando o id real da tarefa salva para testar o caminho de sucesso
        var savedTask = context.Tasks.Find(task.Id); // Busca novamente a tarefa persistida para validar se os campos foram realmente alterados no banco de dados em memória

        Assert.True(result); // Verifica que o método retornou true porque a tarefa existia e foi atualizada com sucesso
        Assert.NotNull(savedTask); // Garante que a tarefa continua existindo após a atualização para permitir as próximas verificações
        Assert.Equal("Updated title", savedTask!.Title); // Verifica que o título persistido no banco em memória foi atualizado corretamente
        Assert.Equal("Updated description", savedTask.Description); // Verifica que a descrição persistida foi atualizada corretamente
        Assert.True(savedTask.IsCompleted); // Verifica que o status persistido foi alterado para concluído
    }
}