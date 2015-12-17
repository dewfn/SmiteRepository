using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace SmiteRepository
{

    public class Fileds
    {
        /// <summary>
        /// 用于包括多个显示字段的处理
        /// </summary>
        /// <param name="files"></param>
        /// <returns></returns>
        public delegate Fileds DisplaysMethod(params IConvertible[] files);
        /// <summary>
        /// 用于包括显示字段的处理
        /// </summary>
        /// <param name="files"></param>
        /// <returns></returns>
        public delegate Fileds DisplayMethod<TResult>(TResult files);
        public delegate Fileds DisplayMethod(IConvertible files);
        public static Fileds Include(params IConvertible[] fileds) {
            return null;
        }
        public static Fileds Include<TResult>(TResult fileds)
        {
            return null;
        }
        public static Fileds Include(IConvertible fileds)
        {
            return null;
        }
    }
}
