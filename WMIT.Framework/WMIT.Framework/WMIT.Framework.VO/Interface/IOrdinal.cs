
namespace WMIT.Framework.VO
{
    public interface IOrdinal
    {
        int Codigo { get; set; }
        int Ativo { get; set; }
        int CodigoUsuarioCadastro { get; set; }
        int Cadastro { get; set; }
        int CodigoUsuarioAtualizacao { get; set; }
        int Atualizacao { get; set; }
        BitwiseSellection Fields { get; set; }
        BitwiseSellection Parameters { get; set; }
        BitwiseSellection Siblings { get; set; }
        IBitwise BaseField { get; set; }
        bool SiblingsBitwise();
        bool Bitwise();
    }

    public interface IOrdinal<A> : IOrdinal
    {
        A Field { get; set; }
    }
}
