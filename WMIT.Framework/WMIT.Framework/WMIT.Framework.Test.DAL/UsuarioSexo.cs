using System;

namespace WMIT.Framework.Test.DAL
{
    [Serializable]
    public class UsuarioSexo : Framework.DAL.AbstractDal
    {
        internal const string cEntity = "usuario_sexo";
        internal const string cEntityKey = "us_sexo_id";
        internal const string cDescricao = "us_sexo_descricao";
        public override string cAtivo { get { return "us_sexo_ativo"; } }
        public override string cCodigoUsuarioCadastro { get { return "us_sexo_cadastro_us_id"; } }
        public override string cCadastro { get { return "us_sexo_cadastro"; } }
        public override string cCodigoUsuarioAtualizacao { get { return "us_sexo_atualizacao_us_id"; } }
        public override string cAtualizacao { get { return "us_sexo_atualizacao"; } }


        public UsuarioSexo()
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
            return new VO.UsuarioSexo()
            {
                Descricao = GetString(lOrdinal.Field.Descricao, lOrdinal.Descricao)
            };
        }

        public override void GetPersistenceParameters(Framework.VO.IVO pObjeto)
        {
            var lObjeto = (VO.UsuarioSexo)pObjeto;

            AddParameter(cDescricao, lObjeto.Descricao);
        }

        private VO.Ordinal.UsuarioSexo lOrdinal { get; set; }
        public override Framework.VO.IOrdinal Ordinal
        {
            get
            {
                return (Framework.VO.IOrdinal)lOrdinal;
            }
            set
            {
                lOrdinal = (VO.Ordinal.UsuarioSexo)value;
            }
        }
    }
}