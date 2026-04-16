using SmartScheduling.Domain.Enums;
using SmartScheduling.Domain.ValueObjects;

namespace SmartScheduling.Domain.Entities;

public class Conversa : Entity
{
    public PhoneNumber TelefoneCliente { get; private set; }
    public Guid EstabelecimentoId { get; private set; }
    public StatusConversa Status { get; private set; }
    public Guid? AgendamentoPendenteId { get; private set; }

    private readonly List<Mensagem> _mensagens = [];
    public IReadOnlyCollection<Mensagem> Mensagens => _mensagens.AsReadOnly();

    private Conversa() { TelefoneCliente = default!; }

    public static Conversa Iniciar(string telefoneCliente, Guid estabelecimentoId) =>
        new() { TelefoneCliente = PhoneNumber.Create(telefoneCliente), EstabelecimentoId = estabelecimentoId, Status = StatusConversa.Ativa };

    public void AdicionarMensagem(string conteudo, TipoMensagem tipo, bool veioDoCliente) =>
        _mensagens.Add(Mensagem.Criar(Id, conteudo, tipo, veioDoCliente));

    public void AguardarConfirmacao(Guid id) { Status = StatusConversa.AguardandoConfirmacao; AgendamentoPendenteId = id; MarkAsUpdated(); }
    public void Concluir() { Status = StatusConversa.Concluida; MarkAsUpdated(); }
    public void Abandonar() { Status = StatusConversa.Abandonada; MarkAsUpdated(); }
}
