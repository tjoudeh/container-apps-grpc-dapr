#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Expenses.Grpc.Server/Expenses.Grpc.Server.csproj", "Expenses.Grpc.Server/"]
RUN dotnet restore "Expenses.Grpc.Server/Expenses.Grpc.Server.csproj"
COPY . .
WORKDIR "/src/Expenses.Grpc.Server"
RUN dotnet build "Expenses.Grpc.Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Expenses.Grpc.Server.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Expenses.Grpc.Server.dll"]