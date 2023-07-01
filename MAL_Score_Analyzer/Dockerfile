#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["MAL_Score_Analyzer/MAL_Score_Analyzer.csproj", "MAL_Score_Analyzer/"]
COPY ["Models/Models.csproj", "Models/"]
RUN dotnet restore "MAL_Score_Analyzer/MAL_Score_Analyzer.csproj"
COPY . .
WORKDIR "/src/MAL_Score_Analyzer"
RUN dotnet build "MAL_Score_Analyzer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MAL_Score_Analyzer.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MAL_Score_Analyzer.dll"]