
using SmiteRepository.Page;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
namespace SmiteRepository
{

    internal struct ORMTEntity// where TEntity : BaseEntity, new()
    {
        public object _ORMrepository;
        public ORMTEntity(object ORMrepository)
        { this._ORMrepository = ORMrepository; }
        public IORMRepository<TEntity> ORMrepository<TEntity>() where TEntity : BaseEntity, new()
        {
            return (IORMRepository<TEntity>)_ORMrepository;
        }
    }
    public interface IORMRepository<TEntity>
     where TEntity : BaseEntity, new()
    {
        /// <summary>
        /// 求某字段平均值
        /// </summary>
        /// <param name="fields">要显示和字段,必需是数字类型</param>
        /// <returns></returns>
       TResult Avg<TResult>(Expression<Func<Fileds.DisplayMethod, TEntity, Fileds>> fields) where TResult:IConvertible;
        /// <summary>
        /// 求某字段平均值
        /// </summary>
        /// <param name="fields">要显示和字段，必需是数字类型</param>
        /// <param name="where">过滤条件</param>
        /// <returns></returns>
       TResult Avg<TResult>(Expression<Func<Fileds.DisplayMethod, TEntity, Fileds>> fields, Expression<Predicate<TEntity>> where) where TResult:IConvertible;
        /// <summary>
        /// 通过条件删除数据
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        int Delete(Expression<Predicate<TEntity>> where);
        /// <summary>
        /// 根据条件查找一个符合条件的数据
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        TEntity Find(Expression<Predicate<TEntity>> where);
        /// <summary>
        /// 根据条件查找一个符合条件的数据
        /// </summary>
        /// <param name="where"></param>
        /// <param name="fields">可设置需要显示的字段</param>
        /// <returns></returns>
        TEntity Find(Expression<Predicate<TEntity>> where, Expression<Func<Fileds.DisplaysMethod, TEntity, Fileds>> fields);
        /// <summary>
        /// 根据条件查找一个符合条件的数据
        /// </summary>
        /// <param name="where"></param>
        /// <param name="fields">可设置需要显示的字段</param>
        /// <returns></returns>
        TEntity Find(Expression<Predicate<TEntity>> where, Expression<Func<Fileds.DisplaysMethod, TEntity, Fileds>> fields, Expression<Func<Fileds.OrderByMethod, TEntity, Fileds>> orderFields);
        /// <summary>
        /// 根据条件查找符合条件的数据集
        /// </summary>
        /// <returns></returns>
        List<TEntity> FindAll();
        /// <summary>
        /// 根据条件查找符合条件的数据集
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        List<TEntity> FindAll(Expression<Predicate<TEntity>> where);
        /// <summary>
        /// 根据条件查找符合条件的数据集
        /// </summary>
        /// <param name="fields"></param>
        /// <returns></returns>
        //List<TEntity> FindAll(Expression<Func<Fileds.DisplaysMethod, TEntity, Fileds>> fields);        
        /// <summary>
        /// 根据条件查找符合条件的数据集
        /// </summary>
        /// <param name="where"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        List<TEntity> FindAll(Expression<Predicate<TEntity>> where, Expression<Func<Fileds.DisplaysMethod, TEntity, Fileds>> fields);
        /// <summary>
        /// 分页查寻用
        /// </summary>
        /// <param name="pView">分页数据</param>
        /// <param name="where">查寻条件</param>
        /// <param name="fields">要显示的条件</param>
        /// <returns></returns>
        PagedList<TEntity> GetPage(PageView pView, Expression<Predicate<TEntity>> where = null, Expression<Func<Fileds.DisplaysMethod, TEntity, Fileds>> fields = null);
      
        /// <summary>
        /// 把实体插入到数据库
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        long Add(TEntity entity);
        /// <summary>
        /// 求某字段最大值
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="fields"></param>
        /// <returns></returns>
        TResult Max<TResult>(Expression<Func<Fileds.DisplayMethod<TResult>, TEntity, Fileds>> fields) ;
        /// <summary>
        /// 求符合条件的，某字段最大值
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="fields"></param>
        /// <param name="where"></param>
        /// <returns></returns>
        TResult Max<TResult>(Expression<Func<Fileds.DisplayMethod<TResult>, TEntity, Fileds>> fields, Expression<Predicate<TEntity>> where);
        /// <summary>
        /// 求某字段最小值
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="fields"></param>
        /// <returns></returns>
        TResult Min<TResult>(Expression<Func<Fileds.DisplayMethod<TResult>, TEntity, Fileds>> fields);
        TResult Min<TResult>(Expression<Func<Fileds.DisplayMethod<TResult>, TEntity, Fileds>> fields, Expression<Predicate<TEntity>> where);
        TResult Scalar<TResult>(Expression<Func<Fileds.DisplayMethod<TResult>, TEntity, Fileds>> fields);
        TResult Scalar<TResult>(Expression<Func<Fileds.DisplayMethod<TResult>, TEntity, Fileds>> fields, Expression<Predicate<TEntity>> where);
        TResult Sum<TResult>(Expression<Func<Fileds.DisplayMethod, TEntity, Fileds>> fields) where TResult:IConvertible;
        TResult Sum<TResult>(Expression<Func<Fileds.DisplayMethod, TEntity, Fileds>> fields, Expression<Predicate<TEntity>> where) where TResult : IConvertible;

        int Count(Expression<Predicate<TEntity>> where);
        int Count();

        /// <summary>
        /// 判断是否存在此条件数据
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        bool Exists(Expression<Predicate<TEntity>> where);
        /// <summary>
        /// 是否存在数据
        /// </summary>
        /// <returns></returns>
        bool Exists();
        int Update(TEntity entity);
        /// <summary>
        /// 根据条件更新实体
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="where"></param>
        /// <returns></returns>
        int Update(TEntity entity, Expression<Predicate<TEntity>> where);

        string ConnectionString { set;}
         
    }
}
