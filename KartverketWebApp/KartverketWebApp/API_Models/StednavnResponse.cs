﻿using System.Text.Json.Serialization;

namespace KartverketWebApp.API_Models
{
    public class StednavnResponse
    {
        [JsonPropertyName("fylkesnavn")]
        public string? Fylkesnavn { get; set; }

        [JsonPropertyName("fylkesnummer")]
        public string? Fylkesnummer { get; set; }

        [JsonPropertyName("kommunenavn")]
        public string? Kommunenavn { get; set; }

        [JsonPropertyName("kommunenummer")]
        public string? Kommunenummer { get; set; }
    }
}
