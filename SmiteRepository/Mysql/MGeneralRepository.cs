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


namespace SmiteRepository.Mysql
{
    /// <summary>
    /// 通用的DB实现类，可做为通用类，不继承BaseRepository使用
    /// </summary>
    public class MGeneralRepository : BaseMysqlRepository
    {
        private string connectionString;
        public MGeneralRepository(string ConnectionString)
            : base(ConnectionString)
        {
            this.connectionString = ConnectionString;
        }

        public PagedList<T> PageGet<T>(PageView view, string sql,object param) where T : class,new()
        {
            return base.PageGet<T>(view,sql,param);
        }

        public  PagedList<T> PageGet<T>(PageView view, string sql) where T : class,new()
        {
            return base.PageGet<T>(view, sql);            
        }
  
        
        public int ExecuteCommand(string safeSql, CommandType cmdType = CommandType.Text)
        {
            return base.ExecuteCommand(safeSql, cmdType);      

        }
        //带参数的执行命令  
        public int ExecuteCommand(string safeSql,object param, CommandType cmdType = CommandType.Text)
        {
            return base.ExecuteCommand( safeSql, param,cmdType);
        }

        public T GetScalar<T>(string safeSql, CommandType cmdType = CommandType.Text) where T : IConvertible
        {
            return base.GetScalar<T>(safeSql, cmdType);
        }
        public T GetScalar<T>(string safeSql,object param, CommandType cmdType = CommandType.Text) where T : IConvertible
        {
            return base.GetScalar<T>(safeSql, param, cmdType);
        }

        public IDataReader GetReader(string safeSql, CommandType cmdType = CommandType.Text)
        {
            return base.GetReader(safeSql,cmdType);
        }

        public IDataReader GetReader(string safeSql,object param, CommandType cmdType = CommandType.Text)
        {
            return base.GetReader(safeSql,param, cmdType);
        }

        public  DataTable GetDataSet( string safeSql,CommandType cmdType=CommandType.Text)
        {
            return base.GetDataSet(safeSql, cmdType);
        }

        public DataTable GetDataSet(string safeSql,object param, CommandType cmdType = CommandType.Text)
        {
            return base.GetDataSet(safeSql,param, cmdType);
        }



        public List<TResult> Query<TResult>(string safeSql,CommandType cmdType=CommandType.Text) where TResult : class,new()
        {
            return base.Query<TResult>(safeSql,cmdType);
        }


        public List<TResult> Query<TResult>(string safeSql,object param, CommandType cmdType = CommandType.Text) where TResult : class,new()
        {
            return base.Query<TResult>(safeSql,param,cmdType);
        }

        public TResult Get<TResult>(string safeSql, CommandType cmdType = CommandType.Text) where TResult : class,new()
        {
            return base.Get<TResult>(safeSql, cmdType);
        }
        public TResult Get<TResult>(string safeSql,object param, CommandType cmdType = CommandType.Text) where TResult : class,new()
        {
            return base.Get<TResult>(safeSql,param, cmdType);
        }
    }
}
