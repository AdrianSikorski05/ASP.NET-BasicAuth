using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AplikacjaAndroid
{
    public class ActivityBook
    {
        public int UserId { get; set; }
        public Book Book { get; set; }
        public BookStatus Status { get; set; }
    }
}
