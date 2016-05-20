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
    /// DB层基类，需要继承实现
    /// </summary>
    public abstract class BaseRepository
    {
        private string connectionString;
        /// <summary>
        /// 传入连接字符串
        /// </summary>
        /// <param name="ConnectionString">连接字符串</param>
        public BaseRepository(string ConnectionString)
        {
            this.connectionString = ConnectionString;
        }
        private readonly object _lockdic = new object();
        private Dictionary<Type, ORMTEntity> dicORM = new Dictionary<Type, ORMTEntity>();
        /// <summary>
        /// 得到一个支持LINQ的可持久化实体类
        /// </summary>
        /// <typeparam name="TEntity">要得到的实体类型</typeparam>
        /// <returns>可以操作数据库</returns>
        public virtual IORMRepository<TEntity> For<TEntity>() where TEntity : BaseEntity, new()
        {

            return For<TEntity>(this.connectionString);
        }
        /// <summary>
        /// 得到一个支持LINQ的可持久化实体类，慎用
        /// </summary>
        /// <typeparam name="TEntity">要得到的实体类型</typeparam>
        /// <param name="ConnectionString">数据库连接（每次会覆盖当前Repository得到此类的所有连接字符串）</param>
        /// <returns>可以操作数据库</returns>
        public virtual IORMRepository<TEntity> For<TEntity>(string ConnectionString) where TEntity : BaseEntity, new()
        {

            ORMTEntity rep;
            IORMRepository<TEntity> rrmRe;
            
            lock (_lockdic)
            {
                if (dicORM.TryGetValue(typeof(TEntity), out rep))
                {
                    rrmRe= rep.ORMrepository<TEntity>();
                    rrmRe.ConnectionString = ConnectionString;
                }
                else
                {
                    EntityMeta meta = EntityReflect.GetDefineInfoFromType(typeof(TEntity));
                    if (meta.IsCustomTableName)
                        rrmRe = new ORMRepository_Ext<TEntity>(ConnectionString,meta);
                    else
                        rrmRe = new ORMRepository<TEntity>(ConnectionString,meta);
                    dicORM.Add(typeof(TEntity), new ORMTEntity(rrmRe));
                   
                }

            }
            return rrmRe;
        }
        /// <summary>
        /// 通过SQL得到分页列表
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="view">分页，一般通过页面传入</param>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        protected PagedList<T> PageGet<T>(PageView view, string sql, object param, int? timeOut = null) where T : class,new()
        { 

            PagedList<T> pList = new PagedList<T>();

            if (view.Total == 0)
                pList.Total = this.GetScalar<int>(EntityMSSQLBuilder.BuildPageCountSql(sql), param,CommandType.Text ,null,timeOut);
            else
                pList.Total = view.Total;

            if (pList.Total > 0)
            {
               //object param2 = null;
               // if (param != null)
               // {
               //     param2 = param.Select(x => new DataParameter(x.ParameterName, x.Value)).ToArray();
               // }

                var list = this.Query<T>(EntityMSSQLBuilder.BuildPageSql(sql, view), param, CommandType.Text,null, timeOut);
                pList.DataList = list;
            }
            else
                pList.DataList = new List<T>();
            
            pList.PageIndex = view.PageIndex;
            pList.PageSize = view.PageSize;
            return pList;
        }
        /// <summary>
        /// 通过SQL得到分页列表
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="view">分页，一般通过页面传入</param>
        /// <param name="sql"></param>
        /// <returns></returns>
        protected PagedList<T> PageGet<T>(PageView view, string sql,  int? timeOut = null) where T : class,new()
        {
            return PageGet<T>(view, sql, null,timeOut);
        }

        /// <summary>
        /// 执行一条SQL语句或存储过程
        /// </summary>
        /// <param name="safeSql">语句或过程</param>
        /// <param name="cmdType">SQL类型，默认为SQL语句</param>
        /// <returns></returns>
        protected int ExecuteCommand(string safeSql, CommandType cmdType = CommandType.Text, IDbTransaction transaction = null, int? timeOut = null)
        {
            return ExecuteCommand(safeSql, null,cmdType,transaction,timeOut);

        }
        //带参数的执行命令  
        protected int ExecuteCommand(string safeSql, object param, CommandType cmdType = CommandType.Text, IDbTransaction transaction = null, int? timeOut = null)
        {
            return DBHelper.ExecuteCommand(this.connectionString, safeSql, param, cmdType, transaction, timeOut);
        }

        protected T GetScalar<T>(string safeSql, CommandType cmdType = CommandType.Text, IDbTransaction transaction = null, int? timeOut = null) where T : IConvertible
        {
            return GetScalar<T>(safeSql, null, cmdType, transaction, timeOut);
        }
        protected T GetScalar<T>(string safeSql, object param, CommandType cmdType = CommandType.Text, IDbTransaction transaction = null, int? timeOut = null) where T : IConvertible
        {
            return DBHelper.GetScalar<T>(this.connectionString, safeSql, param, cmdType, transaction, timeOut);            
            
        }

        protected IDataReader GetReader(string safeSql, CommandType cmdType = CommandType.Text, IDbTransaction transaction = null, int? timeOut = null)
        {
            return GetReader(safeSql, null, cmdType, transaction, timeOut);
        }

        protected IDataReader GetReader(string safeSql, object param, CommandType cmdType = CommandType.Text, IDbTransaction transaction = null, int? timeOut = null)
        {
            return DBHelper.GetReader(this.connectionString, safeSql, param, cmdType, transaction, timeOut);
        }

        protected DataTable GetDataSet(string safeSql, CommandType cmdType = CommandType.Text, IDbTransaction transaction = null, int? timeOut = null)
        {
            return GetDataSet(safeSql, null, cmdType, transaction, timeOut);
        }

        protected DataTable GetDataSet(string safeSql, object param, CommandType cmdType = CommandType.Text, IDbTransaction transaction = null, int? timeOut = null)
        {
            return DBHelper.GetDataSet(this.connectionString, safeSql, param, cmdType, transaction, timeOut);
        }

        protected List<TResult> Query<TResult>(string safeSql, CommandType cmdType = CommandType.Text, IDbTransaction transaction = null, int? timeOut = null) where TResult : class,new()
        {
            return Query<TResult>(safeSql, null, cmdType, transaction, timeOut);
        }


        protected List<TResult> Query<TResult>(string safeSql, object param, CommandType cmdType = CommandType.Text, IDbTransaction transaction = null, int? timeOut = null) where TResult : class,new()
        {
            return DBHelper.Query<TResult>(connectionString,safeSql, param,cmdType, transaction ,  timeOut);
        }


        protected TResult Get<TResult>(string safeSql, CommandType cmdType = CommandType.Text, IDbTransaction transaction = null, int? timeOut = null) where TResult : class,new()
        {
            return Get<TResult>(safeSql, null, cmdType, transaction ,  timeOut);
        }
        protected TResult Get<TResult>(string safeSql, object param, CommandType cmdType = CommandType.Text,IDbTransaction transaction = null, int? timeOut=null) where TResult : class,new()
        {
            return Query<TResult>(safeSql,param,cmdType, transaction ,  timeOut).FirstOrDefault();
        }

      
    }
}
