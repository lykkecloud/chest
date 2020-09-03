namespace Chest.Core
{
    public class ValidationError
    {
        public string Key { get; }
        public string Message { get; }

        public ValidationError(string key, string message)
        {
            Key = key;
            Message = message;
        }
    }
}