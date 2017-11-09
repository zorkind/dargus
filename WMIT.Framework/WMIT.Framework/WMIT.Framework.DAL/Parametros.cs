using System;
using System.Collections.Generic;
using System.Text;
using WMIT.Framework.VO;
using System.Linq;

namespace WMIT.Framework.DAL
{
    [Serializable]
    public class ParametrosBuilder : VO.IParametrosBuilder
    {
        private bool ParenthesesOpen { get; set; }
        private Dictionary<string, object> _QueryParameters;
        public Dictionary<string, object> QueryParameters
        {
            get
            {
                if (_QueryParameters == null)
                    _QueryParameters = new Dictionary<string, object>();

                return _QueryParameters;
            }
            set 
            {
                _QueryParameters = value;
            }
        }

        private StringBuilder _Builder;
        public StringBuilder Builder
        {
            get
            {
                if (_Builder == null)
                    _Builder = new StringBuilder();

                return _Builder;
            }
            set 
            {
                _Builder = value;
            }
        }

        public void BuildWhereDefaults(IAbstractDal pEntity, VO.IParametros pParametros)
        {
            if (Builder.Length > 0)
                Builder.Clear();

            Builder.AppendLine(" where 1 = 1 ");

            if (pParametros.Codigo == 0)
            {
                string Campo = pEntity.EntityKey;

                Campo = Campo.Remove(Campo.LastIndexOf("_"));

                Campo = string.Concat(Campo, "_ativo");

                if (pParametros.Ativo != null && pParametros.Ativo.Value)
                {
                    Builder.Append(string.Concat(" and [", pEntity.EntityAlias, "].[", Campo, "] = 1")).AppendLine();
                }

                if (pParametros.Ativo != null && !pParametros.Ativo.Value)
                {
                    Builder.Append(string.Concat(" and [", pEntity.EntityAlias, "].[", Campo, "] = 0")).AppendLine();
                }
            }
            else
            {
                Builder.Append(string.Concat(" and [", pEntity.EntityAlias, "].[", pEntity.EntityKey, "] = @", pEntity.EntityAlias, "_id")).AppendLine();
                QueryParameters.Add(string.Concat("@", pEntity.EntityAlias, "_id"), pParametros.Codigo);
            }

            if (pParametros.Codigos.Count > 0)
            {
                var Virgula = false;
                if (pParametros.Codigos.Any(c => c > 0))
                {
                    Builder.Append(string.Concat(" and [", pEntity.EntityAlias, "].[", pEntity.EntityKey, "] in("));
                    foreach (int p in pParametros.Codigos.Where(c => c > 0))
                    {
                        if (Virgula) Builder.Append(",");
                        Builder.Append(p);
                        Virgula = true;
                    }
                    Builder.Append(")");
                }

                if (pParametros.Codigos.Any(c => c < 0))
                {
                    Virgula = false;
                    Builder.Append(string.Concat(" and [", pEntity.EntityAlias, "].[", pEntity.EntityKey, "] not in("));
                    foreach (int p in pParametros.Codigos.Where(c => c < 0))
                    {
                        if (Virgula) Builder.Append(",");
                        Builder.Append((p * -1));
                        Virgula = true;
                    }
                    Builder.Append(")");
                }
            }

            if (pEntity.Query.Where.Length > 0)
                pEntity.Query.Where.Clear();

            pEntity.Query.Where.Append(pEntity.BuildWhere(pParametros));
        }

        public void AppendEntity(int pValue, IAbstractDal pEntity)
        {
            var lParameter = string.Concat("@", pEntity.EntityAlias, pEntity.EntityKey);

            if (ParenthesesOpen)
                Builder.Append(" ");
            else
                Builder.Append(" and ");

            Builder.Append(string.Concat("[", pEntity.EntityAlias, "].[", pEntity.EntityKey, "] = ", lParameter)).AppendLine();
            
            QueryParameters.Add(lParameter, pValue);
        }

