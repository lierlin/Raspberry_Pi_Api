using System;
using System.Threading.Tasks;
using IServices.IBaseServices;
using Model.MySql;

namespace IServices
{
    public interface IPiTestServices : IBaseServices<tb_pi_test>
    {
        Task<bool> Led(int Gpio, bool Is_High);

        bool Transaction_Test();
    }
}