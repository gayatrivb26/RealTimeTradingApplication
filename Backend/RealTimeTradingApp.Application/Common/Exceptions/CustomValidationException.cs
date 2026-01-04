

namespace RealTimeTradingApp.Application.Common.Exceptions
{
    [Serializable]
    public class CustomValidationException: Exception
    {
        public List<string> Errors { get; } = new();

        public CustomValidationException(string message): base(message)
        {
            Errors.Add(message);
        }

        public CustomValidationException(IEnumerable<string> errors)
            : base("Validation failed")
        {
            Errors.AddRange(errors);
        }
    }
}
