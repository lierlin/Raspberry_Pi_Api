using IRepository;
using Model.pi;
using Repository.BaseRepository;

namespace Repository
{
    public class PiTestRepository : BaseRepository<tb_pi_test>, IPiTestRepository
    {
        public PiTestRepository(piDBContext piDBContext) : base(piDBContext)
        {
        }
    }
}