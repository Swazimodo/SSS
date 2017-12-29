using SSS.Utilities.Interfaces;

namespace SSS.Web.Models
{
    /// <summary>
    /// Returns a pageable list of values
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PageableResults<T> : Results<T>, IPageable<T>
        where T : IPageableItem
    {
        public int CurrentPageNumber { get; set; }
        public bool HasNextPage { get; set; }
    }
}
