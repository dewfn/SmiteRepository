using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmiteRepository.ORMapping
{
    internal interface IPropertyManager
    {
        object GetValue(object instance, string memberName);
        void SetValue(object  instance, string memberName, object value);
    }
}
