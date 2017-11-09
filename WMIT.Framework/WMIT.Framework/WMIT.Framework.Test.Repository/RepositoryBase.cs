using System;
using System.Collections.Generic;

namespace WMIT.Framework.Test.Repository
{
    public class RepositoryBase<A, B, C, D, E> : BLL.BLLBase, Model.Repository.IRepositoryBase<A, B, C, D>, IDisposable
        where A : Framework.VO.IVO, new()
        where B : Framework.VO.IBitwise, new()
        where C : Framework.VO.BaseOrdinal<B>, new()
        where D : Framework.VO.Parametros, new()
        where E : Framework.VO.IAbstractDal, new()
    {
        public override void BeginTransaction()
        {
            base.BeginTransaction(System.Data.IsolationLevel.ReadUncommitted);
        }

        protected Exception _Erro;
        public Exception Erro
        {
            get { return _Erro; }
            set { _Erro = value; }
        }

        public VO.Usuario Usuario { get; set; }

        public IList<A> Get(D Parametros = null, C Ordinal = null)
        {
            IList<A> Collection = null;
            if (Parametros == null) Parametros = new D();
            if (Ordinal == null) Ordinal = new C();

            try
            {
                using (Framework.DAL.TransactionControler objControler = Framework.DAL.TransactionControler.GetObject())
                {
                    Collection = objControler.BuildObject<A, E>(Parametros, Ordinal);
                }

                return Collection;
            }
            catch (Exception ex)
            {
                _Erro = ex;
            }

            return null;
        }

        public bool Salvar(A obj)
        {
            try
            {
                if (!Validate(obj) || Usuario == null || Usuario.Codigo == 0)
                    return false;

                if (obj.isNew)
                    obj.CodigoUsuarioCadastro = this.Usuario.Codigo;
                else if (obj.CodigoUsuarioAtualizacao == 0)
                    obj.CodigoUsuarioAtualizacao = this.Usuario.Codigo;

                using (Framework.DAL.TransactionControler objControler = Framework.DAL.TransactionControler.GetObject())
                {
                    return objControler.PersisteObject<E, A>(obj);
                }
            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                _Erro = ex;
            }
            catch (Exception ex)
            {
                _Erro = ex;
            }

            return false;
        }

        public virtual bool Validate(A obj)
        {
            return true;
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool pDisposing)
        {
            if (pDisposing)
            {
                GC.SuppressFinalize(this);
            }
        }
    }
}
