using System;

namespace YouMap.Framework.Environment
{
    /// <summary>
    /// Id generation
    /// </summary>
    public interface IIdGenerator
    {
        /// <summary>
        /// Returns newly generated ID
        /// </summary>
        String Generate();

        
    }
}