using System;
using System.Collections.Specialized;
using System.Web;

namespace MvcContrib.TestHelper.ControllerBuilderStrategies
{
	public class MoqControllerBuilder : IControllerBuilderStrategy
	{
		private static Exception _loadException;
		private static MoqProxy _proxy;

		static MoqControllerBuilder()
		{
			try
			{
				_proxy = new MoqProxy();
			}
			catch (Exception ex)
			{
				_loadException = ex;
			}
			
		}

		public void Setup(TestControllerBuilder testControllerBuilder)
		{
			if (_proxy == null)
			{
				throw new InvalidOperationException("Cannot use MoqControllerBuilder because an error occured while loading Moq.",
				                                    _loadException);
			}

			var httpContext = _proxy.DynamicMock<HttpContextBase>();

			var request = _proxy.DynamicMock<HttpRequestBase>();
			var response = _proxy.DynamicMock<HttpResponseBase>();
			var server = _proxy.DynamicMock<HttpServerUtilityBase>();
			var cache = HttpRuntime.Cache;

			httpContext.ReturnFor(c => c.Request, request.Object);
			httpContext.ReturnFor(c => c.Response, response.Object);
			httpContext.ReturnFor(c => c.Session, testControllerBuilder.Session);
			httpContext.ReturnFor(c => c.Server, server.Object);
			httpContext.ReturnFor(c => c.Cache, cache);
			httpContext.SetupProperty(c => c.User);

			request.ReturnFor(r => r.QueryString, testControllerBuilder.QueryString);
			request.ReturnFor(r => r.Form, testControllerBuilder.Form);
			request.ReturnFor(r => r.Files, (HttpFileCollectionBase)testControllerBuilder.Files);
			request.CallbackFor(r => r.AcceptTypes, () => testControllerBuilder.AcceptTypes);
			request.CallbackFor(r => r.Params, () => new NameValueCollection { testControllerBuilder.QueryString, testControllerBuilder.Form });
			request.CallbackFor(r => r.AppRelativeCurrentExecutionFilePath, () => testControllerBuilder.AppRelativeCurrentExecutionFilePath);
			request.CallbackFor(r => r.ApplicationPath, () => testControllerBuilder.ApplicationPath);
			request.CallbackFor(r => r.PathInfo, () => testControllerBuilder.PathInfo);
			request.CallbackFor(r => r.RawUrl, () => testControllerBuilder.RawUrl);
			response.SetupProperty(r => r.Status);

			testControllerBuilder.HttpContext = httpContext.Object;
		}
	}
}