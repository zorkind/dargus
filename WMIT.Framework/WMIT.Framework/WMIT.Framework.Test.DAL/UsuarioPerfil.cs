using System;

namespace WMIT.Framework.Test.DAL
{
    public class UsuarioPerfil : Framework.DAL.AbstractDal
    {
        internal const string cEntity = "usuario_perfil";
        internal const string cEntityKey = "us_perfil_id";
        internal const string cDescricao = "us_perfil_descricao";
        public override string cAtivo { get { return "us_perfil_ativo"; } }
        public override string cCodigoUsuarioCadastro { get { return "us_perfil_cadastro_us_id"; } }
        public override string cCadastro { get { return "us_perfil_cadastro"; } }
        public override string cCodigoUsuarioAtualizacao { get { return "us_perfil_atualizacao_us_id"; } }
        public override string cAtualizacao { get { return "us_perfil_atualizacao"; } }

        public UsuarioPerfil()
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
            return new VO.UsuarioPerfil()
            {
                Descricao = GetString(lOrdinal.Field.Descricao, lOrdinal.Descricao)
            };
        }

        public override void EvaluateParameters(Framework.VO.IParametros pParametros)
        {
            var lPar = (VO.Parametros.UsuarioPerfil)pParametros;

            if (lPar.Codigo != 0)
                lOrdinal.Parameters.Add(VO.Parametros.UsuarioPerfil.FieldsBitwise.Codigo);

            if (!string.IsNullOrEmpty(lPar.Descricao))
                lOrdinal.Parameters.Add(VO.Parametros.UsuarioPerfil.FieldsBitwise.Descricao);
        }

        public override string BuildWhere(Framework.VO.IParametros pParametros)
        {
            var lPar = (VO.Parametros.UsuarioPerfil)pParametros;

            if (lOrdinal.Parameters.Contains(VO.Parametros.UsuarioPerfil.FieldsBitwise.Codigo))
                Query.Parametros.AppendEqual(lPar.Codigo, EntityAlias, cEntityKey);

            if (lOrdinal.Parameters.Contains(VO.Parametros.UsuarioPerfil.FieldsBitwise.Descricao))
                Query.Parametros.AppendEqual(lPar.Descricao, EntityAlias, cDescricao);

            return Query.Parametros.Builder.ToString();
        }

        public override void GetPersistenceParameters(Framework.VO.IVO pObjeto)
        {
            var lObjeto = (VO.UsuarioPerfil)pObjeto;

            AddParameter(cDescricao, lObjeto.Descricao);
        }

        private VO.Ordinal.UsuarioPerfil lOrdinal { get; set; }
        public override Framework.VO.IOrdinal Ordinal
        {
            get
            {
                return (Framework.VO.IOrdinal)lOrdinal;
            }
            set
            {
                lOrdinal = (VO.Ordinal.UsuarioPerfil)value;
            }
        }
    }
}