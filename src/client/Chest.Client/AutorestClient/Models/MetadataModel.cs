// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace Chest.Client.AutorestClient.Models
{
    using Newtonsoft.Json;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    public partial class MetadataModel
    {
        /// <summary>
        /// Initializes a new instance of the MetadataModel class.
        /// </summary>
        public MetadataModel()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the MetadataModel class.
        /// </summary>
        public MetadataModel(string key = default(string), IDictionary<string, string> data = default(IDictionary<string, string>))
        {
            Key = key;
            Data = data;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "key")]
        public string Key { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "data")]
        public IDictionary<string, string> Data { get; set; }

    }
}