        public void AppendEntity(int pValue, IAbstractDal pEntity, string pCustomKey, CustomKeyHand pKeyHand)
        {
            var lKeyLeftHand = string.Empty;
            var lKeyRightHand = string.Empty;
            switch (pKeyHand)
            {
                case CustomKeyHand.Left:
                    lKeyLeftHand = pCustomKey;
                    lKeyRightHand = pEntity.EntityKey;
                    break;
                case CustomKeyHand.Right:
                    lKeyRightHand = pCustomKey;
                    lKeyLeftHand = pEntity.EntityKey;
                    break;
                case CustomKeyHand.Both:
                    lKeyLeftHand = lKeyRightHand = pCustomKey;
                    break;
            }

            var lParameter = string.Concat("@", pEntity.EntityAlias, lKeyRightHand);

            if (ParenthesesOpen)
                Builder.Append(" ");
            else
                Builder.Append(" and ");

            Builder.Append(string.Concat("[", pEntity.EntityAlias, "].[", lKeyLeftHand, "] = ", lParameter)).AppendLine();

            QueryParameters.Add(lParameter, pValue);
        }

        public void AppendEntity(int pValue, IAbstractDal pEntity, string pCustomKey)
        {
            var lParameter = string.Concat("@", pEntity.EntityAlias, pCustomKey);

            if (ParenthesesOpen)
                Builder.Append(" ");
            else
                Builder.Append(" and ");

            Builder.Append(string.Concat("[", pEntity.EntityAlias, "].[", pEntity.EntityKey, "] = ", lParameter)).AppendLine();

            QueryParameters.Add(lParameter, pValue);
        }

        public void AppendEntity(int pValue, IAbstractDal pEntity, string pCustomLeftKey, string pCustomRightKey)
        {
            var lParameter = string.Concat("@", pEntity.EntityAlias, pCustomRightKey);

            if (ParenthesesOpen)
                Builder.Append(" ");
            else
                Builder.Append(" and ");

            Builder.Append(string.Concat("[", pEntity.EntityAlias, "].[", pCustomLeftKey, "] = ", lParameter)).AppendLine();

            QueryParameters.Add(lParameter, pValue);
        }

        public void AppendAnd()
        {
            Builder.AppendFormat(" and ").AppendLine();
        }

        public void AppendOr()
        {
            Builder.AppendFormat(" or ").AppendLine();
        }

        public void AppendOpenParentheses()
        {
            Builder.AppendFormat("(").AppendLine();
            ParenthesesOpen = true;
        }

        public void AppendCloseParentheses()
        {
            Builder.AppendFormat(")").AppendLine();
            ParenthesesOpen = false;
        }

        public string AppendExists(IAbstractDal pFromEntity, IAbstractDal pExistsEntity, CustomKeyHand pKeyHand, bool pAutoClose = true)
        {
            var lKeyHand = string.Empty;
            var lParameter = string.Concat(pFromEntity.Entity, "_", pExistsEntity.Entity);
            var lEntityFrom = string.Empty;

            switch (pKeyHand)
            {
                case CustomKeyHand.Left:
                    lKeyHand = pFromEntity.EntityKey;
                    break;
                case CustomKeyHand.Right:
                    lKeyHand = pExistsEntity.EntityKey;
                    break;
            }

            if (string.IsNullOrEmpty(pFromEntity.EntityAlias))
                lEntityFrom = string.Concat("[", pFromEntity.Database, "].[", pFromEntity.Schema, "].[", pFromEntity.Entity, "]");
            else
                lEntityFrom = string.Concat("[", pFromEntity.EntityAlias, "]");

            if (ParenthesesOpen)
                Builder.Append(" ");
            else
                Builder.Append(" and ");

            if (pAutoClose)
                Builder.AppendFormat("exists(select 1 from [{0}].[{1}].[{2}] ex{5} where ex{5}.[{4}] = {3}.[{4}] and ex{5}.[{6}] = 1)",
                            pExistsEntity.Database,
                            pExistsEntity.Schema,
                            pExistsEntity.Entity,
                            lEntityFrom,
                            lKeyHand,
                            lParameter,
                            string.Concat(pExistsEntity.EntityKey.Remove(pExistsEntity.EntityKey.LastIndexOf('_')), "_ativo")).AppendLine();

            else
                Builder.AppendFormat("exists(select 1 from [{0}].[{1}].[{2}] ex{5} where ex{5}.[{4}] = {3}.[{4}] and ex{5}.[{6}] = 1",
                pExistsEntity.Database,
                pExistsEntity.Schema,
                pExistsEntity.Entity,
                lEntityFrom,
                lKeyHand,
                lParameter,
                string.Concat(pExistsEntity.EntityKey.Remove(pExistsEntity.EntityKey.LastIndexOf('_')), "_ativo"));


            return string.Concat("ex", lParameter);
        }

