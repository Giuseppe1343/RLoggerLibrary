using System;

namespace RLoggerThread
{
    /// <summary>
    /// Factory class for creating a new <see cref="IRLoggerThread"/>.
    /// </summary>
    public static class RLoggerThreadFactory
    {

        /// <summary>
        /// Create a new <see cref="IRLoggerThread"/> with the <paramref name="creationOptionsFunc"/>. <br/>
        /// <paramref name="creationOptionsFunc"/> should return the options for the logger. <br/>
        /// </summary>
        /// <param name="creationOptionsFunc"></param>
        /// <returns> The created <see cref="IRLoggerThread"/>. </returns>
        public static IRLoggerThread CreateLogger(Func<RLoggerThreadCreationOptions> creationOptionsFunc) => new RLoggerThread(creationOptionsFunc());

        /// <summary>
        /// Create a new <see cref="IRLoggerThread"/> with the <paramref name="creationOptions"/>. <br/>
        /// If the <paramref name="creationOptions"/> <see langword="is null"/>, the default options will be used.
        /// </summary>
        /// <param name="creationOptions"> The options for the logger. </param>
        /// <returns> The created <see cref="IRLoggerThread"/>. </returns>
        public static IRLoggerThread CreateLogger(RLoggerThreadCreationOptions? creationOptions = null) => creationOptions is null ? new RLoggerThread() : new RLoggerThread(creationOptions);
    }
}
