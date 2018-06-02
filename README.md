# Chest #

Lykke Web API to store and retrieve key value data
It stores any key value pairs against a unique key

### Prerequisites

This project requires a running instance of [Postgres](https://www.postgresql.org/) and the connection string to be configured (see configuration section below).  

To download and install Postgres you can follow the instructions [here](https://www.postgresql.org/download/).
It is further possible to install Postgres as a [stand-alone installation](http://www.postgresonline.com/journal/archives/172-Starting-PostgreSQL-in-windows-without-install.html) from the binaries or run postgres in a docker container using the following command:
```
docker run --name postgres -e POSTGRES_PASSWORD=<password> -e POSTGRES_DB=chest -d -p 5432:5432 postgres:10.1-alpine
```
NOTE: If you are running Chest inside a docker container pointing to Postgres running on your Windows machine then make sure to set the host in the connection string to ```docker.for.win.localhost```.

### Configuration

#### User Secrets Configuration

This project requires specification of user secrets in order to function. The secrets configuration mechanism differs when running the project directly or running inside a container.

- If running the project from Visual Studio:  
You need to configure the [user secrets](https://blogs.msdn.microsoft.com/mihansen/2017/09/10/managing-secrets-in-net-core-2-0-apps/) for the project.
The contents of the `secrets.json` configuration file should match the [expected required configuration](src/Chest/Extensions/ConfigurationExtensions.cs?fileviewer=file-view-default#ConfigurationExtensions.cs-21).  
eg. (please note: secret values are invalid)

    ```json
    {
      "ConnectionStrings": {
        "Chest": "Host=localhost;Database=chest;Username=username;Password=password;"
      }
    }
    ```

- If you are running the project from the command line:  
You need to configure the [user secrets](https://blogs.msdn.microsoft.com/mihansen/2017/09/10/managing-secrets-in-net-core-2-0-apps/) for the project. This can be done via the command line in either Windows or Linux. You can set the secrets using the following command from within the ```src/Chest``` folder. You may need to run a ```dotnet restore``` before you try the following commands.

    ```cmd
    dotnet user-secrets set "ConnectionStrings:Chest" "Host=localhost;Database=chest;Username=username;Password=password;"
    ```


- If running the project inside a container:  
You need to configure the [environment variables](https://docs.docker.com/compose/environment-variables/#the-env_file-configuration-option) used to run the docker container.
To do this you need to create an `.env` file in the `src/Docker` folder and enter key/value pairs in the format `KEY=VALUE` for each secret.
The contents of the `.env` configuration file should match the [expected required configuration](src/Chest/Extensions/ConfigurationExtensions.cs?fileviewer=file-view-default#ConfigurationExtensions.cs-21).  
eg.  (please note: secret values are invalid)

    ```cmd
    CHEST_CONNECTIONSTRING=Host=localhost;Database=chest;Username=username;Password=password;
    ```

#### Optional Machine Specific Configuration

In addition, you can configure aspects of the application for the machine it is running on.

- If running the project directly (eg. from Visual Studio):  
You can to configure the ```appSettings.json``` for the project. You can do this by adding a file called ```appSettings.Custom.json``` with machine specific configuration which will override the default ```appSettings.json```.
eg.
    ```json
    {
      "serilog": {
        "writeTo": [
          {
            "Name": "Async",
            "Args": {
              "configure": [
                {
                  "Name": "RollingFile",
                  "Args": { "pathFormat": "C:\\logs\\chest\\chest-developer-{Date}.log" }
                }
              ]
            }
          }
        ]
      }
    }
    ```

- If running the project inside a container:  
You need to add any machine specific configuration to the `.env` file (mentioned in _User Secrets Configuration_).  
eg.
    ```cmd
    LOG_PATH=S:\Logs
    ```

### How to Debug

#### Using Visual Studio

Set the start-up project to ```Chest```. Hit F5.  
This will run the project directly using dotnet.exe. The application will listen on port 5011 and you can navigate to it using http://localhost:5011.

#### Using Visual Studio Tools for Docker

Set the start-up project to ```docker-compose```. Hit F5.  
This will run the project inside a docker container running behind nginx. Nginx will listen on port 5011 and forward calls to the application. You can navigate to it using http://localhost:5011.

#### From the Command Line

Navigate to the ```src/Chest``` folder and type ```dotnet run```.  
This will run the project directly using dotnet.exe without attaching the debugger. You will need to use your debugger of choice to attach to the dotnet.exe process.

### It exposes following end-points

GET /api/metadata/{key}

POST /api/metadata

Content-Type: application/json

#### BODY

{"key": "my-unique-key", "data": {["key_name", "value"]}}

