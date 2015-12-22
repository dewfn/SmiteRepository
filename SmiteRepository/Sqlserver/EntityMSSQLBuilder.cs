using SmiteRepository.ORMapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmiteRepository.Sqlserver
{
    internal class EntityMSSQLBuilder
    {
       
        public static string EntityMetaToReplaceInsertSQL(EntityMeta meta)
        {
            if (meta.Columns == null || meta.Columns.Count == 0)
                return string.Empty;

            StringBuilder sqlbuilder = new StringBuilder();
            //sqlbuilder.AppendFormat("REPLACE INTO [{0}] (", meta.TableName);
            sqlbuilder.Append("REPLACE INTO [{0}] (");
            for (int i = 0, j = 0; i < meta.Columns.Count; i++)
            {
                if (meta.Columns[i].Identity)
                {
                    continue;
                }
                if (j > 0)
                {
                    sqlbuilder.Append(",");
                }
                sqlbuilder.Append("`" + meta.Columns[i].ColumnName + "`");
                j++;
            }
            sqlbuilder.Append(") VALUES (");
            for (int i = 0, j = 0; i < meta.Columns.Count; i++)
            {
                if (meta.Columns[i].Identity)
                {
                    continue;
                }
                if (j > 0)
                {
                    sqlbuilder.Append(",");
                }
                sqlbuilder.Append("@" + meta.Columns[i].FieldName + "");
                j++;

            }
            sqlbuilder.Append(");");

            return sqlbuilder.ToString();
        }

        public static string EntityMetaToInsertSQL(EntityMeta meta)
        {                
            if (meta.Columns == null || meta.Columns.Count == 0)
                return string.Empty;

            StringBuilder sqlbuilder = new StringBuilder();
            //sqlbuilder.AppendFormat("INSERT INTO [{0}] (", meta.TableName);
            sqlbuilder.Append("INSERT INTO [{0}] (");
            for (int i = 0, j = 0; i < meta.Columns.Count; i++)
            {
                if (meta.Columns[i].Identity)
                {
                    continue;
                }
                if (j > 0)
                {
                    sqlbuilder.Append(",");
                }
                sqlbuilder.Append("[" + meta.Columns[i].ColumnName + "]");
                j++;
            }
            sqlbuilder.Append(") VALUES (");
            for (int i = 0, j = 0; i < meta.Columns.Count; i++)
            {
                if (meta.Columns[i].Identity)
                {
                    continue;
                }
                if (j > 0)
                {
                    sqlbuilder.Append(",");
                }
                sqlbuilder.Append("@" + meta.Columns[i].FieldName + "");
                j++;

            }
            sqlbuilder.Append(");");

            if (meta.Columns.Exists(x=>x.Identity))
            {
                sqlbuilder.Append(" SELECT CAST(SCOPE_IDENTITY() as bigint) as Id;"); //sqlbuilder.Append("SELECT SCOPE_IDENTITY();");
            }
            else
            {
                sqlbuilder.Append(" SELECT  CAST(0 as bigint) as Id;");
            }

            return sqlbuilder.ToString();          
        }

        public static string EntityMetaToUpdateSQL(EntityMeta meta)
        {
            if (meta.Columns == null || meta.Columns.Count == 0)
                return string.Empty;


            var keys = meta.Columns.FindAll(_ => _.PrimaryKey);

            StringBuilder sqlbuilder = new StringBuilder();
            //sqlbuilder.AppendFormat("UPDATE [{0}] SET ", meta.TableName);
            sqlbuilder.Append("UPDATE [{0}] SET ");

            for (int i = 0, j = 0; i < meta.Columns.Count; i++)
            {               
                if (!meta.Columns[i].PrimaryKey)
                {
                    if (j > 0)
                    {
                        sqlbuilder.Append(",");
                    }
                    j++;
                    sqlbuilder.Append("[" + meta.Columns[i].ColumnName + "]=@" + meta.Columns[i].FieldName + "");

                }
            }
            sqlbuilder.Append(" WHERE ");
            for (int i = 0; i < keys.Count; i++)
            {
                if (i > 0)
                {
                    sqlbuilder.Append(" AND ");
                }
                sqlbuilder.Append("[" + keys[i].ColumnName + "]=@" + keys[i].FieldName);
            }

            return sqlbuilder.ToString();
        }   

        internal static string BuildInsertSql(EntityMeta meta, List<string> list)
        {
            if (list == null || list.Count == 0)
                return string.Empty;

            StringBuilder sqlbuilder = new StringBuilder();
            //sqlbuilder.AppendFormat("INSERT INTO [{0}] (", meta.TableName);
            sqlbuilder.Append("INSERT INTO [{0}] (");
            for (int i = 0, j = 0; i < meta.Columns.Count; i++)
            {
                if (meta.Columns[i].Identity) continue;

                if (meta.Columns[i].Nullable && !list.Contains(meta.Columns[i].FieldName)) //列可为空，并且没有设置过值，则不提交数据库
                    continue;
                if (j > 0)
                {
                    sqlbuilder.Append(",");
                }
                sqlbuilder.AppendFormat("[{0}]", meta.Columns[i].ColumnName);
                j++;
            }
            sqlbuilder.Append(") VALUES (");
            for (int i = 0, j = 0; i < meta.Columns.Count; i++)
            {
                if (meta.Columns[i].Identity ) continue;

                if (meta.Columns[i].Nullable && !list.Contains(meta.Columns[i].FieldName)) //列可为空，并且没有设置过值，则不提交数据库
                    continue;


                if (j > 0)
                {
                    sqlbuilder.Append(",");
                }
                sqlbuilder.Append("@" + meta.Columns[i].FieldName + "");
                j++;

            }
            sqlbuilder.Append(");");

            if (meta.Columns.Exists(x => x.Identity))
            {
                sqlbuilder.Append(" SELECT CAST(SCOPE_IDENTITY() as bigint) as Id;"); //sqlbuilder.Append("SELECT SCOPE_IDENTITY();");
            }
            else
            {
                sqlbuilder.Append(" SELECT  CAST(0 as bigint) as Id;");
            }

            return sqlbuilder.ToString();          
        }

        internal static string BuildUpdateSql(EntityMeta meta, List<string> list,bool HasWhere)
        {
            if (list == null || list.Count == 0)
                return string.Empty;

            StringBuilder sqlbuilder = new StringBuilder();
            //sqlbuilder.AppendFormat("UPDATE [{0}] SET ", meta.TableName);
            sqlbuilder.Append("UPDATE [{0}] SET ");

            for (int i = 0, j = 0; i < meta.Columns.Count; i++)
            {
                if ( list.Contains(meta.Columns[i].FieldName)&&!meta.Columns[i].Identity) //字段不为自增列，并且必需赋值
                {
                    if (j > 0)
                    {
                        sqlbuilder.Append(",");
                    }
                    j++;
                    sqlbuilder.AppendFormat("[{0}]=@{1}" ,meta.Columns[i].ColumnName, meta.Columns[i].FieldName);
                }
            }

            if (HasWhere)
            {
                var keys = meta.Columns.FindAll(_ => _.PrimaryKey);
                if (keys.Count == 0)
                    throw new ORMException(string.Format("需要where条件时，类型[{0}]必需要有主键",meta.TableName));

                sqlbuilder.Append(" WHERE ");
                for (int i = 0, j = keys.Count; i < j; i++)
                {
                    if (!list.Contains(keys[i].ColumnName))
                    {
                        throw new ORMException(string.Format("主键字段[{0}]必需设置值", keys[i].FieldName));
                    }
                    if (i > 0)
                    {
                        sqlbuilder.Append(" AND ");
                    }
                    sqlbuilder.AppendFormat("[{0}]=@{1}", keys[i].ColumnName, keys[i].FieldName);
                }
            }
            

            return sqlbuilder.ToString();
        }

        internal static string BuildPageCountSql (string sql)
        {
            string pageSql = string.Format(" SELECT Count(1) FROM ( {0} ) AS TEMP ",sql);
            return pageSql;
        }
        internal static string BuildPageSql(string sql,Page.PageView view)
        {
            if (string.IsNullOrEmpty(view.Primary))
                return BuildPageSql(sql, view, view.Primary);
            sql=sql.Substring(sql.IndexOf("SELECT ",StringComparison.InvariantCultureIgnoreCase)+7);
            string pageSql = string.Format(" SELECT TOP {0} *  FROM (SELECT ROW_NUMBER() OVER({1}) AS {2},{3} ) AS TEMP_TABLE  WHERE {2} >{4} ORDER BY {2}  ",
                view.PageSize, view.GetSqlOrder(), Define.MYROWID, sql, view.PageIndex * view.PageSize);
            return pageSql;
        }
        internal static string BuildPageSql(string sql, Page.PageView view,string pk)
        {
            sql = sql.Substring(sql.IndexOf("SELECT ", StringComparison.InvariantCultureIgnoreCase) + 7);
            string pageSql = string.Format(" SELECT TOP {0} *  FROM (SELECT ROW_NUMBER() OVER({1}) AS {2},{3} ) AS TEMP_TABLE  WHERE {2} >{4} ORDER BY {2}  ",
                view.PageSize, view.GetSqlOrder(), Define.MYROWID, sql, view.PageIndex * view.PageSize);
            return pageSql;
        }
    }
}
