using SmiteRepository.ORMapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;


namespace SmiteRepository.ORMapping
{
    /// <summary>
    /// 字段反射赋值用
    /// </summary>
    internal class FiledsReflect<TEntiy, TProperty> : IFiledsReflect
    {
       
        public FiledsReflect()
        {
           
        }
        private  Action<TEntiy, TProperty> SetValueDelegate;
        private  Func<TEntiy, TProperty> GetValueDelegate;
        public void InitAction(PropertyInfo property)
        {

            DynamicMethod method = new DynamicMethod("SetValueTemp", null, new Type[] { typeof(TEntiy), property.PropertyType }, true);
            ILGenerator ilGenerator = method.GetILGenerator();
            ilGenerator.Emit(OpCodes.Ldarg_0);
            ilGenerator.Emit(OpCodes.Ldarg_1);
            ilGenerator.EmitCall(OpCodes.Callvirt, property.GetSetMethod(), null);
            ilGenerator.Emit(OpCodes.Ret);

            method.DefineParameter(1, ParameterAttributes.In, "instance");
            method.DefineParameter(2, ParameterAttributes.In, "value");
            
            SetValueDelegate= (Action<TEntiy, TProperty>)method.CreateDelegate(typeof(Action<TEntiy, TProperty>));

            
                DynamicMethod method2 = new DynamicMethod("GetValueTemp", property.PropertyType, new Type[] { typeof(TEntiy) }, true);
                ILGenerator ilGenerator2 = method2.GetILGenerator();
                ilGenerator2.Emit(OpCodes.Ldarg_0);
                ilGenerator2.EmitCall(OpCodes.Callvirt, property.GetGetMethod(), null);
                ilGenerator2.Emit(OpCodes.Ret);

                method2.DefineParameter(1, ParameterAttributes.In, "instance");
                GetValueDelegate = (Func<TEntiy, TProperty>)method2.CreateDelegate(typeof(Func<TEntiy, TProperty>));
      
        }


        public object GetValue(object instance)
        {

            return GetValueDelegate((TEntiy)instance);
        }
        public void SetValue(object instance, object value)
        { 
            SetValueDelegate((TEntiy)instance, (TProperty)value);
        }
 
    }
}
