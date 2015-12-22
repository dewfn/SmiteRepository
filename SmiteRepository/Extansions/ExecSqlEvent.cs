using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmiteRepository.Extansions
{
   public delegate string ExecHandleDelegate(string Sql, object Params);
    /// <summary>
   /// 注册所有SQL执行前事件，不支持线程安全，所以启动时注册
    /// </summary>
   public class ExecSqlHandle
   {
       /// <summary>
       /// 程序内所有执行SQL会回执的方法。此方法会堵塞SQL执行 ，请小心使用
       /// </summary>
       public static ExecHandleDelegate CallbackExecSqlMethod;
       public static bool IsAsyncCallback = false;
       public static bool IsRegisterCall = false;
       /// <summary>
       /// 开发人员调用使用
       /// </summary>
       /// <param name="Sql"></param>
       /// <param name="Params"></param>
       /// <returns></returns>
       public static string ExecHandleDelegate(string Sql, object Params)
       {
           if (CallbackExecSqlMethod != null)
           {
               if (IsAsyncCallback)
               {
                   CallbackExecSqlMethod.BeginInvoke(Sql, Params, null, null);
               }
               else
               {
                   Sql = CallbackExecSqlMethod(Sql, Params);
               }
           }
           return Sql;
      }

       public static bool RegisterExecHandle(ExecHandleDelegate Method, bool IsAsync=false) 
       {
           if (IsRegisterCall)//已注册过
               return false;
           CallbackExecSqlMethod = Method;
           IsRegisterCall = true;
           IsAsyncCallback = IsAsync;
           return true;
       }
      
   }
}
