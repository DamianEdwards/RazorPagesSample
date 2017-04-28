using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace RazorPagesSample.TagHelpers
{
    [HtmlTargetElement("*", Attributes = "asp-display-for")]
    public class DisplayForTagHelper : TagHelper
    {
        [HtmlAttributeName("asp-display-for")]
        public ModelExpression For { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            // HACK: Too hard to pull out the full display template version using ModelExpression
            output.PostContent.AppendHtml(For.ModelExplorer.GetSimpleDisplayText());
        }
    }
}
