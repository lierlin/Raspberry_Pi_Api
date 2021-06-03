using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace IRepository.IBaseRepository
{
    /// <summary>
    /// 基类 仓储接口
    /// ErLin Li
    /// 2021/4/15
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface IBaseRepository<TEntity> where TEntity : class
    {
        public int Add<T>(T model) where T : class;

        public void AddNo<T>(T model) where T : class;

        public int SaveChange();
    }
}