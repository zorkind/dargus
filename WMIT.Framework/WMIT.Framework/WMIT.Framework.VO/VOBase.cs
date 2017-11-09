using System;

namespace WMIT.Framework.VO
{
    public abstract class VOBase : IVO
    {
        public int Codigo { get; set; }
        public bool isDirty { get; set; }
        public int CodigoUsuarioCadastro { get; set; }
        public int CodigoUsuarioAtualizacao { get; set; }
        public virtual IVO UsuarioCadastro { get; set; }
        public virtual IVO UsuarioAtualizacao { get; set; }
        public DateTime Cadastro { get; set; }
        public DateTime Atualizacao { get; set; }

        public bool isNew { get { return Codigo == 0; } }

        private bool _Ativo;
        public bool Ativo
        {
            get
            {
                if (isNew && !_Ativo)
                    _Ativo = true;

                return _Ativo;
            }
            set
            {
                _Ativo = value;
            }
        }
    }
}
