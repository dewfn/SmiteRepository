using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OracleClient;
using System.Linq;
using System.Text;
using SmiteRepository.Dapper;

namespace SmiteRepository.Oracle
{
    public class OracleDBHelper
        {

            private static OracleConnection GetConnection(string connectionString)
            {
                var connection = new OracleConnection(connectionString);
                if (connection.State != ConnectionState.Open)
                    connection.Open();
                return connection;
            }


            public static int ExecuteCommand(string connectionString, string sql, object values, CommandType cmdType = CommandType.Text)
            {
                using (var connection = GetConnection(connectionString))
                {
                    return connection.Execute(sql, values, null, null, cmdType);
                }
            }

            public static T GetScalar<T>(string connectionString, string sql, object values, CommandType cmdType = CommandType.Text)
            {
                using (var connection = GetConnection(connectionString))
                {
                    return connection.ExecuteScalar<T>(sql, values, null, null, cmdType);
                }
            }

            public static List<T> Query<T>(string connectionString, string sql, object values, CommandType cmdType = CommandType.Text)
            {
                using (var connection = GetConnection(connectionString))
                {
                    return connection.Query<T>(sql, values, null, true, null, cmdType).ToList();
                }
            }
            public static IDataReader GetReader(string connectionString, string sql, object values, CommandType cmdType = CommandType.Text)
            {
                var connection = GetConnection(connectionString);
                return connection.ExecuteReader(sql, values);
            }

            public static DataTable GetDataSet(string connectionString, string sql, object values, CommandType cmdType = CommandType.Text)
            {

                DataTable table = new DataTable();
                var connection = GetConnection(connectionString);

                using (var reader = connection.ExecuteReader(sql, values))
                {
                    table.Load(reader);
                }
                return table;
            }
         


        }  
    
}
