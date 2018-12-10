// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Chest.Tests.Sdk
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Net.Http;
    using System.Threading;
    using Chest.Client;
    using Chest.Client.AutorestClient;
    using Microsoft.Extensions.Configuration;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using Newtonsoft.Json.Serialization;

    public sealed class ChestFixture : IDisposable
    {
        private readonly string connectionString = "<pass_it_from_testsettings.json>";

        private static readonly string DockerContainerId = Guid.NewGuid().ToString("N", CultureInfo.InvariantCulture).Substring(12);

        private readonly Process chestProcess;

        public ChestFixture()
        {
            var config = new ConfigurationBuilder().AddJsonFile("testsettings.json").Build();

            this.connectionString = config.GetConnectionString("Chest");

            this.ServiceUrl = new Uri(config.GetValue<string>("serviceUrl"));

            this.chestProcess = this.StartChest();
        }

        public Uri ServiceUrl { get; }

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

        [DebuggerStepThrough]
        private Process StartChest()
        {
            var path = string.Format(
                CultureInfo.InvariantCulture,
                "..{0}..{0}..{0}..{0}..{0}Chest{0}Chest.csproj",
                Path.DirectorySeparatorChar);

            Process.Start(
                new ProcessStartInfo("dotnet", $"run -p {path} --connectionStrings:chest \"{connectionString}\"")
                {
                    UseShellExecute = true,
                });

            var processId = default(int);
            using (var client = new ChestClient(this.ServiceUrl, new[] { new SuccessHandler() }))
            {
                var attempt = 0;
                while (true)
                {
                    Thread.Sleep(500);
                    try
                    {
                        var response = client.Root.GetStatusAsync().GetAwaiter().GetResult();
                        processId = response.ProcessId;
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
    }
}