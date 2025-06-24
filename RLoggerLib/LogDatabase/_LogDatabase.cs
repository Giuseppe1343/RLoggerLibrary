using System;
using System.Collections.Generic;
using System.Text;

namespace RLoggerLib.LogDatabase
{
    internal abstract class LogDatabase
    {
        protected string TableName =>  $"LogTable_{DateTime.Today:yyyyMMdd}";
        protected LogDatabase() { }
    }
}
