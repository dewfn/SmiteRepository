/*
 License: http://www.apache.org/licenses/LICENSE-2.0 
 Home page: http://code.google.com/p/dapper-dot-net/

 Note: to build on C# 3.0 + .NET 3.5, include the CSHARP30 compiler symbol (and yes,
 I know the difference between language and runtime versions; this is a compromise).
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading;
using System.Text.RegularExpressions;


namespace SmiteRepository.ORMapping{
/// <summary>
/// Dapper, a light weight object mapper for ADO.NET
/// </summary>
internal  class SqlMapper
{
    private static SqlMapper mapper;
    private readonly static object _lockthis = new object();
    private readonly  object _lockdic = new object();
    private SqlMapper() { 
     
    }
    public static SqlMapper getInstance() {
        if (mapper == null)
        {
            lock (_lockthis)
            {
                if (mapper == null)
                    mapper = new SqlMapper();
            }
        }
        return mapper;
    }
   Dictionary<Type, IPropertyManager> dic = new Dictionary<Type, IPropertyManager>();

   public IPropertyManager ValueViaEmit(Type type)
   {

       IPropertyManager pr;
      
       lock (_lockdic)
       {
           if (dic.TryGetValue(type, out pr))
           {
               return pr;
           }
           else
           {

               pr = Activator.CreateInstance(typeof(PropertyManager<>).MakeGenericType(type)) as IPropertyManager;
               dic.Add(type, pr);
               return pr;
           }
       }
   }

   public void SetPropertyValueViaEmit<TEntiy>(TEntiy entiy, object value, string memberName)
    {

        IPropertyManager pr;

        lock (_lockdic)
        {

            if (dic.TryGetValue(entiy.GetType(), out pr))
            {
                pr.SetValue(entiy, memberName, value);
            }
            else
            {

                pr = Activator.CreateInstance(typeof(PropertyManager<>).MakeGenericType(entiy.GetType())) as IPropertyManager;

                dic.Add(entiy.GetType(), pr);
                pr.SetValue(entiy, memberName, value);
            }
        }


    }
   public object GetPropertyValueViaEmit<TEntiy>(TEntiy entiy, string memberName)
  {
      IPropertyManager pr;

      lock (_lockdic)
      {

          if (dic.TryGetValue(entiy.GetType(), out pr))
          {
              return pr.GetValue(entiy, memberName);
          }
          else
          {

              pr = Activator.CreateInstance(typeof(PropertyManager<>).MakeGenericType(entiy.GetType())) as IPropertyManager;

              dic.Add(entiy.GetType(), pr);
              return pr.GetValue(entiy, memberName);
          }
      }
  }



}

}