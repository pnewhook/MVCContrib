using System;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting.Web;
using System.Collections.Specialized;

namespace T4MVCHostMvcApp.Tests
{
    
    
    /// <summary>
    ///This is a test class for T4ExtensionsTest and is intended
    ///to contain all T4ExtensionsTest Unit Tests
    ///</summary>
    [TestClass()]
    public class T4MVCTest {
        private TestContext testContextInstance;
        private static HtmlHelper Html { get; set; }

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext {
            get {
                return testContextInstance;
            }
            set {
                testContextInstance = value;
            }
        }


        class TestViewDataContainer : IViewDataContainer {
            #region IViewDataContainer Members

            public ViewDataDictionary ViewData {
                get {
                    throw new System.NotImplementedException();
                }
                set {
                    throw new System.NotImplementedException();
                }
            }

            #endregion
        }


        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext) {
            T4MVCHelpers.ProcessVirtualPath = ProcessVirtualPath;

            Html = new HtmlHelper(new ViewContext(), new TestViewDataContainer());
        }


        // API TESTS

        [TestMethod()]
        public void TestInitMVCT4Result() {
            var result = (IT4MVCActionResult)MVC.Home.About();
            string area = "some area";
            string controller = "some controller";
            string action = "some action";
            T4Extensions.InitMVCT4Result(result, area, controller, action);

            Assert.AreEqual(controller, result.Controller);
            Assert.AreEqual(controller, result.RouteValueDictionary["controller"]);
            Assert.AreEqual(action, result.Action);
            Assert.AreEqual(action, result.RouteValueDictionary["action"]);
            Assert.AreEqual(area, result.RouteValueDictionary["area"]);
        }


        // AREA NAMES TESTS

        [TestMethod()]
        public void TestAreaNameConstants() {
            Assert.AreEqual("Home", MVC.HomeArea.Name);
            Assert.AreEqual("break", MVC.@break.Name);
        }

        [TestMethod()]
        public void TestAreaNameConstantsViewController() {
            Assert.AreEqual("", MVC.T4Ctrl.Area);
            Assert.AreEqual("Home", MVC.HomeArea.Home.Area);
            Assert.AreEqual("break", MVC.@break.Post.Area);
        }


        // CONTROLLER NAMES TESTS

        [TestMethod()]
        public void TestControllerName() {
            Assert.AreEqual("Home", MVC.Home.Name);
        }

        [TestMethod()]
        public void TestAreaControllerName() {
            Assert.AreEqual("Home", MVC.HomeArea.Home.Name);
            Assert.AreEqual("Post", MVC.@break.Post.Name);
        }

        [TestMethod()]
        public void TestT4SubFolderControllerName() {
            Assert.AreEqual("T4Ctrl", MVC.T4Ctrl.Name);
        }


        // ACTION NAMES TESTS

        [TestMethod()]
        public void TestSimpleActionName() {
            Assert.AreEqual("Index", MVC.Home.ActionNames.Index);
        }

        [TestMethod()]
        public void TestRenamedActionName() {
            Assert.AreEqual("The About Action", MVC.Home.ActionNames.About);
            Assert.AreEqual("New-Name for Blah", MVC.Home.ActionNames.Blah);
        }

        [TestMethod()]
        public void TestT4SubFolderControlleActionName() {
            Assert.AreEqual("Qqq", MVC.T4Ctrl.ActionNames.Qqq);
        }

        [TestMethod()]
        public void TestAreaActionName() {
            Assert.AreEqual("The Index", MVC.HomeArea.Home.ActionNames.Index);
        }



        // VIEW PATHS TESTS

        [TestMethod()]
        public void TestSimpleViewName() {
            Assert.AreEqual("~/Views/Home/Index.aspx", MVC.Home.Views.Index);
        }

