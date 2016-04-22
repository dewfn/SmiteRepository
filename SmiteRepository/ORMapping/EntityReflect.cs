using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
namespace SmiteRepository.ORMapping
{
    internal class EntityReflect
    {
        private static Dictionary<Type, EntityMeta> _cache = new Dictionary<Type, EntityMeta>();
        //private static readonly object lockobject = new object();
        private static ReaderWriterLock _rwlock = new ReaderWriterLock();
        public static EntityMeta GetDefineInfoFromType(Type type)
        {
            EntityMeta tdefine = null;
            string typeKey = type.FullName;



            _rwlock.AcquireReaderLock(100);

            if (_cache.TryGetValue(type, out tdefine))
            {
                _rwlock.ReleaseReaderLock();
                return tdefine;
            }

            _rwlock.ReleaseReaderLock();

            tdefine = new EntityMeta();

            tdefine.EntityType = type;
            TableNameAttribute tableAttribute = (TableNameAttribute)Attribute.GetCustomAttribute(type, typeof(TableNameAttribute));
            if (tableAttribute != null)
            {
                tdefine.TableName = tableAttribute.TableName;
            }
            else
            {
                tdefine.TableName = type.Name;
            }

            PropertyInfo[] pinfos = type.GetProperties();
            foreach (PropertyInfo p in pinfos)
            {
                Attribute[] attrs = Attribute.GetCustomAttributes(p);
                EntityColumnMeta ecmeta = new EntityColumnMeta();
                ecmeta.ColumnName = p.Name;
                ecmeta.FieldName = p.Name;

                foreach (Attribute cusattr in attrs)
                {
                    if (cusattr is IgnoreAttribute)
                    {
                        ecmeta = null;
                        break;
                    }
                    if (cusattr is PrimaryKeyAttribute)
                    {
                        ecmeta.PrimaryKey = true;
                    }
                    if (cusattr is MapFieldAttribute)
                    {
                        ecmeta.ColumnName = ((MapFieldAttribute)cusattr).MapFieldName;
                    }
                    if (cusattr is IdentityAttribute)
                    {
                        ecmeta.Identity = true;
                    }
                    if (cusattr is NullableAttribute)
                    {
                        ecmeta.Nullable = true;
                    }
                }
                if (ecmeta != null)
                {
                    tdefine.Columns.Add(ecmeta);

                }
            }

            _rwlock.AcquireWriterLock(100);
            if (!_cache.ContainsKey(type))
            {
                _cache.Add(type, tdefine);
            }
            _rwlock.ReleaseWriterLock();


            return tdefine;
        }
    }

    internal class EntityMeta
    {
        private Type entityType;

        public Type EntityType
        {
            get { return entityType; }
            set { entityType = value; }
        }
        public bool IsCustomTableName { set; get; }
        public string TableName { get; set; }
        private List<EntityColumnMeta> _Columns;
        public List<EntityColumnMeta> Columns
        {
            get
            {
                if (_Columns == null)
                {
                    _Columns = new List<EntityColumnMeta>();
                }
                return _Columns;
            }
        }
        private string _selectStringColumns;
        public string SelectStringColumns
        {
            get
            {
                if (_selectStringColumns == null)
                {
                    StringBuilder sb = new StringBuilder();
                    for (int i = 0, j = _Columns.Count(); i < j; i++)
                    {
                        if (i != 0)
                            sb.Append(",");
                        sb.Append(_Columns[i].ColumnName);

                    }
                    _selectStringColumns = sb.ToString();
                }
                return _selectStringColumns;
            }
        }
        public Dictionary<string, Delegate> DicCustomTableName_Select { get; set; }
        public Dictionary<string, Delegate> DicCustomTableName_Update { get; set; }
        public Delegate ACustomTableName_Insert { get; set; }
        public Dictionary<string, Delegate> DicCustomTableName_Delete { get; set; }
    }
    public delegate IEnumerable<string> CustomTableNameDelegate<TEntity>(object[] SqlParams, TEntity Entity);
    public delegate string OneCustomTableNameDelegate<TEntity>(TEntity Entity);

    internal class EntityColumnMeta
    {
        public string ColumnName { get; set; }
        public bool PrimaryKey { get; set; }
        public bool Nullable { get; set; }
        public bool Identity { get; set; }
        public string FieldName { get; set; }
    }

}

