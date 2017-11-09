
namespace WMIT.Framework.VO
{
    /// <summary>
    /// Define os campos básicos das classes VO. 
    /// <para>Leia os remarks para maiores informações e exemplos.</para>
    /// <para>&#160;</para>
    /// <para>Tenha certeza de adicionar constantes a partir de 6, exemplo:</para>
    /// <para><c>public int Exemplo { get { return 1 &lt;&lt; 6; } }</c></para>
    /// <para>&#160;</para>
    /// <para>No final da classe não se esqueça dos Totalizadores:</para>
    /// <code>
    /// <para>public override int All { get { return (1 &lt;&lt; 11) - 1; } }</para>
    /// </code>
    /// </summary>
    public abstract class BaseBitwise : IBitwise
    {
        public int Nenhum { get { return 0; } }
        public int Codigo { get { return 1 << 0; } }
        public int Ativo { get { return 1 << 1; } }
        public int CodigoUsuarioCadastro { get { return 1 << 2; } }
        public int Cadastro { get { return 1 << 3; } }
        public int CodigoUsuarioAtualizacao { get { return 1 << 4; } }
        public int Atualizacao { get { return 1 << 5; } }
        public int AllBase { get { return (1 << 6) - 1; } }
        public int AllBaseNonAudit { get { return 3; } }
        public int AllBaseAudit { get { return AllBase - AllBaseNonAudit; } }
        public virtual int All { get { return (1 << 6) - 1; } }
        public virtual int AllNonAudit { get { return All - AllBaseAudit; } }
    }
}
