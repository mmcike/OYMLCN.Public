using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace OYMLCN.Web.Mvc
{
    /// <summary>
    /// TagHelper
    /// </summary>
    [HtmlTargetElement("taghelper")]
    public class TagHelper : Microsoft.AspNetCore.Razor.TagHelpers.TagHelper
    {
        /// <summary>
        /// ViewContext
        /// </summary>
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        /// <summary>
        /// 传入属性值对比是否等于当前请求的Controller
        /// </summary>
        /// <param name="controller"></param>
        /// <returns></returns>
        public bool IsEqualController(string controller) => controller.IsEqual(ViewContext.RouteData.Values["controller"]?.ToString(), StringComparison.OrdinalIgnoreCase);
        /// <summary>
        /// 传入属性值对比是否等于当前请求的Action
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public bool IsEqualAction(string action) => action.IsEqual(ViewContext.RouteData.Values["action"]?.ToString(), StringComparison.OrdinalIgnoreCase);
        /// <summary>
        /// 传入属性值对比是否等于当前请求的Action与Controller
        /// </summary>
        /// <param name="action"></param>
        /// <param name="controller"></param>
        /// <returns></returns>
        public bool IsEqualAction(string action, string controller) => IsEqualController(controller) && IsEqualAction(action);
    }

    /// <summary>
    /// CDNImageHelper
    /// </summary>
    [HtmlTargetElement("img", Attributes = "cdn-src")]
    public class CDNImageHelper : TagHelper
    {
        /// <summary>
        /// CDNImageHelper
        /// </summary>
        /// <param name="configuration"></param>
        public CDNImageHelper(IConfiguration configuration) => Configuration = configuration;
        IConfiguration Configuration { get; set; }

        /// <summary>
        /// 若要使用，请在 appsettings 配置文件中配置 QCloud:CDNHost 参数 
        /// </summary>
        /// <example>
        /// eg:
        /// {
        ///   "QCloud": {
        ///     "CDNHost": "//host"
        ///   }
        /// }
        /// </example>
        [HtmlAttributeName("cdn-src")]
        public string Attribute { get; set; }

        /// <summary>
        /// Process
        /// </summary>
        /// <param name="context"></param>
        /// <param name="output"></param>
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.RemoveAttribute("src");

            output.Attributes.Add("src", $"{Configuration.GetValue<string>("QCloud:CDNHost").TrimEnd('/')}/{Attribute.TrimStart('~', '/')}");
            base.Process(context, output);
        }
    }

}

namespace OYMLCN
{
    /// <summary>
    /// TagHelperExtension
    /// </summary>
    public static class TagHelperExtension
    {
        /// <summary>
        /// 获取属性
        /// </summary>
        /// <param name="output"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static TagHelperAttribute GetAttribute(this TagHelperOutput output, string name) =>
            output.Attributes.Where(d => d.Name.IsEqualIgnoreCase(name)).FirstOrDefault();
        /// <summary>
        /// 移除属性
        /// </summary>
        /// <param name="output"></param>
        /// <param name="name"></param>
        /// <returns>被移除的属性</returns>
        public static TagHelperAttribute RemoveAttribute(this TagHelperOutput output, string name)
        {
            var old = output.GetAttribute(name);
            if (old.IsNotNull())
                output.Attributes.Remove(old);
            return old;
        }


        /// <summary>
        /// 添加 Class 属性
        /// </summary>
        /// <param name="output"></param>
        /// <param name="className"></param>
        public static void AddClass(this TagHelperOutput output, string className)
        {
            var classNames = output.RemoveAttribute("class")?.Value.ToString().SplitBySign(" ").ToList() ?? new List<string>();
            classNames.Add(className);
            output.Attributes.Add("class", classNames.Distinct().Join(" "));
        }
    }
}