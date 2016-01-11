using SmiteRepository.ORMapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace SmiteRepository
{
    /// <summary>
    /// Safety编译的支持线程安全
    /// </summary>
    public abstract class BaseEntity
    {
        public BaseEntity() { }
        //private static Dictionary<Type, string> _InsertSqlCache = new Dictionary<Type, string>();
        //private static Dictionary<Type, string> _UpdateSqlCache = new Dictionary<Type, string>();
        //private static Dictionary<Type, string> _ReplaceSqlCache = new Dictionary<Type, string>();

#if Safety
        public readonly object lockobject = new object();
#endif

        private List<string> _PropertyChangedList = new List<string>();

        internal List<string> PropertyChangedList
        {
            get { return _PropertyChangedList; }
        }

        #region 属性
        [Ignore]
        internal bool FullUpdate { get; set; }
        #endregion
        #region 公开方法
        //internal string GetInsertSQL()
        //{
        //    //if (FullUpdate)
        //    //{
        //    //    return GetInsertFullSql();
        //    //}
        //    return GetInsertChangeColumnsSql();
        //}

        //internal string GetUpdateSQL(bool IsHasWhere)
        //{
        //    //if (FullUpdate)
        //    //{
        //    //    return GetUpdateFullSql();
        //    //}
        //    return GetUpdateChangeColumnsSql(IsHasWhere);
        //}
        //private string GetInsertChangeColumnsSql(Func<EntityMeta,List<string>,string> InsertFunc)
        //{

        //    EntityMeta metadeta = EntityReflect.GetDefineInfoFromType(this.GetType());
        //    return EntitySQLBuilder.BuildInsertSql(metadeta, this._PropertyChangedList);
        //}

        //private string GetUpdateChangeColumnsSql(bool HasWhere)
        //{
        //    EntityMeta metadeta = EntityReflect.GetDefineInfoFromType(this.GetType());
        //    return EntitySQLBuilder.BuildUpdateSql(metadeta, this._PropertyChangedList, HasWhere);
        //}

        internal void RemoveUpdateColumn(string pName)
        {
#if Safety
            lock (lockobject)  
            {
#endif
            if (_PropertyChangedList.Contains(pName))
            {
                _PropertyChangedList.Remove(pName);
            }
#if Safety
            }
#endif
        }
        #endregion

        internal void RemoveUpdateColumn()
        {
#if Safety
            lock (lockobject)  
            {
#endif
            _PropertyChangedList.Clear();
#if Safety
            }
#endif
        }

        protected void OnPropertyChanged(string pName)
        {
#if Safety
            lock (lockobject)
            {
#endif
            if (!_PropertyChangedList.Contains(pName))
            {
                _PropertyChangedList.Add(pName);
            }
#if Safety
             }
#endif
        }

        //internal string GetReplaceInsertSQL()
        //{
        //    Type t = this.GetType();
        //    if (!_ReplaceSqlCache.ContainsKey(t))
        //    {
        //        EntityMeta metadeta = EntityReflect.GetDefineInfoFromType(t);
        //        string sql = EntitySQLBuilder.EntityMetaToReplaceInsertSQL(metadeta);
        //        lock (lockobject)
        //        {
        //            if (!_ReplaceSqlCache.ContainsKey(t))
        //            {
        //                _ReplaceSqlCache.Add(t, sql);
        //            }
        //        }

        //    }
        //    return _ReplaceSqlCache[t];
        //}

        #region 私有方法
        //private string GetInsertFullSql()
        //{
        //    Type t = this.GetType();
        //    if (!_InsertSqlCache.ContainsKey(t))
        //    {
        //        EntityMeta metadeta = EntityReflect.GetDefineInfoFromType(t);
        //        string sql = EntitySQLBuilder.EntityMetaToInsertSQL(metadeta);
        //        lock (lockobject)
        //        {
        //            if (!_InsertSqlCache.ContainsKey(t))
        //            {
        //                _InsertSqlCache.Add(t, sql);
        //            }
        //        }

        //    }
        //    return _InsertSqlCache[t];
        //}
        //private string GetUpdateFullSql()
        //{
        //    Type t = this.GetType();
        //    if (!_UpdateSqlCache.ContainsKey(t))
        //    {
        //        EntityMeta metadeta = EntityReflect.GetDefineInfoFromType(t);
        //        string sql = EntitySQLBuilder.EntityMetaToUpdateSQL(metadeta);
        //        lock (lockobject)
        //        {
        //            if (!_UpdateSqlCache.ContainsKey(t))
        //            {
        //                _UpdateSqlCache.Add(t, sql);
        //            }
        //        }

        //    }
        //    return _UpdateSqlCache[t];
        //}

        #endregion
    }
}
