
namespace WMIT.Framework.VO
{
    public interface IBitwise
    {
        int Codigo { get; }
        int Ativo { get; }
        int CodigoUsuarioCadastro { get; }
        int Cadastro { get; }
        int CodigoUsuarioAtualizacao { get; }
        int Atualizacao { get; }
        int AllBase { get; }
        int AllBaseNonAudit { get; }
        int AllBaseAudit { get; }
        int All { get; }
        int AllNonAudit { get; }
    }
}
