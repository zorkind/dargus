using System;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Collections.Generic;
using WMIT.Framework.VO;

namespace WMIT.Framework.DAL
{
    public class Query : VO.IQuery
    {
        private static string DatabaseDefault, SchemaDefault;
        public IParametrosBuilder Parametros { get; set; }
        public StringBuilder Fields { get; set; }
        public StringBuilder FieldsWithoutAlias { get; set; }
        public StringBuilder Entities { get; set; }
        public StringBuilder Where { get; set; }
        public StringBuilder ConsultQuery { get; set; }
        public string InsertQuery { get; set; }
        public string UpdateQuery { get; set; }
        public int FieldCount { get; set; }
        public int EntityCount { get; set; }
        public bool EnableGroupBy { get; set; }

        private Dictionary<string, string> _SaveAtributes;
        public Dictionary<string, string> SaveAtributes
        {
            get { return _SaveAtributes ?? (_SaveAtributes = new Dictionary<string, string>()); }
            set { _SaveAtributes = value; }
        }

        public int? Top { get; set; }
        public string[] OrderBy { get; set; }
        public VO.Paginate Paginate { get; set; }
        public bool? DisableDateFieldPersistence { get; set; }
        public bool Persistence { get; set; }

        public Query()
        {
            Parametros = new ParametrosBuilder();
            Fields = new StringBuilder();
            Entities = new StringBuilder();
            Where = new StringBuilder();
            ConsultQuery = new StringBuilder();
            FieldsWithoutAlias = new StringBuilder();
            FieldCount = 0;
            EntityCount = 0;
            InsertQuery = string.Empty;
            UpdateQuery = string.Empty;

            if (Persistence)
                SaveAtributes = new Dictionary<string, string>();

            if (string.IsNullOrEmpty(DatabaseDefault))
                DatabaseDefault = ConfigurationManager.AppSettings["DatabaseDefault"].ToString();

            if (string.IsNullOrEmpty(SchemaDefault))
                SchemaDefault = ConfigurationManager.AppSettings["SchemaDefault"].ToString();

            if (!DisableDateFieldPersistence.HasValue)
                if (ConfigurationManager.AppSettings["DisableDateFieldPersistence"] != null)
                    DisableDateFieldPersistence = Convert.ToBoolean(ConfigurationManager.AppSettings["DisableDateFieldPersistence"].ToString());
                else
                    DisableDateFieldPersistence = false;
        }

        public int AddField(string pAttribute)
        {
            if (EntityCount == 1)
                return AddField(EntityCount.ToString(), pAttribute);
            else if (EntityCount > 1)
                return AddField(EntityCount.ToString(), pAttribute);
            else
                return 0;
        }

        public int AddField(string pEntity, string pAttribute)
        {
            if (Fields.Length != 0)
            {
                Fields.Append(",");
                FieldsWithoutAlias.Append(",");
            }

            if (EntityCount >= 1)
                Fields.Append(string.Concat(" [", pEntity, "].[", pAttribute, "] as [", ++FieldCount, "]")).AppendLine();
            else
                return 0;

            FieldsWithoutAlias.Append(string.Concat(" [", pEntity, "].[", pAttribute, "]")).AppendLine();

            return FieldCount - 1; //IMPORTANTE Retorna -1 porque o GetData busca os campos por indice e não por nome.
        }

        public int AddCustomField(string Query)
        {
            if (Fields.Length != 0)
            {
                Fields.Append(",");
                FieldsWithoutAlias.Append(",");
            }

            if (EntityCount >= 1)
                Fields.Append(string.Concat(" ", Query, " as [", ++FieldCount, "]")).AppendLine();
            else
                return 0;

            FieldsWithoutAlias.Append(string.Concat(" ", Query)).AppendLine();

            return FieldCount - 1; //IMPORTANTE Retorna -1 porque o GetData busca os campos por indice e não por nome.
        }
        
