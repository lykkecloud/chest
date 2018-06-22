// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Chest.Client
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using Newtonsoft.Json;

    /// <summary>
    /// Extension methods for object to <see cref="Dictionary{TKey, TValue}"/> and vice versa
    /// </summary>
    public static class SerializationExtensions
    {
        /// <summary>
        /// Converts an instance object to <see cref="Dictionary{TKey, TValue}"/>
        /// </summary>
        /// <typeparam name="T">Any class type T</typeparam>
        /// <param name="instance">The instance object of given generic type T</param>
        /// <returns>A <see cref="Dictionary{TKey, TValue}"/> which can be stored as Metadata</returns>
        public static Dictionary<string, string> ToMetadataDictionary<T>(this T instance)
            where T : class
        {
            var properties = instance
                .GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.Public);

            if (!properties.Any(p => p.PropertyType.IsClass && p.PropertyType != typeof(string)))
            {
                return properties.ToDictionary(p => p.Name, p => Convert.ToString(p.GetValue(instance, null), CultureInfo.InvariantCulture));
            }

            Dictionary<string, string> dictionary = new Dictionary<string, string>();

            foreach (var property in properties)
            {
                var value = property.GetValue(instance, null);

                if (property.PropertyType.IsClass
                    && property.PropertyType != typeof(string))
                {
                    dictionary[property.Name] = JsonConvert.SerializeObject(value);
                    continue;
                }

                dictionary[property.Name] = Convert.ToString(value, CultureInfo.InvariantCulture);
            }

            return dictionary;
        }

        /// <summary>
        /// Converts a metadata dictionary to instance object of the given generic type
        /// </summary>
        /// <typeparam name="T">Any class type T having parameter-less constructor</typeparam>
        /// <param name="metadataDictionary">A <see cref="Dictionary{TKey, TValue}"/> dictionary</param>
        /// <returns>Instance object of the generic type T</returns>
        public static T To<T>(this IDictionary<string, string> metadataDictionary)
            where T : class, new()
        {
            var t = new T();

            var properties = t.GetType().GetProperties();

            foreach (var property in properties)
            {
                metadataDictionary.TryGetValue(property.Name, out var value);

                if (string.IsNullOrWhiteSpace(value))
                {
                    continue;
                }

                if (property.PropertyType.IsEnum)
                {
                    property.SetValue(t, Enum.Parse(property.PropertyType, value));
                    continue;
                }

                if (property.PropertyType == typeof(string)
                    || !property.PropertyType.IsClass)
                {
                    property.SetValue(t, Convert.ChangeType(value, property.PropertyType, CultureInfo.InvariantCulture));

                    continue;
                }

                var objectValue = JsonConvert.DeserializeObject(value, property.PropertyType);

                property.SetValue(t, objectValue);
            }

            return t;
        }
    }
}
