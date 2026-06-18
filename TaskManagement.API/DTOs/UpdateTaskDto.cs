using System.ComponentModel.DataAnnotations; // Importa as Data Annotations para validar a entrada antes da atualização da tarefa

namespace TaskManagement.API.DTOs; // Organiza os contratos da API separados das entidades do banco de dados

public class UpdateTaskDto // Define um DTO para atualização sem expor a entidade diretamente no contrato HTTP
{
    [Required(ErrorMessage = "The title is required.")] // Garante que a tarefa atualizada continue com um título válido
    [StringLength(100, MinimumLength = 3, ErrorMessage = "The title must have between 3 and 100 characters.")] // Mantém as mesmas regras de validação do título para garantir a consistência dos dados
    public string Title { get; set; } = string.Empty; // Inicializa com string vazia para evitar null em campo obrigatório

    [StringLength(500, ErrorMessage = "The description can have a maximum of 500 characters.")] // Controla o tamanho da descrição também no fluxo de atualização
    public string? Description { get; set; } // Permite atualizar a descrição sem obrigar preenchimento

    public bool IsCompleted { get; set; } // Permite atualizar o status de conclusão da tarefa
}