using System;
using System.Collections.Generic;

namespace WMIT.Framework.Test.BLL
{
    public class Usuario : BLLBase<VO.Usuario, VO.Usuario.Bitwise, VO.Ordinal.Usuario, VO.Parametros.Usuario>
    {
        public Usuario()
        { }

        public Usuario(VO.Usuario Usuario)
            : base(Usuario)
        { }

        public bool Get()
        {
            return base.Get<DAL.Usuario>();
        }

        public bool Salvar(VO.Usuario obj)
        {
            return base.Salvar<DAL.Usuario>(obj);
        }
    }
}
