using IServices;
using Model.MySql;
using Services.BaseServices;
using System;
using System.Device.Gpio;
using System.Threading.Tasks;

namespace Services
{
    public class PiTestServices : BaseServices<tb_pi_test>, IPiTestServices
    {
        public PiDBContext _db;

        public PiTestServices(PiDBContext piDbContext) : base(piDbContext)
        {
            _db = piDbContext;
        }

        public async Task<bool> Led(int Gpio, bool Is_High)
        {
            GpioController controller = new GpioController(PinNumberingScheme.Board);
            // 判断某个引脚是否打开
            if (!controller.IsPinOpen(Gpio))
            {
                // 注意：引脚连续打开会抛出异常
                controller.OpenPin(Gpio);
                controller.SetPinMode(Gpio, PinMode.Output);
            }

            if (!Is_High)
            {
                controller.Write(Gpio, PinValue.Low);
                return true;
            }
            controller.Write(Gpio, PinValue.High);
            return true;
        }

        public bool Transaction_Test()
        {
            using (var transaction = _db.Database.BeginTransaction())
            {
                tb_pi_test tb_pi_test = new tb_pi_test();
                tb_pi_test.id = Guid.NewGuid();
                test test = new test();
                test.id = Guid.NewGuid();
                _baseRepository.Add(tb_pi_test);
                _baseRepository.Add(test);
                transaction.Commit();
            }

            return true;
        }
    }
}