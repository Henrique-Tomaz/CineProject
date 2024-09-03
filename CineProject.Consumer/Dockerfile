# Use a imagem base do .NET SDK para a fase de build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Defina o diretório de trabalho
WORKDIR /src

# Copie o arquivo do projeto e restaure as dependências
COPY ["CineProject.Consumer/CineProject.Consumer.csproj", "CineProject.Consumer/"]
RUN dotnet restore "CineProject.Consumer/CineProject.Consumer.csproj"

# Copie o restante do código
COPY . .

# Compile e publique o projeto
RUN dotnet publish "CineProject.Consumer/CineProject.Consumer.csproj" -c Release -o /app/publish

# Use a imagem base do .NET Runtime para a fase de runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

# Defina o diretório de trabalho no contêiner
WORKDIR /app

# Copie o resultado da publicação do estágio de build
COPY --from=build /app/publish .

# Defina o comando para iniciar o aplicativo
ENTRYPOINT ["dotnet", "CineProject.Consumer.dll"]
