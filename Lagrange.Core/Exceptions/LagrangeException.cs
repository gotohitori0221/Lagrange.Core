namespace Lagrange.Core.Exceptions;




public class LagrangeException : Exception
{
    
    
    
    public LagrangeException() { }

    
    
    
    
    public LagrangeException(string message) : base(message) { }

    
    
    
    
    
    public LagrangeException(string? message, Exception innerException) : base(message, innerException) { }
}