using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Appxs.Exceptions
{
    public static class Extensions
    {
        public static void UseExceptionsMiddlewares(this IApplicationBuilder app)
        {
            app.UseMiddleware<Middlewares>();
        }
    }
}