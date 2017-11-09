using System;
using System.Collections.Generic;

namespace WMIT.Framework.VO
{
    public interface IParametros
    {
        int UsuarioCadastro { get; set; }
        int UsuarioAtualizacao { get; set; }
        Between<DateTime> Cadastro { get; set; }
        Between<DateTime> Atualizacao { get; set; }
        bool? Ativo { get; set; }
        int Codigo { get; set; }
        int Top { get; set; }
        List<int> Codigos { get; set; }
        string[] OrderBy { get; set; }
        Paginate Paginate { get; set; }
        bool EnableGroup { get; set; }
    }
}
