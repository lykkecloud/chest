// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Chest.Tests.Sdk
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Net.Http;
    using System.Net.Sockets;
    using System.Threading;
    using Microsoft.Extensions.Configuration;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using Newtonsoft.Json.Serialization;
    using Npgsql;

    public sealed class ChestFixture : IDisposable
    {
        private const string ConnectionString = "Host=localhost;Database=Chest;Username=postgres;Password=postgres;";

        private static readonly string DockerContainerId = Guid.NewGuid().ToString("N", CultureInfo.InvariantCulture).Substring(12);

        private readonly Process postgresProcess;
        private readonly Process chestProcess;

        public ChestFixture()
        {
            var config = new ConfigurationBuilder().AddJsonFile("testsettings.json").Build();

            this.Service = config.GetValue<string>("service");

            this.postgresProcess = this.StartPostgres();
            this.chestProcess = this.StartChest();
        }

        public string Service { get; }

        public void Dispose()
        {
            try
            {
                this.chestProcess.Kill();
            }
            catch (InvalidOperationException)
            {
            }

            this.chestProcess.Dispose();

            try
            {
                this.postgresProcess.Kill();
            }
            catch (InvalidOperationException)
            {
            }

            this.postgresProcess.Dispose();

            // NOTE (Cameron): Remove the docker container.
            Process.Start(new ProcessStartInfo("docker", $"stop {DockerContainerId}"))
                .WaitForExit(10000);
        }

        private static JsonSerializerSettings GetJsonSerializerSettings()
        {
            var settings = new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver { NamingStrategy = new SnakeCaseNamingStrategy() },
                NullValueHandling = NullValueHandling.Ignore,
            };

            settings.Converters.Add(new StringEnumConverter());

            return settings;
        }

        private Process StartPostgres()
        {
            var process = Process.Start(
                new ProcessStartInfo("docker", $"run --rm --name {DockerContainerId} -e POSTGRES_PASSWORD=postgres -e POSTGRES_DB=Chest -p 5432:5432 postgres:10.1-alpine")
                {
                    UseShellExecute = true,
                });

            // NOTE (Cameron): Trying to find a sensible value here so as to not throw during a debug session.
            Thread.Sleep(3500);

            using (var connection = new NpgsqlConnection(ConnectionString))
            {
                var attempt = 0;
                while (true)
                {
                    Thread.Sleep(500);
                    try
                    {
                        connection.Open();
                        break;
                    }
                    catch (Exception ex) when (ex is NpgsqlException || ex is SocketException || ex is EndOfStreamException)
                    {
                        if (++attempt >= 20)
                        {
                            throw;
                        }
                    }
                }
            }

            return process;
        }

        [DebuggerStepThrough]
        private Process StartChest()
        {
            var path = string.Format(
                CultureInfo.InvariantCulture,
                "..{0}..{0}..{0}..{0}..{0}Chest{0}Chest.csproj",
                Path.DirectorySeparatorChar);

            Process.Start(
                new ProcessStartInfo("dotnet", $"run -p {path} --connectionString \"{ConnectionString}\"")
                {
                    UseShellExecute = true,
                });

            var processId = default(int);
            using (var client = new HttpClient())
            {
                var attempt = 0;
                while (true)
                {
                    Thread.Sleep(500);
                    try
                    {
                        using (var response = client.GetAsync(new Uri(this.Service + "/api")).GetAwaiter().GetResult())
                        {
                            var api = JsonConvert.DeserializeObject<ChestApi>(response.Content.ReadAsStringAsync().GetAwaiter().GetResult(), GetJsonSerializerSettings());
                            processId = int.Parse(api.ProcessId, CultureInfo.InvariantCulture);
                        }

                        break;
                    }
                    catch (HttpRequestException)
                    {
                        if (++attempt >= 20)
                        {
                            throw;
                        }
                    }
                }
            }

            return Process.GetProcessById(processId);
        }

#pragma warning disable CA1812
        private class ChestApi
        {
            public string ProcessId { get; set; }
        }
    }
}