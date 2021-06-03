using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using EFCore.BulkExtensions;
using IRepository.IBaseRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Repository.BaseRepository
{
    /// <summary>
    /// 泛型方法，直接注入EF上下文
    /// </summary>

    /// <summary>
    /// 基类 实现仓储类
    /// ErLin Li
    /// 2021/4/15
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : class, new()
    {
        public DbContext db;

        /// <summary>
        /// 在使用的时候，自动注入db上下文
        /// </summary>
        /// <param name="db"></param>
        public BaseRepository(DbContext db)
        {
            this.db = db;
            //关闭全局追踪的代码
            //db.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        /****************************************下面进行方法的封装（同步）***********************************************/
        //1. 直接提交数据库

        #region 01-数据源

        public IQueryable<T> Entities<T>() where T : class
        {
            return db.Set<T>();
        }

        public IQueryable<T> EntitiesNoTrack<T>() where T : class
        {
            return db.Set<T>().AsNoTracking();
        }

        #endregion 01-数据源

        #region 02-新增

        public int Add<T>(T model) where T : class
        {
            try
            {
                db.Entry(model).State = EntityState.Added;
                return db.SaveChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        #endregion 02-新增

        #region 03-删除

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="model">需要删除的实体</param>
        /// <returns></returns>
        public int Del<T>(T model) where T : class
        {
            db.Entry(model).State = EntityState.Deleted;
            return db.SaveChanges();
        }

        #endregion 03-删除

        #region 04-根据条件删除(支持批量删除)

        /// <summary>
        /// 根据条件删除(支持批量删除)
        /// </summary>
        /// <param name="delWhere">传入Lambda表达式(生成表达式目录树)</param>
        /// <returns></returns>
        public int DelBy<T>(Expression<Func<T, bool>> delWhere) where T : class
        {
            List<T> listDels = db.Set<T>().Where(delWhere).ToList();
            listDels.ForEach(model =>
            {
                db.Entry(model).State = EntityState.Deleted;
            });
            return db.SaveChanges();
        }

        #endregion 04-根据条件删除(支持批量删除)

        #region 05-单实体修改

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="model">修改后的实体</param>
        /// <returns></returns>
        public int Modify<T>(T model) where T : class
        {
            db.Entry(model).State = EntityState.Modified;
            return db.SaveChanges();
        }

        #endregion 05-单实体修改

        #region 06-批量修改（非lambda）

        /// <summary>
        /// 批量修改（非lambda）
        /// </summary>
        /// <param name="model">要修改实体中 修改后的属性 </param>
        /// <param name="whereLambda">查询实体的条件</param>
        /// <param name="proNames">lambda的形式表示要修改的实体属性名</param>
        /// <returns></returns>
        public int ModifyBy<T>(T model, Expression<Func<T, bool>> whereLambda, params string[] proNames) where T : class
        {
            List<T> listModifes = db.Set<T>().Where(whereLambda).ToList();
            Type t = typeof(T);
            List<PropertyInfo> proInfos = t.GetProperties(BindingFlags.Instance | BindingFlags.Public).ToList();
            Dictionary<string, PropertyInfo> dicPros = new Dictionary<string, PropertyInfo>();
            proInfos.ForEach(p =>
            {
                if (proNames.Contains(p.Name))
                {
                    dicPros.Add(p.Name, p);
                }
            });
            foreach (string proName in proNames)
            {
                if (dicPros.ContainsKey(proName))
                {
                    PropertyInfo proInfo = dicPros[proName];
                    object newValue = proInfo.GetValue(model, null);
                    foreach (T m in listModifes)
                    {
                        proInfo.SetValue(m, newValue, null);
                    }
                }
            }
            return db.SaveChanges();
        }

        #endregion 06-批量修改（非lambda）

        #region 07-根据条件查询

        /// <summary>
        /// 根据条件查询
        /// </summary>
        /// <param name="whereLambda">查询条件(lambda表达式的形式生成表达式目录树)</param>
        ///  <param name="isTrack">是否跟踪状态，默认是跟踪的</param>
        /// <returns></returns>
        public List<T> GetListBy<T>(Expression<Func<T, bool>> whereLambda, bool isTrack = true) where T : class
        {
            if (isTrack)
            {
                return db.Set<T>().Where(whereLambda).ToList();
            }
            else
            {
                return db.Set<T>().Where(whereLambda).AsNoTracking().ToList();
            }
        }

        #endregion 07-根据条件查询

        #region 08-根据条件排序和查询

        /// <summary>
        /// 根据条件排序和查询
        /// </summary>
        /// <typeparam name="Tkey">排序字段类型</typeparam>
        /// <param name="whereLambda">查询条件</param>
        /// <param name="orderLambda">排序条件</param>
        /// <param name="isAsc">升序or降序</param>
        ///  <param name="isTrack">是否跟踪状态，默认是跟踪的</param>
        /// <returns></returns>
        public List<T> GetListBy<T, Tkey>(Expression<Func<T, bool>> whereLambda, Expression<Func<T, Tkey>> orderLambda, bool isAsc = true, bool isTrack = true) where T : class
        {
            IQueryable<T> data = null;
            if (isTrack)
            {
                data = db.Set<T>().Where(whereLambda);
            }
            else
            {
                data = db.Set<T>().Where(whereLambda).AsNoTracking();
            }
            if (isAsc)
            {
                data = data.OrderBy(orderLambda);
            }
            else
            {
                data = data.OrderByDescending(orderLambda);
            }
            return data.ToList();
        }

        #endregion 08-根据条件排序和查询

        #region 09-分页查询(根据Lambda排序)

        /// <summary>
        /// 根据条件排序和查询
        /// </summary>
        /// <typeparam name="Tkey">排序字段类型</typeparam>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">页容量</param>
        /// <param name="whereLambda">查询条件</param>
        /// <param name="orderLambda">排序条件</param>
        /// <param name="isAsc">升序or降序</param>
        ///  <param name="isTrack">是否跟踪状态，默认是跟踪的</param>
        /// <returns></returns>
        public List<T> GetPageList<T, Tkey>(int pageIndex, int pageSize, Expression<Func<T, bool>> whereLambda, Expression<Func<T, Tkey>> orderLambda, bool isAsc = true, bool isTrack = true) where T : class
        {
            IQueryable<T> data = null;
            if (isTrack)
            {
                data = db.Set<T>().Where(whereLambda);
            }
            else
            {
                data = db.Set<T>().Where(whereLambda).AsNoTracking();
            }
            if (isAsc)
            {
                data = data.OrderBy(orderLambda).Skip((pageIndex - 1) * pageSize).Take(pageSize);
            }
            else
            {
                data = data.OrderByDescending(orderLambda).Skip((pageIndex - 1) * pageSize).Take(pageSize);
            }
            return data.ToList();
        }

        #endregion 09-分页查询(根据Lambda排序)

        #region 10-分页查询(根据名称排序)

        /// <summary>
        /// 分页查询输出总行数（根据名称排序）
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="rowCount">输出的总数量</param>
        /// <param name="whereLambda">查询条件</param>
        /// <param name="sortName">排序名称</param>
        /// <param name="sortDirection">asc 或 desc</param>
        ///  <param name="isTrack">是否跟踪状态，默认是跟踪的</param>
        /// <returns></returns>
        public List<T> GetPageListByName<T>(int pageIndex, int pageSize, Expression<Func<T, bool>> whereLambda, string sortName, string sortDirection, bool isTrack = true) where T : class
        {
            List<T> list = null;
            if (isTrack)
            {
                list = db.Set<T>().Where(whereLambda).DataSorting(sortName, sortDirection)
                 .Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            }
            else
            {
                list = db.Set<T>().Where(whereLambda).AsNoTracking().DataSorting(sortName, sortDirection)
                 .Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            }
            return list;
        }

        #endregion 10-分页查询(根据名称排序)

        #region 11-分页查询输出总行数（根据Lambda排序）

        /// <summary>
        /// 根据条件排序和查询
        /// </summary>
        /// <typeparam name="Tkey">排序字段类型</typeparam>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">页容量</param>
        /// <param name="whereLambda">查询条件</param>
        /// <param name="orderLambda">排序条件</param>
        /// <param name="isAsc">升序or降序</param>
        ///  <param name="isTrack">是否跟踪状态，默认是跟踪的</param>
        /// <returns></returns>
        public List<T> GetPageList<T, Tkey>(int pageIndex, int pageSize, out int rowCount, Expression<Func<T, bool>> whereLambda, Expression<Func<T, Tkey>> orderLambda, bool isAsc = true, bool isTrack = true) where T : class
        {
            int count = db.Set<T>().Where(whereLambda).Count();
            IQueryable<T> data = null;
            if (isTrack)
            {
                data = db.Set<T>().Where(whereLambda);
            }
            else
            {
                data = db.Set<T>().Where(whereLambda).AsNoTracking();
            }
            if (isAsc)
            {
                data = data.OrderBy(orderLambda).Skip((pageIndex - 1) * pageSize).Take(pageSize);
            }
            else
            {
                data = data.OrderByDescending(orderLambda).Skip((pageIndex - 1) * pageSize).Take(pageSize);
            }
            rowCount = count;
            return data.ToList();
        }

        #endregion 11-分页查询输出总行数（根据Lambda排序）

        #region 12-分页查询输出总行数（根据名称排序）

        /// <summary>
        /// 分页查询输出总行数（根据名称排序）
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">页容量</param>
        /// <param name="rowCount">输出的总数量</param>
        /// <param name="whereLambda">查询条件</param>
        /// <param name="sortName">排序名称</param>
        /// <param name="sortDirection">asc 或 desc</param>
        ///  <param name="isTrack">是否跟踪状态，默认是跟踪的</param>
        /// <returns></returns>
        public List<T> GetPageListByName<T>(int pageIndex, int pageSize, out int rowCount, Expression<Func<T, bool>> whereLambda, string sortName, string sortDirection, bool isTrack = true) where T : class
        {
            int count = 0;
            count = db.Set<T>().Where(whereLambda).Count();
            List<T> list = null;
            if (isTrack)
            {
                list = db.Set<T>().Where(whereLambda).DataSorting(sortName, sortDirection)
                 .Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            }
            else
            {
                list = db.Set<T>().Where(whereLambda).AsNoTracking().DataSorting(sortName, sortDirection)
                   .Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            }
            rowCount = count;
            return list;
        }

        #endregion 12-分页查询输出总行数（根据名称排序）

        //2. SaveChange剥离出来，处理事务

        #region 01-批量处理SaveChange()

        /// <summary>
        /// 事务批量处理
        /// </summary>
        /// <returns></returns>
        public int SaveChange()
        {
            try
            {
                return db.SaveChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        #endregion 01-批量处理SaveChange()

        #region 02-新增

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="model">需要新增的实体</param>
        public void AddNo<T>(T model) where T : class
        {
            db.Entry(model).State = EntityState.Added;
        }

        #endregion 02-新增

        #region 03-删除

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="model">需要删除的实体</param>
        public void DelNo<T>(T model) where T : class
        {
            db.Entry(model).State = EntityState.Deleted;
        }

        #endregion 03-删除

        #region 04-根据条件删除

        /// <summary>
        /// 条件删除
        /// </summary>
        /// <param name="delWhere">需要删除的条件</param>
        public void DelByNo<T>(Expression<Func<T, bool>> delWhere) where T : class
        {
            List<T> listDels = db.Set<T>().Where(delWhere).ToList();
            listDels.ForEach(model =>
            {
                db.Entry(model).State = EntityState.Deleted;
            });
        }

        #endregion 04-根据条件删除

        #region 05-修改

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="model">修改后的实体</param>
        public void ModifyNo<T>(T model) where T : class
        {
            db.Entry(model).State = EntityState.Modified;
        }

        #endregion 05-修改

        //3. EF调用sql语句

        #region 01-执行增加,删除,修改操作(或调用相关存储过程)

        /// <summary>
        /// 执行增加,删除,修改操作(或调用存储过程)
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="pars"></param>
        /// <returns></returns>
        public int ExecuteSql(string sql, params SqlParameter[] pars)
        {
            return db.Database.ExecuteSqlRaw(sql, pars);
        }

        #endregion 01-执行增加,删除,修改操作(或调用相关存储过程)

        #region 02-执行查询操作（调用查询类的存储过程）

        /// <summary>
        /// 执行查询操作
        /// 注：查询必须返回实体的所有属性字段；结果集中列名必须与属性映射的项目匹配；查询中不能包含关联数据
        /// 除Select以外其他的SQL语句无法执行
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="isTrack">是否跟踪状态，默认是跟踪的</param>
        /// <param name="pars"></param>
        /// <returns></returns>
        public List<T> ExecuteQuery<T>(string sql, bool isTrack = true, params SqlParameter[] pars) where T : class
        {
            if (isTrack)
            {
                //表示跟踪状态（默认是跟踪的）
                return db.Set<T>().FromSqlRaw(sql, pars).ToList();
            }
            else
            {
                //表示不跟踪状态
                return db.Set<T>().FromSqlRaw(sql, pars).AsNoTracking().ToList();
            }
        }

        #endregion 02-执行查询操作（调用查询类的存储过程）

        #region 03-执行查询操作（与Linq相结合）

        /// <summary>
        /// 执行查询操作
        /// 注：查询必须返回实体的所有属性字段；结果集中列名必须与属性映射的项目匹配；查询中不能包含关联数据
        /// 除Select以外其他的SQL语句无法执行
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        ///  <param name="whereLambda">查询条件</param>
        /// <param name="isTrack">是否跟踪状态，默认是跟踪的</param>
        /// <param name="pars"></param>
        /// <returns></returns>
        public List<T> ExecuteQueryWhere<T>(string sql, Expression<Func<T, bool>> whereLambda, bool isTrack = true, params SqlParameter[] pars) where T : class
        {
            if (isTrack)
            {
                //表示跟踪状态（默认是跟踪的）
                return db.Set<T>().FromSqlRaw(sql, pars).Where(whereLambda).ToList();
            }
            else
            {
                //表示不跟踪状态
                return db.Set<T>().FromSqlRaw(sql, pars).Where(whereLambda).AsNoTracking().ToList();
            }
        }

        #endregion 03-执行查询操作（与Linq相结合）

        /****************************************下面进行方法的封装（异步）***********************************************/

        #region 01-新增

        public async Task<int> AddAsync<T>(T model) where T : class
        {
            await db.AddAsync(model);
            return await db.SaveChangesAsync();
        }

        #endregion 01-新增

        #region 02-删除

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="model">需要删除的实体</param>
        /// <returns></returns>
        public async Task<int> DelAsync<T>(T model) where T : class
        {
            db.Entry(model).State = EntityState.Deleted;
            return await db.SaveChangesAsync();
        }

        #endregion 02-删除

        #region 03-根据条件删除(支持批量删除)

        /// <summary>
        /// 根据条件删除(支持批量删除)
        /// </summary>
        /// <param name="delWhere">传入Lambda表达式(生成表达式目录树)</param>
        /// <returns></returns>
        public async Task<int> DelByAsync<T>(Expression<Func<T, bool>> delWhere) where T : class
        {
            List<T> listDels = await db.Set<T>().Where(delWhere).ToListAsync();
            listDels.ForEach(model =>
            {
                db.Entry(model).State = EntityState.Deleted;
            });
            return await db.SaveChangesAsync();
        }

        #endregion 03-根据条件删除(支持批量删除)

        #region 04-单实体修改

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="model">修改后的实体</param>
        /// <returns></returns>
        public async Task<int> ModifyAsync<T>(T model) where T : class
        {
            db.Entry(model).State = EntityState.Modified;
            return await db.SaveChangesAsync();
        }

        #endregion 04-单实体修改

        #region 05-批量修改（非lambda）

        /// <summary>
        /// 批量修改（非lambda）
        /// </summary>
        /// <param name="model">要修改实体中 修改后的属性 </param>
        /// <param name="whereLambda">查询实体的条件</param>
        /// <param name="proNames">lambda的形式表示要修改的实体属性名</param>
        /// <returns></returns>
        public async Task<int> ModifyByAsync<T>(T model, Expression<Func<T, bool>> whereLambda, params string[] proNames) where T : class
        {
            List<T> listModifes = await db.Set<T>().Where(whereLambda).ToListAsync();
            Type t = typeof(T);
            List<PropertyInfo> proInfos = t.GetProperties(BindingFlags.Instance | BindingFlags.Public).ToList();
            Dictionary<string, PropertyInfo> dicPros = new Dictionary<string, PropertyInfo>();
            proInfos.ForEach(p =>
            {
                if (proNames.Contains(p.Name))
                {
                    dicPros.Add(p.Name, p);
                }
            });
            foreach (string proName in proNames)
            {
                if (dicPros.ContainsKey(proName))
                {
                    PropertyInfo proInfo = dicPros[proName];
                    object newValue = proInfo.GetValue(model, null);
                    foreach (T m in listModifes)
                    {
                        proInfo.SetValue(m, newValue, null);
                    }
                }
            }
            return await db.SaveChangesAsync();
        }

        #endregion 05-批量修改（非lambda）

        #region 06-根据条件查询

        /// <summary>
        /// 根据条件查询
        /// </summary>
        /// <param name="whereLambda">查询条件(lambda表达式的形式生成表达式目录树)</param>
        ///  <param name="isTrack">是否跟踪状态，默认是跟踪的</param>
        /// <returns></returns>
        public async Task<List<T>> GetListByAsync<T>(Expression<Func<T, bool>> whereLambda, bool isTrack = true) where T : class
        {
            if (isTrack)
            {
                return await db.Set<T>().Where(whereLambda).ToListAsync();
            }
            else
            {
                return await db.Set<T>().Where(whereLambda).AsNoTracking().ToListAsync();
            }
        }

        #endregion 06-根据条件查询

        #region 07-根据条件排序和查询

        /// <summary>
        /// 根据条件排序和查询
        /// </summary>
        /// <typeparam name="Tkey">排序字段类型</typeparam>
        /// <param name="whereLambda">查询条件</param>
        /// <param name="orderLambda">排序条件</param>
        /// <param name="isAsc">升序or降序</param>
        ///  <param name="isTrack">是否跟踪状态，默认是跟踪的</param>
        /// <returns></returns>
        public async Task<List<T>> GetListByAsync<T, Tkey>(Expression<Func<T, bool>> whereLambda, Expression<Func<T, Tkey>> orderLambda, bool isAsc = true, bool isTrack = true) where T : class
        {
            IQueryable<T> data = null;
            if (isTrack)
            {
                data = db.Set<T>().Where(whereLambda);
            }
            else
            {
                data = db.Set<T>().Where(whereLambda).AsNoTracking();
            }
            if (isAsc)
            {
                data = data.OrderBy(orderLambda);
            }
            else
            {
                data = data.OrderByDescending(orderLambda);
            }
            return await data.ToListAsync();
        }

        #endregion 07-根据条件排序和查询

        #region 08-分页查询(根据Lambda排序)

        /// <summary>
        /// 根据条件排序和查询
        /// </summary>
        /// <typeparam name="Tkey">排序字段类型</typeparam>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">页容量</param>
        /// <param name="whereLambda">查询条件</param>
        /// <param name="orderLambda">排序条件</param>
        /// <param name="isAsc">升序or降序</param>
        ///  <param name="isTrack">是否跟踪状态，默认是跟踪的</param>
        /// <returns></returns>
        public async Task<List<T>> GetPageListAsync<T, Tkey>(int pageIndex, int pageSize, Expression<Func<T, bool>> whereLambda, Expression<Func<T, Tkey>> orderLambda, bool isAsc = true, bool isTrack = true) where T : class
        {
            IQueryable<T> data = null;
            if (isTrack)
            {
                data = db.Set<T>().Where(whereLambda);
            }
            else
            {
                data = db.Set<T>().Where(whereLambda).AsNoTracking();
            }
            if (isAsc)
            {
                data = data.OrderBy(orderLambda).Skip((pageIndex - 1) * pageSize).Take(pageSize);
            }
            else
            {
                data = data.OrderByDescending(orderLambda).Skip((pageIndex - 1) * pageSize).Take(pageSize);
            }
            return await data.ToListAsync();
        }

        #endregion 08-分页查询(根据Lambda排序)

        #region 09-分页查询(根据名称排序)

        /// <summary>
        /// 分页查询输出总行数（根据名称排序）
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="rowCount">输出的总数量</param>
        /// <param name="whereLambda">查询条件</param>
        /// <param name="sortName">排序名称</param>
        /// <param name="sortDirection">asc 或 desc</param>
        ///  <param name="isTrack">是否跟踪状态，默认是跟踪的</param>
        /// <returns></returns>
        public async Task<List<T>> GetPageListByNameAsync<T>(int pageIndex, int pageSize, Expression<Func<T, bool>> whereLambda, string sortName, string sortDirection, bool isTrack = true) where T : class
        {
            List<T> list = null;
            if (isTrack)
            {
                list = await db.Set<T>().Where(whereLambda).DataSorting(sortName, sortDirection)
                 .Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
            }
            else
            {
                list = await db.Set<T>().Where(whereLambda).AsNoTracking().DataSorting(sortName, sortDirection)
                 .Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
            }
            return list;
        }

        #endregion 09-分页查询(根据名称排序)

        //2. SaveChange剥离出来，处理事务

        #region 01-批量处理SaveChange()

        /// <summary>
        /// 事务批量处理
        /// </summary>
        /// <returns></returns>
        public async Task<int> SaveChangeAsync()
        {
            return await db.SaveChangesAsync();
        }

        #endregion 01-批量处理SaveChange()

        #region 02-新增

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="model">需要新增的实体</param>
        public async Task<EntityEntry<T>> AddNoAsync<T>(T model) where T : class
        {
            return await db.AddAsync(model);
        }

        #endregion 02-新增

        #region 03-根据条件删除

        /// <summary>
        /// 条件删除
        /// </summary>
        /// <param name="delWhere">需要删除的条件</param>
        public async Task DelByNoAsync<T>(Expression<Func<T, bool>> delWhere) where T : class
        {
            List<T> listDels = await db.Set<T>().Where(delWhere).ToListAsync();
            listDels.ForEach(model =>
            {
                db.Entry(model).State = EntityState.Deleted;
            });
        }

        #endregion 03-根据条件删除

        //3. EF调用sql语句

        #region 01-执行增加,删除,修改操作(或调用存储过程)

        /// <summary>
        /// 执行增加,删除,修改操作(或调用存储过程)
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="pars"></param>
        /// <returns></returns>
        public async Task<int> ExecuteSqlAsync(string sql, params SqlParameter[] pars)
        {
            return await db.Database.ExecuteSqlRawAsync(sql, pars);
        }

        #endregion 01-执行增加,删除,修改操作(或调用存储过程)

        /****************************************下面是基于【EFCore.BulkExtensions】大数据的处理 （同步）***********************************************/

        #region 01-增加

        /// <summary>
        /// 增加
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        public void BulkInsert<T>(List<T> list) where T : class
        {
            db.BulkInsert<T>(list);
        }

        #endregion 01-增加

        #region 02-修改

        /// <summary>
        /// 修改
        /// PS：传入的实体如果不赋值,则更新为null,即传入的实体每个字段都要有值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        public void BulkUpdate<T>(List<T> list) where T : class
        {
            db.BulkUpdate<T>(list);
        }

        #endregion 02-修改

        #region 03-删除

        /// <summary>
        /// 删除
        /// PS：传入的list中的实体仅需要主键有值,它是根据主键删除的
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        public void BulkDelete<T>(List<T> list) where T : class
        {
            db.BulkDelete<T>(list);
        }

        #endregion 03-删除

        #region 04-条件删除

        /// <summary>
        /// 条件删除
        /// </summary>
        /// <param name="delWhere">需要删除的条件</param>
        public int BatchDelete<T>(Expression<Func<T, bool>> delWhere) where T : class
        {
            return db.Set<T>().Where(delWhere).BatchDelete();
        }

        #endregion 04-条件删除

        #region 05-条件更新1

        /// <summary>
        /// 条件更新
        /// PS：要更新哪几个字段，就给传入的实体中的哪几个字段赋值
        /// </summary>
        /// <param name="delWhere">需要更新的条件</param>
        /// <param name="model">更新为的实体</param>
        public int BatchUpdate<T>(Expression<Func<T, bool>> delWhere, T model) where T : class, new()
        {
            return db.Set<T>().Where(delWhere).BatchUpdate(model);
        }

        #endregion 05-条件更新1

        #region 06-条件更新2

        /// <summary>
        /// 条件更新
        /// PS：要更新哪几个字段，就给传入的实体中的哪几个字段赋值
        /// </summary>
        /// <param name="delWhere">需要更新的条件</param>
        /// <param name="model">更新为的实体</param>
        public int BatchUpdate2<T>(Expression<Func<T, bool>> delWhere, Expression<Func<T, T>> modelWhere) where T : class, new()
        {
            return db.Set<T>().Where(delWhere).BatchUpdate(modelWhere);
        }

        #endregion 06-条件更新2

        /****************************************下面是基于【EFCore.BulkExtensions】大数据的处理 （异步）***********************************************/

        #region 01-增加

        /// <summary>
        /// 增加
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        public async void BulkInsertAsync<T>(List<T> list) where T : class
        {
            await db.BulkInsertAsync<T>(list);
        }

        #endregion 01-增加

        #region 02-修改

        /// <summary>
        /// 修改
        /// PS：传入的实体如果不赋值,则更新为null,即传入的实体每个字段都要有值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        public async void BulkUpdateAsync<T>(List<T> list) where T : class
        {
            await db.BulkUpdateAsync<T>(list);
        }

        #endregion 02-修改

        #region 03-删除

        /// <summary>
        /// 删除
        /// PS：传入的list中的实体仅需要主键有值,它是根据主键删除的
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        public async void BulkDeleteAsync<T>(List<T> list) where T : class
        {
            await db.BulkDeleteAsync<T>(list);
        }

        #endregion 03-删除

        #region 04-条件删除

        /// <summary>
        /// 条件删除
        /// </summary>
        /// <param name="delWhere">需要删除的条件</param>
        public async Task<int> BatchDeleteAsync<T>(Expression<Func<T, bool>> delWhere) where T : class
        {
            return await db.Set<T>().Where(delWhere).BatchDeleteAsync();
        }

        #endregion 04-条件删除

        #region 05-条件更新1

        /// <summary>
        /// 条件更新
        /// PS：要更新哪几个字段，就给传入的实体中的哪几个字段赋值
        /// </summary>
        /// <param name="delWhere">需要更新的条件</param>
        /// <param name="model">更新为的实体</param>
        public async Task<int> BatchUpdateAsync<T>(Expression<Func<T, bool>> delWhere, T model) where T : class, new()
        {
            return await db.Set<T>().Where(delWhere).BatchUpdateAsync(model);
        }

        #endregion 05-条件更新1

        #region 06-条件更新2

        /// <summary>
        /// 条件更新
        /// PS：要更新哪几个字段，就给传入的实体中的哪几个字段赋值
        /// </summary>
        /// <param name="delWhere">需要更新的条件</param>
        /// <param name="model">更新为的实体</param>
        public async Task<int> BatchUpdate2Async<T>(Expression<Func<T, bool>> delWhere, Expression<Func<T, T>> modelWhere) where T : class, new()
        {
            return await db.Set<T>().Where(delWhere).BatchUpdateAsync(modelWhere);
        }

        #endregion 06-条件更新2
    }

    /// <summary>
    /// 排序的扩展
    /// </summary>
    public static class SortExtension
    {
        #region 01-根据string名称排序扩展(单字段)

        /// <summary>
        /// 根据string名称排序扩展(单字段)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">排序数据源</param>
        /// <param name="sortName">排序名称</param>
        /// <param name="sortDirection">排序方式 asc或desc</param>
        /// <returns></returns>
        public static IQueryable<T> DataSorting<T>(this IQueryable<T> source, string sortName, string sortDirection)
        {
            string sortingDir = string.Empty;
            if (sortDirection.ToUpper().Trim() == "ASC")
            {
                sortingDir = "OrderBy";
            }
            else if (sortDirection.ToUpper().Trim() == "DESC")
            {
                sortingDir = "OrderByDescending";
            }
            ParameterExpression param = Expression.Parameter(typeof(T), sortName);
            PropertyInfo pi = typeof(T).GetProperty(sortName);
            Type[] types = new Type[2];
            types[0] = typeof(T);
            types[1] = pi.PropertyType;
            Expression expr = Expression.Call(typeof(Queryable), sortingDir, types, source.Expression, Expression.Lambda(Expression.Property(param, sortName), param));
            IQueryable<T> query = source.AsQueryable().Provider.CreateQuery<T>(expr);
            return query;
        }

        #endregion 01-根据string名称排序扩展(单字段)

        #region 02-根据多个string名称排序扩展(多字段)

        /// <summary>
        ///  根据多个string名称排序扩展(多字段)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data">数据源</param>
        /// <param name="orderParams">排序类</param>
        /// <returns></returns>
        public static IQueryable<T> DataManySorting<T>(this IQueryable<T> data, params FiledOrderParam[] orderParams) where T : class
        {
            var parameter = Expression.Parameter(typeof(T), "p");
            if (orderParams != null && orderParams.Length > 0)
            {
                for (int i = 0; i < orderParams.Length; i++)
                {
                    var property = typeof(T).GetProperty(orderParams[i].PropertyName);
                    if (property != null)
                    {
                        var propertyAccess = Expression.MakeMemberAccess(parameter, property);
                        var orderByExpr = Expression.Lambda(propertyAccess, parameter);
                        string methodName = i > 0 ?
                            orderParams[i].IsDesc ? "ThenByDescending" : "ThenBy"
                            : orderParams[i].IsDesc ? "OrderByDescending" : "OrderBy";
                        var resultExp = Expression.Call(
                            typeof(Queryable), methodName,
                            new Type[] { typeof(T), property.PropertyType },
                            data.Expression, Expression.Quote(orderByExpr)
                            );
                        data = data.Provider.CreateQuery<T>(resultExp);
                    }
                }
            }
            return data;
        }

        #endregion 02-根据多个string名称排序扩展(多字段)
    }

    /// <summary>
    /// 排序类
    /// </summary>
    public class FiledOrderParam
    {
        //是否降序
        public bool IsDesc { get; set; }

        //排序名称
        public string PropertyName { get; set; }
    }
}