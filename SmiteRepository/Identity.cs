using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace SmiteRepository
{

    internal class Identity : IEquatable<Identity>
    {
        private Type _entityType;
        private string _filedName;
        public readonly int hashCode;
        internal Identity(Type entityType,string filedName)
        {
            this._entityType = entityType;
            this._filedName = filedName;
           
            unchecked
            {
                hashCode = 17;
                hashCode = hashCode * 23 + (_filedName == null ? 0 : _filedName.GetHashCode());
                hashCode = hashCode * 23 + (_entityType == null ? 0 : _entityType.GetHashCode());              
            }
        }
        public override int GetHashCode()
        {
            return hashCode;
        }
        public bool Equals(Identity other)
        {
            return
               hashCode == other.hashCode;
        }
        public override bool Equals(object obj)
        {
            return Equals(obj as Identity);
        }
         
    }
}