        public int AddEntityLeftJoin(IAbstractDal pObj, string pEntityFrom)
        {
            if (string.IsNullOrEmpty(pObj.Database))
                pObj.Database = DatabaseDefault;

            if (string.IsNullOrEmpty(pObj.Schema))
                pObj.Schema = SchemaDefault;

            pObj.EntityLeft = true;
            pObj.EntityAlias = (++EntityCount).ToString();

            var lEntity = string.Concat("[", pObj.Database, "].[", pObj.Schema, "].[", pObj.Entity, "]", " as [", pObj.EntityAlias, "]");
            Entities.Append(string.Concat(" left join ", lEntity, " on [", pEntityFrom, "].[", pObj.EntityKey, "] = [", pObj.EntityAlias, "].[", pObj.EntityKey, "] ")).AppendLine();

            pObj.Query = this;

            return EntityCount;
        }
        
        public int AddEntityLeftJoin(IAbstractDal pObj, string pEntityFrom, bool pReadOnly)
        {
            if (string.IsNullOrEmpty(pObj.Database))
                pObj.Database = DatabaseDefault;

            if (string.IsNullOrEmpty(pObj.Schema))
                pObj.Schema = SchemaDefault;

            pObj.EntityLeft = true;
            pObj.EntityAlias = (++EntityCount).ToString();
            Entities.AppendFormat(" left join [{0}].[{1}].[{2}] as [{3}] on [{4}].[{5}] = [{3}].[{5}] ",
                            pObj.Database,
                            pObj.Schema,
                            pObj.Entity,
                            pObj.EntityAlias,
                            pEntityFrom,
                            pObj.EntityKey
                        ).AppendLine();

            pObj.Query = this;

            return EntityCount;
        }

        public int AddEntityJoin(IAbstractDal pObj, string pEntityFrom)
        {
            if (pObj.Parent != null && pObj.Parent.EntityLeft)
                return AddEntityLeftJoin(pObj, pEntityFrom);
            else
            {
                if (string.IsNullOrEmpty(pObj.Database))
                    pObj.Database = DatabaseDefault;

                if (string.IsNullOrEmpty(pObj.Schema))
                    pObj.Schema = SchemaDefault;

                pObj.EntityAlias = (++EntityCount).ToString();
                var lEntity = string.Concat("[", pObj.Database, "].[", pObj.Schema, "].[", pObj.Entity, "]", " as [", pObj.EntityAlias, "]");
                Entities.Append(string.Concat(" inner join ", lEntity, " on [", pEntityFrom, "].[", pObj.EntityKey, "] = [", pObj.EntityAlias, "].[", pObj.EntityKey, "] ")).AppendLine();

                if (pEntityFrom == "1" && Persistence)
                    SaveAtributes.Add(pObj.EntityKey, string.Concat("@", pObj.EntityKey));

                pObj.Query = this;

                return EntityCount;
            }
        }

        public int AddEntityJoinCustomKey(IAbstractDal pObj, string pEntityFrom, string pKey)
        {
            if (string.IsNullOrEmpty(pObj.Database))
                pObj.Database = DatabaseDefault;

            if (string.IsNullOrEmpty(pObj.Schema))
                pObj.Schema = SchemaDefault;

            if (pObj.Parent != null && pObj.Parent.EntityLeft)
                return AddEntityLeftJoinCustomKey(pObj, pEntityFrom, pKey);
            else
            {
                pObj.EntityAlias = (++EntityCount).ToString();
                var lEntity = string.Concat("[", pObj.Database, "].[", pObj.Schema, "].[", pObj.Entity, "]", " as [", pObj.EntityAlias, "]");
                Entities.Append(string.Concat(" inner join ", lEntity, " on [", pEntityFrom, "].[", pKey, "] = [", pObj.EntityAlias, "].[", pObj.EntityKey, "] ")).AppendLine();

                if (pEntityFrom == "1" && Persistence)
                    SaveAtributes.Add(pKey, string.Concat("@", pKey));

                pObj.Query = this;

                return EntityCount;
            }
        }

