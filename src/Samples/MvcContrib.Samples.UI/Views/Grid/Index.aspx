<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<Person>>" %>
<%@ Import Namespace="MvcContrib.Samples.UI.Models"%>
<%@ Import Namespace="MvcContrib.UI.Grid" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
	<title>Simple Grid Example</title>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2>Simple Grid Example</h2>

	<%= Html.Grid(Model).Columns(column => {
     		column.For(x => x.Id).Named("Person ID");
     		column.For(x => x.Name);
     		column.For(x => x.Gender);
     		column.For(x => x.DateOfBirth).Format("{0:d}");
			//The column containing the link is not automatically encoded because the grid is aware of helpers that return MvcHtmlString.
			column.For(x => Html.ActionLink("View Person", "Show", new { id = x.Id }));
     	}) %>

</asp:Content>
