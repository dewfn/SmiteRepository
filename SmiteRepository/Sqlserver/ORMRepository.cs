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
using SmiteRepository.Dapper;
namespace SmiteRepository.Sqlserver
{
    
    
    /// <summary>
    /// SQLSQERVER使用
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class ORMRepository<TEntity> : IORMRepository<TEntity> where TEntity : BaseEntity, new()
     {
        internal ORMRepository(string connectionString, EntityMeta Meta)
         {
             this.connectionString = connectionString;
             meta = Meta;
         }
        public ORMRepository(string connectionString)
        {
            meta=EntityReflect.GetDefineInfoFromType(typeof(TEntity));
            this.connectionString = connectionString; 
        }
         private string connectionString;
         EntityMeta meta;
        
         public int Test<tr>(Expression<Func<TEntity,bool>> where, Expression<Action<TEntity>> fields)
         {
             
            
             return 0;
         }

         #region 删除相关方法
         public int Delete(Expression<Predicate<TEntity>> where) 
         {
             string whereSql = string.Empty;
             DynamicParameters param = null;
             if (where != null)
             {
                 ResolveExpress r = new ResolveExpress(where, meta);
                 whereSql = " WHERE " + r.SqlWhere;


                 param = new DynamicParameters();
                 foreach (var arg in r.Argument)
                 {
                     param.Add(arg.Key, arg.Value);
                 }
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
             DynamicParameters param = new DynamicParameters(entity);

             if (where != null)
             {

                 //条件参数化
                 ResolveExpress r = new ResolveExpress(where, meta);

                 whereSql = " WHERE " + r.SqlWhere;
                 foreach (var arg in r.Argument)
	            {
		           param.Add(arg.Key,arg.Value);
	            }
             }
             else
             {
                 ////主键参数化
                 //var pkeys = meta.Columns.FindAll(_ => _.PrimaryKey);
                 //foreach (EntityColumnMeta pk in pkeys)
                 //{
                 //    string key = pk.ColumnName;
                 //    string fileName = pk.FieldName;

                 //}
             }
             

             string sql = this.GetUpdateChangeColumnsSql(where == null,entity,meta); //表达式为空，表示需要 以主键为where条件
             sql = string.Format(sql, meta.TableName);
             sql += whereSql;

            int result= DBHelper.ExecuteCommand(this.connectionString, sql,param);

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
            
             string sql = this.GetInsertChangeColumnsSql(entity,meta);
             sql = string.Format(sql, meta.TableName);
             long id= DBHelper.GetScalar<long>(connectionString, sql, entity);
             entity.RemoveUpdateColumn();//清除掉已更新的字段
             return id;
         }

         #endregion

         #region 查找相关方法

         public TEntity Find(Expression<Predicate<TEntity>> where) 
         {
             string showFields = meta.SelectStringColumns;
             return Find(where, showFields, null);
         }
         public TEntity Find(Expression<Predicate<TEntity>> where, Expression<Func<Fileds.DisplaysMethod, TEntity, Fileds>> fields)
         {
           
             string showFields = GetShowFields(fields, meta);
             return Find(where, showFields, null);

         }
         public TEntity Find(Expression<Predicate<TEntity>> where, Expression<Func<Fileds.DisplaysMethod, TEntity, Fileds>> fields, Expression<Func<Fileds.OrderByMethod, TEntity, Fileds>> order_fields)
         {
             string showFields = GetShowFields(fields, meta);
             string orderFields = GetOrderByFields(order_fields, meta);
             return Find(where, showFields, orderFields);
         }
         private TEntity Find(Expression<Predicate<TEntity>> where, string showFields, string orderFields)
         {
             string whereSql = string.Empty;
             DynamicParameters param = null;
             Dictionary<string, object> dicArgument = null;
             string sql = string.Empty;
             if (where != null)
             {

                 ResolveExpress r = new ResolveExpress(where, meta);
                 whereSql = " WHERE " + r.SqlWhere;
                 dicArgument = r.Argument;
                 param = new DynamicParameters();
                 foreach (var arg in r.Argument)
                 {
                     param.Add(arg.Key, arg.Value);
                 }

             }
            
                 if (!string.IsNullOrEmpty(orderFields))
                     orderFields = " ORDER BY " + orderFields;
                 sql = string.Format("SELECT TOP 1 {0} from {1} {2} {3}", showFields, meta.TableName, whereSql, orderFields);
             


             return DBHelper.Query<TEntity>(connectionString, sql, param).FirstOrDefault();
         }
         public List<TEntity> FindAll(Expression<Predicate<TEntity>> where) 
         {
             string showFields = meta.SelectStringColumns;
             return FindAll(where, showFields, meta);
         }
         public List<TEntity> FindAll()
         {
             string showFields = meta.SelectStringColumns;
             return FindAll(null, showFields, meta);
         }
         public List<TEntity> FindAll(Expression<Func<Fileds.DisplaysMethod, TEntity, Fileds>> fields)
         {
             string showFields = GetShowFields(fields, meta);
             return FindAll(null, showFields, meta);
         }
         public List<TEntity> FindAll(Expression<Predicate<TEntity>> where, Expression<Func<Fileds.DisplaysMethod, TEntity, Fileds>> fields) 
         {
             string showFields = GetShowFields(fields, meta);
             return FindAll(where, showFields, meta);
         }
       
         private List<TEntity> FindAll(Expression<Predicate<TEntity>> where, string showFields, EntityMeta meta) 
         {
             string whereSql = string.Empty;
             DynamicParameters param = null;
             if (where != null)
             {

                 ResolveExpress r = new ResolveExpress(where, meta);

                 whereSql = " WHERE " + r.SqlWhere;

                 param = new DynamicParameters();
                 foreach (var arg in r.Argument)
	            {
		           param.Add(arg.Key,arg.Value);
	            }
             }

             string sql = string.Format("SELECT {0} from {1} {2}", showFields, meta.TableName, whereSql);
             return DBHelper.Query<TEntity>(connectionString, sql, param);
         }

         #endregion

         #region 分页查找方法
         public PagedList<TEntity> GetPage(PageView pView, Expression<Predicate<TEntity>> where=null, Expression<Func<Fileds.DisplaysMethod, TEntity, Fileds>> fields=null)
         {
             string showFields = GetShowFields(null, meta);
             return GetPage(where, showFields, meta, pView);
         }
         private PagedList<TEntity> GetPage(Expression<Predicate<TEntity>> where, string showFields, EntityMeta meta,PageView view)
         {
             string whereSql = string.Empty;
             DynamicParameters param = null;
             if (where != null)
             {
                 param = new DynamicParameters();
                 ResolveExpress r = new ResolveExpress(where, meta);

                 whereSql = " WHERE " + r.SqlWhere;
                 foreach (var arg in r.Argument)
                 {
                     param.Add(arg.Key, arg.Value);
                 }
             }

             string sql = string.Format("SELECT {0} from {1} {2}", showFields, meta.TableName, whereSql);



             PagedList<TEntity> pList = new PagedList<TEntity>();

             if (view.Total == 0)
                 pList.Total = DBHelper.GetScalar<int>(this.connectionString, EntityMSSQLBuilder.BuildPageCountSql(sql), param);
             else
                 pList.Total = view.Total;

             if (pList.Total > 0)
             {
                 pList.DataList=DBHelper.Query<TEntity>(this.connectionString, EntityMSSQLBuilder.BuildPageSql(sql, view), param);
                
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
                 throw new ORMException("传入显示字段表达式错误!");
             LambdaExpression lambda = fields as LambdaExpression;
             string bodyStr = lambda.Body.ToString();

             string args = lambda.Parameters[1].Name + ".";
             bodyStr = bodyStr.Replace(')', ' ').Replace('}', ' ');
             var fieldArray = bodyStr.Split(',').Distinct();
             EntityColumnMeta fileColumn;
             StringBuilder showFields = new StringBuilder();
             foreach (var fieldFilter in fieldArray)
             {
                 string item = fieldFilter.Trim();
                 int argIndex = item.IndexOf(args);
                 if (argIndex == -1)
                     continue;
                 item = item.Substring(argIndex + args.Length);
                 fileColumn = meta.Columns.Find(x => x.FieldName == item);
                 if (fileColumn == null)
                     throw new ORMException(string.Format("映射列名[{0}]找不到", item));
                 showFields.Append(fileColumn.ColumnName).Append(",");
             }
            
             return showFields.Length > 0 ? showFields.Remove(showFields.Length - 1, 1).ToString() : string.Empty;
         }
         private string GetOrderByFields(Expression fields, EntityMeta meta)
         {
             if (fields == null)
                 return meta.SelectStringColumns;
             if (!(fields is LambdaExpression))
                 throw new ORMException("传入排序字段表达式错误!");
             LambdaExpression lambda = fields as LambdaExpression;
             string bodyStr = lambda.Body.ToString();

             string args = lambda.Parameters[1].Name + ".";
             bodyStr = bodyStr.Replace(')', ' ').Replace('}', ' ').Replace('(', ' ');
             var fieldArray = bodyStr.Split(',').Distinct();
             EntityColumnMeta fileColumn;
             StringBuilder showFields = new StringBuilder();
             string[] orderArray;
             foreach (var fieldFilter in fieldArray)
             {
                 string item = fieldFilter.Trim();
                 int argIndex = item.IndexOf(args);
                 if (argIndex == -1)
                     continue;
                 item = item.Substring(argIndex + args.Length);
                 orderArray = item.Split('.');
                 fileColumn = meta.Columns.Find(x => x.FieldName == orderArray[0]);
                 if (fileColumn == null)
                     throw new ORMException(string.Format("映射列名[{0}]找不到", item));
                 showFields.Append(fileColumn.ColumnName).Append(orderArray[1].Replace("()", "")).Append(",");
             }

             return showFields.Length > 0 ? showFields.Remove(showFields.Length - 1, 1).ToString() : string.Empty;
         }

         protected TResult ExecOne<TResult>(Expression fields, Expression where, string execType)
         {
             string whereSql = string.Empty;
             DynamicParameters param = null;
             if (where != null)
             {
                 param = new DynamicParameters();
                 ResolveExpress r = new ResolveExpress(where, meta);

                 whereSql = " WHERE " + r.SqlWhere;
                 foreach (var arg in r.Argument)
                 {
                     param.Add(arg.Key, arg.Value);
                 }
             }
             string showName;
             if (execType == ExecType.Count||execType==ExecType.Exists)
                 showName = "1";
             else
                 showName = GetShowFields(fields, meta);

             if (string.IsNullOrEmpty(showName))
                 throw new ORMException("找不到需要显示出的字段");

             string sql = string.Format("SELECT  {0}({1}) FROM {2} {3}", execType, showName, meta.TableName, whereSql);
             return DBHelper.GetScalar<TResult>(this.connectionString, sql, param);

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

         public string ConnectionString { set { this.connectionString = value; } }
     }
}
