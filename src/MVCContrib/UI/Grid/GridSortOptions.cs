using System.Web.Mvc;

namespace MvcContrib.UI.Grid
{
	/// <summary>
	/// Sorting information for use with the grid.
	/// </summary>
	public class GridSortOptions
	{
		public string Column { get; set; }
		public SortDirection SortDirection { get; set; }
	}

	public enum SortDirection
	{
		Ascending, Descending
	}
}