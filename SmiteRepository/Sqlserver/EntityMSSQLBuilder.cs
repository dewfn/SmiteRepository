using SmiteRepository.ORMapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

                if ( !list.Contains(meta.Columns[i].FieldName)) //列可为空，并且没有设置过值，则不提交数据库
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

                if ( !list.Contains(meta.Columns[i].FieldName)) //列可为空，并且没有设置过值，则不提交数据库
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
            var sqlDic = GetSqlRegex(sql);
            string tablename = sqlDic["tname"];
            string wheresql = sqlDic["where"];
           
           return string.Format(pageCountSql,  tablename,  wheresql);
        }
        internal static string BuildPageSql(string sql,Page.PageView view)
        {
            //if (!string.IsNullOrWhiteSpace(view.Primary))
            //    return BuildPageSql(sql, view, view.Primary);
            var sqlDic = GetSqlRegex(sql);
            string selectColums = sqlDic["col"];
            string tablename = sqlDic["tname"];
            string wheresql = sqlDic["where"];
            string ordersql = sqlDic["order"];

            string viewOrder = view.GetSqlOrder();
            if (!string.IsNullOrEmpty(viewOrder))
                ordersql = viewOrder;


            string pageSql = string.Format(pageSql2,
                view.PageIndex * view.PageSize, ordersql, Define.MYROWID, selectColums, (view.PageIndex + 1) * view.PageSize, tablename, wheresql);
            return pageSql;
        }
        static string pageCountSql = " SELECT Count(1) AS COUNT_ROW FROM {0} {1}";
        static string sqlRegexString = "select(?<col>((?!select|from)[\\S\\s])+(((?'Open'select)((?!select|from)[\\S\\s])+)+((?'-Open'from)((?!select|from)[\\S\\s])+)+)*(?(Open)(?!)))from\\s+(?<tname>(?(\\()(\\([^\\(\\)]*((?'TNGroup'\\()[^\\(\\)]*((?'-TNGroup'\\))[^\\(\\)]*)*)*(?(TNGroup)(?!))\\))|\\S+)(\\s+AS)*((?!WHERE)[\\S\\s])*)(WHERE\\s+(?<where>((?!ORDER\\s+BY)[\\S\\s])+))?(ORDER\\s+BY\\s+(?<order>[\\S\\s]+))?";
        static string pageSql = @"SELECT {0} FROM 
                                {1}
                                INNER JOIN
                                (SELECT ROW_NUMBER() OVER( ORDER BY {2}) AS ROWID,{3} AS TEMP_IDENTITY  FROM 
                                  {1} 
                                  {4}) AS TEMP_PAGE_TABLE ON TEMP_PAGE_TABLE.TEMP_IDENTITY={3}
                                    WHERE TEMP_PAGE_TABLE.ROWID BETWEEN {5} AND {6} ORDER BY TEMP_PAGE_TABLE.ROWID";
        static string pageSql2 = @" SELECT *  FROM 
                            (SELECT ROW_NUMBER() OVER( ORDER BY {1}) AS {2},{3} 
                                    FROM {5} 
                                    {6}) AS TEMP_TABLE 
                                WHERE {2} BETWEEN {0} AND {4} ORDER BY {2}  ";
        internal static string BuildPageSql(string sql, Page.PageView view,string pk)
        {
            var sqlDic = GetSqlRegex(sql);
            string selectColums = sqlDic["col"];
            string tablename = sqlDic["tname"];
            string wheresql = sqlDic["where"];
            string ordersql = sqlDic["order"];
                string viewOrder= view.GetSqlOrder();
                if (!string.IsNullOrEmpty(viewOrder))
                    ordersql = viewOrder;
                return string.Format(pageSql, selectColums, tablename, ordersql, pk, wheresql,  view.PageIndex * view.PageSize, (view.PageIndex+1) * view.PageSize);
        }


        private static Dictionary<string, string> GetSqlRegex(string sql)
        {
            sql = new Regex("--[\\s\\S]+?\\r\\n|--[\\s\\S]+?$").Replace(sql, "\r\n"); //去注释
            List<string> listReplace= new List<string>();
            //替换到所有子查询为/sign:{0}/
            sql = new Regex("(\\([^\\(\\)]*select[^\\(\\)]*((?'TNGroup'\\()[^\\(\\)]*((?'-TNGroup'\\))[^\\(\\)]*)*)*(?(TNGroup)(?!))\\))"
                , RegexOptions.IgnoreCase).Replace(sql,
                new MatchEvaluator(delegate(Match m)
            {
                listReplace.Add(m.Value);
                return string.Format("/sign:{0}/", listReplace.Count - 1);
            }));
            Regex sqlRegex = new Regex(sqlRegexString, RegexOptions.IgnoreCase);
            Match match = sqlRegex.Match(sql);
            if (!match.Success)
            {
                throw new ORMException("SQL解析错误或解析不了，请确认你的SQL正解", sql);
            }
            string selectColums = match.Groups["col"].Value;
            string tablename = match.Groups["tname"].Value;
            string wheresql = match.Groups["where"].Value;
            string ordersql = match.Groups["order"].Value;
            for (int i = 0; i < listReplace.Count; i++)
            {
                string replaceStr=string.Format("/sign:{0}/", i);
                selectColums = selectColums.Replace(replaceStr, listReplace[i]);
                tablename = tablename.Replace(replaceStr, listReplace[i]);
                wheresql = wheresql.Replace(replaceStr, listReplace[i]);
            }
            if (!string.IsNullOrWhiteSpace(wheresql))
            {

                wheresql = " WHERE " + wheresql;
            }

            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("col", selectColums);
            dic.Add("tname", tablename);
            dic.Add("where", wheresql);
            dic.Add("order", ordersql);
            return dic;

        }
    }
}
