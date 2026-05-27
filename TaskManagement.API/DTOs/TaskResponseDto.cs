namespace TaskManagement.API.DTOs; // Organiza os contratos de saída da API separados das entidades do banco de dados

public class TaskResponseDto // Define um DTO para retornar dados da tarefa ao cliente sem expor a entidade diretamente
{
    public int Id { get; set; } // Expõe o identificador da tarefa para permitir referência no cliente

    public string Title { get; set; } = string.Empty; // Retorna o título da tarefa já com valor inicial para evitar null na serialização

    public string? Description { get; set; } // Retorna a descrição da tarefa sem obrigar preenchimento quando ela não existir

    public bool IsCompleted { get; set; } // Informa o estado da tarefa para o cliente saber se ela foi concluída

    public DateTime CreatedAt { get; set; } // Retorna a data de criação da tarefa para dar contexto temporal ao registro
}