namespace ProjetoCep.Api.Models
{
    public class CepResponseDto
    {
        public string Cep { get; set; } = string.Empty;
        public string Logradouro { get; set; } = string.Empty;
        public string? Complemento { get; set; }
        public string Bairro { get; set; } = string.Empty;
        public string Localidade { get; set; } = string.Empty;
        public string Uf { get; set; } = string.Empty;
        public string? Ibge { get; set; }
        public string? Gia { get; set; }
        public string? Ddd { get; set; }
        public string? Siafi { get; set; }

        public static CepResponseDto FromEntity(CepEntity entity)
        {
            return new CepResponseDto
            {
                Cep = entity.Cep,
                Logradouro = entity.Logradouro,
                Complemento = entity.Complemento,
                Bairro = entity.Bairro,
                Localidade = entity.Localidade,
                Uf = entity.Uf,
                Ibge = entity.Ibge,
                Gia = entity.Gia,
                Ddd = entity.Ddd,
                Siafi = entity.Siafi
            };
        }
    }
}