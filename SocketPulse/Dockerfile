# Use the official .NET Core SDK base image
FROM mcr.microsoft.com/dotnet/sdk:7.0

# Set the working directory in the container
WORKDIR /app

# Copy csproj and restore any dependencies (via dotnet restore)
COPY ./SocketPulse.Receiver.Test.Integration/SocketPulse.Receiver.Test.Integration.csproj ./
COPY ./SocketPulse.Receiver/SocketPulse.Receiver.csproj ../SocketPulse.Receiver/
COPY ./SocketPulse.Shared/SocketPulse.Shared.csproj ../SocketPulse.Shared/
COPY ./SocketPulse.Test.Shared/SocketPulse.Test.Shared.csproj ../SocketPulse.Test.Shared/
RUN dotnet nuget locals all --clear
RUN dotnet restore

# Copy the project files into the container
COPY ./ ./
COPY ./SocketPulse.Receiver/ ../SocketPulse.Receiver/
COPY ./SocketPulse.Shared/ ../SocketPulse.Shared/
COPY ./SocketPulse.Test.Shared/ ../SocketPulse.Test.Shared/

# Build and test
RUN dotnet restore SocketPulse.Receiver.Test.Integration/SocketPulse.Receiver.Test.Integration.csproj
CMD ["dotnet", "test", "./SocketPulse.Receiver.Test.Integration/SocketPulse.Receiver.Test.Integration.csproj", "--no-restore", "--verbosity", "normal"]