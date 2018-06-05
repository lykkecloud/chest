// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Chest.Tests.Unit
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using Chest.Client;
    using FluentAssertions;
    using Xbehave;

    public class SerializationExtensionsTests
    {
        [Scenario]
        public void DataShouldNotLostWhenSerializedDeserialized()
        {
            ExampleClass expected = null;
            ExampleClass actual = null;
            Dictionary<string, string> dictionary = default(Dictionary<string, string>);

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
                    dictionary = expected.ToMetadataDictionary();
                });

            "And deserialized back to ExampleClass"
                .x(() =>
                {
                    actual = dictionary.To<ExampleClass>();
                });

            "Then deserialized ExampleClass should be same as given ExampleClass"
                .x(() =>
                {
                    actual.Should().BeEquivalentTo(expected);
                });
        }

#pragma warning disable CA1034 // Nested types should not be visible
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