        [TestMethod()]
        public void TestConflictingViewNames() {
            Assert.AreEqual("~/Views/Home/Qqq.txt", MVC.Home.Views.Qqq);
            Assert.AreEqual("~/Views/Home/QqQ.txt2", MVC.Home.Views.QqQ);
            Assert.AreEqual("~/Views/Home/Qqq.txt3", MVC.Home.Views.Qqq_txt3);
        }

        [TestMethod()]
        public void TestComplexViewName() {
            Assert.AreEqual("~/Views/Home/7 Some Home.View-Hello.txt", MVC.Home.Views._7_Some_Home_View_Hello);
        }

        [TestMethod()]
        public void TestNestedViewNameWithSameNameAsParentFolder() {
            Assert.AreEqual("~/Views/Home/Sub Home/Qqq.txt", MVC.Home.Views.Sub_Home.Qqq);
        }

        [TestMethod()]
        public void TestViewNameMatchingLanguageKeyword() {
            Assert.AreEqual("~/Views/Home/Sub Home/string.txt", MVC.Home.Views.Sub_Home.@string);
        }

        [TestMethod()]
        public void TestSuperNestedViewWithComplexName() {
            Assert.AreEqual("~/Views/Home/Sub Home/Nested-Sub/99 Super~Nested-View.txt", MVC.Home.Views.Sub_Home.Nested_Sub._99_Super_Nested_View);
        }

        [TestMethod()]
        public void TestViewThatGeneratesFile() {
            Assert.AreEqual("~/Views/Home/Sub Home/T4View.tt", MVC.Home.Views.Sub_Home.T4View);
        }

        [TestMethod()]
        public void TestSharedView() {
            Assert.AreEqual("~/Views/Shared/LogOnUserControl.ascx", MVC.Shared.Views.LogOnUserControl);
        }

        [TestMethod()]
        public void TestAreaView() {
            Assert.AreEqual("~/Areas/Home/Views/Home/SomeHomeView.txt", MVC.HomeArea.Home.Views.SomeHomeView);
        }

        [TestMethod()]
        public void TestAreaSharedView() {
            Assert.AreEqual("~/Areas/break/Views/Shared/SharedAreaView.txt", MVC.@break.Shared.Views.SharedAreaView);
        }



        // ROUTE VALUES TESTS

        [TestMethod()]
        public void TestRouteValuesForDefaultAreaNoParamAction() {
            var actionRes = (IT4MVCActionResult)MVC.T4Ctrl.Qqq();

            TestAreaControllerActionNames(actionRes, "", "T4Ctrl", "Qqq");
        }

        [TestMethod()]
        public void TestRouteValuesForRenamedActionWithParams() {
            var actionRes = (IT4MVCActionResult)MVC.Home.Blah("Hello", 123);

            TestAreaControllerActionNames(actionRes, "", "Home", "New-Name for Blah");
            TestRouteValue(actionRes, "name", "Hello");
            TestRouteValue(actionRes, "age", 123);
        }

        [TestMethod()]
        public void TestRouteValuesForRenamedActionUsingGeneratedNoParamOverload() {
            var actionRes = (IT4MVCActionResult)MVC.Home.Blah();

            TestAreaControllerActionNames(actionRes, "", "Home", "New-Name for Blah");
            TestRouteValue(actionRes, "name", null);
            TestRouteValue(actionRes, "age", null);
        }

#if NOTYET
        [TestMethod()]
        public void TestRouteValuesForActionWithObjectParam() {
            var actionRes = (IT4MVCActionResult)MVC.@break.Post.ActionThatTakesAnObject(
                new T4MVCHostMvcApp.Areas.Break.Controllers.MyParamObject() {
                    Name = "David",
                    Age = 123
                }
            );

            TestAreaControllerActionNames(actionRes, "break", "Post", "ActionThatTakesAnObject");
            TestRouteValue(actionRes, "Name", "David");
            TestRouteValue(actionRes, "Age", 111);
        }
#endif

