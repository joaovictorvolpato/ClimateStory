# Usar uma imagem base oficial do .NET SDK
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build

# Definir o diretório de trabalho dentro do container
WORKDIR /app

# Instalar git para clonar o repositório
RUN apt-get update && apt-get install -y git

# Clonar o repositório do GitHub
RUN git clone https://github.com/joaovictorvolpato/ClimateStory.git .

# Restaurar as dependências do projeto .NET
RUN dotnet restore

# Publicar o aplicativo (compilar para produção)
RUN dotnet publish -c Release -o /out

# Usar uma imagem mais leve do .NET runtime para a execução
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS runtime

# Definir o diretório de trabalho
WORKDIR /app

# Copiar os arquivos do estágio de build para o estágio de runtime
COPY --from=build /out .

# Expor a porta que o aplicativo irá utilizar (caso seja uma aplicação web)
EXPOSE 5000

# Definir o comando de entrada para iniciar o aplicativo
ENTRYPOINT ["dotnet", "ClimateStory.dll"]
