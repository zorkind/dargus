using System;
using System.Data;
using System.Reflection;
using WMIT.Framework.DAL;

namespace WMIT.Framework.BLL
{
    public abstract class BLLBase
    {
        /// <summary>
        /// Propriedade para recuperar o código de erro nas Procedures
        /// </summary>
        public int CodigoErro { get; set; }

        /// <summary>
        /// Propriedade para recuperar a mensagem de erro nas Procedures
        /// </summary>
        public string MensagemErro { get; set; }

        /// <summary>
        /// Propriedade para recuperar a Connection String
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// Inicia uma transação utilizando IsolationLevel.ReadCommitted como default.
        /// Para modificar este comportamento, sobrescreva este método.
        /// </summary>
        public virtual void BeginTransaction()
        {
            BeginTransaction(IsolationLevel.ReadCommitted);
        }

        public void BeginTransaction(IsolationLevel objIso)
        {
            try
            {
                using (var lTC = TransactionControler.GetObject(ConnectionString))
                    lTC.BeginTran(objIso);
            }
            catch (DALException ex)
            {
                throw new BLLException(ex.Message,
                        this.GetType().Name,
                        MethodBase.GetCurrentMethod().Name,
                        Properties.Mensagens.Erro_BeginTransaction,
                        ex);
            }
            catch (BLLException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new BLLException(ex.Message,
                        this.GetType().Name,
                        MethodBase.GetCurrentMethod().Name,
                        Properties.Mensagens.Erro_Geral,
                        ex);
            }
        }

        public virtual void CommitTransaction()
        {
            try
            {
                using (var lTC = TransactionControler.GetObject(ConnectionString))
                    lTC.CommitTran();
            }
            catch (DALException ex)
            {
                throw new BLLException(ex.Message,
                        this.GetType().Name,
                        MethodBase.GetCurrentMethod().Name,
                        Properties.Mensagens.Erro_CommitTransaction,
                        ex);
            }
            catch (BLLException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new BLLException(ex.Message,
                        this.GetType().Name,
                        MethodBase.GetCurrentMethod().Name,
                        Properties.Mensagens.Erro_Geral,
                        ex);
            }
        }

        public virtual void RollbackTransaction()
        {
            try
            {
                using (var lTC = TransactionControler.GetObject(ConnectionString))
                    lTC.RollbackTran();
            }
            catch (DALException ex)
            {
                throw new BLLException(ex.Message,
                        this.GetType().Name,
                        MethodBase.GetCurrentMethod().Name,
                        Properties.Mensagens.Erro_RollbackTransaction,
                        ex);
            }
            catch (BLLException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new BLLException(ex.Message,
                        this.GetType().Name,
                        MethodBase.GetCurrentMethod().Name,
                        Properties.Mensagens.Erro_Geral,
                        ex);
            }
        }

        public virtual bool IsTransactionStarted()
        {
            try
            {
                using (var lTC = TransactionControler.GetObject(ConnectionString))
                    return lTC.IsTransactionStarted;
            }
            catch (DALException ex)
            {
                throw new BLLException(ex.Message,
                        this.GetType().Name,
                        MethodBase.GetCurrentMethod().Name,
                        Properties.Mensagens.Erro_CommitTransaction,
                        ex);
            }
            catch (BLLException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new BLLException(ex.Message,
                        this.GetType().Name,
                        MethodBase.GetCurrentMethod().Name,
                        Properties.Mensagens.Erro_Geral,
                        ex);
            }
        }
    }
}
