using SSS.Utilities;

namespace SSS.Web.Models
{
    public class MenuOptionsSecuredItem : ISecuredItem
    {
        public string Title { get; set; }
        public string Id { get; set; }

        public MenuOptionsSecuredItem() { }

        public void Load<T>(ISecurableItem<T> obj, string key)
        {
            MenuOptionsItem<T> item = obj as MenuOptionsItem<T>;
            if (item == null)
                throw new ValidationException("Load object is not of the right type.");

            Id = key;
            Title = item.Title;
        }
    }
}
