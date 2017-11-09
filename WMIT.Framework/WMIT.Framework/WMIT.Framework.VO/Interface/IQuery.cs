using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMIT.Framework.VO
{
    public interface IQuery
    {
        IParametrosBuilder Parametros { get; set; }
        StringBuilder Fields { get; set; }
        StringBuilder FieldsWithoutAlias { get; set; }
        StringBuilder Entities { get; set; }
        StringBuilder Where { get; set; }
        StringBuilder ConsultQuery { get; set; }
        string InsertQuery { get; set; }
        string UpdateQuery { get; set; }
        int FieldCount { get; set; }
        int EntityCount { get; set; }
        bool EnableGroupBy { get; set; }

        Dictionary<string, string> SaveAtributes { get; set; }

        int? Top { get; set; }
        string[] OrderBy { get; set; }
        VO.Paginate Paginate { get; set; }
        bool? DisableDateFieldPersistence { get; set; }
        bool Persistence { get; set; }

        int AddField(string pAttribute);

        int AddField(string pEntity, string pAttribute);

        int AddCustomField(string Query);

        int AddEntityLeftJoin(IAbstractDal pObj, string pEntityFrom);

        int AddEntityLeftJoin(IAbstractDal pObj, string pEntityFrom, bool pReadOnly);

        int AddEntityJoin(IAbstractDal pObj, string pEntityFrom);

        int AddEntityJoinCustomKey(IAbstractDal pObj, string pEntityFrom, string pKey);

        int AddEntityLeftJoinCustomKey(IAbstractDal pObj, string pEntityFrom, string pKey, CustomKeyHand pHand = CustomKeyHand.Left);

        int AddEntityLeftJoinCustomKey(IAbstractDal pObj, string pEntityFrom, string pKey, CustomKeyHand pHand, bool pReadOnly);

        int AddCustomEntityJoin(string Query);

        int AddEntityFrom(string pDataBase, string pSchema, string pEntityFrom);

        void BuildQuery();

        void BuildPersistence(IAbstractDal obj, bool isNew = true);

        void AddOutputSelect(IAbstractDal obj);
    }
}
