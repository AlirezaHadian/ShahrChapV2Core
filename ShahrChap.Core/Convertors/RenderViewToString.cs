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

namespace ShahrChap.Core.Convertors
{
    public interface IViewRenderService
    {
        Task<string> RenderToStringAsync(string viewName, object model);
    }
    public class RenderViewToString : IViewRenderService
    {
        private readonly IRazorViewEngine _razorViewEngine;
        private readonly ITempDataProvider _tempDataProvider;
        private readonly IServiceProvider _serviceProvider;

        public RenderViewToString(IRazorViewEngine razorViewEngine,
            ITempDataProvider tempDataProvider,
            IServiceProvider serviceProvider)
        {
            _razorViewEngine = razorViewEngine;
            _tempDataProvider = tempDataProvider;
            _serviceProvider = serviceProvider;
        }

        public async Task<string> RenderToStringAsync(string viewName, object model)
        {
            var httpContext = new DefaultHttpContext
            {
                RequestServices = _serviceProvider
            };

            var actionContext = new ActionContext(
                httpContext,
                new RouteData(),
                new ActionDescriptor()
            );

            var viewResult = _razorViewEngine.FindView(
                actionContext,
                viewName,
                isMainPage: false
            );

            if (!viewResult.Success)
            {
                throw new InvalidOperationException(
                    $"View '{viewName}' پیدا نشد."
                );
            }

            using var sw = new StringWriter();

            var viewData = new ViewDataDictionary(
                new EmptyModelMetadataProvider(),
                new ModelStateDictionary()
            )
            {
                Model = model
            };

            var tempData = new TempDataDictionary(
                actionContext.HttpContext,
                _tempDataProvider
            );

            var viewContext = new ViewContext(
                actionContext,
                viewResult.View,
                viewData,
                tempData,
                sw,
                new HtmlHelperOptions()
            );

            await viewResult.View.RenderAsync(viewContext);

            return sw.ToString();
        }
    }

}