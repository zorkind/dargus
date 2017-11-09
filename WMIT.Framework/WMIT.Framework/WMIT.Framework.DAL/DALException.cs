using System;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Diagnostics;
using System.Security;

namespace WMIT.Framework.DAL
{
    [Serializable]
    public sealed class DALException : ApplicationException
    {
        private DALException()
        {

        }

        public DALException(string message, Exception innerException)
            : base(message, innerException)
        {
            SavarLog();
        }

        public DALException(string message)
            : base(message)
        {
            SavarLog();
        }

        public DALException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            Classe = info.GetString("Classe");
            Metodo = info.GetString("Metodo");
            CheckPoint = info.GetInt32("CheckPoint");
            MensagemUsuario = info.GetString("MensagemUsuario");
            Logged = info.GetBoolean("Logged");
        }

        public DALException(string message, string classe, string metodo, string mensagemUsuario, Exception innerException)
            : base(message, innerException)
        {
            Classe = classe;
            Metodo = metodo;
            MensagemUsuario = mensagemUsuario;
            SavarLog();
        }

        public DALException(int checkpoint, string message, string classe, string metodo, string mensagemUsuario, Exception innerException)
            : base(message, innerException)
        {
            Classe = classe;
            Metodo = metodo;
            CheckPoint = checkpoint;
            MensagemUsuario = mensagemUsuario;
            SavarLog();
        }
        
        [SecurityCritical]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("Classe", Classe);
            info.AddValue("Metodo", Metodo);
            info.AddValue("CheckPoint", CheckPoint);
            info.AddValue("MensagemUsuario", MensagemUsuario);
            info.AddValue("Logged", Logged);
        }

        public string Classe { get; private set; }
        public string Metodo { get; private set; }
        public int CheckPoint { get; private set; }
        public string MensagemUsuario { get; private set; }
        public bool Logged { get; private set; }

        private void SavarLog()
        {
            StringBuilder objDados = new StringBuilder();

            try
            {
                objDados.Append("DAL Exception Log  ").AppendLine(DateTime.Now.TimeOfDay.ToString());
                objDados.AppendLine(Properties.Resources.Log_Separador);
                objDados.Append("Classe : ").AppendLine(Classe);
                objDados.Append("Método : ").AppendLine(Metodo);
                objDados.Append("CheckPoint : ").AppendLine(CheckPoint.ToString());
                objDados.Append("Mensagem ao Usuário : ").AppendLine(MensagemUsuario);
                objDados.Append("Mensagem de erro : ").AppendLine(Message);
                objDados.Append("Mensagem de erro InnerException : ").AppendLine(InnerException.Message);
                objDados.AppendLine();
                objDados.AppendLine("Informações do Trace:");
                objDados.AppendLine(InnerException.ToString());
                objDados.AppendLine();

                using (FileStream objFile = File.Open(AppDomain.CurrentDomain.BaseDirectory + GerarNomeArquivo(), FileMode.Append, FileAccess.Write))
                {
                    Byte[] objBytes = Encoding.UTF8.GetBytes(objDados.ToString());
                    objFile.Write(objBytes, 0, objBytes.Length);
                }
                Logged = true;

                if (!EventLog.SourceExists(AppDomain.CurrentDomain.FriendlyName))
                    EventLog.CreateEventSource(AppDomain.CurrentDomain.FriendlyName, String.Empty);

                EventLog.WriteEntry(AppDomain.CurrentDomain.FriendlyName, objDados.ToString(), EventLogEntryType.Warning, CheckPoint);
            }
            catch (Exception ex)
            {
                MensagemUsuario += (char)13 + (char)10 + Properties.Mensagens.Erro_Geral_Log + ex.Message;
            }
        }

        private string GerarNomeArquivo()
        {
            StringBuilder retorno = new StringBuilder();
            string path = Properties.Resources.Arquivo_Log;
            try
            {
                if (!Directory.Exists(path.Substring(0, path.LastIndexOf('\\'))))
                {
                    Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + path.Substring(0, path.LastIndexOf('\\')));
                }

                retorno.Append(DateTime.Now.Year.ToString());
                retorno.Append(DateTime.Now.Month.ToString());
                retorno.Append(DateTime.Now.Day.ToString());

                return Properties.Resources.Arquivo_Log.Replace(((Char)42).ToString(), retorno.ToString());
            }
            finally
            {
                retorno = null;
            }
        }
    }
}