        public int AddEntityLeftJoinCustomKey(IAbstractDal pObj, string pEntityFrom, string pKey, VO.CustomKeyHand pHand = VO.CustomKeyHand.Left)
        {
            return AddEntityLeftJoinCustomKey(pObj, pEntityFrom, pKey, pHand, false);
        }

        public int AddEntityLeftJoinCustomKey(IAbstractDal pObj, string pEntityFrom, string pKey, VO.CustomKeyHand pHand, bool pReadOnly)
        {
            string lLeftKey = pObj.EntityKey;
            string lRightKey = pObj.EntityKey;

            switch (pHand)
            {
                case VO.CustomKeyHand.Left:
                    lLeftKey = pKey;
                    break;
                case VO.CustomKeyHand.Right:
                    lRightKey = pKey;
                    break;
                case VO.CustomKeyHand.Both:
                    lLeftKey = lRightKey = pKey;
                    break;
            }

            if (string.IsNullOrEmpty(pObj.Database))
                pObj.Database = DatabaseDefault;

            if (string.IsNullOrEmpty(pObj.Schema))
                pObj.Schema = SchemaDefault;

            pObj.EntityLeft = true;
            pObj.EntityAlias = (++EntityCount).ToString();

            var lEntity = string.Concat("[", pObj.Database, "].[", pObj.Schema, "].[", pObj.Entity, "]", " as [", pObj.EntityAlias, "]");
            Entities.Append(string.Concat(" left join ", lEntity, " on [", pEntityFrom, "].[", lLeftKey, "] = [", pObj.EntityAlias, "].[", lRightKey, "] ")).AppendLine();
            
            if (pEntityFrom == "1" && (!pReadOnly && Persistence))
                SaveAtributes.Add(pKey, string.Concat("@", pKey));

            pObj.Query = this;

            return EntityCount;
        }

        public int AddCustomEntityJoin(string Query)
        {
            Entities.AppendFormat(Query, ++EntityCount).AppendLine();

            return EntityCount;
        }

        public override string ToString()
        {
            if (ConsultQuery == null)
                return base.ToString();

            return ConsultQuery.ToString();
        }

        public int AddEntityFrom(string pDataBase, string pSchema, string pEntityFrom)
        {
            if (string.IsNullOrEmpty(pDataBase))
                pDataBase = DatabaseDefault;

            if (string.IsNullOrEmpty(pSchema))
                pSchema = SchemaDefault;

            Entities.Append(string.Concat(" from [", pDataBase, "].[", pSchema, "].[", pEntityFrom, "]", " as [", ++EntityCount, "]")).AppendLine();

            return EntityCount;
        }

        public void BuildQuery()
        {
            if (ConsultQuery.Length != 0)
                ConsultQuery.Clear();

            //Setup Top
            if (Top != null && Top != 0)
                ConsultQuery.AppendLine("select top ").Append(Top.Value.ToString()).Append(" ").Append(Fields.ToString()).Append(Entities.ToString()).Append(Where.ToString());
            else
                ConsultQuery.AppendLine("select ").Append(Fields.ToString()).Append(Entities.ToString()).Append(Where.ToString());

            if (EnableGroupBy)
                ConsultQuery.AppendFormat("group by {0}", FieldsWithoutAlias.ToString());

            //Setup OrderBy
            if (OrderBy.Length != 0)
                ConsultQuery.Append(" order by ").AppendLine(string.Join(", ", OrderBy));

            //Setup Pagination
            if (Paginate.Enabled)
            {
                StringBuilder TotalRowsCount = new StringBuilder();
                if (EnableGroupBy)
                    TotalRowsCount.Append("set @totalRows = (select count(*) from (select 1 as [1] ")
                                  .Append(Entities.ToString())
                                  .Append(Where.ToString())
                                  .AppendFormat("group by {0}", FieldsWithoutAlias.ToString())
                                  .Append(") as tbCC)").AppendLine().AppendLine();
                else
                    TotalRowsCount.Append("set @totalRows = (select count(*) ").Append(Entities.ToString()).Append(Where.ToString()).Append(")").AppendLine().AppendLine();

                ConsultQuery.Insert(0, TotalRowsCount).AppendLine(" OFFSET ((@PageNumber - 1) * @RowspPage) ROWS").AppendLine(" FETCH NEXT @RowspPage ROWS ONLY");
            }
        }

