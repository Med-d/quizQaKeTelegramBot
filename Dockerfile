FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore "quizQaKeTelegramBot.csproj"
RUN dotnet publish "quizQaKeTelegramBot.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
COPY ./photo_quest.db ./
ENTRYPOINT ["dotnet", "quizQaKeTelegramBot.dll"]
