namespace RealTimeTradingApp.Application.Common.Exceptions
{
    [Serializable]
    public class NotFoundException: Exception
    {
        public string? EntityName { get; }
        public object? Key { get; }

        public NotFoundException(string message): base(message)
        {

        }

        public NotFoundException(string entityName, object key)
            : base($"{entityName} with Key ({key}) was not found.")
        {
            EntityName = entityName;
            Key = key;
        }
    }
}
