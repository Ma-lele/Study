using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Model;

namespace XHS.Build.Repository.Base
{
    public interface IBaseRepository<TEntity> where TEntity : class
    {
        string CurrentDb { get; set; }
        ISqlSugarClient Db { get; }
        Task<TEntity> QueryById(object objId);
        Task<TEntity> QueryById(object objId, bool blnUseCache = false);
        Task<List<TEntity>> QueryByIDs(object[] lstIds);

        Task<int> Add(TEntity model);

        Task<TEntity> AddEntity(TEntity entity);

        Task<int> Add(List<TEntity> listEntity);

        Task<bool> DeleteById(object id);

        Task<bool> Delete(TEntity model);

        Task<bool> DeleteByIds(object[] ids);

        Task<bool> Update(TEntity model);
        Task<bool> Update(TEntity entity, string strWhere);
        Task<bool> Update(object operateAnonymousObjects);

        Task<bool> Update(TEntity entity, List<string> lstColumns = null, List<string> lstIgnoreColumns = null, string strWhere = "");

        Task<List<TEntity>> Query();
        Task<List<TEntity>> Query(string strWhere);
        Task<List<TEntity>> Query(Expression<Func<TEntity, bool>> whereExpression);
        Task<List<TEntity>> Query(Expression<Func<TEntity, bool>> whereExpression, string strOrderByFileds);
        Task<List<TEntity>> Query(Expression<Func<TEntity, bool>> whereExpression, Expression<Func<TEntity, object>> orderByExpression, bool isAsc = true);
        Task<List<TEntity>> Query(string strWhere, string strOrderByFileds);

        Task<List<TEntity>> Query(Expression<Func<TEntity, bool>> whereExpression, int intTop, string strOrderByFileds);
        Task<List<TEntity>> Query(string strWhere, int intTop, string strOrderByFileds);
        Task<List<TEntity>> QuerySql(string strSql, SugarParameter[] parameters = null);
        Task<DataTable> QueryTable(string strSql, SugarParameter[] parameters = null);

        Task<List<TEntity>> Query(
            Expression<Func<TEntity, bool>> whereExpression, int intPageIndex, int intPageSize, string strOrderByFileds);
 
        Task<PageOutput<TEntity>> QueryPage(Expression<Func<TEntity, bool>> whereExpression, int intPageIndex = 1, int intPageSize = 20, string strOrderByFileds = null);

    }
}
