using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AXProductApp
{
     public static class StringExtension
    {
        public static bool ToBool(this string str)
        {
            int value;
            if (int.TryParse(str, out value))
            {
                if (value > 0)
                    return true;
            }
           return false;
        }

    }
}
