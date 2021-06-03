using System;
using System.Threading.Tasks;
using IServices.IBaseServices;
using Model.pi;

namespace IServices
{
    public interface IPiTestServices : IBaseServices<tb_pi_test>
    {
        Task<bool> Led(int Gpio, bool Is_High);

        Task<bool> Transaction_TestAsync();
    }
}