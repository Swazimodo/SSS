using System;
using Newtonsoft.Json;

namespace SSS.Web.Models
{
    /// <summary>
    /// Class for returning a json exception through a rest api
    /// </summary>
    public class APIError
    {
        #region Properties

        /// <summary>
        /// User error message
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Type of exception thrown
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Exception details
        /// </summary>
        public InnerAPIError InnerException { get; set; }

        /// <summary>
        /// True if this is an error
        /// </summary>
        public bool HasError { get; set; }

        /// <summary>
        /// a safe action the user can follow (ex. refresh the browser)
        /// </summary>
        public string Action { get; set; }

        /// <summary>
        /// GUID that can be used to cross-reference an error to the application log
        /// </summary>
        public Guid ReferenceNum { get; set; }

        #endregion

        #region Contructors

        /// <summary>
        /// Generic error constructor
        /// </summary>
        public APIError()
        {
            HasError = true;
            InnerException = null;
            Type = "";
            Action = "";
            Message = "There was an issue processing the request";
            ReferenceNum = Guid.NewGuid();
        }

        /// <summary>
        /// Detailed error constructor for non prod
        /// </summary>
        /// <param name="ex">Exception that was thrown</param>
        public APIError(Exception ex)
        {
            HasError = true;
            Type = ex.GetType().ToString();
            Message = ex.Message;
            Action = "";
            ReferenceNum = Guid.NewGuid();

            if (ex.InnerException != null)
                InnerException = new InnerAPIError(ex.InnerException);
        }

        #endregion

        #region Overrides

        /// <summary>
        /// Converts this object into JSON
        /// </summary>
        /// <returns>JSON string</returns>
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this,
                new JsonSerializerSettings()
                {
                    //format "ExampleProperty" as "exampleProperty" to match framework default
                    ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
                });
        }

        #endregion

        #region Inner Class

        /// <summary>
        /// Will give full exception details in non prod environments
        /// </summary>
        public class InnerAPIError
        {
            public string Message { get; set; }
            public string Type { get; set; }
            public InnerAPIError InnerException { get; set; }

            public InnerAPIError(Exception ex)
            {
                Type = ex.GetType().ToString();
                Message = ex.Message;

                if (ex.InnerException != null)
                    InnerException = new InnerAPIError(ex.InnerException);
            }
        }

        #endregion
    }
}
