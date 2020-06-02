// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using Chest.Client.Api;
using Lykke.HttpClientGenerator;

namespace Chest.Tests.Sdk
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Net.Http;
    using System.Threading;
    using Microsoft.Extensions.Configuration;

    public sealed class ChestFixture : IDisposable
    {
        private readonly string _connectionString = "<pass_it_from_testsettings.json>";

        private readonly Process _chestProcess;

        public ChestFixture()
        {
            var config = new ConfigurationBuilder().AddJsonFile("testsettings.json").Build();

            _connectionString = config.GetConnectionString("Chest");

            ServiceUrl = config.GetValue<string>("serviceUrl");
            ApiKey = config.GetValue<string>("apiKey");

            _chestProcess = StartChest();
        }

        public string ServiceUrl { get; }
        
        public string ApiKey { get; }

        public void Dispose()
        {
            try
            {
                _chestProcess.Kill();
            }
            catch (InvalidOperationException)
            {
            }

            _chestProcess.Dispose();
        }

        [DebuggerStepThrough]
        private Process StartChest()
        {
            var path = string.Format(
                CultureInfo.InvariantCulture,
                "..{0}..{0}..{0}..{0}Chest{0}Chest.csproj",
                Path.DirectorySeparatorChar);

            Process.Start(
                new ProcessStartInfo("dotnet", $"run -p {path} --connectionStrings:chest \"{_connectionString}\"")
                {
                    UseShellExecute = true,
                });

            var processId = default(int);
            var clientGenerator = HttpClientGenerator
                .BuildForUrl(ServiceUrl)
                .Create();

            var client = clientGenerator.Generate<IIsAlive>();

            var attempt = 0;
            while (true)
            {
                Thread.Sleep(500);
                try
                {
                    throw new NotImplementedException();//todo it's all not working..
                    var response = client.Get().GetAwaiter().GetResult();
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

            return Process.GetProcessById(processId);
        }
    }
}