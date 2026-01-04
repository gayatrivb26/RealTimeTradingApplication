namespace RealTimeTradingApp.Application.Common.Exceptions
{
    [Serializable]
    public class UnauthorizedException: Exception
    {
        public UnauthorizedException(string message = "Unauthorized access")
            :base(message)
        {
            
        }
    }
}
