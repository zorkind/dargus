#region Usings
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
#endregion

namespace WMIT.Framework.Utilitarios
{
    public static class Helper
    {
        #region FormatarCPF_CNPJ
        /// <summary>
        /// Método para formatar CPF e CNPJ
        /// </summary>
        /// <param name="texto">O CPF ou CNPJ que será formatado</param>
        /// <returns>Retorna o texto informado formatado para CNPJ ou CPF.</returns>
        public static string FormatarCPF_CNPJ(string texto)
        {
            Regex objRegCPF = new Regex("^?([0-9]{3})?([0-9]{3})?([0-9]{3})?([0-9]{2})$");
            Regex objRegCNPJ = new Regex("^?([0-9]{2})?([0-9]{3})?([0-9]{4})?([0-9]{2})$");

            //Formata os dados
            if (objRegCNPJ.IsMatch(texto))
                return objRegCNPJ.Replace(texto, "$1.$2/$3-$4");
            else if (objRegCPF.IsMatch(texto))
                return objRegCPF.Replace(texto, "$1.$2.$3-$4");
            else
                return texto;
        }
        #endregion

        #region ToCamelCase
        public static string ToCamelCase(string texto)
        {
            return ToCamelCase(texto.ToLower().Trim(), 0);
        }
        private static string ToCamelCase(string texto, int StartIndex)
        {
            System.Text.StringBuilder objBuild;

            objBuild = new System.Text.StringBuilder();
            objBuild.Append(texto.Substring(0, StartIndex));
            objBuild.Append(texto.Substring(StartIndex, 1).ToUpper());
            objBuild.Append(texto.Substring(++StartIndex));

            StartIndex = texto.IndexOf(" ", StartIndex);
            if (StartIndex > -1)
                return ToCamelCase(objBuild.ToString(), ++StartIndex);
            else
                return objBuild.ToString();
        }
        #endregion

        #region ValidMailAddress
        public static bool ValidMailAddress(string address)
        {
            string patternStrict = @"^(([^<>()[\]\\.,;:\s@\""]+"
                                  + @"(\.[^<>()[\]\\.,;:\s@\""]+)*)|(\"".+\""))@"
                                  + @"((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}"
                                  + @"\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+"
                                  + @"[a-zA-Z]{2,}))$";
            Regex reStrict = new Regex(patternStrict);

            return reStrict.IsMatch(address);
        }
        #endregion

        #region Formata Erros
        public static string RetornaHtmlErros(List<String> erros)
        {
            System.Text.StringBuilder objBuild = new System.Text.StringBuilder();

            //objBuild.AppendLine("Alguns erros foram encontrados:<br/><br/>");
            objBuild.AppendLine("<center><div style='text-align:left; width:400px;'>");
            foreach (string msg in erros)
            {
                objBuild.AppendLine(String.Format("•  {0}<br/>", msg));
            }
            objBuild.AppendLine("</div></center>");
            return objBuild.ToString();
        }
        #endregion

        #region GerarSenha
        public static string GerarSenha()
        {
            string retorno = String.Empty;
            Random objR = new Random();

            //Primeiro caracter, letra em maiúsculo
            retorno += (char)objR.Next(65, 90);

            //3 próximos, letras minúsculas.
            for (int i = 0; i < 3; i++)
            {
                retorno += (char)objR.Next(97, 122);
            }

            //um underline
            retorno += "_";

            //4 próximos, números.
            for (int i = 0; i < 4; i++)
            {
                retorno += (char)objR.Next(48, 57);
            }

            return retorno;
        }
        #endregion

        #region RemoverHtml
        public static string RemoverHtml(string html)
        {
            if (html.IndexOf('<') > -1)
                return RemoverHtml(html.Remove(html.IndexOf('<'), (html.IndexOf('>') - html.IndexOf('<')) + 1));
            else
                return html;
        }
        #endregion

        #region IsEmpty
        public static object IsEmpty(object Check, object Retorno)
        {
            if (Check == null || Check is DBNull)
                return Retorno;
            else
                return Check;
        }

        public static string IsEmpty(object Check, string Retorno)
        {
            if (Check == null || Check is DBNull)
                return Retorno;
            else
                if (Check.ToString() != "")
                    return Check.ToString();
                else
                    return Retorno;
        }

        public static int IsEmpty(object Check, int Retorno)
        {
            if (Check == null || Check is DBNull)
                return Retorno;
            else
                return Convert.ToInt32(Check);
        }

        public static double IsEmpty(object Check, double Retorno)
        {
            if (Check == null || Check is DBNull)
                return Retorno;
            else
                return Convert.ToDouble(Check);
        }

        public static string IsEmpty(double Check, string Retorno)
        {
            return Check.ToString("N");
        }

        public static int IsEmpty(string Check, int Retorno)
        {
            if (Check == null)
                return Retorno;
            else
                if (Check != "")
                    return Convert.ToInt32(Check);
                else
                    return Retorno;
        }

        public static string IsEmpty(string Check, string Retorno)
        {
            if (Check == null)
                return Retorno;
            else
                if (Check != "")
                    return Check.ToString();
                else
                    return Retorno;
        }

        public static string IsEmpty(DateTime Check, string Retorno)
        {
            if (Check != DateTime.MinValue)
                return Check.ToShortDateString();
            else
                return Retorno;
        }

        public static DateTime IsEmpty(DateTime Check, DateTime Retorno)
        {
            if (Check == null)
                return Retorno;
            else if (Check != DateTime.MinValue)
                return Check;
            else
                return Retorno;
        }

        public static DateTime IsEmpty(object Check, DateTime Retorno)
        {
            if (Check == null)
                return Retorno;
            else if ((DateTime)Check != DateTime.MinValue)
                return (DateTime)Check;
            else
                return Retorno;
        }

        public static DateTime IsEmpty(string Check, DateTime Retorno)
        {
            if (Check == null)
                return Retorno;
            else
                if (Check != "")
                {
                    DateTime retorno;
                    if (DateTime.TryParse(Check, out retorno))
                        return retorno;
                    else
                        return Retorno;
                }
                else
                    return Retorno;
        }
        #endregion

        #region DataGridView_AddRow
        /// <summary>
        /// Adiciona valores as colunas de uma nova linha no gridview.
        /// Utilizado para grids com colunas dinâmicas.
        /// </summary>
        /// <param name="dg">O controle gridview que receberá a nova linha</param>
        /// <param name="arrPropriedades">Array com as propriedades do objeto VO que irá popular as colunas.</param>
        static public void DataGridView_AddRow(ref DataGridView dg, object[,] arrPropriedades)
        {
            object[] arrValores = new object[dg.Columns.Count];

            foreach (DataGridViewColumn objCol in dg.Columns)
            {
                for (int i = 0; i < arrPropriedades.GetLength(0); i++)
                {
                    if (arrPropriedades[i, 0].ToString() == objCol.DataPropertyName)
                    {
                        arrValores[objCol.Index] = arrPropriedades[i, 1];
                        break;
                    }
                }
            }
            dg.Rows.Add(arrValores);
        }
        #endregion
    }
}
