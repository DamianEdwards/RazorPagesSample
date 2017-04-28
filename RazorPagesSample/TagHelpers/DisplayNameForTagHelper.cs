using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace RazorPagesSample.TagHelpers
{
    [HtmlTargetElement("*", Attributes = "asp-display-name-for")]
    public class DisplayNameForTagHelper : TagHelper
    {
        private readonly IHtmlHelper _htmlHelper;

        public DisplayNameForTagHelper(IHtmlHelper htmlHelper)
        {
            _htmlHelper = htmlHelper;
        }

        [HtmlAttributeName("asp-display-name-for")]
        public ModelExpression For { get; set; }

        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            ((IViewContextAware)_htmlHelper).Contextualize(ViewContext);

            output.PostContent.AppendHtml(_htmlHelper.DisplayName(For.Metadata.PropertyName));
        }
    }
}