        [TestMethod()]
        public void TestRouteValuesForActionWithBindPrefixAttrib() {
            var actionRes = (IT4MVCActionResult)MVC.@break.Post.ActionWithBindPrefixAttribute("Hello");

            TestAreaControllerActionNames(actionRes, "break", "Post", "ActionWithBindPrefixAttribute");
            TestRouteValue(actionRes, "newParamName", "Hello");
        }

        [TestMethod()]
        public void TestRouteValuesForActionWithBindNoPrefixAttrib() {
            var actionRes = (IT4MVCActionResult)MVC.@break.Post.ActionWithBindNoPrefixAttribute("Hello");

            TestAreaControllerActionNames(actionRes, "break", "Post", "ActionWithBindNoPrefixAttribute");
            TestRouteValue(actionRes, "fieldName", "Hello");
        }

        [TestMethod()]
        public void TestRouteValuesForActionWithArrayParam() {
            var strings = new string[] { "cat", "dog" };
            var actionRes = (IT4MVCActionResult)MVC.Home.ActionWithArrayParam(strings);

            TestAreaControllerActionNames(actionRes, "", "Home", "ActionWithArrayParam");
            TestRouteValue(actionRes, "someStrings", strings);
        }

        [TestMethod()]
        public void TestRouteValuesForAreaNoParamAction() {
            var actionRes = (IT4MVCActionResult)MVC.@break.Post.Index();

            TestAreaControllerActionNames(actionRes, "break", "Post", "Index");
        }

        [TestMethod()]
        public void TestRouteValuesForSameAssemblyBaseAction() {
            var actionRes = (IT4MVCActionResult)MVC.@break.Post.SameProjectBaseControllerMethod("Hello");

            TestAreaControllerActionNames(actionRes, "break", "Post", "SameProjectBaseControllerMethod");
            TestRouteValue(actionRes, "s", "Hello");
        }

        [TestMethod()]
        public void TestRouteValuesForCompiledBaseAction() {
            var actionRes = (IT4MVCActionResult)MVC.@break.Post.CompiledControllerVirtualMethod(17);

            TestAreaControllerActionNames(actionRes, "break", "Post", "CompiledControllerVirtualMethod");
            TestRouteValue(actionRes, "n", 17);
        }

        [TestMethod()]
        public void TestRouteValuesForDerivedBuiltInActionResultTypes() {
            var viewResultActionRes = (IT4MVCActionResult)MVC.Home.SomeViewResultAction();
            var jsonResultActionRes = (IT4MVCActionResult)MVC.Home.SomeJsonResultAction();
            var fileContentResultActionRes = (IT4MVCActionResult)MVC.Home.SomeFileContentResultAction();
            //var fileStreamResultActionRes = (IT4MVCActionResult)MVC.Home.SomeFileStreamResultAction();    // Throws null ref exception!
            var fileResultActionRes = (IT4MVCActionResult)MVC.Home.SomeFileResultAction();

            TestAreaControllerActionNames(viewResultActionRes, "", "Home", "SomeViewResultAction");
            TestAreaControllerActionNames(jsonResultActionRes, "", "Home", "SomeJsonResultAction");
            TestAreaControllerActionNames(fileContentResultActionRes, "", "Home", "SomeFileContentResultAction");
            //TestAreaControllerActionNames(fileStreamResultActionRes, "", "Home", "SomeFileStreamResultAction");
            TestAreaControllerActionNames(fileResultActionRes, "", "Home", "SomeFileResultAction");
        }

        [TestMethod()]
        public void TestRouteValuesForCustomActionResultType() {
            var actionRes = (IT4MVCActionResult)MVC.Home.SomeCustomResultAction();

            TestAreaControllerActionNames(actionRes, "", "Home", "SomeCustomResultAction");
        }

        [TestMethod()]
        public void TestRouteValuesWithAddedValues() {
            var actionRes = (IT4MVCActionResult)MVC.Home.Index().AddRouteValues(new { foo1 = "bar", foo2 = 234 });

            TestAreaControllerActionNames(actionRes, "", "Home", "Index");
            TestRouteValue(actionRes, "foo1", "bar");
            TestRouteValue(actionRes, "foo2", 234);
        }

