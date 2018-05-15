// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Chest.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using Chest.Dto;
    using Newtonsoft.Json;

    /// <summary>
    /// Extensions to be used by API Client consumers and ease the Objects conversion to MetadataDto
    /// </summary>
    public static class MetadataExtensions
    {
        /// <summary>
        /// Extension to be used by API Client consumers and ease the Objects conversion to Metadata
        /// </summary>
        /// <param name="metadataDto">Object that implements IMetadataDto to be converted to Metadata</param>
        /// <returns>Returns the Object in Metadata form, using same key and additional fields as dictionary in Data field</returns>
        public static Metadata ToMetadata(this IMetadataDto metadataDto)
        {
            return new Metadata
            {
                Key = metadataDto.Key,
                Data = metadataDto.ToMetadataDictionary()
            };
        }

        private static Dictionary<string, string> ToMetadataDictionary(this IMetadataDto instance)
        {
            var dictionary = new Dictionary<string, string>();

            var properties = instance
                .GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(property => property.Name != "Key");

            foreach (var property in properties)
            {
                var value = string.Empty;
                if (property.PropertyType.IsClass && property.PropertyType == typeof(IMetadataDto))
                {
                    value = JsonConvert.SerializeObject((property.GetValue(instance, null) as IMetadataDto).ToMetadataDictionary());
                }
                else
                {
                    value = Convert.ToString(property.GetValue(instance, null), CultureInfo.InvariantCulture);
                }

                dictionary[property.Name] = value;
            }

            return dictionary;
        }
    }
}