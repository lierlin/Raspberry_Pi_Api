using System.Threading.Tasks;
using IRepository.IBaseRepository;
using IServices.IBaseServices;
using Microsoft.EntityFrameworkCore;
using Model.pi;
using Repository.BaseRepository;

namespace Services.BaseServices
{
    /// <summary>
    /// 基类 实现服务接口类
    /// ErLin Li
    /// 2021/4/15
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class BaseServices<TEntity> : IBaseServices<TEntity> where TEntity : class, new()
    {
        public IBaseRepository<TEntity> _baseRepository;
        public IBaseRepository<TEntity> _baseRepository1;

        //public BaseServices(IBaseRepository<TEntity> baseRepository)
        //{
        //    _baseRepository = baseRepository;
        //}
        public BaseServices(DbContext piDBContext)
        {
            _baseRepository = new BaseRepository<TEntity>(piDBContext);
        }

        public BaseServices(DbContext DbContext1, DbContext DbContext2)
        {
            _baseRepository = new BaseRepository<TEntity>(DbContext1);
            _baseRepository1 = new BaseRepository<TEntity>(DbContext2);
        }

        public int Add<T>(T model) where T : class
        {
            return _baseRepository.Add(model);
        }

        public void AddNo<T>(T model) where T : class
        {
            _baseRepository.AddNo(model);
        }

        public int SaveChange()
        {
            return _baseRepository.SaveChange();
        }
    }
}