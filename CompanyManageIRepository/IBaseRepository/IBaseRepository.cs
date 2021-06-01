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
        public int Add<TEntity>(TEntity model) where TEntity : class;
    }
}