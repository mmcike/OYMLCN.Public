using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using OYMLCN;

namespace Microsoft.AspNetCore.Razor.TagHelpers
{
    /// <summary>
    /// TagHelper
    /// </summary>
    [HtmlTargetElement("__taghelper__")]
    public class TagHelperBase : TagHelper
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
        public bool IsEqualController(string controller) => controller.IsEqual(ViewContext.RouteData.Values["controller"]?.ToString());
        /// <summary>
        /// 传入属性值对比是否等于当前请求的Controller
        /// </summary>
        /// <param name="controllers"></param>
        /// <returns></returns>
        public bool IsEqualControllers(string controllers) => IsEqualControllers(controllers.SplitAuto());
        /// <summary>
        /// 传入属性值对比是否等于当前请求的Controller
        /// </summary>
        /// <param name="controllers"></param>
        /// <returns></returns>
        public bool IsEqualControllers(params string[] controllers) => controllers.Contains(ViewContext.RouteData.Values["controller"]?.ToString());
        /// <summary>
        /// 传入属性值对比是否等于当前请求的Action
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public bool IsEqualAction(string action) => action.IsEqual(ViewContext.RouteData.Values["action"]?.ToString());
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
    public class CDNImageHelper : TagHelperBase
    {
        string CDN_Url { get; set; }
        /// <summary>
        /// CDNImageHelper
        /// </summary>
        /// <param name="configuration"></param>
        public CDNImageHelper(IConfiguration configuration) =>
            CDN_Url = configuration.GetValue<string>("TencentCloud:CDN")?.TrimEnd('/');

        /// <summary>
        /// 若要使用，请在 appsettings 配置文件中配置 string TencentCloud:CDN 参数 
        /// </summary>
        [HtmlAttributeName("cdn-src")]
        public string Attribute { get; set; }

        /// <summary>
        /// Process
        /// </summary>
        /// <param name="context"></param>
        /// <param name="output"></param>
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.Attributes.SetAttribute("src", $"{CDN_Url}/{Attribute.TrimStart('~', '/')}");
            base.Process(context, output);
        }
    }
    [HtmlTargetElement("input", Attributes = "asp-checked")]
    public class InputCheckedHelper : TagHelperBase
    {
        [HtmlAttributeName("asp-checked")]
        public bool? Attribute { get; set; }

        /// <summary>
        /// Process
        /// </summary>
        /// <param name="context"></param>
        /// <param name="output"></param>
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (Attribute == true)
                output.Attributes.SetAttribute("checked", null);
            else
                output.RemoveAttribute("checked");

            base.Process(context, output);
        }
    }
    [HtmlTargetElement("option", Attributes = "asp-selected")]
    public class SelectOptionSelectedHelper : TagHelperBase
    {
        [HtmlAttributeName("asp-selected")]
        public bool? Attribute { get; set; }

        /// <summary>
        /// Process
        /// </summary>
        /// <param name="context"></param>
        /// <param name="output"></param>
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (Attribute == true)
                output.Attributes.SetAttribute("selected", null);
            else
                output.RemoveAttribute("selected");
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
        /// 移除并返回属性
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