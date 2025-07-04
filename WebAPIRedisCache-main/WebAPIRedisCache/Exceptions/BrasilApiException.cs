namespace ProjetoCep.Api.Exceptions
{
    public class BrasilApiException : Exception
    {
        public BrasilApiException() : base("Erro ao consultar a BrasilAPI.") { }
        public BrasilApiException(string message) : base(message) { }
        public BrasilApiException(string message, Exception innerException) : base(message, innerException) { }
    }
}