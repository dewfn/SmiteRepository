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


namespace SmiteRepository.ORMapping
{
    /// <summary>
    /// øÿ÷∆∑¥…‰”√
    /// </summary>
    internal class SqlMapperOne
    {
        private static SqlMapperOne mapper;
        private readonly static object _lockthis = new object();
        private readonly static object _lockdic = new object();
        private ReaderWriterLock rwLock = new ReaderWriterLock();
        private SqlMapperOne()
        {

        }
        public static SqlMapperOne getInstance()
        {
            if (mapper == null)
            {
                lock (_lockthis)
                {
                    if (mapper == null)
                        mapper = new SqlMapperOne();
                }
            }
            return mapper;
        }

        Dictionary<Identity, IFiledsReflect> dic = new Dictionary<Identity, IFiledsReflect>();
        public IFiledsReflect ValueViaEmit<TEntiy>(string memberName)
        {

            IFiledsReflect fr;
            Identity id = new Identity(typeof(TEntiy), memberName);
            lock (_lockdic)
            {

                if (dic.TryGetValue(id, out fr))
                {
                    return fr;
                }
                else
                {

                    PropertyInfo pr = typeof(TEntiy).GetProperty(memberName);
                    fr = Activator.CreateInstance(typeof(FiledsReflect<,>).MakeGenericType(typeof(TEntiy), pr.PropertyType)) as IFiledsReflect;
                    fr.InitAction(pr);


                    dic.Add(id, fr);
                    return fr;
                }
            }
        }

        public void SetPropertyValueViaEmit<TEntiy>(TEntiy entiy, object value, string memberName)
        {

            IFiledsReflect fr;
            Identity id = new Identity(entiy.GetType(), memberName);
            lock (_lockdic) { 
         
            if (dic.TryGetValue(id, out fr))
            {
                fr.SetValue(entiy, value);
            }
            else
            {

                PropertyInfo pr = entiy.GetType().GetProperty(memberName);
                fr = Activator.CreateInstance(typeof(FiledsReflect<,>).MakeGenericType(typeof(TEntiy), pr.PropertyType)) as IFiledsReflect;
                fr.InitAction(pr);


                dic.Add(id, fr);
                fr.SetValue(entiy, value);
            }
        }
        }
        public object GetPropertyValueViaEmit<TEntiy>(TEntiy entiy, string memberName)
        {
            IFiledsReflect fr;
            Identity id = new Identity(entiy.GetType(), memberName);

            lock (_lockdic)
            {
                if (dic.TryGetValue(id, out fr))
                {
                    return fr.GetValue(entiy);
                }
                else
                {

                    PropertyInfo pr = entiy.GetType().GetProperty(memberName);
                    fr = Activator.CreateInstance(typeof(FiledsReflect<,>).MakeGenericType(typeof(TEntiy), pr.PropertyType)) as IFiledsReflect;
                    fr.InitAction(pr);

                    dic.Add(id, fr);
                    return fr.GetValue(entiy);
                }

            }
            
        }



    }

}