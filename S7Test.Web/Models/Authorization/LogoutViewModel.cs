using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace S7Test.Web.Models.Authorization
{
    public class LogoutViewModel
    {
        [BindNever]
        public string RequestId { get; set; }
    }
}
