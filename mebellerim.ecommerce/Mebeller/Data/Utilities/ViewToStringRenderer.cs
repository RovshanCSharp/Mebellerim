using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Mebeller.Data.Utilities;

public static class ViewToStringRenderer
{
    public static async Task<string> RenderViewToStringAsync<TModel>(IServiceProvider requestServices, string viewName,
        TModel model)
    {
        var viewEngine =
            requestServices.GetRequiredService(typeof(IRazorViewEngine)) as IRazorViewEngine;
        var viewEngineResult =
            viewEngine?.GetView(null, viewName, false);
        if (viewEngineResult?.View == null)
            if (viewEngineResult?.SearchedLocations != null)
                throw new Exception("Could not find the View file. Searched locations:\r\n" +
                                    string.Join("\r\n", viewEngineResult.SearchedLocations));

        var view = viewEngineResult?.View;
        var httpContextAccessor =
            (IHttpContextAccessor)requestServices.GetRequiredService(typeof(IHttpContextAccessor));
        var actionContext =
            new ActionContext(httpContextAccessor.HttpContext ?? throw new InvalidOperationException(), new RouteData(), new ActionDescriptor());
        var tempDataProvider =
            requestServices.GetRequiredService(typeof(ITempDataProvider)) as ITempDataProvider;

        using var outputStringWriter =
            new StringWriter();
        if (view != null && tempDataProvider != null)
        {
            var viewContext = new ViewContext(
                actionContext,
                view,
                new ViewDataDictionary<TModel>(new EmptyModelMetadataProvider(), new ModelStateDictionary())
                    { Model = model },
                new TempDataDictionary(actionContext.HttpContext, tempDataProvider),
                outputStringWriter,
                new HtmlHelperOptions());

            await view.RenderAsync(viewContext);
        }

        return outputStringWriter.ToString();
    }
}