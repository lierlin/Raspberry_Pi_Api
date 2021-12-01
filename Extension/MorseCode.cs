using System.Runtime.InteropServices;
using System.Threading;

namespace Extensions
{
    /// <summary>
    /// 摩尔斯密码编码解码类
    /// </summary>
    public static class MorseCode
    {
        /// <summary>
        /// 摩尔斯密码表
        /// </summary>
        private static volatile string[,] CodeTable =
        {
        {"A",".-"},
        {"B","-..."},
        {"C","-.-."},
        {"D","-.."},
        {"E","."},
        {"E","..-.."},
        {"F","..-."},
        {"G","--."},
        {"H","...."},
        {"I",".."},
        {"J",".---"},
        {"K","-.-"},
        {"L",".-.."},
        {"M","--"},
        {"N","-."},
        {"O","---"},
        {"P",".--."},
        {"Q","--.-"},
        {"R",".-."},
        {"S","..."},
        {"T","-"},
        {"U","..-"},
        {"V","...-"},
        {"W",".--"},
        {"X","-..-"},
        {"Y","-.--"},
        {"Z","--.."},
        {"0","-----"},
        {"1",".----"},
        {"2","..---"},
        {"3","...--"},
        {"4","....-"},
        {"5","....."},
        {"6","-...."},
        {"7","--..."},
        {"8","---.."},
        {"9","----."},
        {".",".-.-.-"},
        {",","--..--"},
        {":","---..."},
        {"?","..--.."},
        {"\'",".----."},
        {"-","-....-"},
        {"/","-..-."},
        {"(","-.--."},
        {")","-.--.-"},
        {"\"",".-..-."},
        {"=","-...-"},
        {"+",".-.-."},
        {"*","-..-"},
        {"@",".--.-."},
        {"{UNDERSTOOD}","...-."},
        {"{ERROR}","........"},
        {"{INVITATION TO TRANSMIT}","-.-"},
        {"{WAIT}",".-..."},
        {"{END OF WORK}","...-.-"},
        {"{STARTING SIGNAL}","-.-.-"},
        {" ","\u2423"}
    };
        /// <summary>
        /// 编码
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns>编码后字符串，每个编码以空格分开</returns>
        public static string Enc(string str)
        {
            int i;
            string ret = string.Empty;
            if (str != null && (str = str.ToUpper()).Length > 0)
                foreach (char asc in str)
                    if ((i = Find(asc.ToString(), 0)) > -1)
                        ret += " " + CodeTable[i, 1];
            return ret;
        }

        /// <summary>
        /// 解码
        /// </summary>
        /// <param name="str">需要解码的字符串，每个编码需要空格隔开</param>
        /// <returns>解码后字符串</returns>
        public static string Dec(string str)
        {

            int i;
            string[] splits;
            string ret = string.Empty;
            if (str != null && (splits = str.Split(' ')).Length > 0)
            {
                foreach (string split in splits)
                    if ((i = Find(split, 1)) > -1)
                        ret += CodeTable[i, 0];
                return ret;
            }
            return "{#}";
        }

        /// <summary>
        /// 查表
        /// </summary>
        /// <param name="str">需要查找字符串</param>
        /// <param name="cols">查表方式 0：编码 1：解码</param>
        /// <returns></returns>
        private static int Find(string str, int cols)
        {
            int i = 0, len = CodeTable.Length / 2; // len / rank  
            while (i < len)
            {
                if (CodeTable[i, cols] == str)
                    return i;
                i++;
            };
            return -1;
        }

        /// <summary>
        /// 调用系统dll文件实现蜂鸣器播放
        /// </summary>
        /// <param name="frequency">声音频率</param>
        /// <param name="duration">播放时间</param>
        /// <returns></returns>
        [DllImport("Kernel32.dll")]
        public static extern bool Beep(int frequency, int duration);
        /// <summary>
        /// 使用电脑蜂鸣器播放声音
        /// </summary>
        /// <param name="_str">需要播放的摩尔斯密码</param>
        public static void Play(string _str)
        {
            foreach (char c in _str)
            {
                switch (c)
                {
                    case '-':
                        Beep(3000, 1000);        //3000HZ蜂鸣器响，时间300ms
                        Thread.Sleep(300);      //当前线程睡眠100ms
                        break;
                    case '.':
                        Beep(3000, 200);        //3000HZ蜂鸣器响，时间100ms
                        Thread.Sleep(100);      //当前线程睡眠100ms
                        break;
                    case ' ':
                        Thread.Sleep(300);      //当前线程睡眠300ms
                        break;
                }
            }
        }
    }
}