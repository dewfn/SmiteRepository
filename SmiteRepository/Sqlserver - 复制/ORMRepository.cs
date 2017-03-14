using SmiteRepository.Command;
using SmiteRepository.ORMapping;
using SmiteRepository.Page;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;


namespace SmiteRepository.Sqlserver
{
    
    
    /// <summary>
    /// SQLSQERVER使用
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class ORMRepository<TEntity> : IORMRepository<TEntity> where TEntity : BaseEntity, new()
     {
         public ORMRepository(string connectionString)
         {
             this.connectionString = connectionString;
         }
         private string connectionString;


         public int Test<tr>(Expression<Func<TEntity, bool>> where, Expression<Action<TEntity>> fields)
         { 
         
             //   return ExecOne<TEntity, int>(fields, where, ExecType.Avg);
             return 0;
         }

         #region 删除相关方法
         public int Delete(Expression<Predicate<TEntity>> where) 
         {
             string whereSql = string.Empty;
            object param = null;
             EntityMeta meta = EntityReflect.GetDefineInfoFromType(typeof(TEntity));
             if (where != null)
             {
                 ResolveExpress r = new ResolveExpress(where, meta);
                 whereSql = " WHERE " + r.SqlWhere;
                 param = r.Argument.Select(x => new DataParameter(x.Key,  x.Value)).ToArray();
             }

             string sql = string.Format("DELETE FROM {0} {1}", meta.TableName, whereSql);
             return DBHelper.ExecuteCommand(connectionString, sql, param);
         }
         #endregion


         #region 更新相关方法
         private string GetUpdateChangeColumnsSql(bool HasWhere, TEntity entity, EntityMeta metadeta)
         {             
             return EntityMSSQLBuilder.BuildUpdateSql(metadeta, entity.PropertyChangedList,HasWhere);
         }

         public int Update(TEntity entity) 
         {
             return Update(entity, null);
         }

         public int Update(TEntity entity, Expression<Predicate<TEntity>> where) 
         {
             string whereSql = string.Empty;
             List<DataParameter> param = new List<DataParameter>();
             EntityMeta meta = EntityReflect.GetDefineInfoFromType(typeof(TEntity));

             if (where != null)
             {
                 //条件参数化
                 ResolveExpress r = new ResolveExpress(where, meta);

                 whereSql = " WHERE " + r.SqlWhere;
                 param = r.Argument.Select(x => new DataParameter(x.Key, x.Value)).ToList();

             }
             else
             {
                 //主键参数化
                 var pkeys = meta.Columns.FindAll(_ => _.PrimaryKey);
                 foreach (EntityColumnMeta pk in pkeys)
                 {
                     string key = pk.ColumnName;
                     string fileName = pk.FieldName;
                    object val= SqlMapper.getInstance().GetPropertyValueViaEmit<TEntity>(entity, fileName);
                     //object val = typeof(TEntity).GetProperty(fileName).GetValue(entity);

                     if (val == null)
                         throw new Exception("主键值不能为空");
                     param.Add(new DataParameter(key, val));
                 }
             }


             foreach (string propertyName in entity.PropertyChangedList)//会被更新到数据库的字段
             {

                 EntityColumnMeta fileColumn = meta.Columns.Find(x => x.FieldName == propertyName);

                 if (fileColumn == null)
                     continue;
                 if (fileColumn.PrimaryKey) //如果为主键则跳出
                     continue;
                 string key = fileColumn.ColumnName;
                 //object val = typeof(TEntity).GetProperty(propertyName).GetValue(entity);
                 object val = SqlMapper.getInstance().GetPropertyValueViaEmit<TEntity>(entity, fileColumn.FieldName);

                 #region  注释后，是否为空验证规则在程序端验证  可注释的代码
                 if (!fileColumn.Nullable && val == null)
                     throw new Exception(string.Format("字段[{0}]值不能为NULL", fileColumn.FieldName));
                 #endregion

                 val = val ?? DBNull.Value;
                 param.Add(new DataParameter(key, val));
             }
             string sql = this.GetUpdateChangeColumnsSql(where == null,entity,meta); //表达式为空，表示需要 以主键为where条件
             sql += whereSql;
             int result=DBHelper.ExecuteCommand(this.connectionString, sql, param.ToArray());
             entity.RemoveUpdateColumn();//清除掉已更新的字段
             return result;
         }

         #endregion

         #region 插入相关方法
         private string GetInsertChangeColumnsSql(TEntity entity, EntityMeta metadeta)
         {
             return EntityMSSQLBuilder.BuildInsertSql(metadeta, entity.PropertyChangedList);
         }

         public long Insert(TEntity entity) 
         {
             List<DataParameter> param = new List<DataParameter>();
             EntityMeta meta = EntityReflect.GetDefineInfoFromType(typeof(TEntity));

             foreach (EntityColumnMeta fileColumn in meta.Columns)
             {
                 if (fileColumn.Identity)
                     continue;
                 if (fileColumn.Nullable && !entity.PropertyChangedList.Contains(fileColumn.FieldName)) //列可为空，并且没有设置过值，则不提交数据库
                     continue;

                 string key = fileColumn.ColumnName;
                // object val = meta.EntityType.GetProperty(fileColumn.FieldName).GetValue(entity);
                 object val = SqlMapper.getInstance().GetPropertyValueViaEmit<TEntity>(entity, fileColumn.FieldName);

                 #region  注释后，是否为空验证规则在程序端验证  可注释的代码
                 if (!fileColumn.Nullable && val == null)
                     throw new Exception(string.Format("字段[{0}]值不能为NULL", fileColumn.FieldName));
                 #endregion

                 val = val ?? DBNull.Value;

                 param.Add(new DataParameter(key, val));
             }

             string sql = this.GetInsertChangeColumnsSql(entity,meta);
             object id= DBHelper.GetScalar(this.connectionString, sql, param.ToArray());
             entity.RemoveUpdateColumn();//清除掉已更新的字段
             return  Convert.ToInt64(id);
         }

         #endregion

         #region 查找相关方法

         public TEntity Find(Expression<Predicate<TEntity>> where) 
         {
             EntityMeta meta = EntityReflect.GetDefineInfoFromType(typeof(TEntity));
             string showFields = meta.SelectStringColumns;
             List<TEntity> list = FindAll(where, showFields, meta);
             return list.FirstOrDefault();
         }
         public TEntity Find(Expression<Predicate<TEntity>> where, Expression<Func<Fileds.DisplaysMethod, TEntity, Fileds>> fields)
         {
             EntityMeta meta = EntityReflect.GetDefineInfoFromType(typeof(TEntity));
             string showFields = GetShowFields(fields, meta);
             List<TEntity> list = FindAll(where, showFields, meta);

             return list.FirstOrDefault();
         }

         public List<TEntity> FindAll(Expression<Predicate<TEntity>> where) 
         {
             EntityMeta meta = EntityReflect.GetDefineInfoFromType(typeof(TEntity));
             string showFields = meta.SelectStringColumns;
             return FindAll(where, showFields, meta);
         }
         public List<TEntity> FindAll()
         {
             EntityMeta meta = EntityReflect.GetDefineInfoFromType(typeof(TEntity));
             string showFields = meta.SelectStringColumns;
             return FindAll(null, showFields, meta);
         }
         public List<TEntity> FindAll(Expression<Func<Fileds.DisplaysMethod, TEntity, Fileds>> fields)
         {
             EntityMeta meta = EntityReflect.GetDefineInfoFromType(typeof(TEntity));
             string showFields = GetShowFields(fields, meta);
             return FindAll(null, showFields, meta);
         }
         public List<TEntity> FindAll(Expression<Predicate<TEntity>> where, Expression<Func<Fileds.DisplaysMethod, TEntity, Fileds>> fields) 
         {
             EntityMeta meta = EntityReflect.GetDefineInfoFromType(typeof(TEntity));
             string showFields = GetShowFields(fields, meta);
             return FindAll(where, showFields, meta);
         }
       
         private List<TEntity> FindAll(Expression<Predicate<TEntity>> where, string showFields, EntityMeta meta) 
         {
             string whereSql = string.Empty;
            object param = null;
             if (where != null)
             {
                 ResolveExpress r = new ResolveExpress(where, meta);

                 whereSql = " WHERE " + r.SqlWhere;
                 param = r.Argument.Select(x => new DataParameter(x.Key, x.Value)).ToArray();
             }

             string sql = string.Format("SELECT {0} from {1} {2}", showFields, meta.TableName, whereSql);
             using (IDataReader reader = DBHelper.GetReader(connectionString, sql, param))
             {
                 return ConvertHelper.ConvertToList<TEntity>(reader, meta);
             }
         }

         #endregion

         #region 分页查找方法
         public PagedList<TEntity> GetPage(PageView pView, Expression<Predicate<TEntity>> where=null, Expression<Func<Fileds.DisplaysMethod, TEntity, Fileds>> fields=null)
         {
             EntityMeta meta = EntityReflect.GetDefineInfoFromType(typeof(TEntity));
             string showFields = GetShowFields(null, meta);
             return GetPage(where, showFields, meta, pView);
         }
         private PagedList<TEntity> GetPage(Expression<Predicate<TEntity>> where, string showFields, EntityMeta meta,PageView view)
         {
             string whereSql = string.Empty;
            object param = null;
             if (where != null)
             {
                 ResolveExpress r = new ResolveExpress(where, meta);

                 whereSql = " WHERE " + r.SqlWhere;
                 param = r.Argument.Select(x => new DataParameter(x.Key, x.Value)).ToArray();
             }

             string sql = string.Format("SELECT {0} from {1} {2}", showFields, meta.TableName, whereSql);



             PagedList<TEntity> pList = new PagedList<TEntity>();

             if (view.Total == 0)
                 pList.Total = (int)DBHelper.GetScalar(connectionString, EntityMSSQLBuilder.BuildPageCountSql(sql), param);
             else
                 pList.Total = view.Total;

             if (pList.Total > 0)
             {
                //object param2 = null;
                // if (param != null)
                // {
                //     param2 = param.Select(x => new DataParameter(x.ParameterName, x.Value)).ToArray();
                // }

                 using (IDataReader reader = DBHelper.GetReader(connectionString, EntityMSSQLBuilder.BuildPageSql(sql, view), param))
                 {
                     pList.DataList= ConvertHelper.ConvertToList<TEntity>(reader, meta);
                 }

             }
             pList.PageIndex = view.PageIndex;
             pList.PageSize = view.PageSize;

             return pList;            
         }

         #endregion


         #region 内部方法
         private string GetShowFields(Expression fields, EntityMeta meta)
         {
             if (fields == null)
                 return meta.SelectStringColumns;
             if (!(fields is LambdaExpression))
                 throw new Exception("传入显示字段表达式错误!");
             LambdaExpression lambda = fields as LambdaExpression;
             string bodyStr = lambda.Body.ToString();

             string args = lambda.Parameters[1].Name + ".";
             bodyStr = bodyStr.Replace(')', ' ').Replace('}', ' ');
             string[] fieldArray = bodyStr.Split(',');
             EntityColumnMeta fileColumn;
             StringBuilder showFields = new StringBuilder();
             for (int i = 0; i < fieldArray.Length; i++)
             {
                 string item = fieldArray[i].Trim();
                 int argIndex = item.IndexOf(args);
                 if (argIndex == -1)
                     continue;
                 item = item.Substring(argIndex + args.Length);
                 fileColumn = meta.Columns.Find(x => x.FieldName == item);
                 if (fileColumn == null)
                     throw new Exception(string.Format("映射列名[{0}]找不到", item));
                 showFields.Append(fileColumn.ColumnName).Append(",");
             }
             return showFields.Length > 0 ? showFields.Remove(showFields.Length - 1, 1).ToString() : string.Empty;
         }


         protected TResult ExecOne<TResult>(Expression fields, Expression where, string execType)
         {
             string whereSql = string.Empty;
            object param = null;
             EntityMeta meta = EntityReflect.GetDefineInfoFromType(typeof(TEntity));
             if (where != null)
             {
                 ResolveExpress r = new ResolveExpress(where, meta);

                 whereSql = " WHERE " + r.SqlWhere;
                 param = r.Argument.Select(x => new DataParameter(x.Key, x.Value)).ToArray();
             }
             string showName;
             if (execType == ExecType.Count||execType==ExecType.Exists)
                 showName = "1";
             else
                 showName = GetShowFields(fields, meta);

             if (string.IsNullOrEmpty(showName))
                 throw new Exception("找不到需要显示出的字段");

             string sql = string.Format("SELECT  {0}({1}) FROM {2} {3}", execType, showName, meta.TableName, whereSql);
             object result=DBHelper.GetScalar(connectionString, sql, param);

             return result==DBNull.Value?default(TResult):(TResult)result;
         }
         #endregion

         #region 聚合函数方法
         public TResult Scalar<TResult>(Expression<Func<Fileds.DisplayMethod<TResult>, TEntity, Fileds>> fields, Expression<Predicate<TEntity>> where) 
         {
             return ExecOne< TResult>(fields, where, ExecType.Scalar);
         }

         public TResult Max<TResult>(Expression<Func<Fileds.DisplayMethod<TResult>, TEntity, Fileds>> fields, Expression<Predicate<TEntity>> where) 
         {
             return ExecOne< TResult>(fields, where, ExecType.Max);
         }

         public TResult Min<TResult>(Expression<Func<Fileds.DisplayMethod<TResult>, TEntity, Fileds>> fields, Expression<Predicate<TEntity>> where) 
         {
             return ExecOne<TResult>(fields, where, ExecType.Min);
         }

         public TResult Sum<TResult>(Expression<Func<Fileds.DisplayMethod, TEntity, Fileds>> fields, Expression<Predicate<TEntity>> where) where TResult : IConvertible
         {
             return ExecOne<TResult>(fields, where, ExecType.Sum);
         }
         public TResult Avg<TResult>(Expression<Func<Fileds.DisplayMethod, TEntity, Fileds>> fields, Expression<Predicate<TEntity>> where) where TResult : IConvertible
         {
             return ExecOne<TResult>(fields, where, ExecType.Avg);
         }

         public TResult Scalar<TResult>(Expression<Func<Fileds.DisplayMethod<TResult>, TEntity, Fileds>> fields) 
         {
             return ExecOne< TResult>(fields, null, ExecType.Scalar);
         }

         public TResult Max< TResult>(Expression<Func<Fileds.DisplayMethod<TResult>, TEntity, Fileds>> fields)
         {
             return ExecOne< TResult>(fields, null, ExecType.Max);
         }

         public TResult Min<TResult>(Expression<Func<Fileds.DisplayMethod<TResult>, TEntity, Fileds>> fields) 
         {
             return ExecOne<TResult>(fields, null, ExecType.Min);
         }

         public TResult Sum<TResult>(Expression<Func<Fileds.DisplayMethod, TEntity, Fileds>> fields) where TResult : IConvertible
         {
             return ExecOne<TResult>(fields, null, ExecType.Sum);
         }
         public TResult Avg<TResult>(Expression<Func<Fileds.DisplayMethod, TEntity, Fileds>> fields) where TResult : IConvertible
         {
             return ExecOne<TResult>(fields, null, ExecType.Avg);
         }
         public int Count(Expression<Predicate<TEntity>> where)
         {
             return ExecOne<int>(null, where, ExecType.Count);
         }

         public int Count()
         {
             return ExecOne<int>(null, null, ExecType.Count);
         }
         
        
         #endregion
        public bool Exists(Expression<Predicate<TEntity>> where)
         {
             return ExecOne<int>(null, where, ExecType.Exists)==1;
         }

         public bool Exists()
         {
             return ExecOne<int>(null, null, ExecType.Exists)==1;
         }
     }
}
