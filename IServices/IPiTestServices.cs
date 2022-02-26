using System;
using System.Threading.Tasks;
using IServices.IBaseServices;
using Model.pi;

namespace IServices
{
    public interface IPiTestServices : IBaseServices<tb_pi_test>
    {
        bool Led(int Gpio, bool Is_High);
        bool ManualDrive(string direction, int duration);
        //bool Transaction_Test();
        bool Morse(string msg);
    }
}