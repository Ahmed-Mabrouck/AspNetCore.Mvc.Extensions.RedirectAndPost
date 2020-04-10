using Microsoft.AspNetCore.Mvc;
using System;

namespace AspNetCore.Mvc.Extensions.RedirectAndPost
{
    public static class Extensions
    {
        public static RedirectAndPostActionResult<TData> RedirectAndPost<TData>(this Controller controller, string url, TData data) where TData : new()
        {
            if (controller is null)
            {
                throw new ArgumentNullException(nameof(controller), $"{nameof(controller)} cannot be null.");
            }

            return new RedirectAndPostActionResult<TData>(url, data);
        }
    }
}
