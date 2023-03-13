using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SKFormParser
{
    internal class Settings
    {
        public class AzureOpenAISettings
        {
            [JsonPropertyName("deployment_label")]
            [JsonPropertyOrder(1)]
            public string DeploymentLabel { get; set; } = "";

            [JsonPropertyName("model")]
            [JsonPropertyOrder(2)]
            public string Model { get; set; } = "";

            [JsonPropertyName("endpoint")]
            [JsonPropertyOrder(3)]
            public string Endpoint { get; set; } = "";

            [JsonPropertyName("key")]
            [JsonPropertyOrder(4)]
            public string Key { get; set; } = "";
        }

        public class OpenAISettings
        {
            [JsonPropertyName("model")]
            [JsonPropertyOrder(1)]
            public string Model { get; set; } = "";

            [JsonPropertyName("key")]
            [JsonPropertyOrder(2)]
            public string Key { get; set; } = "";

            [JsonPropertyName("organization")]
            [JsonPropertyOrder(3)]
            public string Organization { get; set; } = "";
        }

        public class RecognizerSettings {
            [JsonPropertyName("endpoint")]
            [JsonPropertyOrder(1)]
            [JsonRequired]
            public string Endpoint { get; set; } = "";

            [JsonPropertyName("key")]
            [JsonPropertyOrder(2)]
            [JsonRequired]
            public string Key { get; set; } = "";

            [JsonPropertyName("model")]
            [JsonPropertyOrder(3)]
            public string Model { get; set; } = "prebuilt-read";
        }

        public IReadOnlyList<string> SUPPORTED_TYPE { get; } = new List<string>{"azure_openai", "openai"};

        [JsonPropertyName("type")]
        [JsonPropertyOrder(1)]
        [JsonRequired]
        public string Type { get; set; } = "";

        [JsonPropertyName("azure_openai")]
        [JsonPropertyOrder(2)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public AzureOpenAISettings? AzureOpenAI { get; set; } = null;

        [JsonPropertyName("openai")]
        [JsonPropertyOrder(3)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public OpenAISettings? OpenAI { get; set; } = null;

        [JsonPropertyName("recognizer")]
        [JsonPropertyOrder(4)]
        [JsonRequired]
        public RecognizerSettings recognizer { get; set; } = new();

        public static Settings FromJson(string jsonString)
        {
            try
            {
                var parse_result = JsonSerializer.Deserialize<Settings>(jsonString);
                if (parse_result is null)
                {
                    throw new Exception("Unable to deserialize prompt template config. The deserialized returned NULL.");
                }
                return parse_result!;
            }
            catch (JsonException ex)
            {
                throw new Exception($"Unable to deserialize prompt template config with error: {ex}");
            }
        }
    }
}
