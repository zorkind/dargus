using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Linq;
using WMIT.Framework.VO;

namespace WMIT.Framework.DAL
{
    public class PersistenceCache
    {
        public string Query { get; set; }
        public Dictionary<string, string> SaveAtributes { get; set; }
    }

    public class TransactionControler : IDisposable
    {
        private int? Top { get; set; }
        private VO.Paginate Paginate { get; set; }
        public static string ConnectionStringDefault { get; set; }
        public Connection<SqlConnection> Conexao { get; set; }
        private static ConcurrentDictionary<string, TransactionControler> objTranList { get; set; }
        private SqlTransaction objDBTran { get; set; }
        public bool IsTransactionStarted { get; private set; }
        private static ConcurrentDictionary<string, PersistenceCache> PersistenceCache { get; set; }
        private static string _ConnectionStringName { get; set; }
        private static string _ConnectionString { get; set; }

        public static void HandleConnectionString()
        {
            if (TransactionControler.ConnectionStringDefault == null)
            {
                if (_ConnectionStringName == null)
                {
                    var lConnectionStringName = ConfigurationManager.AppSettings["ConnectionStringName"];
                    if (lConnectionStringName == null)
                        throw new NullReferenceException(Properties.Mensagens.Erro_ConnectionString_Name);

                    _ConnectionStringName = lConnectionStringName.ToString();
                }

                if (_ConnectionString == null)
                {
                    var lConnectionString = ConfigurationManager.ConnectionStrings[_ConnectionStringName];
                    if (lConnectionString == null)
                        throw new NullReferenceException(Properties.Mensagens.Erro_ConnectionString_Name);

                    _ConnectionString = lConnectionString.ConnectionString;
                }

                //Aproveita que percebeu a falta da connectionstringDefault e recupera os Defaults da AbstractDal.
                var _db = AbstractDal.DatabaseDefault;
                var _sch = AbstractDal.SchemaDefault;

                TransactionControler.ConnectionStringDefault = _ConnectionString;
            }
        }

        public static TransactionControler GetObject()
        {
            HandleConnectionString();

            return GetObject(TransactionControler.ConnectionStringDefault);
        }

        public static TransactionControler GetObject(string ConnectionString)
        {
            return GetObject(ConnectionString, System.Threading.Thread.CurrentThread.ManagedThreadId);
        }

        public static TransactionControler GetObject(string ConnectionString, int ThreadId)
        {
            TransactionControler objTran = null;

            if (string.IsNullOrEmpty(ConnectionString))
            {
                HandleConnectionString();
                ConnectionString = _ConnectionString;
            }

            try
            {
                var lTranKey = string.Concat(ConnectionString, ThreadId.ToString());

                if (objTranList == null)
                {
                    objTranList = new ConcurrentDictionary<string, TransactionControler>();
                    objTran = CriarInstanciaSingleton(lTranKey);
                    objTranList.TryAdd(lTranKey, objTran);
                }
                else
                {
                    objTranList.TryGetValue(lTranKey, out objTran);

                    if (objTran == null)
                    {
                        objTran = CriarInstanciaSingleton(lTranKey);
                        objTranList.TryAdd(lTranKey, objTran);
                    }
                }

                return objTran;
            }
            catch (DALException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new DALException(ex.Message, "TransactionControler", "GetObject Tid:" + ThreadId.ToString(), Properties.Mensagens.Erro_Geral, ex);
            }
        }

        private static TransactionControler CriarInstanciaSingleton(string ConnectionString)
        {
            return new TransactionControler(ConnectionString);
        }

        private TransactionControler(string ConnectionString)
        {
            try
            {
                Conexao = new Connection<SqlConnection>(ConnectionString);
            }
            catch (DALException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new DALException(ex.Message, "TransactionControler", "Constructor", Properties.Mensagens.Erro_Geral, ex);
            }
        }