        public string AppendExists(IAbstractDal pFromEntity, object pExistsValue, IAbstractDal pExistsEntity, CustomKeyHand pKeyHand, bool pAutoClose = true)
        {
            var lKeyHand = string.Empty;
            var lParameter = string.Concat(pFromEntity.Entity, "_", pExistsEntity.Entity);
            var lEntityFrom = string.Empty;

            switch (pKeyHand)
            {
                case CustomKeyHand.Left:
                    lKeyHand = pFromEntity.EntityKey;
                    break;
                case CustomKeyHand.Right:
                    lKeyHand = pExistsEntity.EntityKey;
                    break;
            }

            if (string.IsNullOrEmpty(pFromEntity.EntityAlias))
                lEntityFrom = string.Concat("[", pFromEntity.Database, "].[", pFromEntity.Schema, "].[", pFromEntity.Entity, "]");
            else
                lEntityFrom = string.Concat("[", pFromEntity.EntityAlias, "]");

            if (ParenthesesOpen)
                Builder.Append(" ");
            else
                Builder.Append(" and ");

            if(pAutoClose)
                Builder.AppendFormat("exists(select 1 from [{0}].[{1}].[{2}] ex{5} where {5}.[{4}] = {3}.[{4}] and ex{5}.[{3}] = @{5} and ex{5}.[{6}] = 1)",
                            pExistsEntity.Database,
                            pExistsEntity.Schema,
                            pExistsEntity.Entity,
                            lEntityFrom,
                            lKeyHand,
                            lParameter,
                            string.Concat(pExistsEntity.EntityKey.Remove(pExistsEntity.EntityKey.LastIndexOf('_')), "_ativo")).AppendLine();
            else
                Builder.AppendFormat("exists(select 1 from [{0}].[{1}].[{2}] ex{5} where {5}.[{4}] = {3}.[{4}] and ex{5}.[{3}] = @{5} and ex{5}.[{6}] = 1",
                pExistsEntity.Database,
                pExistsEntity.Schema,
                pExistsEntity.Entity,
                lEntityFrom,
                lKeyHand,
                lParameter,
                string.Concat(pExistsEntity.EntityKey.Remove(pExistsEntity.EntityKey.LastIndexOf('_')), "_ativo"));

            QueryParameters.Add(string.Concat("@", lParameter), pExistsValue);

            return string.Concat("ex", lParameter);
        }

        public string AppendExists(IAbstractDal pFromEntity, object pExistsValue, IAbstractDal pExistsEntity, string pExistsCustomKey, bool pAutoClose = true)
        {
            var lParameter = string.Concat(pFromEntity.Entity, "_", pExistsEntity.Entity, "_", pExistsCustomKey);
            var lEntityFrom = string.Empty;

            if (string.IsNullOrEmpty(pFromEntity.EntityAlias))
                lEntityFrom = string.Concat("[", pFromEntity.Database, "].[", pFromEntity.Schema, "].[", pFromEntity.Entity, "]");
            else
                lEntityFrom = string.Concat("[", pFromEntity.EntityAlias, "]");

            if (ParenthesesOpen)
                Builder.Append(" ");
            else
                Builder.Append(" and ");

            if(pAutoClose)
                Builder.AppendFormat("exists(select 1 from [{0}].[{1}].[{2}] ex{6} where ex{6}.[{5}] = {4}.[{5}] and ex{6}.[{3}] = @{6} and ex{6}.[{7}] = 1)",
                            pExistsEntity.Database,
                            pExistsEntity.Schema,
                            pExistsEntity.Entity,
                            pExistsCustomKey,
                            lEntityFrom,
                            pFromEntity.EntityKey, 
                            lParameter,
                            string.Concat(pExistsEntity.EntityKey.Remove(pExistsEntity.EntityKey.LastIndexOf('_')), "_ativo")).AppendLine();
            else
                Builder.AppendFormat("exists(select 1 from [{0}].[{1}].[{2}] ex{6} where ex{6}.[{5}] = {4}.[{5}] and ex{6}.[{3}] = @{6} and ex{6}.[{7}] = 1",
                pExistsEntity.Database,
                pExistsEntity.Schema,
                pExistsEntity.Entity,
                pExistsCustomKey,
                lEntityFrom,
                pFromEntity.EntityKey,
                lParameter,
                string.Concat(pExistsEntity.EntityKey.Remove(pExistsEntity.EntityKey.LastIndexOf('_')), "_ativo"));

            QueryParameters.Add(string.Concat("@", lParameter), pExistsValue);

            return string.Concat("ex", lParameter);
        }

