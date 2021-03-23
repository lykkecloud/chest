using Autofac;
using Chest.Data;
using Lykke.Common.MsSql;
using Microsoft.Extensions.Configuration;

namespace Chest.Modules
{
    public class MsSqlModule : Module
    {
        private readonly IConfiguration _configuration;

        public MsSqlModule(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterMsSql(_configuration.GetConnectionString("Chest"),
                connString => new ChestDbContext(connString, false),
                dbConn => new ChestDbContext(dbConn));
        }
    }
}