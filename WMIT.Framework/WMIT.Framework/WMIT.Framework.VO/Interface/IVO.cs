using System;

namespace WMIT.Framework.VO
{
    public interface IVO
    {
        int Codigo { get; set; }
        bool isNew { get; }
        bool isDirty { get; set; }
        bool Ativo { get; set; }
        int CodigoUsuarioCadastro { get; set; }
        int CodigoUsuarioAtualizacao { get; set; }
        IVO UsuarioCadastro { get; set; }
        IVO UsuarioAtualizacao { get; set; }
        DateTime Cadastro { get; set; }
        DateTime Atualizacao { get; set; }
    }
}
