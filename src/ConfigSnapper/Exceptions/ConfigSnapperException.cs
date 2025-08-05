namespace Matiasg19.ConfigSnapper.Exceptions;

public class ConfigSnapperException : Exception
{
    public ConfigSnapperException()
    {
    }

    public ConfigSnapperException(string? message) : base(message)
    {
    }

    public ConfigSnapperException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}