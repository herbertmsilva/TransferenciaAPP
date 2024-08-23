# API de Transferências

## Descrição

Bem-vindo ao projeto API de Transferências! Esta API RESTful foi desenvolvida usando .NET 8 e simula um sistema de transferência bancária simples. Com ela, você pode cadastrar clientes, realizar transferências entre contas, e consultar históricos de transações, tudo de maneira eficiente e segura. Este projeto foi um desafio técnico e uma oportunidade de demonstrar habilidades em desenvolvimento e arquitetura de software.

## Índice

1. [Pré-requisitos](#pré-requisitos)
2. [Instalação](#instalação)
3. [Endpoints da API](#endpoints-da-api)
4. [Testes](#testes)
5. [Tecnologias Usadas](#tecnologias-usadas)

## Pré-requisitos

Antes de começar, você precisará ter as seguintes ferramentas instaladas em seu sistema:

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) ou qualquer editor de código compatível
- [Postman](https://www.postman.com/) ou outra ferramenta para testar APIs
- [Git](https://git-scm.com/)

## Instalação

Siga os passos abaixo para configurar o projeto localmente:

1. **Clone o repositório para a sua máquina local**:
   Abra um terminal e execute o seguinte comando para clonar o repositório:
    ```bash
   git clone https://github.com/herbertmsilva/TransferenciaAPP.git
    ```

2. **Abra o projeto no Visual Studio**:
   - Navegue até a pasta onde você clonou o repositório.
   - Clique duas vezes no arquivo da solução (.sln) para abrir o projeto no Visual Studio 2022 (ou outra versão compatível).

3. **Restaure as dependências do projeto**:
   - No Visual Studio, abra o Gerenciador de Soluções.
   - Clique com o botão direito do mouse na solução e selecione Restaurar Pacotes NuGet. Isso irá baixar todas as dependências necessárias do projeto.

4. **Compile o projeto**:
   - No menu superior do Visual Studio, selecione Build (Compilar) e clique em Build Solution (Compilar Solução), ou use o atalho Ctrl+Shift+B.
   - Certifique-se de que a solução compile sem erros.

5. **Execute o projeto**:
   - No Visual Studio, clique no botão Iniciar (ou pressione F5) para executar o projeto. Isso iniciará o servidor de desenvolvimento local.
   - A API será executada e estará acessível em [http://localhost:7098.](https://localhost:7098/swagger/index.html)

8. **Acesse a documentação da API com Swagger**:
   - Abra seu navegador e navegue até [http://localhost:5000/swagger](https://localhost:7098/swagger/index.html) para visualizar e testar os endpoints da API.

## Uso

Após configurar e executar o projeto, você pode usar a API para realizar operações de cadastro, consulta e transferências entre contas. Aqui estão alguns exemplos de como interagir com a API usando ferramentas como Postman ou cURL.

### Exemplo de Uso com Postman

1. **Cadastrar um novo cliente**:

   - Método: `POST`
   - URL: `http://localhost:7098/api/v1.0/Cliente`
   - Corpo da Requisição (JSON):
     ```json
     {
       "nome": "João Silva",
       "numeroConta": "1234567890",
       "saldo": 1500.00
     }
     ```

2. **Listar todos os clientes**:

   - Método: `GET`
   - URL: `http://localhost:7098/api/v1.0/Cliente`

3. **Buscar um cliente pelo número da conta**:

   - Método: `GET`
   - URL: `http://localhost:7098/api/v1.0/Cliente/por-conta/1234567890`

4. **Realizar uma transferência entre contas**:

   - Método: `POST`
   - URL: `http://localhost:7098/api/v1.0/Transferencia`
   - Corpo da Requisição (JSON):
     ```json
     {
       "contaOrigemId": "GUID-da-Conta-Origem",
       "contaDestinoId": "GUID-da-Conta-Destino",
       "valor": 500.00,
       "dataTransferencia": "2024-08-30T14:30:00"
     }
     ```
5. **Obter o histórico de transferências para uma conta específica**:

   - Método: `GET`
   - URL: `http://localhost:7098/api/v1.0/Transferencia/historico/{contaId}`
  
## Endpoints da API

A API de Transferências expõe vários endpoints para gerenciamento de clientes e transferências entre contas. A seguir, uma lista dos principais endpoints disponíveis:

### 1. Endpoints de Clientes

- **`GET /api/v1.0/Cliente`**  
  Retorna uma lista de todos os clientes cadastrados. Responde com um código 200 (OK) ao retornar os clientes com sucesso.

- **`GET /api/v1.0/Cliente/{id}`**  
  Retorna os detalhes de um cliente específico com base no ID fornecido. Responde com 200 (OK) se o cliente for encontrado, 400 (Bad Request) para um ID inválido, e 404 (Not Found) se o cliente não for encontrado.

- **`GET /api/v1.0/Cliente/por-conta/{numeroConta}`**  
  Retorna os detalhes de um cliente específico com base no número da conta. Responde com 200 (OK) se o cliente for encontrado, 400 (Bad Request) para um número de conta inválido, e 404 (Not Found) se o cliente não for encontrado.

- **`POST /api/v1.0/Cliente`**  
  Adiciona um novo cliente. O corpo da requisição deve conter os dados do cliente, incluindo nome, número da conta, e saldo inicial. Responde com 201 (Created) se o cliente for adicionado com sucesso e 400 (Bad Request) para dados inválidos.

- **`PUT /api/v1.0/Cliente/{id}`**  
  Atualiza um cliente existente com base no ID fornecido. O corpo da requisição deve conter os dados atualizados do cliente. Responde com 200 (OK) se o cliente for atualizado com sucesso, 400 (Bad Request) se o ID fornecido não corresponder ao ID no corpo da requisição, e 404 (Not Found) se o cliente não for encontrado.

- **`DELETE /api/v1.0/Cliente/{id}`**  
  Remove um cliente existente com base no ID fornecido. Responde com 200 (OK) se o cliente for removido com sucesso, 400 (Bad Request) para um ID inválido, e 404 (Not Found) se o cliente não for encontrado.

### 2. Endpoints de Transferências

- **`GET /api/v1.0/Transferencia`**  
  Retorna uma lista de todas as transferências realizadas. Responde com 200 (OK) ao retornar as transferências com sucesso.

- **`GET /api/v1.0/Transferencia/{id}`**  
  Retorna os detalhes de uma transferência específica com base no ID fornecido. Responde com 200 (OK) se a transferência for encontrada, 400 (Bad Request) para um ID inválido, e 404 (Not Found) se a transferência não for encontrada.

- **`POST /api/v1.0/Transferencia`**  
  Realiza uma nova transferência entre duas contas. O corpo da requisição deve conter os dados da transferência, incluindo conta de origem, conta de destino, valor e data da transferência. Responde com 201 (Created) se a transferência for realizada com sucesso e 400 (Bad Request) para dados inválidos ou saldo insuficiente.

- **`PUT /api/v1.0/Transferencia/cancelar/{id}`**  
  Cancela uma transferência existente com base no ID fornecido. Responde com 200 (OK) se a transferência for cancelada com sucesso, 400 (Bad Request) para um ID inválido, e 404 (Not Found) se a transferência não for encontrada.

- **`DELETE /api/v1.0/Transferencia/{id}`**  
  Exclui uma transferência específica com base no ID fornecido. Responde com 200 (OK) se a transferência for excluída com sucesso, 400 (Bad Request) para um ID inválido, e 404 (Not Found) se a transferência não for encontrada.

- **`GET /api/v1.0/Transferencia/historico/{contaId}`**  
  Retorna o histórico de transferências para uma conta específica, ordenado por data decrescente. Responde com 200 (OK) ao retornar o histórico com sucesso, 400 (Bad Request) para um ID de conta inválido, e 404 (Not Found) se o histórico de transferências não for encontrado.

### Notas Importantes

- Substitua `{id}`, `{numeroConta}`, e `{contaId}` pelos valores reais ao fazer as requisições.
- Certifique-se de que a API esteja em execução antes de tentar acessar os endpoints.
- Utilize ferramentas como Postman ou a interface Swagger (`http://localhost:5000/swagger`) para testar e explorar os endpoints de forma interativa.

## Testes

O projeto inclui testes unitários e de integração para verificar a funcionalidade dos serviços e dos endpoints da API. A seguir estão as instruções para executar esses testes:

### Executando Testes Unitários

1. **Abra o projeto no Visual Studio**:
   - Certifique-se de que a solução esteja aberta no Visual Studio.

2. **Navegue até o Gerenciador de Testes**:
   - No menu superior, selecione `Test -> Test Explorer` para abrir o Gerenciador de Testes.

3. **Executar todos os testes**:
   - No Test Explorer, clique em `Run All` para executar todos os testes unitários. Os resultados dos testes serão exibidos, mostrando quais testes passaram e quais falharam.

### Executando Testes de Integração

Os testes de integração foram configurados para rodar em um banco de dados em memória para simular interações reais com os endpoints da API.

1. **Abrir o Terminal ou Console do Visual Studio**:
   - Certifique-se de estar no diretório raiz do projeto.

2. **Executar os testes de integração**:
   - Use o seguinte comando para executar os testes de integração:

   ```bash
   dotnet test
   ```
   - Esse comando executará todos os testes, incluindo os de integração, e mostrará os resultados no terminal.

### Estrutura dos Testes
-  **Testes Unitários**:
   - Os testes unitários verificam a lógica de negócio de forma isolada, sem dependências externas.
   - Exemplo: Testes de métodos que realizam transferências ou validam dados de clientes.

- **Testes de Integração**:
  - Os testes de integração verificam o comportamento de toda a aplicação e suas interações, incluindo chamadas a banco de dados e endpoints.
  - Exemplo: Testes que realizam chamadas HTTP para os endpoints da API de Cliente e Transferência e verificam as respostas.

## Tecnologias Usadas

Este projeto foi desenvolvido utilizando as seguintes tecnologias e frameworks:

- **.NET 8**: Framework principal usado para desenvolver a API RESTful, garantindo alto desempenho e escalabilidade.
- **Entity Framework Core**: Usado como ORM para interagir com o banco de dados, fornecendo uma interface de alto nível para operações CRUD e manipulação de dados.
- **In-Memory Database**: Utilizado para testes unitários e de integração em ambientes de desenvolvimento para simular a persistência de dados sem necessidade de um banco de dados real.
- **Swagger**: Ferramenta de documentação e teste interativo da API, facilitando a visualização dos endpoints e execução de requisições diretamente pelo navegador.
- **FluentValidation**: Biblioteca usada para validação de entrada de dados, garantindo que os dados fornecidos pelo usuário atendam aos requisitos esperados.
- **xUnit**: Framework de teste utilizado para criar e executar testes unitários e de integração, assegurando a qualidade e a confiabilidade do código.

## Arquitetura do Projeto

A arquitetura do projeto segue um padrão modular, facilitando a escalabilidade e a manutenção. Abaixo está uma visão geral da arquitetura utilizada:

### 1. Camadas Principais

- **Core (Transferencia.Core)**: Contém as entidades de negócios, interfaces, enums e lógica central do sistema. Esta camada representa o núcleo do aplicativo, fornecendo os modelos de dados e os contratos que são implementados em outras camadas.

  - **Entities**: Contém as classes de entidades do sistema, como `ClienteEntity` e `TransferenciaEntity`, que representam os objetos principais e suas propriedades.
  - **Enums**: Define enumeradores como `StatusTransferenciaEnum`, que são usados para representar estados ou categorias específicas dentro do sistema.
  - **Interfaces/Persistence**: Define interfaces para os repositórios e a unidade de trabalho, como `IClienteRepository`, `ITransferenciaRepository`, e `IUnitOfWork`. Essas interfaces especificam contratos que as classes de infraestrutura devem implementar.

- **Application**: Implementa a lógica de negócios e validações específicas da aplicação. Utiliza DTOs (Data Transfer Objects) para transferir dados entre camadas e serviços de aplicação.

- **Persistence**: Esta camada é responsável por todas as interações com o banco de dados. Implementa os repositórios e a unidade de trabalho, usando o Entity Framework Core para manipulação de dados. Ela garante que as operações de leitura e escrita sejam realizadas de forma eficiente e segura.

- **Infrastructure**: Esta camada é reservada para integrações com serviços externos que não estão diretamente relacionados ao banco de dados. Exemplos incluem envio de e-mails, push notifications, logging, e outros serviços externos que a aplicação possa necessitar.

- **API**: Camada de apresentação que expõe os endpoints RESTful. Contém controllers que recebem solicitações HTTP, interagem com os serviços de aplicação e retornam respostas apropriadas. Utiliza middleware para gerenciamento de exceções e autenticação.

### 2. Padrões e Boas Práticas

- **Dependency Injection**: Implementada para fornecer dependências a partir de um contêiner de injeção, facilitando o teste e promovendo baixo acoplamento entre componentes.
- **Repository Pattern**: Usado para abstrair a lógica de acesso a dados e separar a lógica de negócios da manipulação de dados.
- **Unit of Work**: Gerencia transações, garantindo que todas as operações de banco de dados em um determinado escopo sejam concluídas ou revertidas como uma única unidade de trabalho.
- **Middleware de Exceções**: Implementado para tratamento global de exceções, garantindo respostas consistentes e log centralizado de erros.

### 3. Segurança e Validação

- **FluentValidation**: Usado para validar os dados de entrada nos serviços de aplicação, garantindo a integridade e consistência dos dados antes do processamento.
- **Middleware de Exceções**: Fornece um tratamento centralizado para erros, retornando mensagens de erro padronizadas e ajudando na depuração em ambientes de desenvolvimento.

### Notas Importantes

- A estrutura de pastas e namespaces deve ser mantida ao adicionar novas funcionalidades para garantir a organização e a manutenção da base de código.
- Princípios de programação orientada a objetos e boas práticas, como os princípios SOLID, devem ser seguidos para garantir um código de fácil compreensão e manutenção.


