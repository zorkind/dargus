using System;
using WMIT.Framework.VO;

namespace WMIT.Framework.Test.VO
{
    public class Usuario : VOBase
    {
        public int Key { get; set; }
        public string Nome { get; set; }
        public string Senha { get; set; }
        public bool Aceite { get; set; }
        public DateTime Nascimento { get; set; }

        private UsuarioTipoPessoa _UsuarioTipoPessoa;
        public UsuarioTipoPessoa UsuarioTipoPessoa
        {
            get { return _UsuarioTipoPessoa ?? (_UsuarioTipoPessoa = new UsuarioTipoPessoa()); }
            set { _UsuarioTipoPessoa = value; }
        }
        
        private UsuarioPerfil _UsuarioPerfil;
        public UsuarioPerfil UsuarioPerfil
        {
            get { return _UsuarioPerfil ?? (_UsuarioPerfil = new UsuarioPerfil()); }
            set { _UsuarioPerfil = value; }
        }
        
        private UsuarioSexo _UsuarioSexo;
        public UsuarioSexo UsuarioSexo
        {
            get { return _UsuarioSexo ?? (_UsuarioSexo = new UsuarioSexo()); }
            set { _UsuarioSexo = value; }
        }

        public class Bitwise : BaseBitwise
        {
            public int Key { get { return 1 << 6; } }
            public int Nome { get { return 1 << 7; } }
            public int Senha { get { return 1 << 8; } }
            public int Nascimento { get { return 1 << 9; } }
            public int Aceite { get { return 1 << 10; } }
            public override int All { get { return (1 << 11) - 1; } }
        }
    }
}