        public IList<A> BuildObject<A, B>(VO.IParametros pParametros, VO.IOrdinal pOrdinal)
            where A : VO.IVO, new()
            where B : IAbstractDal, new()
        {
            B lObject = default(B);

            try
            {
                IList<A> objRetorno = null;

                lObject = PrepareObjectForOutput<A, B>(pParametros, pOrdinal);

                CommandText = lObject.Query.ToString();
                CommandType = CommandType.Text;
                ExecuteReader();

                objRetorno = new List<A>();
                while (Reader.Read() && lObject.GetData(Reader, objRetorno)) ;

                if (!Reader.IsClosed)
                    Reader.Close();

                if (lObject.Query.Paginate.Enabled)
                {
                    if (Parameter("@totalRows").Value != DBNull.Value)
                        lObject.Query.Paginate.TotalLinhas = (long)Parameter("@totalRows").Value;

                    Paginate = lObject.Query.Paginate;
                }

                Conexao.Command.Parameters.Clear();

                return objRetorno;
            }
            catch (DALException)
            {
                throw;
            }
            catch (Exception ex)
            {
                if (lObject != null)
                {
                    throw new DALException(ex.Message + "\r\n\r\nQuery:" + lObject.Query.ToString(), "TransactionControler", "BuildObject", "Não foi possível materializar o(s) objeto(s).", ex);
                }
                else
                {
                    throw new DALException(ex.Message, "TransactionControler", "BuildObject", "Não foi possível materializar o(s) objeto(s).", ex);
                }
            }
        }

        public bool PersisteObject<A, B>(B pObject)
            where A : IAbstractDal, new()
            where B : VO.IVO, new()
        {
            A lPrepObject;
            if (PrepareObjectForPersistence<A, B>(out lPrepObject, pObject.isNew))
            {
                lPrepObject.GetPersistenceParameters(pObject);

                var lUSCadPar = string.Format("@{0}_cadastro_us_id", lPrepObject.EntityKey.Remove(lPrepObject.EntityKey.LastIndexOf('_')));
                var lUSAtPar = string.Format("@{0}_atualizacao_us_id", lPrepObject.EntityKey.Remove(lPrepObject.EntityKey.LastIndexOf('_')));
                var lAtivoPar = string.Format("@{0}_ativo", lPrepObject.EntityKey.Remove(lPrepObject.EntityKey.LastIndexOf('_')));
                var lEntityKeyPar = string.Format("@{0}", lPrepObject.EntityKey);
                var lAddKeyPar = true;

                if (lPrepObject.PersistenceOutputParameters != null && lPrepObject.PersistenceOutputParameters.Count > 0)
                    lAddKeyPar = !lPrepObject.PersistenceOutputParameters.ContainsKey(lEntityKeyPar);

                if (lAddKeyPar)
                    lAddKeyPar = !lPrepObject.PersistenceParameters.ContainsKey(lEntityKeyPar);

                if (lAddKeyPar)
                    AddOutputParameter(lEntityKeyPar, pObject.Codigo);

                if (!lPrepObject.PersistenceParameters.ContainsKey(lAtivoPar))
                    AddParameter(lAtivoPar, pObject.Ativo);

                if (pObject.isNew)
                {
                    if (!lPrepObject.PersistenceParameters.ContainsKey(lUSCadPar))
                        AddParameter(lUSCadPar, pObject.CodigoUsuarioCadastro);
                }
                else
                    if (!lPrepObject.PersistenceParameters.ContainsKey(lUSAtPar))
                        AddParameter(lUSAtPar, pObject.CodigoUsuarioAtualizacao);

                foreach (var lPar in lPrepObject.PersistenceParameters)
                    AddParameter(lPar.Key, lPar.Value);

                if (lPrepObject.PersistenceOutputParameters != null && lPrepObject.PersistenceOutputParameters.Count > 0)
                {
                    foreach (var lPar in lPrepObject.PersistenceOutputParameters)
                        AddOutputParameter(lPar.Key, lPar.Value);

                    lPrepObject.Query.AddOutputSelect(lPrepObject);
                }

                if (!pObject.isNew)
                    CommandText = lPrepObject.Query.UpdateQuery;
                else
                    CommandText = lPrepObject.Query.InsertQuery;

                ExecuteNonQuery();

                if (lPrepObject.PersistenceOutputParameters != null && lPrepObject.PersistenceOutputParameters.Count > 0)
                {
                    foreach (var lkey in lPrepObject.PersistenceOutputParameters.Keys.ToList())
                    {
                        if (Parameter(lkey).Value != DBNull.Value)
                            lPrepObject.PersistenceOutputParameters[lkey] = Parameter(lkey).Value;
                    }

                    if (lPrepObject.PersistenceOutputParameters.Keys.Count > 0)
                        lPrepObject.SetOutputParameters(pObject);
                }

                if (Parameter(string.Format("@{0}", lPrepObject.EntityKey)).Value != DBNull.Value)
                    pObject.Codigo = Convert.ToInt32(Parameter(string.Format("@{0}", lPrepObject.EntityKey)).Value);

            }
            return true;
        }

