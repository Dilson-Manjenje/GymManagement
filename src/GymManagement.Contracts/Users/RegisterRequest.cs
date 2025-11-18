using System.ComponentModel.DataAnnotations;

namespace GymManagement.Contracts.Users;

public class RegisterRequest
{		
	public required string UserName { get; set; }

	// [Required(ErrorMessage = "Email é obrigatório")]
	// [EmailAddress(ErrorMessage = "Email no formato inválido")]
	// public required string Email { get; set; }

	// [Required(ErrorMessage = "Password é obrigatório")]
	// [MinLength(6, ErrorMessage = "Password deve ter pelo menos 6 caracteres.")]
	public required string Password { get; set; }
}
