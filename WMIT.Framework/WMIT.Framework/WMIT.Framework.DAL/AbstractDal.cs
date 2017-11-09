using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using WMIT.Framework.VO;

namespace WMIT.Framework.DAL
{
    public abstract class AbstractDal : DataReader, VO.IAbstractDal
    {
        private static string _DatabaseDefault;
        internal static string DatabaseDefault
        {
            get
            {
                if (_DatabaseDefault == null)
                    if (ConfigurationManager.AppSettings["DatabaseDefault"] != null)
                        _DatabaseDefault = ConfigurationManager.AppSettings["DatabaseDefault"].ToString();

                return _DatabaseDefault;
            }
        }
        private static string _SchemaDefault;
        internal static string SchemaDefault
        {
            get
            {
                if (_SchemaDefault == null)
                    if (ConfigurationManager.AppSettings["SchemaDefault"] != null)
                        _SchemaDefault = ConfigurationManager.AppSettings["SchemaDefault"].ToString();

                return _SchemaDefault;
            }
        }
        private string _Database;
        public string Database
        {
            get { return _Database ?? (_Database = AbstractDal.DatabaseDefault); }
            set { _Database = value; }
       }
        private string _Schema;
        public string Schema
        {
            get { return _Schema ?? (_Schema = AbstractDal.SchemaDefault); }
            set { _Schema = value; }
        }

        public abstract string cAtivo { get; }
        public abstract string cCodigoUsuarioCadastro { get; }
        public abstract string cCadastro { get; }
        public abstract string cCodigoUsuarioAtualizacao { get; }
        public abstract string cAtualizacao { get; }

        public string Entity { get; set; }
        public string EntityAlias { get; set; }
        public string EntityKey { get; set; }
        public string EntityOrderField { get; set; }
        public bool EntityKeyIdentity { get; set; }
        public bool EntityLeft { get; set; }
        public IAbstractDal Parent { get; set; }
        public int ThreadId { get; set; }
        private IQuery _Query;
        public IQuery Query
        {
            get { return _Query ?? (_Query = new Query()); }
            set { _Query = value; }
        }
        public bool Persistence { get; set; }
        public Dictionary<string, object> PersistenceParameters { get; set; }
        public Dictionary<string, object> PersistenceOutputParameters { get; set; }

        public virtual string BuildWhere(VO.IParametros pParametros)
        {
            return string.Empty;
        }

        public virtual void EvaluateParameters(VO.IParametros pParametros)
        { }

        public AbstractDal()
        {
            EntityKeyIdentity = true;
        }

        public void AddParameter(string pKey, object pValue)
        {
            pKey = string.Concat("@", pKey);

            PersistenceParameters.Add(pKey, pValue);
        }

        public void AddParameterOutput(string pKey, object pValue)
        {
            pKey = string.Concat("@", pKey);

            PersistenceOutputParameters.Add(pKey, pValue);
        }

        public object GetParameterOutput(string pKey)
        {
            object outObject = null;
            pKey = string.Concat("@", pKey);

            PersistenceOutputParameters.TryGetValue(pKey, out outObject);

            return outObject;
        }

        private void GetBasicFieldOrdinal()
        {
            Ordinal.Codigo = AddField(Ordinal.BaseField.Codigo, EntityKey);

            GetFieldOrdinal();

            Ordinal.Ativo = AddField(Ordinal.BaseField.Ativo, cAtivo);
            Ordinal.CodigoUsuarioCadastro = AddField(Ordinal.BaseField.CodigoUsuarioCadastro, cCodigoUsuarioCadastro);
            Ordinal.Cadastro = AddField(Ordinal.BaseField.Cadastro, cCadastro);
            Ordinal.CodigoUsuarioAtualizacao = AddField(Ordinal.BaseField.CodigoUsuarioAtualizacao, cCodigoUsuarioAtualizacao);
            Ordinal.Atualizacao = AddField(Ordinal.BaseField.Atualizacao, cAtualizacao);
        }

        protected abstract void GetFieldOrdinal();

        protected virtual void GetSiblingsOrdinal()
        { }

        public void GetOrdinal(IOrdinal pOrdinal)
        {
            try
            {
                Ordinal = pOrdinal;
                GetOrdinal();
            }
            catch (Exception ex)
            {
                throw new DALException(ex.Message,
                                        System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name,
                                        System.Reflection.MethodBase.GetCurrentMethod().Name,
                                        "Não foi possível obter o Ordinal.", ex);
            }
        }

        public void GetOrdinal()
        {
            GetBasicFieldOrdinal();

            if (EntityAlias == null)
                EntityAlias = Query.EntityCount.ToString();

            GetSiblingsOrdinal();
        }

        public bool GetData<A>(System.Data.IDataReader pReader, IList<A> pOutput)
            where A : IVO, new()
        {
            Reader = pReader;
            return GetData(pOutput);
        }

