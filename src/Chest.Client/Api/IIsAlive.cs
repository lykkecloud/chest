// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using JetBrains.Annotations;
using Refit;

namespace Chest.Client.Api
{
    [PublicAPI]
    public interface IIsAlive
    {
        [Get("/api/isAlive")]
        Task<RootModel> Get();
    }
}