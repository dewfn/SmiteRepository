using SmiteRepository.Command;
using SmiteRepository.ORMapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;


namespace SmiteRepository.Extansions
{
    /// <summary>
    /// 启动时注册，不支持线程安全
    /// </summary>
    public static class RegisterORM
    {
        public static void Register_CustomTableNameToUpdate<TEntity>(Expression<Predicate<TEntity>> where, CustomTableNameDelegate<TEntity> Delegate)
        {
             string wheresql=string.Empty;
             EntityMeta meta = EntityReflect.GetDefineInfoFromType(typeof(TEntity));
             if (where != null) 
             { 
               ResolveExpress resolve=new ResolveExpress(where,meta);
               wheresql= " WHERE "+resolve.SqlWhere;
             }

             if (meta.DicCustomTableName_Update == null)
                 meta.DicCustomTableName_Update = new Dictionary<string, Delegate>();
            if (meta.DicCustomTableName_Update.ContainsKey(wheresql))
            {
                meta.DicCustomTableName_Update[wheresql] = Delegate;
            }
            else {
                meta.DicCustomTableName_Update.Add(wheresql, Delegate);
            }
            meta.IsCustomTableName |= true;
        }
        public static void Register_CustomTableNameToDelete<TEntity>(Expression<Predicate<TEntity>> where, CustomTableNameDelegate<TEntity> Delegate)
        {
            EntityMeta meta = EntityReflect.GetDefineInfoFromType(typeof(TEntity));
            ResolveExpress resolve = new ResolveExpress(where, meta);
            string wheresql = " WHERE " + resolve.SqlWhere;
            if (meta.DicCustomTableName_Delete == null)
                meta.DicCustomTableName_Delete = new Dictionary<string, Delegate>();
            if (meta.DicCustomTableName_Delete.ContainsKey(wheresql))
            {
                meta.DicCustomTableName_Delete[wheresql] = Delegate;
            }
            else
            {
                meta.DicCustomTableName_Delete.Add(wheresql, Delegate);
            } meta.IsCustomTableName |= true;
        }
        public static void Register_CustomTableNameToInsert<TEntity>(OneCustomTableNameDelegate<TEntity> Delegate)
        {
            EntityMeta meta = EntityReflect.GetDefineInfoFromType(typeof(TEntity));
            meta.ACustomTableName_Insert = Delegate;
            meta.IsCustomTableName |= true;
        }
        public static void Register_CustomTableNameToSelect<TEntity>(Expression<Predicate<TEntity>> where, CustomTableNameDelegate<TEntity> Delegate)
        {
            EntityMeta meta = EntityReflect.GetDefineInfoFromType(typeof(TEntity));
            ResolveExpress resolve = new ResolveExpress(where, meta);
            string wheresql = " WHERE " + resolve.SqlWhere;
            if (meta.DicCustomTableName_Select == null)
                meta.DicCustomTableName_Select = new Dictionary<string, Delegate>();
            if (meta.DicCustomTableName_Select.ContainsKey(wheresql))
            {
                meta.DicCustomTableName_Select[wheresql] = Delegate;
            }
            else
            {
                meta.DicCustomTableName_Select.Add(wheresql, Delegate);
            } meta.IsCustomTableName |= true;
        }
       
    }
}
