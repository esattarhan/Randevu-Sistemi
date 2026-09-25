using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApplication4.Extensions
{
    public static class Extension // Mutlaka static olmalı
    {
        public static IHtmlContent CustomTextBox(this IHtmlHelper htmlHelper, string name, string value)
        {
            // return ekledik ki ekrana içeriği yollasın
            return htmlHelper.TextBox(name, value, new
            {
                style = "background-color:green; color:white; font-size:11px;",
                @class = "form-input"
            });
        }
    }
}