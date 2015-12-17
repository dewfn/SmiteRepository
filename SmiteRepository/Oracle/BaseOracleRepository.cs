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
namespace SmiteRepository.Oracle
{
    /// <summary>
    /// DB层基类，需要继承实现
    /// </summary>
    public abstract class BaseOracleRepository
    {
        private string connectionString;
        /// <summary>
        /// 传入连接字符串
        /// </summary>
        /// <param name="ConnectionString">连接字符串</param>
        public BaseOracleRepository(string ConnectionString)
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

            ORMTEntity rep;
            lock (_lockdic)
            {
                if (dicORM.TryGetValue(typeof(TEntity), out rep))
                {
                    return rep.ORMrepository<TEntity>();
                }
                else
                {
                    IORMRepository<TEntity> rrmRe = new ORMOracleRepository<TEntity>(this.connectionString);
                    dicORM.Add(typeof(TEntity), new ORMTEntity(rrmRe));
                    return rrmRe;
                }

            }
        }
        /// <summary>
        /// 通过SQL得到分页列表
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="view">分页，一般通过页面传入</param>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        protected PagedList<T> PageGet<T>(PageView view, string sql, object param) where T : class,new()
        {
            string sqlOrder = view.GetSqlOrder();

            PagedList<T> pList = new PagedList<T>();

            if (view.Total == 0)
                pList.Total = this.GetScalar<int>(EntityORASQLBuilder.BuildPageCountSql(sql), param);
            else
                pList.Total = view.Total;

            if (pList.Total > 0)
            {
                //object param2 = null;
                // if (param != null)
                // {
                //     param2 = param.Select(x => new DataParameter(x.ParameterName, x.Value)).ToArray();
                // }

                var list = this.Query<T>(EntityORASQLBuilder.BuildPageSql(sql, view), param);
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
        protected PagedList<T> PageGet<T>(PageView view, string sql) where T : class,new()
        {
            return PageGet<T>(view, sql, null);
        }

        /// <summary>
        /// 执行一条SQL语句或存储过程
        /// </summary>
        /// <param name="safeSql">语句或过程</param>
        /// <param name="cmdType">SQL类型，默认为SQL语句</param>
        /// <returns></returns>
        protected int ExecuteCommand(string safeSql, CommandType cmdType = CommandType.Text)
        {
            return ExecuteCommand(safeSql, null, cmdType);

        }
        //带参数的执行命令  
        protected int ExecuteCommand(string safeSql, object param, CommandType cmdType = CommandType.Text)
        {
            return OracleDBHelper.ExecuteCommand(this.connectionString, safeSql, param, cmdType);
        }

        protected T GetScalar<T>(string safeSql, CommandType cmdType = CommandType.Text) where T : IConvertible
        {
            return GetScalar<T>(safeSql, null, cmdType);
        }
        protected T GetScalar<T>(string safeSql, object param, CommandType cmdType = CommandType.Text) where T : IConvertible
        {
            return OracleDBHelper.GetScalar<T>(this.connectionString, safeSql, param, cmdType);

        }

        protected IDataReader GetReader(string safeSql, CommandType cmdType = CommandType.Text)
        {
            return GetReader(safeSql, null, cmdType);
        }

        protected IDataReader GetReader(string safeSql, object param, CommandType cmdType = CommandType.Text)
        {
            return OracleDBHelper.GetReader(this.connectionString, safeSql, param, cmdType);
        }

        protected DataTable GetDataSet(string safeSql, CommandType cmdType = CommandType.Text)
        {
            return GetDataSet(safeSql, null, cmdType);
        }

        protected DataTable GetDataSet(string safeSql, object param, CommandType cmdType = CommandType.Text)
        {
            return OracleDBHelper.GetDataSet(this.connectionString, safeSql, param, cmdType);
        }

        protected List<TResult> Query<TResult>(string safeSql, CommandType cmdType = CommandType.Text) where TResult : class,new()
        {
            return Query<TResult>(safeSql, null, cmdType);
        }


        protected List<TResult> Query<TResult>(string safeSql, object param, CommandType cmdType = CommandType.Text) where TResult : class,new()
        {
            return OracleDBHelper.Query<TResult>(connectionString, safeSql, param, cmdType);
        }


        protected TResult Get<TResult>(string safeSql, CommandType cmdType = CommandType.Text) where TResult : class,new()
        {
            return Get<TResult>(safeSql, null, cmdType);
        }
        protected TResult Get<TResult>(string safeSql, object param, CommandType cmdType = CommandType.Text) where TResult : class,new()
        {
            return Query<TResult>(safeSql, param, cmdType).FirstOrDefault();
        }
    }
}
