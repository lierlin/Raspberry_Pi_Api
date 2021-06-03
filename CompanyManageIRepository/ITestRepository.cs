using IRepository.IBaseRepository;
using Model.MySql;

namespace IRepository
{
    public interface ITestRepository : IBaseRepository<test>
    {
    }
}