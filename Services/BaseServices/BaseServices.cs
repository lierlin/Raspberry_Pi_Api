using IRepository.IBaseRepository;
using IServices.IBaseServices;
using Model.MySql;
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

        //public BaseServices(IBaseRepository<TEntity> baseRepository)
        //{
        //    _baseRepository = baseRepository;
        //}
        public BaseServices(PiDBContext piDbContext)
        {
            _baseRepository = new BaseRepository<TEntity>(piDbContext);
        }

        public int Add<T>(T model) where T : class
        {
            return _baseRepository.Add(model);
        }
    }
}