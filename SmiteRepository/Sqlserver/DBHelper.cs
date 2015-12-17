using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using SmiteRepository.Dapper;

namespace SmiteRepository.Sqlserver
{
        public class DBHelper
        {
            private static SqlConnection GetConnection(string connectionString)
            {
                var connection = new SqlConnection(connectionString);
                if (connection.State != ConnectionState.Open)
                    connection.Open();
                return connection;
            }


            public static int ExecuteCommand(string connectionString, string sql, object values, CommandType cmdType = CommandType.Text, IDbTransaction transaction = null, int? timeOut=null)
            {
                using (SqlConnection connection = GetConnection(connectionString))
                {
                    return connection.Execute(sql, values, transaction, timeOut, cmdType);
                }
            }

            public static T GetScalar<T>(string connectionString, string sql, object values, CommandType cmdType = CommandType.Text, IDbTransaction transaction = null,int? timeOut=null)
            {
                using (SqlConnection connection =GetConnection(connectionString))
                {
                    return connection.ExecuteScalar<T>(sql, values, transaction, timeOut, cmdType);
                }
            }

            public static List<T> Query<T>(string connectionString, string sql, object values, CommandType cmdType = CommandType.Text, IDbTransaction transaction = null, int? timeOut=null)
            {
                using (SqlConnection connection = GetConnection(connectionString))
                {
                    return connection.Query<T>(sql, values, transaction, true, timeOut, cmdType).ToList();
                }
            }
            public static IDataReader GetReader(string connectionString, string sql, object values, CommandType cmdType = CommandType.Text, IDbTransaction transaction = null, int? timeOut=null)
            {
                SqlConnection connection = GetConnection(connectionString);
                return connection.ExecuteReader(sql, values, transaction, timeOut, cmdType);
            }

            public static DataTable GetDataSet(string connectionString, string sql, object values, CommandType cmdType = CommandType.Text, IDbTransaction transaction = null, int? timeOut=null)
            {
               
               DataTable table = new DataTable();
               SqlConnection connection = GetConnection(connectionString);
            
               using (var reader = connection.ExecuteReader(sql, values,transaction,timeOut,cmdType))
                 {
                     table.Load(reader);
                 }
               return table;
            }


            //private static SqlParameter[] GetDataParameter(object values)
            //{
            //    if (values is DataParameter)
            //    {
            //        DataParameter dataP = values as DataParameter;
            //        return new SqlParameter[]{
            //            new SqlParameter(dataP.ParameterName,dataP.Value)
            //        };
            //    }
            //    if (values is DataParameter[])
            //    {
            //        return (values as DataParameter[]).Select<DataParameter, SqlParameter>(s => new SqlParameter(s.ParameterName, s.Value)).ToArray();
            //    }
            //    return values.GetType().GetProperties().Select<System.Reflection.PropertyInfo, SqlParameter>(x =>
            //        new SqlParameter("@"+x.Name, x.GetValue(values, null))).ToArray();
            //}
        }  
    
}
