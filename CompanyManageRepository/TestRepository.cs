using IRepository;
using Model.pi;
using Repository.BaseRepository;

namespace Repository
{
    public class TestRepository : BaseRepository<test>, IPiTestRepository
    {
        public TestRepository(piDBContext piDBContext) : base(piDBContext)
        {
        }
    }
}