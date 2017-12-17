namespace SSS.Web.Models
{
    public class ModelBase
    {
        public bool HasError { get; set; }
        public string Message { get; set; }

        public ModelBase()
        {
            HasError = false;
            Message = string.Empty;
        }
    }
}
