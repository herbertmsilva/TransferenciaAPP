## Descrição

Este projeto é uma API RESTful desenvolvida em .NET 8 para gerenciar transferências bancárias, incluindo funcionalidades de cadastro e listagem de clientes, transferência de valores entre contas e consulta de histórico de transações. O objetivo principal do projeto é demonstrar habilidades em desenvolvimento e arquitetura de software, implementando práticas de 
controle de concorrência. Este projeto segue o conceito KISS (Keep It Simple, Stupid), visando uma implementação simples e eficaz.

## Pré-requisitos

Antes de começar, você precisará ter as seguintes ferramentas instaladas em seu sistema:

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0): O SDK do .NET 8 é necessário para compilar e executar a API.
- [Visual Studio 2022](https://visualstudio.microsoft.com/) ou qualquer editor de código compatível: Recomenda-se o uso do Visual Studio 2022 para uma melhor experiência de desenvolvimento, mas outros editores como Visual Studio Code também podem ser usados.
- [Postman](https://www.postman.com/) ou outra ferramenta para testar APIs: Para testar os endpoints da API e verificar suas respostas.
- [Git](https://git-scm.com/): Para clonar o repositório e gerenciar o controle de versão do projeto.

## Instalação

Siga os passos abaixo para configurar o projeto localmente:

1. **Clone o repositório para a sua máquina local**:

   Abra um terminal e execute o seguinte comando para clonar o repositório:

   ```bash
   git clone https://github.com/seu-usuario/seu-repositorio.git
   cd seu-repositorio
Restaure as dependências do projeto:

Navegue até o diretório do projeto e execute o seguinte comando para restaurar as dependências necessárias:

bash
Copiar código
dotnet restore
Compile o projeto:

Após restaurar as dependências, compile o projeto usando o comando abaixo:

bash
Copiar código
dotnet build
Execute as migrações do banco de dados (se necessário):

Caso o projeto utilize migrações, você pode aplicar as migrações ao banco de dados com o seguinte comando:

bash
Copiar código
dotnet ef database update
Inicie o servidor de desenvolvimento:

Execute o seguinte comando para iniciar o servidor localmente:

bash
Copiar código
dotnet run
O servidor será iniciado e você poderá acessar a API em http://localhost:5000.
