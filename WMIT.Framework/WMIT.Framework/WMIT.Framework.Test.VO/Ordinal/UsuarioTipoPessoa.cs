using System;
using WMIT.Framework.VO;

namespace WMIT.Framework.Test.VO.Ordinal
{
    public class UsuarioTipoPessoa : BaseOrdinal<VO.UsuarioTipoPessoa.Bitwise>
    {
        public int Descricao { get; set; }

        public override bool SiblingsBitwise()
        {
            return Bitwise();
        }
    }
}