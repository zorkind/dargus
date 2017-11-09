using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMIT.Framework.VO;

namespace WMIT.Framework.VO
{
    public interface IDataReader
    {
        System.Data.IDataReader Reader { get; set; }
        IOrdinal Ordinal { get; set; }
    }
}
