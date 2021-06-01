using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Model
{
    /// <summary>
    /// api返回值
    /// ErLin Li
    /// 2021/4/15
    /// </summary>
    [DataContract]
    public class ApiMessageEntity
    {
        /// <summary>
        /// true 成功 fasle 失败
        /// </summary>
        public bool requstResult
        { get; set; }

        /// <summary>
        /// 要返回的数据
        /// </summary>
        public object data
        { get; set; }

        /// <summary>
        /// 返回条数
        /// </summary>
        public int total
        { get; set; }

        public string msg { get; set; }

        /// <summary>
        /// 请求成功
        /// </summary>
        /// <param name="data">返回的数据</param>
        /// <param name="total">返回的数据条数</param>
        /// <returns></returns>
        public static ApiMessageEntity Ok(object data, int total = 0)
        {
            return new ApiMessageEntity { requstResult = true, data = data, total = total };
        }

        public static ApiMessageEntity Ok()
            => new ApiMessageEntity { requstResult = true };

        public static ApiMessageEntity Ok(string msg)
            => new ApiMessageEntity { requstResult = true, msg = msg };

        public static ApiMessageEntity Ok(string msg, object obj)
            => new ApiMessageEntity { requstResult = true, msg = msg, data = obj };

        /// <summary>
        /// 请求成功
        /// </summary>
        /// <param name="data">返回的数据</param>
        /// <returns></returns>
        public static ApiMessageEntity Ok(object data)
        {
            return new ApiMessageEntity { requstResult = true, data = data };
        }

        /// <summary>
        /// 失败
        /// </summary>
        /// <param name="str">错误信息</param>
        /// <param name="code">状态码</param>
        /// <returns></returns>
        public static ApiMessageEntity DefaultError()
        {
            return new ApiMessageEntity { requstResult = false, msg = "服务出错了" };
        }

        /// <summary>
        /// 失败
        /// </summary>
        /// <param name="str">错误信息</param>
        /// <param name="code">状态码</param>
        /// <returns></returns>
        public static ApiMessageEntity Error(string str)
        {
            return new ApiMessageEntity { requstResult = false, msg = str };
        }

        /// <summary>
        /// 失败
        /// </summary>
        /// <param name="str">错误信息</param>
        /// <param name="code">状态码</param>
        /// <returns></returns>
        public static ApiMessageEntity Error(Exception exception)
        {
            return new ApiMessageEntity { requstResult = false, msg = JsonConvert.SerializeObject(exception) };
        }
    }
}