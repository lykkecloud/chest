using JetBrains.Annotations;

namespace Chest.Client
{
    /// <summary>
    /// The enumeration to indicate the settings bulk update strategy
    /// </summary>
    [PublicAPI]
    public enum BulkUpdateStrategy
    {
        /// <summary>
        /// Only matched keys within provided category and collection will be updated.
        /// If there were no data for provided category and collection then all keys will be added.
        /// If there were data for provided category and collection but new keys provided then exception will be raised.
        /// </summary>
        UpdateMatchedOnly,
        
        /// <summary>
        /// All the existing keys for provided category and collection will be replaced by new keys.
        /// New keys will be added.
        /// Absent keys will be deleted.
        /// If there were no data for provided category and collection then all keys will be added.
        /// </summary>
        Replace
    }
}