        [TestMethod()]
        public void TestRouteValuesWithAddedValue() {
            var actionRes = (IT4MVCActionResult)MVC.Home.Index().AddRouteValue("foo", "bar");

            TestAreaControllerActionNames(actionRes, "", "Home", "Index");
            TestRouteValue(actionRes, "foo", "bar");
        }

        [TestMethod()]
        public void TestRouteValuesWithAddedValuesUsingGeneratedNoParamOverload() {
            var actionRes = (IT4MVCActionResult)MVC.Home.Blah().AddRouteValue("name", "Hello").AddRouteValues(new { age = 123, foo = true });

            TestAreaControllerActionNames(actionRes, "", "Home", "New-Name for Blah");
            TestRouteValue(actionRes, "name", "Hello");
            TestRouteValue(actionRes, "age", 123);
            TestRouteValue(actionRes, "foo", true);
        }

        [TestMethod()]
        public void TestRouteValuesFromNameValueCollection() {
            var nameValueCollection = new NameValueCollection();
            nameValueCollection["key1"] = "val1";
            nameValueCollection["key2"] = "val2";
            nameValueCollection["key3"] = "val3";

            var actionRes = (IT4MVCActionResult)MVC.Home.Blah().AddRouteValue("name", "Hello").AddRouteValues(nameValueCollection);

            TestAreaControllerActionNames(actionRes, "", "Home", "New-Name for Blah");
            TestRouteValue(actionRes, "name", "Hello");
            TestRouteValue(actionRes, "key1", "val1");
            TestRouteValue(actionRes, "key2", "val2");
            TestRouteValue(actionRes, "key3", "val3");
        }

        [TestMethod()]
        [ExpectedExceptionAttribute(typeof(InvalidOperationException), "")]
        public void TestErrorHandlingWhenPassingRealControllerAction() {
            var controller = new T4MVCHostMvcApp.Controllers.HomeController();

            Html.ActionLink("Test", controller.SomeViewResultAction());
        }


        // STATIC FILES TESTS

        [TestMethod()]
        public void TestLinkWithComplexFileName() {
            Assert.AreEqual("/Content/7 My.Text-File Space.txt", Links.Content._7_My_Text_File_Space_txt);
        }

        [TestMethod()]
        public void TestLinkInFolderNamedAfterKeyword() {
            Assert.AreEqual("/Content/default/Zzz.txt", Links.Content.@default.Zzz_txt);
        }

        [TestMethod()]
        public void TestLinkToT4File() {
            Assert.AreEqual("/Content/Sub Content-folder.test/SomeT4.tt", Links.Content.Sub_Content_folder_test.SomeT4_tt);
        }

        [TestMethod()]
        public void TestScriptFile() {
            Assert.AreEqual("/Scripts/jquery-1.3.2.js", Links.Scripts.jquery_1_3_2_js);
        }

        [TestMethod()]
        public void TestNoLinkGeneratedForIgnoredExtension() {
            var field = typeof(Links.Content).GetField("ShouldNotBeALink_cs");

            Assert.AreEqual(null, field);
        }


        // HELPER METHODS

        private void TestAreaControllerActionNames(IT4MVCActionResult actionResult, string area, string controller, string action) {
            TestRouteValue(actionResult, "area", area);
            TestRouteValue(actionResult, "controller", controller);
            TestRouteValue(actionResult, "action", action);
        }

        private void TestRouteValue(IT4MVCActionResult actionResult, string name, object value) {
            Assert.AreEqual(value, actionResult.RouteValueDictionary[name]);
        }

        private static string ProcessVirtualPath(string virtualPath) {
            // The path that comes in starts with ~/ and must first be made absolute
            if (virtualPath.StartsWith("~/"))
                virtualPath = virtualPath.Substring(1);

            return virtualPath;
        }
    }
}
