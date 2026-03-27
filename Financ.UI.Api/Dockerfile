# Estágio de construção
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copia o arquivo de solução e todos os arquivos de projeto para restaurar as dependências
COPY *.sln .
COPY ["Financ.UI.Api/Financ.UI.Api.csproj", "Financ.UI.Api/"]
COPY ["Financ.Application/Financ.Application.csproj", "Financ.Application/"]
COPY ["Financ.Domain/Financ.Domain.csproj", "Financ.Domain/"]
COPY ["Financ.Infra.Data/Financ.Infra.Data.csproj", "Financ.Infra.Data/"]
COPY ["Financ.Infra.IoC/Financ.Infra.IoC.csproj", "Financ.Infra.IoC/"]
COPY ["Financ.Infra.Security/Financ.Infra.Security.csproj", "Financ.Infra.Security/"]

RUN dotnet restore "Financ.UI.Api/Financ.UI.Api.csproj"

# Copia o restante dos arquivos da aplicação e constrói
COPY . .
WORKDIR /src/Financ.UI.Api
RUN dotnet build "Financ.UI.Api.csproj" -c Release -o /app/build

# Estágio de publicação
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS publish
WORKDIR /app
COPY --from=build /app/build .

COPY wait-for-it.sh .
RUN chmod +x wait-for-it.sh

# Ponto de entrada da aplicação
ENTRYPOINT ["dotnet", "Financ.UI.Api.dll"]