        public B PrepareObjectForOutput<A, B>(VO.IParametros pParametros, VO.IOrdinal pOrdinal)
            where A : VO.IVO, new()
            where B : IAbstractDal, new()
        {
            B lDal = new B();

            try
            {
                if (pOrdinal.Fields.All() == 0)
                    throw new DALException("Não foram encontrados campos selecionados para a consulta.", MethodBase.GetCurrentMethod().DeclaringType.Name,
                                        MethodBase.GetCurrentMethod().Name, "Não foi possível preparar o objeto.", null);

                lDal.Ordinal = pOrdinal;
                lDal.Query.AddEntityFrom(
                        lDal.Database,
                        lDal.Schema,
                        lDal.Entity);

                lDal.EvaluateParameters(pParametros);

                lDal.GetOrdinal();

                lDal.Query.Top = pParametros.Top;
                lDal.Query.OrderBy = pParametros.OrderBy;
                lDal.Query.Paginate = pParametros.Paginate;
                lDal.Query.EnableGroupBy = pParametros.EnableGroup;

                lDal.Query.Parametros.BuildWhereDefaults(lDal, pParametros);

                if (pParametros.OrderBy.Length == 0 && !string.IsNullOrEmpty(lDal.EntityOrderField))
                    lDal.Query.OrderBy = new string[] { string.Format("[{0}].[{1}]", lDal.EntityAlias, lDal.EntityOrderField) };

                lDal.Query.BuildQuery();

                foreach (var lPar in lDal.Query.Parametros.QueryParameters)
                    AddParameter(lPar.Key, lPar.Value);

                if (lDal.Query.Paginate.Enabled)
                {
                    if (lDal.Query.Paginate.Pagina == 0)
                        lDal.Query.Paginate.Pagina = 1;

                    if (lDal.Query.Paginate.LinhasPorPagina == 0)
                        lDal.Query.Paginate.LinhasPorPagina = 10;

                    AddOutputParameter("@totalRows", default(long));
                    AddParameter("@PageNumber", lDal.Query.Paginate.Pagina);
                    AddParameter("@RowspPage", lDal.Query.Paginate.LinhasPorPagina);
                }

                return lDal;
            }
            catch (DALException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new DALException(ex.Message, "TransactionControler", "PrepareObjectForOutput", "Não foi possível preparar o objeto.", ex);
            }
        }

        public bool PrepareObjectForPersistence<A, B>(out A pDal, bool isNew = true)
            where A : IAbstractDal, new()
            where B : VO.IVO, new()
        {
            try
            {
                pDal = new A();
                PersistenceCache lPers = null;

                if (PersistenceCache == null)
                    PersistenceCache = new ConcurrentDictionary<string, PersistenceCache>();
                else
                {
                    if (PersistenceCache.ContainsKey(string.Concat(pDal.Entity, isNew.ToString())))
                    {
                        if (PersistenceCache.TryGetValue(string.Concat(pDal.Entity, isNew.ToString()), out lPers))
                        {
                            if (isNew)
                                pDal.Query.InsertQuery = lPers.Query;
                            else
                                pDal.Query.UpdateQuery = lPers.Query;

                            pDal.Query.SaveAtributes = lPers.SaveAtributes;
                            pDal.PersistenceParameters = new Dictionary<string, object>();
                            pDal.PersistenceOutputParameters = new Dictionary<string, object>();

                            return true;
                        }
                    }
                }

                pDal.PersistenceParameters = new Dictionary<string, object>();
                pDal.PersistenceOutputParameters = new Dictionary<string, object>();
                pDal.Persistence = true;
                pDal.Query.Persistence = true;

                pDal.Query.AddEntityFrom(
                    pDal.Database,
                    pDal.Schema,
                    pDal.Entity);

                pDal.GetOrdinal();

                pDal.Query.BuildPersistence(pDal, isNew);

                PersistenceCache.TryAdd(string.Concat(pDal.Entity, isNew.ToString()), new PersistenceCache()
                {
                    Query = (isNew ? pDal.Query.InsertQuery : pDal.Query.UpdateQuery),
                    SaveAtributes = pDal.Query.SaveAtributes
                });

                return true;
            }
            catch (DALException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new DALException(ex.Message, "TransactionControler", "PrepareObjectForPersistence", "Não foi possível preparar o objeto.", ex);
            }
        }

