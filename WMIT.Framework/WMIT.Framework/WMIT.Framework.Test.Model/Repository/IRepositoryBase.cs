using System;
using System.Collections.Generic;

namespace WMIT.Framework.Test.Model.Repository
{
    public interface IRepositoryBase<A, B, C, D>
        where A : Framework.VO.IVO, new()
        where B : Framework.VO.IBitwise, new()
        where C : Framework.VO.BaseOrdinal<B>, new()
        where D : Framework.VO.Parametros, new()
    {
        Exception Erro { get; set; }

        VO.Usuario Usuario { get; set; }

        IList<A> Get(D Parametros = null, C Ordinal = null);

        bool Salvar(A obj);

        bool Validate(A obj);
    }
}