        public string AppendNotExists(IAbstractDal pFromEntity, IAbstractDal pExistsEntity, CustomKeyHand pKeyHand, bool pAutoClose = true)
        {
            var lKeyHand = string.Empty;
            var lParameter = string.Concat(pFromEntity.Entity, "_", pExistsEntity.Entity);
            var lEntityFrom = string.Empty;

            switch (pKeyHand)
            {
                case CustomKeyHand.Left:
                    lKeyHand = pFromEntity.EntityKey;
                    break;
                case CustomKeyHand.Right:
                    lKeyHand = pExistsEntity.EntityKey;
                    break;
            }

            if (string.IsNullOrEmpty(pFromEntity.EntityAlias))
                lEntityFrom = string.Concat("[", pFromEntity.Database, "].[", pFromEntity.Schema, "].[", pFromEntity.Entity, "]");
            else
                lEntityFrom = string.Concat("[", pFromEntity.EntityAlias, "]");

            if (ParenthesesOpen)
                Builder.Append(" ");
            else
                Builder.Append(" and ");

            if(pAutoClose)
                Builder.AppendFormat("not exists(select 1 from [{0}].[{1}].[{2}] ex{5} where ex{5}.[{4}] = {3}.[{4}] and ex{5}.[{6}] = 1)",
                            pExistsEntity.Database,
                            pExistsEntity.Schema,
                            pExistsEntity.Entity,
                            lEntityFrom,
                            lKeyHand,
                            lParameter,
                            string.Concat(pExistsEntity.EntityKey.Remove(pExistsEntity.EntityKey.LastIndexOf('_')), "_ativo")).AppendLine();
            else
                Builder.AppendFormat("not exists(select 1 from [{0}].[{1}].[{2}] ex{5} where ex{5}.[{4}] = {3}.[{4}] and ex{5}.[{6}] = 1",
                            pExistsEntity.Database,
                            pExistsEntity.Schema,
                            pExistsEntity.Entity,
                            lEntityFrom,
                            lKeyHand,
                            lParameter,
                            string.Concat(pExistsEntity.EntityKey.Remove(pExistsEntity.EntityKey.LastIndexOf('_')), "_ativo"));

            return string.Concat("ex", lParameter);
        }

        public string AppendNotExists(IAbstractDal pFromEntity, object pExistsValue, IAbstractDal pExistsEntity, CustomKeyHand pKeyHand, bool pAutoClose = true)
        {
            var lKeyHand = string.Empty;
            var lParameter = string.Concat(pFromEntity.Entity, "_", pExistsEntity.Entity);
            var lEntityFrom = string.Empty;

            switch (pKeyHand)
            {
                case CustomKeyHand.Left:
                    lKeyHand = pFromEntity.EntityKey;
                    break;
                case CustomKeyHand.Right:
                    lKeyHand = pExistsEntity.EntityKey;
                    break;
            }

            if (string.IsNullOrEmpty(pFromEntity.EntityAlias))
                lEntityFrom = string.Concat("[", pFromEntity.Database, "].[", pFromEntity.Schema, "].[", pFromEntity.Entity, "]");
            else
                lEntityFrom = string.Concat("[", pFromEntity.EntityAlias, "]");

            if (ParenthesesOpen)
                Builder.Append(" ");
            else
                Builder.Append(" and ");

            if(pAutoClose)
                Builder.AppendFormat("not exists(select 1 from [{0}].[{1}].[{2}] ex{5} where ex{5}.[{4}] = {3}.[{4}] and ex{5}.[{3}] = @{5} and ex{5}.[{6}] = 1)",
                            pExistsEntity.Database,
                            pExistsEntity.Schema,
                            pExistsEntity.Entity,
                            lEntityFrom,
                            lKeyHand,
                            lParameter,
                            string.Concat(pExistsEntity.EntityKey.Remove(pExistsEntity.EntityKey.LastIndexOf('_')), "_ativo")).AppendLine();
            else
                Builder.AppendFormat("not exists(select 1 from [{0}].[{1}].[{2}] ex{5} where ex{5}.[{4}] = {3}.[{4}] and ex{5}.[{3}] = @{5} and ex{5}.[{6}] = 1",
                            pExistsEntity.Database,
                            pExistsEntity.Schema,
                            pExistsEntity.Entity,
                            lEntityFrom,
                            lKeyHand,
                            lParameter,
                            string.Concat(pExistsEntity.EntityKey.Remove(pExistsEntity.EntityKey.LastIndexOf('_')), "_ativo"));


            QueryParameters.Add(string.Concat("@", lParameter), pExistsValue);

            return string.Concat("ex", lParameter);
        }

