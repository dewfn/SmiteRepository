using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;


namespace SmiteRepository.Sqlserver
{
        public class DBHelper
        {

            //带参数的执行命令  
            public static int ExecuteCommand(string connectionString, string sql,object values, CommandType cmdType = CommandType.Text)
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = PrepareCommand(connection, cmdType, sql, values);
                    return cmd.ExecuteNonQuery();
                }
            }

            public static object GetScalar(string connectionString, string sql,object values, CommandType cmdType = CommandType.Text)
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = PrepareCommand(connection, cmdType, sql, values);
                    return cmd.ExecuteScalar();
                }
            }



            public static IDataReader GetReader(string connectionString, string sql,object values, CommandType cmdType = CommandType.Text)
            {
                //using ()
                //{
                SqlConnection connection = new SqlConnection(connectionString);
                    SqlCommand cmd = PrepareCommand(connection, cmdType, sql, values);
                    return cmd.ExecuteReader(CommandBehavior.CloseConnection);
               // }
               
            }


            public static DataTable GetDataSet(string connectionString, string sql,object values, CommandType cmdType = CommandType.Text)
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    DataSet ds = new DataSet();
                    SqlCommand cmd = PrepareCommand(connection, cmdType, sql, values);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(ds);
                    return ds.Tables[0];
                }
            }
            private static SqlCommand PrepareCommand(SqlConnection conn, CommandType cmdType, string cmdText, object values)
            {
                SqlCommand cmd = new SqlCommand();
               
                  
                cmd.Connection = conn;
                cmd.CommandText = cmdText;
                cmd.CommandType = cmdType;
                cmd.CommandTimeout = 5000;
                if (values != null)
                {
                    cmd.Parameters.AddRange(GetDataParameter(values));
                }

                if (conn.State != ConnectionState.Open)
                    conn.Open();
                return cmd;
            }

            private static SqlParameter[] GetDataParameter(object values)
            {
                if (values is DataParameter)
                {
                    DataParameter dataP = values as DataParameter;
                    return new SqlParameter[]{
                        new SqlParameter(dataP.ParameterName,dataP.Value)
                    };
                }
                if (values is DataParameter[])
                {
                    return (values as DataParameter[]).Select<DataParameter, SqlParameter>(s => new SqlParameter(s.ParameterName, s.Value)).ToArray();
                }
                return values.GetType().GetProperties().Select<System.Reflection.PropertyInfo, SqlParameter>(x =>
                    new SqlParameter("@"+x.Name, x.GetValue(values, null))).ToArray();
            }
        }  
    
}
