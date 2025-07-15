# API de Busca de CEP

Esta é uma API desenvolvida em .NET 8 para consulta de endereços a partir de um CEP (Código de Endereçamento Postal). O projeto utiliza cache em Redis e persistência de dados em MongoDB para otimizar as consultas e reduzir a dependência de serviços externos.

---

## Funcionalidades

- **Consulta de CEP**: Obtém informações de endereço (logradouro, bairro, cidade, UF) a partir de um CEP de 8 dígitos.
- **Cache com Redis**: Armazena em cache os CEPs consultados para respostas mais rápidas em requisições futuras.
- **Persistência com MongoDB**: Salva as informações de CEP em um banco de dados MongoDB para consultas locais antes de recorrer à API externa.
- **Fallback para API Externa**: Caso o CEP não seja encontrado localmente (nem no cache, nem no banco de dados), a API consulta a [BrasilAPI](https://brasilapi.com.br/) como fonte externa.
- **Validação de Formato**: A API valida se o CEP informado possui o formato correto de 8 dígitos numéricos.
- **Documentação com Swagger**: A API possui uma interface de documentação e testes gerada automaticamente com o Swashbuckle (Swagger).

---

## Tecnologias Utilizadas

- **.NET 8**: Framework de desenvolvimento da aplicação.
- **ASP.NET Core**: Para a construção da API web.
- **Redis**: Banco de dados em memória utilizado para cache de alta performance.
- **MongoDB**: Banco de dados NoSQL para persistência dos dados de CEP.
- **Serilog**: Para logging de eventos e erros da aplicação.
- **Swashbuckle (Swagger)**: Para geração da documentação da API.
- **BrasilAPI**: API pública utilizada como fonte de dados externa para a consulta de CEPs.

---

## Como Executar o Projeto

### Pré-requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- Uma instância do Redis em execução.
- Uma instância do MongoDB em execução.

### Configuração

1. Clone o repositório.
2. Configure as strings de conexão no arquivo `appsettings.json`:
   - **Redis**: Atualize a `ConnectionString` na seção `Redis`.
   - **MongoDB**: Atualize a `ConnectionString` e o `DatabaseName` na seção `MongoDbConnection`.
   - **BrasilAPI**: A URL base já está configurada na seção `BrasilApi`.

```json
{
  ...
  "Redis": {
    "ConnectionString": "sua_connection_string_do_redis"
  },
  "MongoDbConnection": {
    "ConnectionString": "sua_connection_string_do_mongodb",
    "DatabaseName": "seu_database_name"
  },
  "BrasilApi": {
    "BaseUrl": "https://brasilapi.com.br/api/cep/v1/"
  }
}
```

### Execução

Execute o seguinte comando na raiz do projeto WebAPIRedisCache:

```bash
dotnet run
```

A API estará disponível em `https://localhost:7075` e a documentação do Swagger em `https://localhost:7075/swagger`.

---

## Endpoint da API

### GET /Cep/{cep}

Busca as informações de um endereço a partir do CEP informado.

**Parâmetros de URL:**
- `cep` (string, obrigatório): O CEP a ser consultado (deve conter 8 dígitos, sem traço. Ex: 01001000).

**Respostas:**

#### 200 OK
Retorna um objeto JSON com os dados do endereço.

```json
{
  "cep": "01001000",
  "logradouro": "Praça da Sé",
  "complemento": "lado ímpar",
  "bairro": "Sé",
  "localidade": "São Paulo",
  "uf": "SP",
  "ibge": "3550308",
  "gia": "1004",
  "ddd": "11",
  "siafi": "7107"
}
```

#### 400 Bad Request
Se o CEP fornecido for inválido.

```json
{
  "message": "Formato de CEP inválido. Use 8 dígitos numéricos."
}
```

#### 404 Not Found
Se o CEP não for encontrado.

```json
{
  "message": "CEP 08190190 não encontrado."
}
```

#### 500 Internal Server Error
Em caso de falhas internas no servidor ou problemas de comunicação com as bases de dados ou a API externa.

---

## Estrutura do Projeto

```
/
├── Clients/          # Clientes para comunicação com APIs externas (BrasilApiClient.cs)
├── Controllers/      # Controladores da API (CepController.cs)
├── Exceptions/       # Classes de exceções customizadas
├── Extensions/       # Métodos de extensão (ex: para o cache distribuído)
├── Interfaces/       # Interfaces para os serviços e repositórios
├── logs/             # Arquivos de log gerados pelo Serilog
├── Models/           # Modelos de dados e DTOs
├── Properties/       # Configurações de inicialização (launchSettings.json)
├── Repositories/     # Repositórios para acesso a dados (CepRepository.cs)
├── Services/         # Lógica de negócio da aplicação (CepService.cs, RedisCacheService.cs)
├── appsettings.json  # Arquivo de configuração
└── Program.cs        # Ponto de entrada da aplicação e configuração dos serviços
```
