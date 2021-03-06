﻿using System.Threading.Tasks;
using Chest.Client.Models;
using Chest.Client.Models.Requests;
using Chest.Client.Models.Responses;
using Refit;

namespace Chest.Client.Api
{
    /// <summary>
    /// Manages locales
    /// </summary>
    public interface ILocalesApi
    {
        /// <summary>
        /// Gets all locales
        /// </summary>
        /// <returns></returns>
        [Get("/api/v2/locales")]
        Task<GetLocalesResponse> GetAllAsync();

        
        /// <summary>
        /// Upsert locale
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Post("/api/v2/locales")]
        Task<UpsertLocaleErrorCodeResponse> UpsertAsync([Body] UpsertLocaleRequest request);

        /// <summary>
        /// Delete existing locale
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Delete("/api/v2/locales/{id}")]
        Task<ErrorsResponse> DeleteAsync(string id, [Body] DeleteLocaleRequest request);

    }
}