using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace S7Test.Web.Models.Authorization
{
    public class AuthzViewModel
    {
        [Display(Name = "Application")]
        public string ApplicationName { get; set; }

        [BindNever]
        public string RequestId { get; set; }

        [Display(Name = "Scope")]
        public string Scope { get; set; }

        [Display(Name = "State")]
        public string State { get; set; }
    }
}
