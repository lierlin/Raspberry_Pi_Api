using IRepository;
using Model.pi1;
using Repository.BaseRepository;

namespace Repository
{
    public class PiTest1Repository : BaseRepository<tb_pi_test1>, IPiTest1Repository
    {
        public PiTest1Repository(pi1DBContext piDBContext) : base(piDBContext)
        {
        }
    }
}