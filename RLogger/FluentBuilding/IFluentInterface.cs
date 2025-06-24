using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RLogger.FluentBuilding
{
    /// <summary>
    /// This interface is used to hide Object Methods from the fluent interface.
    /// </summary>
    public interface IFluentInterface
    {
        /// <summary>
        /// This method is used to hide the Object.ToString() method.
        /// </summary>
        [Browsable(false)]
        string? ToString();
        /// <summary>
        /// This method is used to hide the Object.Equals() method.
        /// </summary>
        [Browsable(false)]
        bool Equals(object? obj);
        /// <summary>
        /// This method is used to hide the Object.GetHashCode() method.
        /// </summary>
        [Browsable(false)]
        int GetHashCode();
        /// <summary>
        /// This method is used to hide the Object.GetType() method.
        /// </summary>
        [Browsable(false)]
        Type GetType();
    }
}
