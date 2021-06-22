using Autofac.Core;
using System.Linq;
using System.Reflection;

namespace AutofacModule
{
    /// <summary>
    /// 属性注入选择器
    /// </summary>
    public class AutowiredPropertySelector : IPropertySelector
    {
        /// <summary>
        /// 判断哪些属性需要用属性注入
        /// </summary>
        /// <param name="propertyInfo"></param>
        /// <param name="instance"></param>
        /// <returns></returns>
        public bool InjectProperty(PropertyInfo propertyInfo, object instance)
        {
            // 带有 AutowiredAttribute 特性的属性会进行属性注入
            return propertyInfo.CustomAttributes.Any(it => it.AttributeType == typeof(PropertyAttribute));
        }
    }
}