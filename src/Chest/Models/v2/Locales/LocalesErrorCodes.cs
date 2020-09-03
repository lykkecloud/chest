namespace Chest.Models.v2.Locales
{
    public enum LocalesErrorCodes
    {
        None,
        AlreadyExists,
        DoesNotExist,
        CannotDeleteDefaultLocale,
        /// <summary>
        /// The target locale should have all localized values first
        /// </summary>
        CannotSetLocaleAsDefault,
    }
}