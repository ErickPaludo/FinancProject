using Financ.Application.DTOs.Base;
using Financ.Application.DTOs.Convites.Get;
using Financ.Application.DTOs.Convites.Get.MicroDto;
using Financ.Application.DTOs.Usuarios.Get;
using Financ.Domain.Entidades.ContasBancarias;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Application.Mapeamento
{
    public static class ConviteMapper
    {
        public static BasePost<GetCriaConviteDTO> ParaDTO(Convite convite) => new BasePost<GetCriaConviteDTO>(new GetCriaConviteDTO(convite.Id, convite.Acesso));
        public static BaseGetList<GetRetornaConvitesDTO> ParaDTO(IEnumerable<Convite>? convites, object? filtro)
        {
            List<GetRetornaConvitesDTO> listaConvites = new List<GetRetornaConvitesDTO>();
            if (convites != null)
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
                        new GetUsuario(
                            convite.IdUsuarioRemetente,
                            convite.Remetente.Email,
                            convite.Remetente.PrimeiroNome,
                            convite.Remetente.SegundoNome,
                            $"{convite.Remetente.PrimeiroNome} {convite.Remetente.SegundoNome}"),
                          new GetUsuario(
                            convite.IdUsuarioDestinatario,
                            convite.Destinatario.Email,
                            convite.Destinatario.PrimeiroNome,
                            convite.Destinatario.SegundoNome,
                            $"{convite.Destinatario.PrimeiroNome} {convite.Destinatario.SegundoNome}")
                        ));
                }
            return new BaseGetList<GetRetornaConvitesDTO>(listaConvites, new Meta { filtros = filtro });
        }
    }
}
