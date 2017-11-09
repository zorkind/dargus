using System;
using WMIT.Framework.VO;

namespace WMIT.Framework.Test.VO.Ordinal
{
    public class UsuarioPerfil : BaseOrdinal<VO.UsuarioPerfil.Bitwise>
    {
        public int Descricao { get; set; }

        public override bool SiblingsBitwise()
        {
            return Bitwise();
        }
    }
}