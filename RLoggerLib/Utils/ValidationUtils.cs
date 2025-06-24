using System;
using System.Collections.Generic;
using System.Text;

namespace RLoggerLib.Utils
{
    internal class ValidationUtils
    {
        bool ThrowIfNull(object obj, string paramName)
        {
            if (obj == null)
            {
                //throw new Null(paramName);
            }
            return true;
        }
    }
}
