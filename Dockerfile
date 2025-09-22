# Étape 1 : Build avec le SDK
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copier la solution et restaurer les dépendances
COPY *.sln ./
COPY ./*.csproj ./TP-Entropy-back/
RUN dotnet restore ./TP-Entropy-back/TP-Entropy-back.csproj

# Copier le reste du code
COPY . ./TP-Entropy-back/

# Compiler et publier en mode Release
WORKDIR /src/TP-Entropy-back
RUN dotnet publish -c Release -o /app/publish

# Étape 2 : Image finale runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Copier les fichiers publiés depuis l’étape build
COPY --from=build /app/publish .

# Lancer l’API
ENTRYPOINT ["dotnet", "TP-Entropy-back.dll"]