        public void BuildPersistence(IAbstractDal obj, bool isNew = true)
        {
            string lKey = string.Empty;

            if (string.IsNullOrEmpty(obj.Database))
                obj.Database = DatabaseDefault;

            if (string.IsNullOrEmpty(obj.Schema))
                obj.Schema = SchemaDefault;

            if (isNew)
            {
                lKey = string.Empty;
                lKey = SaveAtributes.Keys.DefaultIfEmpty(string.Empty).SingleOrDefault(x => x.EndsWith("_atualizacao_us_id"));

                if (!string.IsNullOrEmpty(lKey))
                    SaveAtributes.Remove(lKey);

                lKey = string.Empty;
                lKey = SaveAtributes.Keys.DefaultIfEmpty(string.Empty).SingleOrDefault(x => x.EndsWith("_cadastro_us_id"));

                if (string.IsNullOrEmpty(lKey))
                    SaveAtributes.Add(string.Concat(obj.EntityKey.Remove(obj.EntityKey.LastIndexOf('_')), "_cadastro_us_id"),
                                    string.Concat("@", obj.EntityKey.Remove(obj.EntityKey.LastIndexOf('_')), "_cadastro_us_id"));
            }
            else
            {
                lKey = string.Empty;
                lKey = SaveAtributes.Keys.DefaultIfEmpty(string.Empty).SingleOrDefault(x => x.EndsWith("_cadastro_us_id"));

                if (!string.IsNullOrEmpty(lKey))
                    SaveAtributes.Remove(lKey);

                lKey = string.Empty;
                lKey = SaveAtributes.Keys.DefaultIfEmpty(string.Empty).SingleOrDefault(x => x.EndsWith("_atualizacao_us_id"));

                if (string.IsNullOrEmpty(lKey))
                    SaveAtributes.Add(string.Concat(obj.EntityKey.Remove(obj.EntityKey.LastIndexOf('_')), "_atualizacao_us_id"),
                                    string.Concat("@", obj.EntityKey.Remove(obj.EntityKey.LastIndexOf('_')), "_atualizacao_us_id"));
            }

            if (!DisableDateFieldPersistence.Value)
            {
                if (isNew)
                {
                    lKey = string.Empty;
                    lKey = SaveAtributes.Keys.DefaultIfEmpty(string.Empty).SingleOrDefault(x => x.EndsWith("_cadastro"));

                    if (string.IsNullOrEmpty(lKey))
                        SaveAtributes.Add(string.Concat(obj.EntityKey.Remove(obj.EntityKey.LastIndexOf('_')), "_cadastro"),"getdate()");
                    else
                        SaveAtributes[lKey] = "getdate()";

                    lKey = string.Empty;
                    lKey = SaveAtributes.Keys.DefaultIfEmpty(string.Empty).SingleOrDefault(x => x.EndsWith("_atualizacao"));

                    if (string.IsNullOrEmpty(lKey))
                        SaveAtributes.Add(string.Concat(obj.EntityKey.Remove(obj.EntityKey.LastIndexOf('_')), "_atualizacao"), "getdate()");
                    else
                        SaveAtributes[lKey] = "getdate()";
                }
                else
                {
                    lKey = string.Empty;
                    lKey = SaveAtributes.Keys.DefaultIfEmpty(string.Empty).SingleOrDefault(x => x.EndsWith("_cadastro"));
                    if (!string.IsNullOrEmpty(lKey))
                        SaveAtributes.Remove(lKey);

                    lKey = string.Empty;
                    lKey = SaveAtributes.Keys.DefaultIfEmpty(string.Empty).SingleOrDefault(x => x.EndsWith("_atualizacao"));

                    if (string.IsNullOrEmpty(lKey))
                        SaveAtributes.Add(string.Concat(obj.EntityKey.Remove(obj.EntityKey.LastIndexOf('_')), "_atualizacao"),"getdate()");
                    else
                        SaveAtributes[lKey] = "getdate()";
                }
            }
            else
                SaveAtributes.Keys.Where(k => k.EndsWith("_cadastro") || k.EndsWith("_atualizacao")).ToList().ForEach(attr => SaveAtributes.Remove(attr));

            if (isNew)
            {
                var insert = "insert into [{0}].[{1}].[{2}] ({3}) values ({4});";

                if (obj.EntityKeyIdentity)
                {
                    SaveAtributes.Remove(obj.EntityKey);
                    insert = string.Concat(insert, " set @", obj.EntityKey, " = scope_identity();");
                }

                //Tratamento para os campos Chave, apenas considera chaves com a mesma nomenclatura do campo Id da tabela, para evitar conflitos entre chaves em ForeignKey
                lKey = obj.EntityKey.Replace("id", "key");
                if (SaveAtributes.Keys.Contains(lKey))
                    insert = string.Concat("set @", lKey, " = convert(int,Rand() * power(10,8)); ", insert);

                InsertQuery = string.Format(insert,
                                            obj.Database,
                                            obj.Schema,
                                            obj.Entity,
                                            string.Join(",", SaveAtributes.Keys),
                                            string.Join(",", SaveAtributes.Values));
            }
            else
            {
                //Remove o atributo ID da lista, pois na atualização este campo não deve sofrer alteração.
                SaveAtributes.Remove(obj.EntityKey);

                lKey = string.Empty;
                lKey = SaveAtributes.Keys.DefaultIfEmpty(string.Empty).SingleOrDefault(x => x.EndsWith("_ativo"));

                if (string.IsNullOrEmpty(lKey))
                    SaveAtributes.Add(string.Format("{0}_ativo", obj.EntityKey.Remove(obj.EntityKey.LastIndexOf('_'))),
                                        string.Format("@{0}_ativo", obj.EntityKey.Remove(obj.EntityKey.LastIndexOf('_'))));

                UpdateQuery = string.Format("update [{0}].[{1}].[{2}] set {3} where [{0}].[{1}].[{2}].[{4}] = @{4}",
                    obj.Database,
                    obj.Schema,
                    obj.Entity,
                    string.Join(",", SaveAtributes.Select(a => string.Format(" [{0}] = {1}", a.Key, a.Value))),
                    obj.EntityKey);
            }
        }

