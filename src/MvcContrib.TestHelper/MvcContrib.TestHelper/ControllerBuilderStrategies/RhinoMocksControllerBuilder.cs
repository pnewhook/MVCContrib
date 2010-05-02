using System;
using System.Collections.Specialized;
using System.Web;
using Rhino.Mocks;

namespace MvcContrib.TestHelper.ControllerBuilderStrategies
{
	public class RhinoMocksControllerBuilder : IControllerBuilderStrategy 
	{
		protected MockRepository _mocks = new MockRepository();

		public void Setup(TestControllerBuilder testControllerBuilder)
		{
			var httpContext = _mocks.DynamicMock<HttpContextBase>();

			var request = _mocks.DynamicMock<HttpRequestBase>();
			var response = _mocks.DynamicMock<HttpResponseBase>();
			var server = _mocks.DynamicMock<HttpServerUtilityBase>();
			var cache = HttpRuntime.Cache;

			SetupResult.For(httpContext.Request).Return(request);
			SetupResult.For(httpContext.Response).Return(response);
			SetupResult.For(httpContext.Session).Return(testControllerBuilder.Session);
			SetupResult.For(httpContext.Server).Return(server);
			SetupResult.For(httpContext.Cache).Return(cache);

			SetupResult.For(request.QueryString).Return(testControllerBuilder.QueryString);
			SetupResult.For(request.Form).Return(testControllerBuilder.Form);
			SetupResult.For(request.AcceptTypes).Do((Func<string[]>)(() => testControllerBuilder.AcceptTypes));
			SetupResult.For(request.Files).Return((HttpFileCollectionBase)testControllerBuilder.Files);

			Func<NameValueCollection> paramsFunc = () => new NameValueCollection { testControllerBuilder.QueryString, testControllerBuilder.Form };
			SetupResult.For(request.Params).Do(paramsFunc);

			SetupResult.For(request.AppRelativeCurrentExecutionFilePath).Do(
				(Func<string>)(() => testControllerBuilder.AppRelativeCurrentExecutionFilePath));
			SetupResult.For(request.ApplicationPath).Do((Func<string>)(() => testControllerBuilder.ApplicationPath));
			SetupResult.For(request.PathInfo).Do((Func<string>)(() => testControllerBuilder.PathInfo));
			SetupResult.For(request.RawUrl).Do((Func<string>)(() => testControllerBuilder.RawUrl));
			SetupResult.For(response.Status).PropertyBehavior();
			SetupResult.For(httpContext.User).PropertyBehavior();

			_mocks.Replay(httpContext);
			_mocks.Replay(request);
			_mocks.Replay(response);

			testControllerBuilder.HttpContext = httpContext;
		}
	}
}