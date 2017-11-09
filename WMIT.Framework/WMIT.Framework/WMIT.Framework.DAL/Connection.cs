using System;
using System.Configuration;
using System.Data;
using System.Reflection;

namespace WMIT.Framework.DAL
{
    public class Connection<T> : IDisposable where T : IDbConnection, new()
    {
        public string ConnectionString { get; set; }
        public System.Data.IDataReader objRead { get; set; }
        public IDbConnection Conn { get; private set; }

        private IDbCommand _command;
        public IDbCommand Command
        {
            get
            {
                return CommandProxy();
            }
            set
            {
                _command = value;
            }
        }

        public IDbCommand CommandProxy()
        {
            if (Conn != null)
            {
                if (Conn.State == ConnectionState.Open && _command == null)
                    _command = Conn.CreateCommand();
                else if (Conn.State != ConnectionState.Open && _command != null)
                    Open();
                else if (Conn.State != ConnectionState.Open && _command == null)
                {
                    Open();
                    _command = Conn.CreateCommand();
                }
            }
            else
            {
                Open();
                _command = Conn.CreateCommand();
            }

            _command.CommandTimeout = 30;
            return _command;
        }

        public Connection()
        {
            //CriaInstancia();
        }

        public Connection(string ConnectionString)
        {
            this.ConnectionString = ConnectionString;
            //CriaInstancia();
        }

        private void CriaInstancia()
        {
            if (Conn != null)
                return;
            
            try
            {
                Conn = new T();

                if (ConnectionString == null)
                    throw new ConfigurationErrorsException(Properties.Mensagens.Erro_ConnectionString_Name);
            }
            catch (Exception ex)
            {
                throw new DALException(ex.Message, "Connection", "CriaInstancia", Properties.Mensagens.Erro_Geral, ex);
            }
        }

        internal void Open()
        {
            CriaInstancia();

            try
            {
                if (Conn.State == ConnectionState.Closed)
                {
                    if (Conn.ConnectionString != ConnectionString)
                        Conn.ConnectionString = ConnectionString;

                    Conn.Open();
                }
            }
            catch (Exception ex)
            {
                throw new DALException(ex.Message, "Connection", "Open", Properties.Mensagens.Erro_Geral, ex);
            }

        }

        internal void Close()
        {
            if (Conn.State == ConnectionState.Open)
                Conn.Close();

            if (objRead != null && !objRead.IsClosed)
            {
                objRead.Close();
                objRead.Dispose();
            }
            else if (objRead != null && objRead.IsClosed)
                objRead.Dispose();

            objRead = null;
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool pDisposing)
        {
            if(pDisposing)
            {
                if (Conn != null)
                    Close();

                if (_command != null)
                    _command.Dispose();

                _command = null;

                if (objRead != null && !objRead.IsClosed)
                {
                    objRead.Close();
                    objRead.Dispose();
                }
                else if (objRead != null && objRead.IsClosed)
                    objRead.Dispose();

                objRead = null;

                if (Conn != null)
                    Conn.Dispose();

                Conn = null;

                GC.SuppressFinalize(this);
            }
        }
    }
}
