FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY ["SignalRchat/SignalRchat.csproj", "SignalRchat/"]
RUN dotnet restore "SignalRchat/SignalRchat.csproj"
COPY . .
WORKDIR "/src/SignalRchat"
RUN dotnet build "SignalRchat.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "SignalRchat.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "SignalRchat.dll"]