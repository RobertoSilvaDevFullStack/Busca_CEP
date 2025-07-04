namespace ProjetoCep.Api.Exceptions
{
    public class CepNotFoundException : Exception
    {
        public CepNotFoundException() : base("CEP não encontrado.") { }
        public CepNotFoundException(string message) : base(message) { }
        public CepNotFoundException(string message, Exception innerException) : base(message, innerException) { }
    }
}