using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Financ.Domain.Validacoes.Mensagens;

namespace Financ.Application.DTOs.Usuarios.Post
{
    public class CadastraUsuarioDTO
    {
        [Required]
        public string PrimeiroNome { get; set; }

        [Required]
        public string SegundoNome { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Senha { get; set; }

        [Required]
        public string ConfirmarSenha { get; set; }
    }
}
