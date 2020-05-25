// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Chest.Tests.Unit
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using Client;
    using FluentAssertions;
    using Newtonsoft.Json;
    using Xbehave;

    public class SerializationExtensionsTests
    {
        [Scenario]
        public void DataShouldNotLostWhenSerializedDeserialized()
        {
            ExampleClass expected = null;
            ExampleClass actual = null;
            var data = string.Empty;

            "Given an instance object of a class ExampleClass"
                .x(() =>
                {
                    expected = new ExampleClass
                    {
                        Id = 5,
                        Name = "DAX_INDEX",
                        Timestamp = DateTime.ParseExact(DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture), "yyyy-MM-dd HH:mm", null),
                        Price = 2500.25m,
                        SomeEnumType = ExampleClass.SomeEnum.Value2,
                        SubType = new SomeSubType
                        {
                            OrderId = 26,
                            Name = "EUR"
                        }
                    };
                });

            "When serialized to Dictionary<string, string>"
                .x(() =>
                {
                    var dictionary = expected.ToMetadataDictionary();
                    data = JsonConvert.SerializeObject(dictionary);
                });

            "And deserialized back to ExampleClass"
                .x(() =>
                {
                    actual = data.To<Dictionary<string, string>>().To<ExampleClass>();
                });

            "Then deserialized ExampleClass should be same as given ExampleClass"
                .x(() =>
                {
                    actual.Should().BeEquivalentTo(expected);
                });
        }

        [Scenario]
        public void CanSerializeDeserializeWatchList()
        {
            WatchList expected = null;
            WatchList actual = null;
            var data = string.Empty;

            var now = DateTime.UtcNow;
            now = new DateTime(now.Ticks - (now.Ticks % TimeSpan.TicksPerSecond), now.Kind);

            "Given an instance object of WatchList"
                .x(() =>
                {
                    expected = new WatchList
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = "Default",
                        Assets = new List<string> { "EURUSD", "EURGBP" },
                        ViewType = "FE",
                        ModifiedTimestamp = now,
                    };
                });

            "When serialized to Dictionary<string, string>"
                .x(() =>
                {
                    var dictionary = expected.ToMetadataDictionary();
                    data = JsonConvert.SerializeObject(dictionary);
                });

            "And deserialized back to WatchList"
                .x(() =>
                {
                    actual = data.To<Dictionary<string, string>>().To<WatchList>();
                });

            "Then deserialized WatchList should be same as given WatchList"
                .x(() =>
                {
                    actual.Should().BeEquivalentTo(expected);
                });
        }

#pragma warning disable CA1034 // Nested types should not be visible
        private class WatchList
        {
            /// <summary>
            /// Gets or sets the watchlist unique identifier
            /// </summary>
            public string Id { get; set; }

            /// <summary>
            /// Gets or sets watchlist display name
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// Gets or sets asset pair ids in this watchlist
            /// </summary>
#pragma warning disable CA2227 // Collection properties should be read only
            public List<string> Assets { get; set; }
#pragma warning restore CA2227 // Collection properties should be read only

            /// <summary>
            /// Gets or sets view type
            /// </summary>
            public string ViewType { get; set; }

            public DateTime? ModifiedTimestamp { get; set; }
        }

        public class ExampleClass
        {
            public int Id { get; set; }

            public string Name { get; set; }

            public DateTime Timestamp { get; set; }

            public decimal Price { get; set; }

            public SomeEnum SomeEnumType { get; set; }

            public SomeSubType SubType { get; set; }

            public enum SomeEnum
            {
                Value1 = 0,
                Value2,
                Value3
            }
        }

        public class SomeSubType
        {
            public int OrderId { get; set; }

            public string Name { get; set; }
        }
    }
}