        public string AppendNotExists(IAbstractDal pFromEntity, object pExistsValue, IAbstractDal pExistsEntity, string pExistsCustomKey, bool pAutoClose = true)
        {
            var lParameter = string.Concat(pFromEntity.Entity, "_", pExistsEntity.Entity, "_", pExistsCustomKey);
            var lEntityFrom = string.Empty;

            if (string.IsNullOrEmpty(pFromEntity.EntityAlias))
                lEntityFrom = string.Concat("[", pFromEntity.Database, "].[", pFromEntity.Schema, "].[", pFromEntity.Entity, "]");
            else
                lEntityFrom = string.Concat("[", pFromEntity.EntityAlias, "]");

            if (ParenthesesOpen)
                Builder.Append(" ");
            else
                Builder.Append(" and ");

            if(pAutoClose)
                Builder.AppendFormat("not exists(select 1 from [{0}].[{1}].[{2}] ex{6} where ex{6}.[{5}] = {4}.[{5}] and ex{6}.[{3}] = @{6} and ex{6}.[{7}] = 1)",
                            pExistsEntity.Database,
                            pExistsEntity.Schema,
                            pExistsEntity.Entity,
                            pExistsCustomKey,
                            lEntityFrom,
                            pFromEntity.EntityKey,
                            lParameter,
                            string.Concat(pExistsEntity.EntityKey.Remove(pExistsEntity.EntityKey.LastIndexOf('_')), "_ativo")).AppendLine();
            else
                Builder.AppendFormat("not exists(select 1 from [{0}].[{1}].[{2}] ex{6} where ex{6}.[{5}] = {4}.[{5}] and ex{6}.[{3}] = @{6} and ex{6}.[{7}] = 1",
                            pExistsEntity.Database,
                            pExistsEntity.Schema,
                            pExistsEntity.Entity,
                            pExistsCustomKey,
                            lEntityFrom,
                            pFromEntity.EntityKey,
                            lParameter,
                            string.Concat(pExistsEntity.EntityKey.Remove(pExistsEntity.EntityKey.LastIndexOf('_')), "_ativo"));

            QueryParameters.Add(string.Concat("@", lParameter), pExistsValue);

            return string.Concat("ex", lParameter);
        }

        public void AppendEqual(object pValue, string pAlias, string pName)
        {
            var lParameter = string.Concat("@", pAlias, pName);

            if (ParenthesesOpen)
                Builder.Append(" ");
            else
                Builder.Append(" and ");

            Builder.Append(string.Concat("[", pAlias, "].[", pName, "] = ", lParameter)).AppendLine();

            QueryParameters.Add(lParameter, pValue);
        }

        public void AppendIsNull(bool pIsNull, string pAlias, string pName)
        {
            if (ParenthesesOpen)
                Builder.Append(" ");
            else
                Builder.Append(" and ");

            if(pIsNull)
                Builder.Append(string.Concat("[", pAlias, "].[", pName, "] is null ")).AppendLine();
            else
                Builder.Append(string.Concat("[", pAlias, "].[", pName, "] is not null ")).AppendLine();
        }
        
        public void AppendBitwise(object pValue, string pAlias, string pName)
        {
            var lParameter = string.Concat("@", pAlias, pName);
            
            if (ParenthesesOpen)
                Builder.Append(" ");
            else
                Builder.Append(" and ");

            Builder.Append(string.Concat("([", pAlias, "].[", pName, "] & ", lParameter, ") != 0")).AppendLine();
            QueryParameters.Add(lParameter, pValue);
        }

