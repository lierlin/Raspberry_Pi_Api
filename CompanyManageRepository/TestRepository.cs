using IRepository;
using Model.MySql;
using Repository.BaseRepository;

namespace Repository
{
    public class TestRepository : BaseRepository<test>, IPiTestRepository
    {
        public TestRepository(PiDBContext piDbContext) : base(piDbContext)
        {
        }
    }
}