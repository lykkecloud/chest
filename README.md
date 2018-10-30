# Chest #

Lykke Web API to store and retrieve key value data.
It stores any key value pairs against a unique key. The key is composed of three parts and is case in-sensitive

1. Category
2. Collection
3. Key

### Prerequisites

This project requires a running instance of [MS Sql Server](https://www.microsoft.com/en-us/sql-server/sql-server-2017) and the connection string to be configured (see configuration section below).  

To download and install MS Sql Server you can follow the [instructions here](https://www.microsoft.com/en-us/sql-server/sql-server-downloads).
It is further possible to run MS Sql Server as per [instructions here](https://docs.microsoft.com/en-us/sql/linux/quickstart-install-connect-docker?view=sql-server-2017)

NOTE: If you are running Chest inside a docker container pointing to MS Sql Server running on your Windows machine then make sure to set the host in the connection string to ```docker.for.win.localhost```.

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
        "Chest": "Server=tcp:database.url,1433;Initial Catalog=dbName;Persist Security Info=False;User ID=username;Password=password;MultipleActiveResultSets=True;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
      }
    }
    ```

- If you are running the project from the command line:  
You need to configure the [user secrets](https://blogs.msdn.microsoft.com/mihansen/2017/09/10/managing-secrets-in-net-core-2-0-apps/) for the project. This can be done via the command line in either Windows or Linux. You can set the secrets using the following command from within the ```src/Chest``` folder. You may need to run a ```dotnet restore``` before you try the following commands.

    ```cmd
    dotnet user-secrets set "ConnectionStrings:Chest" "Server=tcp:database.url,1433;Initial Catalog=dbName;Persist Security Info=False;User ID=username;Password=password;MultipleActiveResultSets=True;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
    ```


- If running the project inside a container:  
You need to configure the [environment variables](https://docs.docker.com/compose/environment-variables/#the-env_file-configuration-option) used to run the docker container.
To do this you need to create an `.env` file in the `src/Docker` folder and enter key/value pairs in the format `KEY=VALUE` for each secret.
The contents of the `.env` configuration file should match the [expected required configuration](src/Chest/Extensions/ConfigurationExtensions.cs?fileviewer=file-view-default#ConfigurationExtensions.cs-21).  
eg.  (please note: secret values are invalid)

    ```cmd
    CHEST_CONNECTIONSTRING=Server=tcp:database.url,1433;Initial Catalog=dbName;Persist Security Info=False;User ID=username;Password=password;MultipleActiveResultSets=True;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;
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


### Add https enforcement for Chest

Set environment variables

```
Kestrel__Certificates__Default__Path:</root/.aspnet/https/certFile.pfx>
Kestrel__Certificates__Default__Password:<certificate password>
```

In order to map path of certificate we need to add additional volume to docker-compose.yml file

```
volumes:
      - ./https/:/root/.aspnet/https/

``` 

Update appsettings.Deployment.json file and mention the https port
 
 ``` 
 "urls": "https://*:443;"
 ```


Configuration of secrets.json file in order to use https

```json
"Kestrel": {
  "EndPoints": {
    "HttpsInlineCertFile": {
      "Url": "https://*:443",
      "Certificate": {
        "Path": "<path to .pfx file>",
        "Password": "<certificate password>"
      }
    }
}
```

Example of Dockerfile

```
FROM microsoft/dotnet:2.1.5-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 443

FROM microsoft/dotnet:2.1.403-sdk AS build
WORKDIR /src
COPY . ./
WORKDIR /src/Chest
RUN dotnet build -c Release -r linux-x64 -o /app

FROM build AS publish
RUN dotnet publish -c Release -r linux-x64 -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Chest.dll"]
```

### It exposes following end-points

You can view those end-points in swagger at the following url

http://localhost:5011/swagger/ui/

1. Add new metadata

POST /api/{category}/{collection}/{key}

Content-Type: application/json

Body:

{"data": {["key_name": "value"]}, "keywords": ["keyword"]}

2. Update existing metadata

PUT /api/{category}/{collection}/{key}

Content-Type: application/json

Body:

{"data": {["key_name": "value"]}, "keywords": ["keyword"]}

3. Delete metadata

DELETE /api/{category}/{collection}/{key}

4. Get metadata

GET /api/{category}/{collection}/{key}

Response:

{"data": {["key_name": "value"]}, "keywords": ["keyword"]}

4. Get all key value pairs in a collection

GET /api/{category}/{collection}?keyword={searchKeyword}

Response:

[
  "key1": {["key_name": "value"]},
  "key2": {["key_name": "value"]}
]

5. Get all collections in a category

GET /api/{category}

Response:

["collection1", "collection2"]

6. Get all categories in the system

GET /api

Response:

["category1", "category2"]

### How to Migrate from Postgres to MS Sql

Before doing the migration, the new version needs to be deployed. EF automatic migration based on Code First will take place, creating the objects on MS Sql database.

#### Using Migration script

The `scripts/migratePostgresToMsSql.py` script can run in any python environment. It connects to source `Postgres` database and copy all the data to a target `MS Sql` database.

Steps to successfully run it:

  1. Open the `scripts/migratePostgresToMsSql.py` script in your preferred editor

  2. Change the connection variables setting ServerURL, Port, DatabaseName, UserName and Password:

    * postgresEngine = getSqlEngine('postgresql+psycopg2', 'postgres.server.url', '5432', 'dbName', 'username', 'password')
    * msSqlEngine = getSqlEngine('mssql+pyodbc', 'mssql.server.url', '1433', 'dbName', 'username', 'password', 'SQL+Server')

  3. Run the script and wait, it will take some minutes.

  4. Check the table `chest.tb_keyValueData` inside the new MS Sql database, it should have the same data as your old Postgres database

#### Using other tools

There are a lot of other migration tools available out there, including the possibility to simply extract the data to a .csv file and import it on your new MS Sql database using the Import Wizard.

Feel free to choose the one that best suits your needs.
