using System;
using System.Collections.Generic;
using System.Configuration;

namespace WMIT.Framework.Test.BLL
{
    public abstract class BLLBase<A, B, C, D> : Framework.BLL.BLLBase, IDisposable
        where A : Framework.VO.IVO, new()
        where B : Framework.VO.IBitwise, new()
        where C : Framework.VO.BaseOrdinal<B>, new()
        where D : Framework.VO.IParametros, new()
    {
        public BLLBase()
        {
            ReadOnly = true;
        }

        private bool ReadOnly = false;

        private IList<A> _Collection;
        public IList<A> Collection
        {
            get { return _Collection ?? (_Collection = new List<A>()); }
            set { _Collection = value; }
        }

        private D _Parametros;
        public D Parametros
        {
            get { return (_Parametros == null ? _Parametros = new D() : _Parametros); }
            set { _Parametros = value; }
        }

        private C _Ordinal;
        public C Ordinal
        {
            get { return (_Ordinal == null ? _Ordinal = new C() : _Ordinal); }
            set { _Ordinal = value; }
        }

        private VO.Usuario _Usuario;
        public VO.Usuario Usuario
        {
            get { return _Usuario ?? (_Usuario = new VO.Usuario()); }
            set { _Usuario = value; }
        }

        protected Exception _Erro;
        public Exception Erro
        {
            get { return _Erro; }
            set { _Erro = value; }
        }

        public BLLBase(VO.Usuario pUsuario)
        {
            Usuario = pUsuario;
        }

        public override void BeginTransaction()
        {
            base.BeginTransaction(System.Data.IsolationLevel.ReadUncommitted);
        }

        public bool Get<E>()
            where E : Framework.VO.IAbstractDal, new()
        {
            try
            {
                using (Framework.DAL.TransactionControler objControler = Framework.DAL.TransactionControler.GetObject())
                {
                    Collection = objControler.BuildObject<A, E>(Parametros, Ordinal);
                }

                return true;
            }
            catch (Exception ex)
            {
                _Erro = ex;
            }

            return false;
        }

        public bool Salvar<E>(A obj)
            where E : Framework.VO.IAbstractDal, new()
        {
            if (ReadOnly)
                return false;

            try
            {
                if (!Validate(obj))
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

        internal virtual bool Validate(A obj)
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
