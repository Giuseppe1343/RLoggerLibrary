using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RLogger.Formatters
{
    public interface ILogFormatter<T>
    {
        T ApplyFormat(LogMessage logMessage);
    }
}
