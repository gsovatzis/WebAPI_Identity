using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI_Identity.DTOs
{
    public class UserDTO
    {
        // This is an object to be used from WebAPI body -> it will be mapped with MyUser through Automapper
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string CurrentPass { get; set; }
        public string Password { get; set; }
    }
}
