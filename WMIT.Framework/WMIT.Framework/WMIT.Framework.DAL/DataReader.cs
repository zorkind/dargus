using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMIT.Framework.VO;

namespace WMIT.Framework.DAL
{
    /// <summary>
    /// Metodos para leitura de classes de dados.
    /// </summary>
    public abstract class DataReader : VO.IDataReader
    {
        public System.Data.IDataReader Reader { get; set; }
        public virtual IOrdinal Ordinal { get; set; }

        public object GetValue(int pField, int pIndex)
        {
            if (Ordinal.Fields.Contains(pField))
                return Reader.IsDBNull(pIndex) ? null : Reader.GetValue(pIndex);
            else
                return null;
        }

        public short GetInt16(int pField, int pIndex, short pOutDefault = default(short))
        {
            if (Ordinal.Fields.Contains(pField))
                return Reader.IsDBNull(pIndex) ? pOutDefault : Reader.GetInt16(pIndex);
            else
                return pOutDefault;
        }

        public int GetInt32(int pField, int pIndex, int pOutDefault = default(int))
        {
            if (Ordinal.Fields.Contains(pField))
                return Reader.IsDBNull(pIndex) ? pOutDefault : Reader.GetInt32(pIndex);
            else
                return pOutDefault;
        }

        public long GetInt64(int pField, int pIndex, long pOutDefault = default(long))
        {
            if (Ordinal.Fields.Contains(pField))
                return Reader.IsDBNull(pIndex) ? pOutDefault : Reader.GetInt64(pIndex);
            else
                return pOutDefault;
        }

        public double GetDouble(int pField, int pIndex, double pOutDefault = default(double))
        {
            if (Ordinal.Fields.Contains(pField))
                return Reader.IsDBNull(pIndex) ? pOutDefault : Reader.GetDouble(pIndex);
            else
                return pOutDefault;
        }

        public decimal GetDecimal(int pField, int pIndex, decimal pOutDefault = default(decimal))
        {
            if (Ordinal.Fields.Contains(pField))
                return Reader.IsDBNull(pIndex) ? pOutDefault : Reader.GetDecimal(pIndex);
            else
                return pOutDefault;
        }

        public string GetString(int pField, int pIndex, string pOutDefault = default(string))
        {
            if (Ordinal.Fields.Contains(pField))
                return Reader.IsDBNull(pIndex) ? pOutDefault : Reader.GetString(pIndex);
            else
                return pOutDefault;
        }

        /// <summary>
        /// Atenção: no sql server não existe tipo Char, portanto é feito Reader.GetString(pIndex)[0], não foi testado no Oracle ou outros bancos.
        /// </summary>
        public char GetChar(int pField, int pIndex, char pOutDefault = default(char))
        {
            //return Reader.IsDBNull(pIndex) ? pOutDefault : Reader.GetChar(pIndex);

            if (Ordinal.Fields.Contains(pField))
                return Reader.IsDBNull(pIndex) ? pOutDefault : Reader.GetString(pIndex)[0];
            else
                return pOutDefault;
        }

        public DateTime GetDateTime(int pField, int pIndex, DateTime pOutDefault = default(DateTime))
        {
            if (Ordinal.Fields.Contains(pField))
            {
                var lReader = (System.Data.SqlClient.SqlDataReader)Reader;
                if(lReader.GetValue(pIndex) is TimeSpan)
                    return Reader.IsDBNull(pIndex) ? pOutDefault : Convert.ToDateTime(lReader.GetTimeSpan(pIndex));
                else
                    return Reader.IsDBNull(pIndex) ? pOutDefault : lReader.GetDateTime(pIndex);
            }
            else
                return pOutDefault;
        }

        public TimeSpan GetTimeSpan(int pField, int pIndex, TimeSpan pOutDefault = default(TimeSpan))
        {
            if (Ordinal.Fields.Contains(pField))
            {
                var lReader = (System.Data.SqlClient.SqlDataReader)Reader;
                return Reader.IsDBNull(pIndex) ? pOutDefault : lReader.GetTimeSpan(pIndex);
            }
            else
                return pOutDefault;
        }

        public bool GetBoolean(int pField, int pIndex, bool pOutDefault = default(bool))
        {
            if (Ordinal.Fields.Contains(pField))
                return Reader.IsDBNull(pIndex) ? pOutDefault : Reader.GetBoolean(pIndex);
            else
                return pOutDefault;
        }
    }
}