        public void AppendIn(int[] pValue, string pAlias, string pName)
        {
            var Virgula = false;
            if (pValue.Any(c => c > 0))
            {
                if (ParenthesesOpen)
                    Builder.Append(" ");
                else
                    Builder.Append(" and ");

                Builder.Append(string.Concat("[", pAlias, "].[", pName, "] in("));

                foreach (int p in pValue.Where(c => c > 0))
                {
                    if (Virgula) Builder.Append(",");
                    Builder.Append(p);
                    Virgula = true;
                }
                Builder.Append(")").AppendLine();
            }

            if (pValue.Any(c => c < 0))
            {
                Virgula = false;
                Builder.Append(string.Concat(" and [", pAlias, "].[", pName, "] not in("));
                foreach (int p in pValue.Where(c => c < 0))
                {
                    if (Virgula) Builder.Append(",");
                    Builder.Append((p * -1));
                    Virgula = true;
                }
                Builder.Append(")").AppendLine();
            }
        }

        public void AppendNotIn(string[] pValue, string pAlias, string pName)
        {
            AppendIn(pValue, pAlias, pName, true);
        }

        public void AppendIn(string[] pValue, string pAlias, string pName, bool pNegado = false)
        {
            var lValue = string.Empty;

            for(var i = 0; i < pValue.Length; i++)
                lValue = string.Concat(lValue, "'", pValue[i], "'");

            if (ParenthesesOpen)
                Builder.Append(" ");
            else
                Builder.Append(" and ");

            if (pNegado)
                Builder.Append(string.Concat("[", pAlias, "].[", pName, "] not in(", lValue, ")")).AppendLine();
            else
                Builder.Append(string.Concat("[", pAlias, "].[", pName, "] in(", lValue, ")")).AppendLine();
        }

        public void AppendLike(object pValue, string pAlias, string pName, bool pInsensitive = false)
        {
            var lParameter = string.Concat("@", pAlias, pName);

            if (ParenthesesOpen)
                Builder.Append(" ");
            else
                Builder.Append(" and ");

            if (pInsensitive)
                Builder.Append(string.Concat("[", pAlias, "].[", pName, "] like '%' + ", lParameter, " + '%' collate Latin1_general_CI_AI")).AppendLine();
            else
                Builder.Append(string.Concat("[", pAlias, "].[", pName, "] like '%' + ", lParameter, " + '%'")).AppendLine();

            QueryParameters.Add(lParameter, pValue);
        }

        public void AppendBiggerThen(object pValue, string pAlias, string pName)
        {
            var lParameter = string.Concat("@", pAlias, pName);

            if (ParenthesesOpen)
                Builder.Append(" ");
            else
                Builder.Append(" and ");

            Builder.Append(string.Concat("[", pAlias, "].[", pName, "] > ", lParameter)).AppendLine();
            QueryParameters.Add(lParameter, pValue);
        }
        
        public void AppendBiggerOrEqualThen(object pValue, string pAlias, string pName)
        {
            var lParameter = string.Concat(pAlias, pName);

            if (ParenthesesOpen)
                Builder.Append(" ");
            else
                Builder.Append(" and ");

            Builder.AppendFormat("[{0}].[{1}] >= @{2}", pAlias, pName, lParameter).AppendLine();
            QueryParameters.Add(string.Format("@{0}", lParameter), pValue);
        }

        public void AppendSmallerThen(object pValue, string pAlias, string pName)
        {
            var lParameter = string.Concat("@", pAlias, pName);

            if (ParenthesesOpen)
                Builder.Append(" ");
            else
                Builder.Append(" and ");

            Builder.Append(string.Concat("[", pAlias, "].[", pName, "] < ", lParameter)).AppendLine();
            QueryParameters.Add(lParameter, pValue);
        }

        public void AppendSmallerOrEqualThen(object pValue, string pAlias, string pName)
        {
            var lParameter = string.Concat(pAlias, pName);

            if (ParenthesesOpen)
                Builder.Append(" ");
            else
                Builder.Append(" and ");

            Builder.AppendFormat("[{0}].[{1}] <= @{2}", pAlias, pName, lParameter).AppendLine();
            QueryParameters.Add(string.Format("@{0}", lParameter), pValue);
        }

        public void AppendBetween(object pValue, object pValue2, string pAlias, string pName)
        {
            var lParameter1 = string.Concat("@", pAlias, pName, "_min");
            var lParameter2 = string.Concat("@", pAlias, pName, "_max");

            if (ParenthesesOpen)
                Builder.Append(" ");
            else
                Builder.Append(" and ");

            Builder.Append(string.Concat("[", pAlias, "].[", pName, "] between ", lParameter1, " and ", lParameter2)).AppendLine();
            QueryParameters.Add(lParameter1, pValue);
            QueryParameters.Add(lParameter2, pValue2);
        }
    }
}
