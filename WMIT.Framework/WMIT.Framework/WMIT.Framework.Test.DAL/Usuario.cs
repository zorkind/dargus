using System;

namespace WMIT.Framework.Test.DAL
{
    public class Usuario : Framework.DAL.AbstractDal
    {
        internal const string cEntity = "usuario";
        internal const string cEntityKey = "us_id";
        internal const string cKey = "us_key";
        internal const string cNome = "us_nome";
        internal const string cSenha = "us_senha";
        internal const string cNascimento = "us_nascimento";
        internal const string cAceite = "us_aceite";
        public override string cAtivo { get { return "us_ativo"; } }
        public override string cCodigoUsuarioCadastro { get { return "us_cadastro_us_id"; } }
        public override string cCadastro { get { return "us_cadastro"; } }
        public override string cCodigoUsuarioAtualizacao { get { return "us_atualizacao_us_id"; } }
        public override string cAtualizacao { get { return "us_atualizacao"; } }

        public Usuario()
        {
            Entity = cEntity;
            EntityKey = cEntityKey;
            EntityOrderField = EntityKey;
        }
        
        protected override void GetFieldOrdinal()
        {
            lOrdinal.Key = AddField(lOrdinal.Field.Key, cKey);
            lOrdinal.Nome = AddField(lOrdinal.Field.Nome, cNome);
            lOrdinal.Senha = AddField(lOrdinal.Field.Senha, cSenha);
            lOrdinal.Nascimento = AddField(lOrdinal.Field.Nascimento, cNascimento);
            lOrdinal.Aceite = AddField(lOrdinal.Field.Aceite, cAceite);
        }
        
        protected override void GetSiblingsOrdinal()
        {
            UsuarioPerfil = AddSibling<UsuarioPerfil>(this, lOrdinal.UsuarioPerfil, lOrdinal.UsuarioPerfil.Fields.Any() || lOrdinal.Siblings.Contains(VO.Parametros.Usuario.EntitiesBitwise.Perfil));
            UsuarioTipoPessoa = AddSibling<UsuarioTipoPessoa>(this, lOrdinal.UsuarioTipoPessoa, lOrdinal.UsuarioTipoPessoa.Fields.Any() || lOrdinal.Siblings.Contains(VO.Parametros.Usuario.EntitiesBitwise.TipoPessoa));
            UsuarioSexo = AddSibling<UsuarioSexo>(this, lOrdinal.UsuarioSexo, lOrdinal.UsuarioSexo.Fields.Any() || lOrdinal.Siblings.Contains(VO.Parametros.Usuario.EntitiesBitwise.Sexo));
        }

        public override Framework.VO.IVO GetFieldData()
        {
            return new VO.Usuario()
            {
                Key = GetInt32(lOrdinal.Field.Key, lOrdinal.Key),
                Nome = GetString(lOrdinal.Field.Nome, lOrdinal.Nome),
                Senha = GetString(lOrdinal.Field.Senha, lOrdinal.Senha),
                Nascimento = GetDateTime(lOrdinal.Field.Nascimento, lOrdinal.Nascimento),
                Aceite = GetBoolean(lOrdinal.Field.Aceite, lOrdinal.Aceite)
            };
        }

