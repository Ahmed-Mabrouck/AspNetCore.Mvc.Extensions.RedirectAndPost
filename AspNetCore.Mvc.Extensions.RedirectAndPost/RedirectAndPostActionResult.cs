using AspNetCore.Mvc.Extensions.RedirectAndPost.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCore.Mvc.Extensions.RedirectAndPost
{
    public class RedirectAndPostActionResult<TData> : ActionResult where TData : new()
    {
        private readonly Uri url;
        private readonly TData data;

        public RedirectAndPostActionResult(string url, TData data)
        {
            this.url = new Uri(url);
            this.data = data;
        }

        public async override Task ExecuteResultAsync(ActionContext context)
        {
            var html = GenerateHtmlPostForm(url, data);
            context.HttpContext.Response.ContentType = "text/html";
            await context.HttpContext.Response.WriteAsync(html);
        }

        private string GenerateHtmlPostForm(Uri url, TData data)
        {
            string result = String.Empty;
            var id = StringHelpers.RandomString(12);

            var pairs = SerializeAsHtmlFormInputKeyPairs(data);

            var builder = new StringBuilder();
            builder.Append($"<form id=\"{id}\" name=\"{0}\" action=\"{url}\" method=\"POST\">");
            foreach (var element in pairs)
            {
                builder.Append($"<input type=\"hidden\" name=\"{element.Key}\" value=\"{element.Value}\"/>");
            }
            builder.Append("</form>")
                .Append("<script language=\"javascript\">")
                .Append($"var f{id} = document.{id};")
                .Append($"f{id}.submit();")
                .Append("</script>");

            return result;
        }

        private List<KeyValuePair<string, object>> SerializeAsHtmlFormInputKeyPairs(TData data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data), $"{nameof(data)} cannot be null.");
            }

            var sourceType = data.GetType();

            if (sourceType.IsPrimitive || sourceType == typeof(Decimal) || sourceType == typeof(String))
            {
                throw new ArgumentException(nameof(data), $"{nameof(data)} cannot be primitive data type or string.");
            }

            var pairs = new List<KeyValuePair<string, object>>();
            FalttenAndCreateHtmlFormInputsKeyPairs(pairs, JToken.FromObject(data), null);
            return pairs;
        }

        private void FalttenAndCreateHtmlFormInputsKeyPairs(in List<KeyValuePair<string, object>> pairs, JToken token, string prefix)
        {

            switch (token.Type)
            {
                case JTokenType.Object:
                    foreach (var property in token.Children<JProperty>())
                    {
                        FalttenAndCreateHtmlFormInputsKeyPairs(pairs, property.Value, StringHelpers.Join(prefix, property.Name));
                    }
                    break;

                case JTokenType.Array:
                    foreach (JToken value in token.Children())
                    {
                        FalttenAndCreateHtmlFormInputsKeyPairs(pairs, value, StringHelpers.Join(prefix, String.Empty));
                    }
                    break;

                default:
                    pairs.Add(new KeyValuePair<string, object>(prefix, ((JValue)token).Value));
                    break;
            }
        }
    }
}
