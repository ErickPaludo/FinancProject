using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Financ.Domain.Validacoes.Mensagens;

namespace Financ.Application.DTOs.Usuarios.Post
{
    public class CadastraUsuarioDTO
    {
        public string PrimeiroNome { get; set; }

        public string SegundoNome { get; set; }

        public string Email { get; set; }

        [Required]
        public string Senha { get; set; }

        [Required]
        public string ConfirmarSenha { get; set; }
    }
}
