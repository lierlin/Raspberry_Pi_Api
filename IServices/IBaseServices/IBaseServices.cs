using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace IServices.IBaseServices
{
    /// <summary>
    /// 基类 服务接口
    /// ErLin Li
    /// 2021/4/15
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface IBaseServices<TEntity> where TEntity : class
    {
        public int Add<TEntity>(TEntity model) where TEntity : class;

        public void AddNo<T>(T model) where T : class;

        public int SaveChange();
    }
}