using SmiteRepository.ORMapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;


namespace SmiteRepository.ORMapping
{
    public class PropertyManager<TEntiy> : IPropertyManager
    {
        public PropertyManager()
        { 
                foreach (var c in EntityReflect.GetDefineInfoFromType(typeof(TEntiy)).Columns)
                {
                    var p = typeof(TEntiy).GetProperty(c.FieldName);
                    IFiledsReflect fr = Activator.CreateInstance(typeof(FiledsReflect<,>).MakeGenericType(typeof(TEntiy),  p.PropertyType)) as IFiledsReflect;
                    fr.InitAction(p);
                    dic.Add(c.FieldName, fr);
                }
            
        }
        private Dictionary<string, IFiledsReflect> dic = new Dictionary<string, IFiledsReflect>();


        private IFiledsReflect GetSetValueAction(string memberName)
        {

            return dic[memberName];
        }

        public object GetValue(object instance, string memberName)
        {
           return dic[memberName].GetValue(instance);
        
        }
        public void SetValue(object instance, string memberName, object value)
        {
           dic[memberName].SetValue(instance, value);
        }
    }
}
