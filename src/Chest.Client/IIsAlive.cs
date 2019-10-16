// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;
using Refit;

namespace Chest.Client
{
    [PublicAPI]
    public interface IIsAlive
    {
        [Get("/api/isAlive")]
        Task<RootModel> Get();
    }
}