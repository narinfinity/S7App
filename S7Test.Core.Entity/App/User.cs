using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace S7Test.Core.Entity.App
{
    public class User : IdentityUser<string>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        //inherited
        public override string PhoneNumber { get; set; }
        public override string Email { get; set; }
        public override string UserName { get; set; }
        public override string PasswordHash { get; set; }

        ////navigation

        public virtual ICollection<IdentityUserRole<string>> Roles { get; } = new List<IdentityUserRole<string>>();

        public virtual ICollection<IdentityUserClaim<string>> Claims { get; } = new List<IdentityUserClaim<string>>();

        public virtual ICollection<IdentityUserLogin<string>> Logins { get; } = new List<IdentityUserLogin<string>>();
    }
}
