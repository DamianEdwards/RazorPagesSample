using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace RazorPagesSample.TagHelpers
{
    [HtmlTargetElement("*", Attributes= "asp-if")]
    public class IfTagHelper : TagHelper
    {
        [HtmlAttributeName("asp-if")]
        public bool RenderTag { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (!RenderTag)
            {
                output.SuppressOutput();
            }
        }
    }
}
