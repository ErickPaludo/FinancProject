using Financ.Application.DTOs.Convites.Get;
using Financ.Application.DTOs.Convites.Get.MicroDto;
using Financ.Application.DTOs.Usuarios.Get;
using Financ.Domain.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Application.Mapeamento
{
    public static class ConvitesMapper
    {
        public static GetCriaConviteDTO ParaDTO(Convites convite) => new GetCriaConviteDTO(convite.Id, convite.Acesso);
        public static List<GetRetornaConvitesDTO> ParaDTO(IEnumerable<Convites> convites)
        {
            List<GetRetornaConvitesDTO> listaConvites = new List<GetRetornaConvitesDTO>();
            foreach (var convite in convites.ToList())
            {
                listaConvites.Add(new GetRetornaConvitesDTO(
                    new GetConvite(
                    convite.Id,
                    convite.Acesso,
                    convite.Aceito,
                    convite.DataEnvio,
                    convite.Expiracao),
                    new GetContaConvite(
                        convite.IdConta,
                        convite.Conta.Titulo,
                        convite.Conta.TipoConta),
                    new GetUsuarioConvite(
                        convite.IdUsuarioRemetente,
                        convite.Remetente.PrimeiroNome,
                        convite.Remetente.SegundoNome,
                        $"{convite.Remetente.PrimeiroNome} {convite.Remetente.SegundoNome}"),
                      new GetUsuarioConvite(
                        convite.IdUsuarioDestinatario,
                        convite.Destinatario.PrimeiroNome,
                        convite.Destinatario.SegundoNome,
                        $"{convite.Destinatario.PrimeiroNome} {convite.Destinatario.SegundoNome}")
                    ));
            }
            return listaConvites;
        }
    }
}
