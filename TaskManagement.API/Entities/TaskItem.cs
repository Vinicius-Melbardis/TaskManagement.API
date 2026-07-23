using System;
namespace TaskManagement.API.Entities // Namespace que organiza as classes relacionadas às entidades do sistema de gerenciamento de tarefas
{
    public class TaskItem // Classe que representa uma tarefa no sistema de gerenciamento de tarefas
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}