        public bool GetData<A>(IList<A> pOutput)
            where A : IVO, new()
        {
            try
            {
                pOutput.Add((A)GetData());

                return true;
            }
            catch (Exception ex)
            {
                throw new DALException(ex.Message,
                                        System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name,
                                        System.Reflection.MethodBase.GetCurrentMethod().Name,
                                        "Não foi possível ler os dados.", ex);
            }
        }

        public abstract void GetPersistenceParameters(IVO pObjeto);

        public virtual void SetOutputParameters(IVO pObjeto)
        { }

        public virtual IVO GetData()
        {
            try
            {
                IVO lRetorno = GetFieldData();

                GetFieldDataDefault(lRetorno);

                GetSiblingsData(lRetorno);

                return lRetorno;
            }
            catch (Exception ex)
            {
                throw new DALException(ex.Message,
                                        System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name,
                                        System.Reflection.MethodBase.GetCurrentMethod().Name,
                                        "Não foi possível ler os dados.", ex);
            }
        }

        public IVO GetData(System.Data.IDataReader pReader)
        {
            Reader = pReader;
            return GetData();
        }

        protected void GetFieldDataDefault(IVO pOutput)
        {
            pOutput.Codigo = GetInt32(Ordinal.BaseField.Codigo, Ordinal.Codigo);
            pOutput.Ativo = GetBoolean(Ordinal.BaseField.Ativo, Ordinal.Ativo);
            pOutput.CodigoUsuarioCadastro = GetInt32(Ordinal.BaseField.CodigoUsuarioCadastro, Ordinal.CodigoUsuarioCadastro);
            pOutput.Cadastro = GetDateTime(Ordinal.BaseField.Cadastro, Ordinal.Cadastro);
            pOutput.CodigoUsuarioAtualizacao = GetInt32(Ordinal.BaseField.CodigoUsuarioAtualizacao, Ordinal.CodigoUsuarioAtualizacao);
            pOutput.Atualizacao = GetDateTime(Ordinal.BaseField.Atualizacao, Ordinal.Atualizacao);
        }

        public abstract IVO GetFieldData();

        public virtual void GetSiblingsData(IVO pRetorno)
        { }

        public int AddField(int pField, string pFieldName, bool pReadOnly = false, bool pCustomQuery = false)
        {
            if (Query.EntityCount == 1 && Persistence && !pReadOnly)
                Query.SaveAtributes.Add(pFieldName, pFieldName.Insert(0, "@"));

            if (Ordinal.Fields.Contains(pField))
                if(pCustomQuery)
                    return Query.AddCustomField(pFieldName);
                else
                    return Query.AddField(pFieldName);
            else
                return 0;
        }

        public static A AddSibling<A>(IAbstractDal pSource, IOrdinal pOrdinal, bool pAddToQuery)
            where A : IAbstractDal, new()
        {
            A lReturn = default(A);

            if (pAddToQuery || pOrdinal.SiblingsBitwise())
            {
                lReturn = new A();
                lReturn.Parent = pSource;
                pSource.Query.AddEntityLeftJoin(lReturn, pSource.EntityAlias);
                lReturn.GetOrdinal(pOrdinal);
            }
            else if (pSource.EntityAlias == "1" && pSource.Persistence)
            {
                lReturn = new A();
                pSource.Query.SaveAtributes.Add(lReturn.EntityKey, lReturn.EntityKey.Insert(0, "@"));
            }

            return lReturn;
        }

        public static A AddSibling<A>(IAbstractDal pSource, IOrdinal pOrdinal, bool pAddToQuery, string pCustomKey, CustomKeyHand pHand = CustomKeyHand.Left)
            where A : IAbstractDal, new()
        {
            return AddSibling<A>(pSource, pOrdinal, pAddToQuery, pCustomKey, false, pHand);
        }

        public static A AddSibling<A>(IAbstractDal pSource, IOrdinal pOrdinal, bool pAddToQuery, string pCustomKey, bool pReadOnly = false, CustomKeyHand pHand = CustomKeyHand.Left)
            where A : IAbstractDal, new()
        {
            A lReturn = default(A);

            if (pAddToQuery || pOrdinal.SiblingsBitwise())
            {
                lReturn = new A();
                lReturn.Parent = pSource;
                pSource.Query.AddEntityLeftJoinCustomKey(lReturn, pSource.EntityAlias, pCustomKey, pHand, pReadOnly);
                lReturn.GetOrdinal(pOrdinal);
            }
            else if (pSource.EntityAlias == "1" && pSource.Persistence && !pReadOnly)
            {
                lReturn = new A();
                pSource.Query.SaveAtributes.Add(pCustomKey, string.Concat("@", pCustomKey));
            }

            return lReturn;
        }

        public static B GetData<A, B>(IAbstractDal pDal, IOrdinal pOrdinal, System.Data.IDataReader pReader, bool pAddToQuery)
            where A : IAbstractDal, new()
            where B : IVO, new()
        {
            if (pAddToQuery)
            {
                pDal.Ordinal = pOrdinal;

                return (B)pDal.GetData(pReader);
            }

            return default(B);
        }
    }
}
