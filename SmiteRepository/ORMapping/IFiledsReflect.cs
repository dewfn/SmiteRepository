using SmiteRepository.ORMapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;


namespace SmiteRepository.ORMapping
{
    internal interface IFiledsReflect
    {

         void InitAction(PropertyInfo p);

         object GetValue(object instance);
         void SetValue(object instance, object value);
         
    }
}
