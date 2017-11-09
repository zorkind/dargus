using WMIT.Framework.VO;

namespace WMIT.Framework.Test.VO.Ordinal
{
    public class Usuario : BaseOrdinal<VO.Usuario.Bitwise>
    {
        public override bool SiblingsBitwise()
        {
            return Bitwise() || UsuarioPerfil.SiblingsBitwise() || UsuarioSexo.SiblingsBitwise() || UsuarioTipoPessoa.SiblingsBitwise();
        }

        public int Key { get; set; }
        public int Nome { get; set; }
        public int Senha { get; set; }
        public int Nascimento { get; set; }
        public int Aceite { get; set; }

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

        private UsuarioTipoPessoa _UsuarioTipoPessoa;
        public UsuarioTipoPessoa UsuarioTipoPessoa
        {
            get { return _UsuarioTipoPessoa ?? (_UsuarioTipoPessoa = new UsuarioTipoPessoa()); }
            set { _UsuarioTipoPessoa = value; }
        }

    }
}