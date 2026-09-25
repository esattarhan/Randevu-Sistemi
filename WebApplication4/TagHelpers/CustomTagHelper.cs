using Microsoft.AspNetCore.Razor.TagHelpers;

namespace WebApplication4.TagHelpers
{
    [HtmlTargetElement("input", Attributes = "asp-custom")]
    public class CustomTagHelper : TagHelper
    {
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.Attributes.SetAttribute("style", "background-color:green; color:white;");
            output.Attributes.SetAttribute("class", "form-input");
        }
    }
}