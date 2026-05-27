using System.ComponentModel.DataAnnotations; // Importa as anotações de validação para usar em DTOs e garantir que os dados recebidos pela API sejam válidos antes de processá-los

namespace TaskManagement.API.DTOs; // Organiza os contratos da API separados das entidades do banco de dados para evitar acoplamento

public class CreateTaskDto // Define um DTO para criação de tarefas sem expor a estrutura interna da entidade do banco de dados diretamente
{
    [Required(ErrorMessage = "The title is required.")] // Garante que a API não aceite a criação de tarefa sem título
    [StringLength(100, MinimumLength = 3, ErrorMessage = "The title must have between 3 and 100 characters.")] // Limita o tamanho para evitar dados inválidos e inconsistentes
    public string Title { get; set; } = string.Empty; // Inicializa o título com string vazia para evitar null em uma propriedade obrigatória

    [StringLength(500, ErrorMessage = "The description can have a maximum of 500 characters.")] // Impede os textos excessivos e mantém o contrato de entrada controlado
    public string? Description { get; set; } // Permite descrição opcional sem obrigar preenchimento no cadastro
}