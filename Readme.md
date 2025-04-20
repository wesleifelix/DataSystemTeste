Sistema para Gerenciamento de Tarefas

API 
.net 7,8 SDK

Banco de Dados MS SQL 2014 ou superior

Dependencias EntityFrameWorkCore

dotnet tool ef,

Visual Studio ou VisualCode.

Para executar a API é necessário ter o .net 8 SDK, 
Banco de MS Sql ou MS Sql Express.

Executando a API de Dados.

Passo 1: configurar banco de dados, na pasta DataSystem edite crie um arquivo "appsettings.json", com a estrutura abaixo.

{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "ConnectionStrings": {
    "DefaultConnection" : "Data Source=SERVER;Initial Catalog=NOME_DO_BANCO;User ID=USER_DB;Password=PASSWORD_DB;Integrated Security=True;TrustServerCertificate=True;"
  },
  "AllowedHosts": "*"
}

SERVER deve ser alterado para o endereco do servidor de dados exemplos 172.160.100.1, {SERVIDOR\\INSTANCIA_DO_BANCO} no meu caso WESLEIFELIX\\MS_WESLEIFELIX.
USER_DB = usuário do Banco de Dados, exemplo SA.
PASSWORD_DB = Senha do Banco de dados, exemplo minhasenha123
NOME_DO_BANCO = Nome do SChema, exemplo TarefasDB

Criando e alterando a Tabela de dados.

Para esse passo, utilizamos Migrations, uma forma de manter sempre as alterações de banco registradas e consolidadas com os demais desenvolvedores.

Inicialmente usamos os comandos Enable-Migrations e Add-Migrations, para este projetos as migrações já foram iniciadas então pularemos este passo.

Como utilizamos projetos separados é premiso ter um pouco de cuidados com a execução das migrações, utilizamos o projeto API como projetos de inicialização mas a migração é realizada no no Projeto de Infra.

O exemplo abaixo é a forma de realizar o procedimento via CMD ou PowerShell.

então chamamos o comando 'dotnet ef [args] database update' para aplicação da base de dados.

abra a pasta do projeto no Terminal 

dotnet ef --startup-project .\DataSystem\Presentation.csproj --project .\DataInfra\Infrastructure.csproj database update

.\DataSystem\Presentation.csproj -> Projeto inicial, que contém a cadeia de conexão,
.\DataInfra\Infrastructure.csproj -> Projeto que conecta ao banco.


Se for necessário criar uma nova Migração
dotnet ef --startup-project .\DataSystem\Presentation.csproj --project .\DataInfra\Infrastructure.csproj migrations add NOME_DA_MIGRACAO