        public void BeginTran()
        {
            BeginTran(IsolationLevel.ReadCommitted);
        }

        public void BeginTran(IsolationLevel IL)
        {
            try
            {
                if (!IsTransactionStarted)
                {
                    Conexao.Open();
                    objDBTran = (SqlTransaction)Conexao.Conn.BeginTransaction(IL);
                    Conexao.Command.Transaction = objDBTran;
                    IsTransactionStarted = true;
                }
            }
            catch (DALException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new DALException(ex.Message, "TransactionControler", "BeginTran", Properties.Mensagens.Erro_Geral, ex);
            }
        }

        public void CommitTran()
        {
            try
            {
                if (objDBTran != null)
                {
                    if (IsTransactionStarted)
                    {
                        objDBTran.Commit();
                        IsTransactionStarted = false;
                    }
                }
                else
                    throw new NullReferenceException("O objeto de transação não possui instância definida.");
            }
            catch (DALException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new DALException(ex.Message, "TransactionControler", "CommitTran", Properties.Mensagens.Erro_Geral, ex);
            }
        }

        public void RollbackTran()
        {
            try
            {
                if (objDBTran != null)
                {
                    if (IsTransactionStarted)
                    {
                        objDBTran.Rollback();
                        objDBTran.Dispose();
                        IsTransactionStarted = false;
                    }
                }
                else
                    throw new NullReferenceException("O objeto de transação não possui instância definida.");
            }
            catch (DALException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new DALException(ex.Message, "TransactionControler", "RollbackTran", Properties.Mensagens.Erro_Geral, ex);
            }
        }

        public void AddParameter(string nome, object value)
        {
            Conexao.Command.Parameters.Add(new SqlParameter(nome, value));
        }

        public void AddOutputParameter(string nome, object value)
        {
            var lParameter = new SqlParameter(nome, value);
            lParameter.Direction = ParameterDirection.InputOutput;

            Conexao.Command.Parameters.Add(lParameter);
        }

        public IDataParameter Parameter(string nome)
        {
            return (IDataParameter)Conexao.Command.Parameters[nome];
        }

        public void ParameterSize(string nome, int Size)
        {
            ((SqlParameter)this.Conexao.Command.Parameters[nome]).Size = Size;
        }

        public string CommandText
        {
            get { return this.Conexao.Command.CommandText; }
            set { this.Conexao.Command.CommandText = value; }
        }

        public CommandType CommandType
        {
            get { return this.Conexao.Command.CommandType; }
            set
            {
                if (value == CommandType.StoredProcedure && this.Conexao.Command.Parameters != null)
                    this.Conexao.Command.Parameters.Clear();

                this.Conexao.Command.CommandType = value;
            }
        }

        public void ExecuteNonQuery()
        {
            this.Conexao.Command.ExecuteNonQuery();
        }

        public void ExecuteReader()
        {
            if (IsTransactionStarted)
                ExecuteReader(CommandBehavior.Default);
            else
                ExecuteReader(CommandBehavior.CloseConnection);
        }

        public void ExecuteReader(CommandBehavior CB)
        {
            if (IsTransactionStarted && CB == CommandBehavior.CloseConnection)
                throw new ApplicationException("Não é possível iniciar um objeto Reader com Behavior: CloseConnection quando uma transação está aberta.");

            this.Conexao.objRead = Conexao.Command.ExecuteReader(CB);
        }

        public System.Data.IDataReader Reader
        {
            get { return this.Conexao.objRead; }
            set { this.Conexao.objRead = value; }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool pDisposing)
        {
            if (pDisposing)
            {
                if (!IsTransactionStarted)
                {
                    if (objDBTran != null)
                    {
                        objDBTran.Dispose();
                        objDBTran = null;
                    }

                    if (Conexao != null && Conexao.Conn.State == ConnectionState.Open)
                        Conexao.Close();

                    GC.SuppressFinalize(this);
                }
                else
                {
                    this.Conexao.Command.CommandText = string.Empty;
                    this.Conexao.Command.CommandType = CommandType.Text;
                    if (this.Conexao.Command.Parameters != null && this.Conexao.Command.Parameters.Count != 0)
                        this.Conexao.Command.Parameters.Clear();

                    if (Conexao.objRead != null && !Conexao.objRead.IsClosed)
                    {
                        Conexao.objRead.Close();
                        Conexao.objRead.Dispose();
                    }
                }
            }
        }
    }
}
