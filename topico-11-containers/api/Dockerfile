# syntax=docker/dockerfile:1
# Estágio 1, "construir": usa a imagem com o SDK do .NET 10, que compila e publica.
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS construir
# Define /origem como pasta de trabalho dentro do estágio; as próximas instruções partem dela.
WORKDIR /origem
# Copia só o arquivo de projeto e o de auditoria do NuGet, antes do código, para o restore ter camada própria.
COPY Directory.Build.props ./
COPY ClinicaVida.Api/ClinicaVida.Api.csproj ClinicaVida.Api/
# Baixa os pacotes NuGet; sem mudança nos .csproj, o Docker reaproveita esta camada do cache.
RUN dotnet restore ClinicaVida.Api/ClinicaVida.Api.csproj
# Agora copia o restante do código; mudar um .cs invalida só as camadas daqui para baixo.
COPY ClinicaVida.Api/ ClinicaVida.Api/
# Publica em Release, sem refazer o restore, para a pasta /publica.
RUN dotnet publish ClinicaVida.Api/ClinicaVida.Api.csproj -c Release --no-restore -o /publica

# Estágio 2, "final": usa a imagem só com o runtime do ASP.NET, sem compilador nem SDK.
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
# Pasta da aplicação dentro do container.
WORKDIR /app
# Cria a pasta do banco e entrega a ela ao usuário "app", que existe na imagem oficial e não é root.
USER root
RUN mkdir /dados && chown app:app /dados
# Declara /dados como ponto de volume: o arquivo SQLite fica fora da camada gravável do container.
VOLUME /dados
# Copia do estágio "construir" apenas o resultado da publicação; o SDK e o código-fonte ficam para trás.
COPY --from=construir /publica ./
# A partir daqui o processo roda como "app", sem privilégios de root.
USER app
# Caminho do banco e origem do front, lidos pelo Program.cs pelo sistema de configuração do ASP.NET Core.
ENV ConnectionStrings__Clinica="Data Source=/dados/clinica.db" \
    Cors__Origem="http://localhost:5173"
# Documenta a porta em que o Kestrel escuta (8080 é o padrão das imagens .NET 8 ou superior).
EXPOSE 8080
# Comando de partida: o processo principal do container é a própria API.
ENTRYPOINT ["dotnet", "ClinicaVida.Api.dll"]
