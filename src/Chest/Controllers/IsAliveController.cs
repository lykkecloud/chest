// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

#pragma warning disable 1998

namespace Chest.Controllers
{
    using System.Diagnostics;
    using System.Net;
    using System.Reflection;
    using System.Threading.Tasks;
    using Chest.Client;
    using Microsoft.AspNetCore.Mvc;
    using Swashbuckle.AspNetCore.Annotations;

    [ApiVersion("1")]
    [ApiVersion("2")]
    [Route("api/isAlive")]
    public class IsAliveController : Controller
    {
        private static readonly RootModel Version =
            new RootModel
            {
                Title = typeof(Program).Assembly.Attribute<AssemblyTitleAttribute>(attribute => attribute.Title),
                Version = typeof(Program).Assembly.Attribute<AssemblyInformationalVersionAttribute>(attribute => attribute.InformationalVersion),
                OS = System.Runtime.InteropServices.RuntimeInformation.OSDescription.TrimEnd(),
                ProcessId = Process.GetCurrentProcess().Id,
            };

        [HttpGet]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(RootModel))]
        public async Task<IActionResult> Get() => this.Ok(Version);
    }
}
