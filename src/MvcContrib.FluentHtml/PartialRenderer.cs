using System;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Compilation;
using System.Web.Mvc;
using MvcContrib.FluentHtml.Elements;
using MvcContrib.FluentHtml.Expressions;

namespace MvcContrib.FluentHtml
{
    public class PartialRenderer<T, TPartialViewModel>
        where T : class
        where TPartialViewModel : class
    {
        private readonly IViewModelContainer<T> view;
        private readonly string partialViewName;
        private readonly Expression<Func<T, TPartialViewModel>> modelExpression;
        private ViewDataDictionary viewData;

        public PartialRenderer(IViewModelContainer<T> view, string partialViewName, Expression<Func<T, TPartialViewModel>> modelExpression)
        {
            this.view = view;
            this.partialViewName = partialViewName;
            this.modelExpression = modelExpression;
        }

        public PartialRenderer<T, TPartialViewModel> ViewData(ViewDataDictionary value)
        {
            viewData = value;
            return this;
        }

        public void Render()
        {
            var partialPath = GetPartialPath();
            var partial = BuildManager.CreateInstanceFromVirtualPath(partialPath, typeof(IViewModelContainer<TPartialViewModel>)) as IViewModelContainer<TPartialViewModel>;
            if (partial == null)
            {
                throw new InvalidOperationException("IViewModelContainer<T>.RenderPartial can only be used to render views that implement IViewModelContainer<T>");
            }

            partial.ViewData = GetViewData();
            partial.HtmlNamePrefix = GetHtmlNamePrefix();

            RenderPartial(partial);
        }

        protected virtual string GetPartialPath()
        {
            var partialView = ViewEngines.Engines.FindPartialView(view.Html.ViewContext, partialViewName).View;
            if (!(partialView is WebFormView))
            {
                throw new InvalidOperationException(string.Format("IViewModelContainer<T>.RenderPartial only supports WebFormViewEngine"));
            }
            return ((WebFormView)partialView).ViewPath;
        }

        private void RenderPartial(IViewModelContainer<TPartialViewModel> partial)
        {
            if (partial is ViewPage<TPartialViewModel>)
            {
                ((ViewPage<TPartialViewModel>)partial).RenderView(view.Html.ViewContext);
            }
            else if (partial is ViewUserControl<TPartialViewModel>)
            {
                ((ViewUserControl<TPartialViewModel>)partial).RenderView(view.Html.ViewContext);
            }
            else if (partial is ViewPage)
            {
                ((ViewPage)partial).RenderView(view.Html.ViewContext);
            }
            else if (partial is ViewUserControl)
            {
                ((ViewUserControl)partial).RenderView(view.Html.ViewContext);
            }
            else
            {
                throw new InvalidOperationException("IViewModelContainer<T>.RenderPartial can only be used to render views that are ViewPage<T>, ViewUserControl<T>, ViewPage or ViewUserControl.");
            }
        }

        private string GetHtmlNamePrefix()
        {
            return modelExpression == null
                ? null
                : modelExpression.GetNameFor(view);
        }

        /// <summary>
        /// Responsible for getting the derived partial, setting up associated data and
        /// invoking the partial.
        /// </summary>
        /// <param name="instanceType">instanceType being passed to the partial</param>
        public void RenderTypedPartial(Type instanceType)
        {
            var viewModelContainerType = typeof(IViewModelContainer<>);

            Type[] typeArgs = { instanceType };
            var containerType = viewModelContainerType.MakeGenericType(typeArgs);
            
            var partialPath = string.Empty;

            try
            {
                partialPath = GetPartialPath();
            }
            catch (InvalidOperationException invalidOperationException)
            {
                throw new InvalidOperationException(
                    string.Format("IViewModelContainer<T>.RenderTypedPartial only supports WebFormViewEngine"),
                    invalidOperationException);
            }

            var partial = BuildManager.CreateInstanceFromVirtualPath(partialPath, containerType);

            if (partial == null)
                throw new InvalidOperationException("Partial not found and was not caught by MVC");

            // make sure the container is properly typed
            var targetType = (from p in partial.GetType().GetInterfaces()
                              where p.IsGenericType
                                 && p.GetGenericTypeDefinition() == viewModelContainerType
                                 && p.GetGenericArguments()[0] == instanceType
                              select p).FirstOrDefault();

            if (targetType == null)
                throw new InvalidOperationException("IViewModelContainer<T>.RenderPartial can only be used to render views that implement IViewModelContainer<T>");

            // set up the partial for rendering with prefix data
            targetType.GetProperty("HtmlNamePrefix").SetValue(partial, GetHtmlNamePrefix(), null);
            typeof(IViewDataContainer).GetProperty("ViewData").SetValue(partial, GetViewData(), null);

            // render the partial
            var control = partial as ViewUserControl;

            if (control == null)
                throw new InvalidOperationException("Partials must be derived from ViewUserControl");

            view.Html.ViewContext.Writer.Write(new TypeStamp(GetHtmlNamePrefix(), instanceType).ToString());

            control.RenderView(view.Html.ViewContext);
        }


        private ViewDataDictionary GetViewData()
        {
            TPartialViewModel model = null;
            if (modelExpression != null)
            {
                model = modelExpression.Compile().Invoke(view.ViewModel);
            }
            return model == null
                ? viewData == null
                    ? new ViewDataDictionary(view.ViewData)
                    : new ViewDataDictionary(viewData)
                : viewData == null
                    ? new ViewDataDictionary(model)
                    : new ViewDataDictionary(viewData) { Model = model };
        }
    }
}