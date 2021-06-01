using IRepository;
using Model.MySql;
using Repository.BaseRepository;

namespace Repository
{
    public class PiTestRepository : BaseRepository<tb_pi_test>, IPiTestRepository
    {
        public PiTestRepository(PiDBContext piDbContext) : base(piDbContext)
        {
        }
    }
}