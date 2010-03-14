<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<MvcContrib.Samples.UI.Models.Person>>" %>
<%@ Import Namespace="MvcContrib.UI.Grid" %>

<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
	<title>Grid Sorting</title>
	<style type="text/css">
		.sort_desc 
		{
			background-image: url(<%= Url.Content("~/content/down.png") %>);
			background-position: right;
			background-repeat: no-repeat;
			padding-right: 20px;
		}
		.sort_asc 
		{
			background-image: url(<%= Url.Content("~/content/up.png") %>);
			background-position: right;
			background-repeat: no-repeat;
			padding-right: 20px;
		}
	</style>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h2>Sorted Grid Example</h2>
    
	<%= Html.Grid(Model)
		.Sort(ViewData["sort"] as GridSortOptions)
		.Columns(column => {
     		column.For(x => x.Id).Named("Person ID");
     		column.For(x => x.Name);
     		column.For(x => x.Gender);
     		column.For(x => x.DateOfBirth).Format("{0:d}");
     	}) %>
</asp:Content>

