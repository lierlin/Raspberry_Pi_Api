using IServices;
using Model.pi;
using Services.BaseServices;
using System;
using System.Device.Gpio;
using System.Text.Unicode;
using System.Threading.Tasks;
using System.Transactions;
using Model.pi1;
using Extensions;
using System.Threading;
using System.Collections.Generic;

namespace Services
{
    public class PiTestServices : BaseServices<tb_pi_test>, IPiTestServices
    {
        public piDBContext _db;

        //public PiTestServices(piDBContext piDBContext, pi1DBContext pi1DBContext) : base(piDBContext, pi1DBContext)
        //{
        //    _db = piDBContext;
        //}

        public bool Led(int Gpio, bool Is_High)
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
        public bool Led(int Gpio, int time)
        {
            GpioController controller = new GpioController(PinNumberingScheme.Board);
            // 判断某个引脚是否打开
            if (!controller.IsPinOpen(Gpio))
            {
                // 注意：引脚连续打开会抛出异常
                controller.OpenPin(Gpio);
                controller.SetPinMode(Gpio, PinMode.Output);
            }
            Console.WriteLine($"{DateTime.Now}打开{Gpio}");
            controller.Write(Gpio, PinValue.High);

            Thread.Sleep(time);
            Console.WriteLine($"{DateTime.Now}关闭{Gpio}");
            controller.Write(Gpio, PinValue.Low);
            return true;
        }

        public bool ManualDrive(string direction, int duration)
        {
      
            List<int> gpioList = new List<int>();
            direction=direction.Trim().ToLower();
            //22,18 |26,24
            //10, 8 |16 ,12
            if (direction=="forward")
            {
                gpioList.Add(26);
                gpioList.Add(22);
                gpioList.Add(10);
                gpioList.Add(16);
                Start(gpioList, duration);
            }
            //22,18 |26,24
            //10, 8 |16 ,12
            else if (direction=="backward")
            {
                gpioList.Add(24);
                gpioList.Add(18);
                gpioList.Add(8);
                gpioList.Add(12);
                Start(gpioList, duration);
            }
            //22,18 |26,24
            //10, 8 |16 ,12
            else if (direction=="leftward")
            {
               
                gpioList.Add(18);
                gpioList.Add(12);
                gpioList.Add(26);
                gpioList.Add(10);
                Start(gpioList, duration);
            }
            //22,18 |26,24
            //10, 8 |16 ,12
            else if (direction=="rightward")
            {
                gpioList.Add(22);
                gpioList.Add(16);
                gpioList.Add(24);
                gpioList.Add(8);
                Start(gpioList, duration);
            }
            return true;

        }
        public bool Start(List<int> gpioList, int duration)
        {

            foreach (var gpio in gpioList)
            {
                Led(gpio, true);
            }
            Thread.Sleep(duration*1000);
            foreach (var gpio in gpioList)
            {
                Led(gpio, false);
            }
            return true;
        }

        public bool Morse(string msg)
        {
            #region 摩尔斯密码操作类调用测试
            string encry = MorseCode.Enc(msg);     //调用编码函数把【SWORLD】换成摩尔斯电码  
            Console.WriteLine(encry);                   //打印编码字符串
            Play(encry);
            #endregion
            return true;
        }
        public void Play(string _str)
        {
            foreach (char c in _str)
            {
                switch (c)
                {
                    case '-':
                        Led(7, 1500);
                        Thread.Sleep(300);
                        break;
                    case '.':
                        Led(7, 500);
                        Thread.Sleep(300);
                        break;
                    case ' ':
                        break;
                }
            }
        }



        //public async Task<bool> Transaction_TestAsync()

        //{
        //    test test = new test();
        //    test.id = Guid.NewGuid();
        //    _baseRepository.Add(test);

        //    #region SaveChanges  事务

        //    #region 单库单表合并提交

        //    //for (int i = 0; i < 5; i++)
        //    //{
        //    //    test test = new test();
        //    //    test.id = Guid.NewGuid();
        //    //    _baseRepository.AddNo(test);
        //    //}
        //    //int count = _baseRepository.SaveChange();
        //    //Console.WriteLine("=====>" + count);

        //    #endregion 单库单表合并提交

        //    #region 单库多表合并提交

        //    //for (int i = 0; i < 5; i++)
        //    //{
        //    //    tb_pi_test tb_pi_test = new tb_pi_test();
        //    //    tb_pi_test.id = Guid.NewGuid();
        //    //    _baseRepository.AddNo(tb_pi_test);

        //    //    test test = new test();
        //    //    test.id = Guid.NewGuid();
        //    //    _baseRepository.AddNo(test);
        //    //}
        //    //int count = _baseRepository.SaveChange();
        //    //Console.WriteLine("=====>" + count);

        //    #endregion 单库多表合并提交

        //    #endregion SaveChanges  事务

        //    #region TransactionScope 事务

        //    #region 多库事务提交 AddNo and  SaveChange

        //    //多库事务提交
        //    //多上下文连接提交  AddNo and  SaveChange
        //    //using (var scope = new TransactionScope())
        //    //{
        //    //    tb_pi_test tb_pi_test = new tb_pi_test();
        //    //    tb_pi_test.id = Guid.NewGuid();
        //    //    test test = new test();
        //    //    test.id = Guid.NewGuid();
        //    //    _baseRepository.AddNo(tb_pi_test);
        //    //    _baseRepository.AddNo(test);
        //    //    tb_pi_test1 tb_pi_test1 = new tb_pi_test1();
        //    //    tb_pi_test1.id = Guid.NewGuid();
        //    //    test1 test1 = new test1();
        //    //    test1.id = Guid.NewGuid();
        //    //    _baseRepository1.AddNo(tb_pi_test1);
        //    //    _baseRepository1.AddNo(test1);
        //    //    //提交
        //    //    var s1 = _baseRepository1.SaveChange();
        //    //    var s2 = _baseRepository.SaveChange();

        //    //    scope.Complete();
        //    //}

        //    #endregion 多库事务提交 AddNo and  SaveChange

        //    #region 多库事务提交 Add

        //    //多库事务提交
        //    //多上下文连接提交 Add
        //    //using (var scope = new TransactionScope())
        //    //{
        //    //    tb_pi_test tb_pi_test = new tb_pi_test();
        //    //    tb_pi_test.id = Guid.NewGuid();
        //    //    test test = new test();
        //    //    test.id = Guid.NewGuid();
        //    //    var s1 = _baseRepository.Add(tb_pi_test);
        //    //    Console.WriteLine("=================>" + s1);
        //    //    _baseRepository.Add(test);
        //    //    tb_pi_test1 tb_pi_test1 = new tb_pi_test1();
        //    //    tb_pi_test1.id = Guid.NewGuid();
        //    //    test1 test1 = new test1();
        //    //    test1.id = Guid.NewGuid();
        //    //    _baseRepository1.Add(tb_pi_test1);
        //    //    _baseRepository1.Add(test1);
        //    //    scope.Complete();
        //    //}

        //    #endregion 多库事务提交 Add

        //    #endregion TransactionScope 事务

        //    #region DbContextTransaction 事务

        //    //using (var transaction = _db.Database.BeginTransaction())
        //    //{
        //    //    //过于简单不再赘述
        //    //    transaction.Commit();
        //    //}

        //    #endregion DbContextTransaction 事务

        //    return true;
        //}
    }
}