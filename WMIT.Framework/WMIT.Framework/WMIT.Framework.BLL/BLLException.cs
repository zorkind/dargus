using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using System.Security;
using System.Text;

namespace WMIT.Framework.BLL
{
    [Serializable]
    public sealed class BLLException : ApplicationException
    {
        private BLLException()
        {

        }

        public BLLException(string message, Exception innerException)
            : base(message, innerException)
        {
            SavarLog();
        }

        public BLLException(string message)
            : base(message)
        {
            SavarLog();
        }

        public BLLException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            Classe = info.GetString("Classe");
            Metodo = info.GetString("Metodo");
            CheckPoint = info.GetInt32("CheckPoint");
            MensagemUsuario = info.GetString("MensagemUsuario");
            Logged = info.GetBoolean("Logged");
        }

        public BLLException(string message, string classe, string metodo, string mensagemUsuario, Exception innerException)
            : base(message, innerException)
        {
            Classe = classe;
            Metodo = metodo;
            MensagemUsuario = mensagemUsuario;
            SavarLog();
        }

        public BLLException(int checkpoint, string message, string classe, string metodo, string mensagemUsuario, Exception innerException)
            : base(message, innerException)
        {
            Classe = classe;
            Metodo = metodo;
            CheckPoint = checkpoint;
            MensagemUsuario = mensagemUsuario;
            SavarLog();
        }

        public string Classe { get; private set; }
        public string Metodo { get; private set; }
        public int CheckPoint { get; private set; }
        public string MensagemUsuario { get; private set; }
        public bool Logged { get; private set; }

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

        private void SavarLog()
        {
            StringBuilder objDados = new StringBuilder();
            try
            {
                objDados.Append("DAO Exception Log  ").AppendLine(DateTime.Now.TimeOfDay.ToString());
                objDados.AppendLine(Properties.Resources.Log_Separador);
                objDados.Append("Classe : ").AppendLine(Classe);
                objDados.Append("Método : ").AppendLine(Metodo);
                objDados.Append("CheckPoint : ").AppendLine(CheckPoint.ToString());
                objDados.Append("Mensagem ao Usuário : ").AppendLine(MensagemUsuario);
                objDados.Append("Mensagem de erro : ").AppendLine(InnerException.Message);
                objDados.AppendLine();
                objDados.AppendLine("Informações do Trace:");
                objDados.AppendLine(InnerException.ToString());
                objDados.AppendLine();

                using (FileStream objFile = File.Open(AppDomain.CurrentDomain.BaseDirectory + GerarNomeArquivo(), FileMode.Append))
                {
                    Byte[] objBytes = System.Text.Encoding.UTF8.GetBytes(objDados.ToString());

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
            finally
            {
                objDados = null;
            }
        }

        private string GerarNomeArquivo()
        {
            StringBuilder retorno = new StringBuilder();

            try
            {
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
