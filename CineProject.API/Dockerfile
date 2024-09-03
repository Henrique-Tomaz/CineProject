# Use a imagem base do .NET SDK para a fase de build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Instale clang e outras dependências necessárias para AOT (se necessário)
RUN apt-get update && apt-get install -y clang gcc

# Defina o diretório de trabalho
WORKDIR /src

# Copie o arquivo do projeto e restaure as dependências
COPY ["CineProject.API/CineProject.API.csproj", "CineProject.API/"]
RUN dotnet restore "CineProject.API/CineProject.API.csproj"

# Copie o restante do código
COPY . .

# Compile e publique o projeto
RUN dotnet publish "CineProject.API/CineProject.API.csproj" -c Release -o /app/publish

# Use a imagem base do .NET ASP.NET Runtime para a fase de runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

# Defina o diretório de trabalho no contêiner
WORKDIR /app

# Copie o resultado da publicação do estágio de build
COPY --from=build /app/publish .

# Defina o comando para iniciar o aplicativo
ENTRYPOINT ["dotnet", "CineProject.API.dll"]
