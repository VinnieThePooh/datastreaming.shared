namespace DataStreaming.Exceptions;

public class DisconnectedPrematurelyException : Exception
{
    public DisconnectedPrematurelyException(string? message = null):base(message)
    {
    }
    
    public DisconnectedPrematurelyException(string? message, Exception? innerException):base(message, innerException)
    {
    }
}