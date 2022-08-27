FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["PlanningPoker.Bff/PlanningPoker.Bff.csproj", "PlanningPoker.Bff/"]
COPY ["PlanningPoker.Core/PlanningPoker.Core.csproj", "PlanningPoker.Core/"]
RUN dotnet restore "PlanningPoker.Bff/PlanningPoker.Bff.csproj"
COPY . .
WORKDIR "/src/PlanningPoker.Bff"
RUN dotnet build "PlanningPoker.Bff.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PlanningPoker.Bff.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

RUN useradd -m plngpoker
USER plngpoker

CMD ASPNETCORE_ENVIRONMENT="Development" ASPNETCORE_URLS="http://*:$PORT" dotnet PlanningPoker.Bff.dll
