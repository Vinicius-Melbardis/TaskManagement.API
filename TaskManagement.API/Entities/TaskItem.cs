using System;
namespace TaskManagement.API.Entities // Namespace que organiza as classes relacionadas às entidades do sistema de gerenciamento de tarefas
{
    public class TaskItem // Classe que representa uma tarefa no sistema de gerenciamento de tarefas
    {
        public int Id { get; set; } // Identificador único da tarefa (chave primária no banco de dados)
        public string? Title { get; set; } // Título da tarefa (obrigatório na regra de negócio)
        public string? Description { get; set; } // Descrição detalhada da tarefa (opcional)
        public bool IsCompleted { get; set; } // Indica se a tarefa foi concluída ou não (padrão: false)
        public DateTime CreatedAt { get; set; } // Data de criação da tarefa (definida automaticamente no momento da criação)
    }
}