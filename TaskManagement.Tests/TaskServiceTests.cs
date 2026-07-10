using System;
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

    [Fact] // Marca o método como um teste unitário executável pelo xUnit
    public void Update_ShouldThrowException_WhenTitleIsEmpty() // Define um nome descritivo para deixar claro que o teste cobre a validação de título obrigatório no Update
    {
        var options = new DbContextOptionsBuilder<AppDbContext>() // Cria o builder das opções do contexto porque o EF Core precisa dessas configurações para instanciar o banco de dados em memória
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Usa um banco em memória com nome único para garantir isolamento deste teste
            .Options; // Finaliza a configuração e gera o objeto de opções do contexto
        
        using var context = new AppDbContext(options); // Cria o contexto de teste para simular o acesso a dados sem depender de banco real
        var task = new TaskItem // Cria uma tarefa válida inicial porque o Update precisa encontrar um registro existente antes de validar a alteração
        {
            Title = "Original title", // Define um título inicial válidado para a tarefa já persistida
            Description = "Original description", // Define uma descrição inicial apenas para compor a entidade salva
            IsCompleted = false // Define um status inicial para completar o registro que será atualizado
        };

        context.Tasks.Add(task); // Adiciona a tarefa ao contexto para que ela exista no banco em memória antes de tentar atualizá-la
        context.SaveChanges(); // Persiste a tarefa para garantir que o método Update encontre o id informado

        var service = new TaskService(context); // Instancia o service com o contexto que contém a tarefa salva
        var updatedTask = new TaskItem // Cria o objeto com dados inválidos que serão enviados para o Update, especificamente com título vazio para testar a validação
        {
            Title = "", // Define um título vazio para acionar a regra de negócio que exige título obrigatório
            Description = "Updated description", // Define uma descrição qualquer porque o foco do teste está no título inválido
            IsCompleted = true // Define um status qualquer porque o foco do teste não é o campo de conclusão
        };

        Action act = () => service.Update(task.Id, updatedTask); // Encapsula a chamada do método em uma Action para permitir a asserção da exceção no padrão Arrange-Act-Assert do xUnit

        var exception = Assert.Throws<Exception>(act); // Verifica que o método lança uma exceção quando o título enviado está vazio
        Assert.Equal("Title is required.", exception.Message); // Confirma que a mensagem da exceção corresponde exatamente à regra implementada no service
    }

    [Fact] // Marca o método como um teste unitário executável pelo xUnit
    public void Create_ShouldCreateTask_WhenDataIsValid() // Define um nome descritivo para deixar claro que o teste cobre o cenário de sucesso do Create
    {
        var options = new DbContextOptionsBuilder<AppDbContext>() // Cria o builder das opções do contexto porque o EF Core precisa dessa configuração para montar o banco em memória
        .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Usa um banco em memória com nome único para garantir isolamento total deste teste
        .Options; // Finaliza a configuração e gera o objeto de opções que será usado pelo AppDbContext

        using var context = new AppDbContext(options); // Cria o contexto de teste para simular persistência sem usar banco real
        var service = new TaskService(context); // Instancia o service com o contexto de teste para exercitar a lógica real do método Create

        var task = new TaskItem // Cria a tarefa de entrada com os dados válidos para testar o fluxo normal de criação
        {
            Title = "New task", // Define um título válido porque esse campo é obrigatório pela regra de negócio
            Description = "Task description", // Define uma descrição qualquer para compor uma entidade válida de entrada
            IsCompleted = true // Define true de propósito para validar que o método sobrescreve esse valor para false
        };

        var result = service.Create(task); // Executa o método Create para salvar a tarefa e aplicar as regras internas do service
        var savedTask = context.Tasks.Find(result.Id); // Busca a tarefa persistida no banco em memória para validar o que foi realmente salvo

        Assert.NotNull(result); // Verifica que o método retornou uma entidade em vez de null
        Assert.True(result.Id > 0); // Verifica que a tarefa recebeu um Id válido após ser salva no banco em memória
        Assert.Equal("New task", result.Title); // Confirma que o título retornado corresponde ao valor enviado na criação
        Assert.Equal("Task description", result.Description); // Confirma que a descrição retornada foi mantida corretamente
        Assert.False(result.IsCompleted); // Verifica a regra de negócio que força toda tarefa nova a começar como não concluída
        Assert.True(result.CreatedAt <= DateTime.UtcNow); // Verifica que a data de criação foi preenchida pelo service no momento da criação
        Assert.NotNull(savedTask); // Garante que a tarefa foi realmente persistida no contexto de teste e não apenas retornada pelo método Create
        Assert.False(savedTask!.IsCompleted); // Revalida na entidade salva que o banco ficou com IsCompleted igual a false, confirmando a regra de negócio aplicada no service
    }

    [Fact] // Marca o método como um teste unitário executável pelo xUnit
    public void Create_ShouldThrowException_WhenTitleIsEmpty() // Define um nome descritivo para deixar claro que o teste cobre a validação de título obrigatório no Create
    {
        var options = new DbContextOptionsBuilder<AppDbContext>() // Cria o builder das opções do contexto porque o EF Core precisa dessa configuração para montar o banco em memória
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Usa um banco em memória com nome único para garantir que este teste fique isolado dos demais
            .Options; // Finaliza a configuração e gera o objeto de opções que será usado pelo AppDbContext

            using var context = new AppDbContext(options); // Cria o contexto de teste para simular persistência sem usar banco real
            var service = new TaskService(context); // Instancia o service com o contexto de teste para exercitar a lógica real do método Create

            var task = new TaskItem // Cria a tarefa de entrada com dados inválidos para acionar a regra de negócio
            {
                Title = "", // Define um título vazio porque esse é exatamente o cenário que deve disparar a exceção de validação
                Description = "Task description", // Define uma descrição qualquer porque o foco do teste está apenas na obrigatoriedade do título
                IsCompleted = false // define um valor qualquer para completar o objeto de entrada, sem interferir no cenário validado
            };

            Action act = () => service.Create(task); // Encapsula a chamada do método em uma Action para permitir a asserção da exceção no padrão Arrange-Act-Assert do xUnit

            var exception = Assert.Throws<Exception>(act); // Verifica que o método lança exceção quando recebe título vazio, confirmando que a validação está funcionando corretamente
            Assert.Equal("Title is required.", exception.Message); // Confirma que a mensagem lançada corresponde exatamente à regra implementada no service
    }

    [Fact] // Marca o método como um teste unitário executável pelo xUnit
    public void Delete_ShouldReturnFalse_WhenTaskDoesNotExist() // Define um nome descritivo para deixar claro que o teste cobre o cenário que o id informado não existe
    {
        var options = new DbContextOptionsBuilder<AppDbContext>() // Cria o builder das opções do contexto porque o EF Core precisa dessa configuração para montar o banco em memória
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Usa um banco em memória com nome único para garantir que este teste fique isolado dos demais
            .Options; // Finaliza a configuração e gera o objeto de opções que será usado pelo AppDbContext

        using var context = new AppDbContext(options); // Cria o contexto de teste para simular o acesso a dados sem usar banco real
        var service = new TaskService(context); // Instancia o service com o contexto de teste para exercitar a lógica real do método Delete

        var result = service.Delete(999); // Executa o método com um id inexistente para validar o comportamento quando a tarefa não é encontrada

        Assert.False(result); // Verifica que o retorno foi false porque o service deve sinalizar falha quando o id não existe
    }

    [Fact] // Marca o método como um teste unitário executável pelo xUnit
    public void Delete_ShouldReturnTrue_WhenTaskExists() // Define um nome descritivo para deixar claro que o teste cobre um cenário em que a tarefa existe e deve ser removida
    {
        var options = new DbContextOptionsBuilder<AppDbContext>() // Cria o builder das opções do contexto porque o EF Core precisa dessa configuração para montar o banco em memória
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Usa um banco em memória com nome único para garantir que este teste fique isolado dos demais
            .Options; // Finaliza a configuração e gera o objeto de opções que será usado pelo AppDbContext

        using var context = new AppDbContext(options); // Cria o contexto de teste para simular o acesso a dados sem usar banco real
        var task = new TaskItem // Cria uma tarefa inicial porque o método Delete precisa de um registro existente para remover
        {
            Title = "Task to delete", // Define um título válido para montar uma entidade consistente no banco em memória
            Description = "Task description", // Define uma descrição qualquer apenas para completar a entidade de teste
            IsCompleted = false // Define um status inicial porque o foco do teste é a exclusão da tarefa, não seu conteúdo
        };

        context.Tasks.Add(task); // Adiciona a tarefa ao contexto para que ela passe a existir no banco em memória
        context.SaveChanges(); // Persiste a tarefa para garantir que o método Delete encontre o id informado

        var service = new TaskService(context); // Instancia o service com o contexto que contém a tarefa salva
        var result = service.Delete(task.Id); // Executa o método Delete com o id real da tarefa salva para testar o caminho de sucesso
        var deletedTask = context.Tasks.Find(task.Id); // Busca novamente a tarefa pelo id para validar se ela realmente foi removida do contexto

        Assert.True(result); // Verifica que o método retornou true porque a tarefa existia e foi excluída com sucesso
        Assert.Null(deletedTask); // Verifica que a tarefa não está mais no banco em memória após a exclusão
    }
}