        public void AddOutputSelect(IAbstractDal obj)
        {
            if (obj.PersistenceOutputParameters != null && obj.PersistenceOutputParameters.Count > 0)
            {
                var postInsertQuery = new StringBuilder();
                postInsertQuery.Append(" select ");
                bool lPrimeiro = true;

                foreach (var lkey in obj.PersistenceOutputParameters.Keys)
                {
                    if (lPrimeiro)
                        postInsertQuery.Append(string.Concat(lkey, " = [", lkey.Replace("@", ""), "]"));
                    else
                        postInsertQuery.AppendFormat(string.Concat(", ", lkey, " = [", lkey.Replace("@", ""), "]"));
                }

                if (obj.EntityKeyIdentity)
                    postInsertQuery.Append(string.Concat(" from [", obj.Database, "].[", obj.Schema, "].[", obj.Entity, "] selOut where selOut.[", obj.EntityKey, "] = scope_identity();"));
                else
                    postInsertQuery.Append(string.Concat(" from [", obj.Database, "].[", obj.Schema, "].[", obj.Entity, "] selOut where selOut.[", obj.EntityKey, "] = @", obj.EntityKey, ";"));

                obj.Query.InsertQuery = string.Concat(obj.Query.InsertQuery, postInsertQuery.ToString());
            }
        }
    }
}
