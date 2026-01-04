namespace RealTimeTradingApp.Application.Common.Exceptions
{
    [Serializable]
    public class ForbiddenException: Exception
    {
        public ForbiddenException(string message): base(message)
        {
            
        }
    }
}
