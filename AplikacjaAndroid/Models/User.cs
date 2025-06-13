using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AplikacjaAndroid
{
    public class User
    {
        /// <summary>
        /// Identifier for the user
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Username of the user
        /// </summary>
        public string? Username { get; set; }
        /// <summary>
        /// Password of the user
        /// </summary>
        public string? Password { get; set; }
        /// <summary>
        /// Role of the user
        /// </summary>
        public string? Role { get; set; }
    }
}
