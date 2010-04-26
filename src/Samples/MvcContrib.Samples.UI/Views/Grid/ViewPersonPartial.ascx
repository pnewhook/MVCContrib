<%@ Import Namespace="MvcContrib.Samples.UI.Models"%>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Person>" %>
<%= Html.ActionLink("View Person", "Show", new { id = Model.Id })%>
