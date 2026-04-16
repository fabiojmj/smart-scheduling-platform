using SmartScheduling.Domain.Enums;

namespace SmartScheduling.Domain.Entities;

public class Mensagem : Entity
{
    public Guid ConversaId { get; private set; }
    public string Conteudo { get; private set; }
    public TipoMensagem Tipo { get; private set; }
    public bool VeioDoCliente { get; private set; }
    public string? TextoTranscrito { get; private set; }

    private Mensagem() { Conteudo = default!; }

    public static Mensagem Criar(Guid conversaId, string conteudo, TipoMensagem tipo, bool veioDoCliente, string? transcricao = null) =>
        new() { ConversaId = conversaId, Conteudo = conteudo, Tipo = tipo, VeioDoCliente = veioDoCliente, TextoTranscrito = transcricao };
}
