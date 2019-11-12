using Microsoft.AspNetCore.Identity;
using System;

namespace WebAPI_Identity.Models
{
    public class MyUser : IdentityUser
    {
        [PersonalData]
        public string FirstName { get; set; }
        [PersonalData]
        public string LastName { get; set; }
    }
}
