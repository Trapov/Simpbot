FROM microsoft/dotnet

COPY ./publish /publish
WORKDIR /publish


ENTRYPOINT ["dotnet", "Simpbot.Cli.dll"]