        public override void GetSiblingsData(Framework.VO.IVO pRetorno)
        {
            var lRetorno = (VO.Usuario)pRetorno;
            
            lRetorno.UsuarioTipoPessoa = GetData<UsuarioTipoPessoa, VO.UsuarioTipoPessoa>(UsuarioTipoPessoa, 
                                                                        lOrdinal.UsuarioTipoPessoa,
                                                                        Reader,
                                                                        lOrdinal.UsuarioTipoPessoa.Fields.Any() ||
                                                                        lOrdinal.Siblings.Contains(VO.Parametros.Usuario.EntitiesBitwise.TipoPessoa));

            lRetorno.UsuarioPerfil = GetData<UsuarioPerfil, VO.UsuarioPerfil>(UsuarioPerfil, 
                                                                    lOrdinal.UsuarioPerfil,
                                                                    Reader,
                                                                    lOrdinal.UsuarioPerfil.Fields.Any() || lOrdinal.Siblings.Contains(VO.Parametros.Usuario.EntitiesBitwise.Perfil));

            lRetorno.UsuarioSexo = GetData<UsuarioSexo, VO.UsuarioSexo>(UsuarioSexo, 
                                                                lOrdinal.UsuarioSexo,
                                                                Reader,
                                                                lOrdinal.UsuarioSexo.Fields.Any() || lOrdinal.Siblings.Contains(VO.Parametros.Usuario.EntitiesBitwise.Sexo));
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

        private UsuarioTipoPessoa _UsuarioTipoPessoa;
        public UsuarioTipoPessoa UsuarioTipoPessoa
        {
            get { return _UsuarioTipoPessoa ?? (_UsuarioTipoPessoa = new UsuarioTipoPessoa()); }
            set { _UsuarioTipoPessoa = value; }
        }

        private VO.Ordinal.Usuario _lOrdinal;
        private VO.Ordinal.Usuario lOrdinal
        {
            get { return _lOrdinal ?? (_lOrdinal = new VO.Ordinal.Usuario()); }
            set { _lOrdinal = value; }
        }

        public override Framework.VO.IOrdinal Ordinal
        {
            get { return (Framework.VO.IOrdinal)lOrdinal; }
            set { lOrdinal = (VO.Ordinal.Usuario)value; }
        }

        public override void EvaluateParameters(Framework.VO.IParametros pParametros)
        {
            var lPar = (VO.Parametros.Usuario)pParametros;

            if (lPar.Codigo != 0)
                lOrdinal.Parameters.Add(VO.Parametros.Usuario.FieldsBitwise.Codigo);

            if (lPar.Key != 0)
                lOrdinal.Parameters.Add(VO.Parametros.Usuario.FieldsBitwise.Key);

            if (!string.IsNullOrEmpty(lPar.Nome))
                lOrdinal.Parameters.Add(VO.Parametros.Usuario.FieldsBitwise.Nome);

            if (!string.IsNullOrEmpty(lPar.Email))
                lOrdinal.Parameters.Add(VO.Parametros.Usuario.FieldsBitwise.Email);

            if (Framework.DAL.Between<DateTime>.HasValue(lPar.Nascimento))
                lOrdinal.Parameters.Add(VO.Parametros.Usuario.FieldsBitwise.Nascimento);

            if (Framework.DAL.Between<DateTime>.HasValue(lPar.Cadastro))
                lOrdinal.Parameters.Add(VO.Parametros.Usuario.FieldsBitwise.Cadastro);

            if (lPar.Perfil != 0)
                lOrdinal.Siblings.Add(VO.Parametros.Usuario.EntitiesBitwise.Perfil);

            if (lPar.TipoPessoa != 0)
                lOrdinal.Siblings.Add(VO.Parametros.Usuario.EntitiesBitwise.TipoPessoa);

            if (lPar.Sexo != 0)
                lOrdinal.Siblings.Add(VO.Parametros.Usuario.EntitiesBitwise.Sexo);
        }

        public override string BuildWhere(Framework.VO.IParametros pParametros)
        {
            var lPar = (VO.Parametros.Usuario)pParametros;

            if (lOrdinal.Parameters.Contains(VO.Parametros.Usuario.FieldsBitwise.Codigo))
                Query.Parametros.AppendEqual(lPar.Codigo, EntityAlias, cEntityKey);

            if (lOrdinal.Parameters.Contains(VO.Parametros.Usuario.FieldsBitwise.Key))
                Query.Parametros.AppendEqual(lPar.Key, EntityAlias, cKey);

            if (lOrdinal.Parameters.Contains(VO.Parametros.Usuario.FieldsBitwise.Nome))
                Query.Parametros.AppendEqual(lPar.Nome, EntityAlias, cNome);

            //if (lOrdinal.Parameters.Contains(VO.Parametros.Usuario.FieldsBitwise.Email))
            //    Query.Parametros.AppendExists(this, pParametros.Email, new DAL.UsuarioEmail(), "usuario_email_endereco");

            if (lOrdinal.Siblings.Contains(VO.Parametros.Usuario.EntitiesBitwise.Perfil))
                Query.Parametros.AppendEntity(lPar.Perfil, UsuarioPerfil);

            //if (Objeto.lOrdinal.Siblings.Contains(EntitiesBitwise.Sexo))
            //    AppendEntity(Sexo, Objeto.UsuarioSexo);

            if (lOrdinal.Parameters.Contains(VO.Parametros.Usuario.FieldsBitwise.Nascimento))
                Framework.DAL.Between<DateTime>.Append(Query.Parametros, lPar.Nascimento, EntityAlias, cNascimento);

            if (lOrdinal.Parameters.Contains(VO.Parametros.Usuario.FieldsBitwise.Cadastro))
                Framework.DAL.Between<DateTime>.Append(Query.Parametros, lPar.Cadastro, EntityAlias, cCadastro);

            return Query.Parametros.Builder.ToString();
        }

        public override void GetPersistenceParameters(Framework.VO.IVO pObjeto)
        {
            var lObjeto = (VO.Usuario)pObjeto;

            AddParameter(cKey, lObjeto.Key);
            AddParameter(cNome, lObjeto.Nome);
            AddParameter(cSenha, lObjeto.Senha);
            AddParameter(cAceite, lObjeto.Aceite);

            if (lObjeto.Nascimento == DateTime.MinValue)
                AddParameter(cNascimento, DBNull.Value);
            else
                AddParameter(cNascimento, lObjeto.Nascimento);

            AddParameter(UsuarioSexo.cEntityKey, lObjeto.UsuarioSexo.Codigo);
            AddParameter(UsuarioPerfil.cEntityKey, lObjeto.UsuarioPerfil.Codigo);
            AddParameter(UsuarioTipoPessoa.cEntityKey, lObjeto.UsuarioTipoPessoa.Codigo);
        }
    }
}