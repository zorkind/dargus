using System.Collections.Generic;
using System.Data;
using WMIT.Framework.VO;

namespace WMIT.Framework.VO
{
    public interface IAbstractDal : IDataReader
    {
        string Database { get; set; }
        string Schema { get; set; }
        string Entity { get; set; }
        string EntityAlias { get; set; }
        string EntityKey { get; set; }
        string EntityOrderField { get; set; }
        bool EntityKeyIdentity { get; set; }
        bool EntityLeft { get; set; }
        IQuery Query { get; set; }
        IAbstractDal Parent { get; set; }
        int ThreadId { get; set; }
        Dictionary<string, object> PersistenceParameters { get; set; }
        Dictionary<string, object> PersistenceOutputParameters { get; set; }
        bool Persistence { get; set; }
        string BuildWhere(VO.IParametros pParametros);
        void EvaluateParameters(VO.IParametros pParametros);
        void GetOrdinal();
        void GetOrdinal(IOrdinal pOrdinal);
        IVO GetData();
        IVO GetData(System.Data.IDataReader pReader);
        IVO GetFieldData();
        void GetSiblingsData(IVO pRetorno);
        bool GetData<A>(IList<A> pOutput) where A : IVO, new();
        bool GetData<A>(System.Data.IDataReader pReader, IList<A> pOutput) where A : IVO, new();
        void GetPersistenceParameters(IVO pObjeto);
        void SetOutputParameters(IVO pObjeto);
    }
}
