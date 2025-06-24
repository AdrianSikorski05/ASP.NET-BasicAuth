using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AplikacjaAndroid
{
    public enum BookStatus
    {
        Default = 0,
        AddedToReaded = 1,
        DeletedFromReaded = 2,
        AddedToToRead = 3,
        DeletedFromToRead = 4
    }

    public enum ActionComment
    {
        Default = 0,
        Add = 1,
        Update = 2,
        Delete = 3
    }

    public enum AnimationType
    { 
        Check = 0,
        Loading = 1
    }
}
