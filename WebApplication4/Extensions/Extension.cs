using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Runtime.CompilerServices;

namespace WebApplication4.Extensions
{
    public class Extension
    {
        public static IHtmlContent CustomTextBox(this IHtmlHelper htmlHelper, string name , string value)
        {
            htmlHelper.TextBox(name, value = "sifrenizi giriniz", new
            {
                style = "background-color:green;color:white,front-size:11px;",
                @class = "from-input",
                a = "a",
                b = "b",
            });
        }
            

    }
}
