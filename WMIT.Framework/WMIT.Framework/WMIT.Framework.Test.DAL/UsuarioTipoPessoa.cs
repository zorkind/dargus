using System;

namespace WMIT.Framework.Test.DAL
{
    public class UsuarioTipoPessoa : Framework.DAL.AbstractDal
    {
        internal const string cEntity = "usuario_tipo_pessoa";
        internal const string cEntityKey = "us_tipo_pessoa_id";
        internal const string cDescricao = "us_tipo_pessoa_desc";
        public override string cAtivo { get { return "us_tipo_pessoa_ativo"; } }
        public override string cCodigoUsuarioCadastro { get { return "us_tipo_pessoa_cadastro_us_id"; } }
        public override string cCadastro { get { return "us_tipo_pessoa_cadastro"; } }
        public override string cCodigoUsuarioAtualizacao { get { return "us_tipo_pessoa_atualizacao_us_id"; } }
        public override string cAtualizacao { get { return "us_tipo_pessoa_atualizacao"; } }

        public UsuarioTipoPessoa()
        {
            Entity = cEntity;
            EntityKey = cEntityKey;
            EntityOrderField = EntityKey;
        }

        protected override void GetFieldOrdinal()
        {
            lOrdinal.Descricao = AddField(lOrdinal.Field.Descricao, cDescricao);
        }

        public override Framework.VO.IVO GetFieldData()
        {
            return new VO.UsuarioTipoPessoa()
            {
                Descricao = GetString(lOrdinal.Field.Descricao, lOrdinal.Descricao)
            };
        }

        public override void GetPersistenceParameters(Framework.VO.IVO pObjeto)
        {
            var lObjeto = (VO.UsuarioTipoPessoa)pObjeto;
            AddParameter(cDescricao, lObjeto.Descricao);
        }

        private VO.Ordinal.UsuarioTipoPessoa lOrdinal { get; set; }
        public override Framework.VO.IOrdinal Ordinal
        {
            get
            {
                return (Framework.VO.IOrdinal)lOrdinal;
            }
            set
            {
                lOrdinal = (VO.Ordinal.UsuarioTipoPessoa)value;
            }
        }
    }
}
