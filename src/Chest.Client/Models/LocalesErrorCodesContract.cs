namespace Chest.Client.Models
{
    public enum LocalesErrorCodesContract
    {
        None,
        AlreadyExists,
        /// <summary>
        /// An attempt to delete a locale that does not exist
        /// </summary>
        DoesNotExist,
        /// <summary>
        /// An attempt to delete the default locale
        /// </summary>
        CannotDeleteDefaultLocale,
        /// <summary>
        /// The target locale should have all localized values first
        /// </summary>
        CannotSetLocaleAsDefault,
        /// <summary>
        /// An attempt to delete a locale that has attached localized values
        /// </summary>
        CannotDeleteLocaleAssignedToAnyLocalizedValue,
    }
}