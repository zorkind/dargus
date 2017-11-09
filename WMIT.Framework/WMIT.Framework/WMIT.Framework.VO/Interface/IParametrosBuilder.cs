using System.Collections.Generic;
using System.Text;

namespace WMIT.Framework.VO
{
    public interface IParametrosBuilder
    {
        StringBuilder Builder { get; set; }
        Dictionary<string, object> QueryParameters { get; set; }
        void BuildWhereDefaults(IAbstractDal pEntity, VO.IParametros pParametros);
        void AppendEntity(int pValue, IAbstractDal pEntity);
        void AppendEntity(int pValue, IAbstractDal pEntity, string pCustomKey);
        void AppendEntity(int pValue, IAbstractDal pEntity, string pCustomLeftKey, string pCustomRightKey);
        void AppendEntity(int pValue, IAbstractDal pEntity, string pCustomKey, CustomKeyHand pKeyHand);
        void AppendEqual(object pValue, string pAlias, string pName);
        void AppendIsNull(bool pIsNull, string pAlias, string pName);
        void AppendBitwise(object pValue, string pAlias, string pName);
        void AppendLike(object pValue, string pAlias, string pName, bool pInsensitive = false);
        void AppendBetween(object pValue, object pValue2, string pAlias, string pName);
        void AppendBiggerThen(object pValue, string pAlias, string pName);
        void AppendSmallerThen(object pValue, string pAlias, string pName);
        void AppendBiggerOrEqualThen(object pValue, string pAlias, string pName);
        void AppendSmallerOrEqualThen(object pValue, string pAlias, string pName);
        void AppendIn(int[] pValue, string pAlias, string pName);
        void AppendIn(string[] pValue, string pAlias, string pName, bool pNegado = false);
        void AppendNotIn(string[] pValue, string pAlias, string pName);
        string AppendExists(IAbstractDal pFromEntity, IAbstractDal pExistsEntity, CustomKeyHand pKeyHand, bool pAutoClose = true);
        string AppendExists(IAbstractDal pFromEntity, object pExistsValue, IAbstractDal pExistsEntity, CustomKeyHand pKeyHand, bool pAutoClose = true);
        string AppendExists(IAbstractDal pFromEntity, object pExistsValue, IAbstractDal pExistsEntity, string pExistsCustomKey, bool pAutoClose = true);
        string AppendNotExists(IAbstractDal pFromEntity, IAbstractDal pExistsEntity, CustomKeyHand pKeyHand, bool pAutoClose = true);
        string AppendNotExists(IAbstractDal pFromEntity, object pExistsValue, IAbstractDal pExistsEntity, CustomKeyHand pKeyHand, bool pAutoClose = true);
        string AppendNotExists(IAbstractDal pFromEntity, object pExistsValue, IAbstractDal pExistsEntity, string pExistsCustomKey, bool pAutoClose = true);
        void AppendAnd();
        void AppendOr();
        void AppendOpenParentheses();
        void AppendCloseParentheses();
    }
}
