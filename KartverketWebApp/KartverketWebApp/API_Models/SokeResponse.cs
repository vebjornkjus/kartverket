using System.Text.Json.Serialization;
namespace KartverketWebApp.API_Models
{
        public class Avgrensningsboks
        {
            [JsonPropertyName("coordinates")]
            public List<List<List<double>>>? Coordinates { get; set; }

            [JsonPropertyName("crs")]
            public Crs? Crs { get; set; }

            [JsonPropertyName("type")]
            public string? Type { get; set; }
        }

        public class Crs
        {
            [JsonPropertyName("properties")]
            public CrsProperties? Properties { get; set; }

            [JsonPropertyName("type")]
            public string? Type { get; set; }
        }

        public class CrsProperties
        {
            [JsonPropertyName("name")]
            public string? Name { get; set; }
        }

        public class GyldigeNavn
        {
            [JsonPropertyName("navn")]
            public string? Navn { get; set; }

            [JsonPropertyName("prioritet")]
            public int? Prioritet { get; set; }

            [JsonPropertyName("sprak")]
            public string? Sprak { get; set; }
        }

        public class PunktIOmrade
        {
            [JsonPropertyName("coordinates")]
            public List<double>? Coordinates { get; set; }

            [JsonPropertyName("crs")]
            public Crs? Crs { get; set; }

            [JsonPropertyName("type")]
            public string? Type { get; set; }
        }

        public class Kommune
        {
            [JsonPropertyName("avgrensningsboks")]
            public Avgrensningsboks? Avgrensningsboks { get; set; }

            [JsonPropertyName("fylkesnavn")]
            public string? Fylkesnavn { get; set; }

            [JsonPropertyName("fylkesnummer")]
            public string? Fylkesnummer { get; set; }

            [JsonPropertyName("gyldigeNavn")]
            public List<GyldigeNavn>? GyldigeNavn { get; set; }

            [JsonPropertyName("kommunenavn")]
            public string? Kommunenavn { get; set; }

            [JsonPropertyName("kommunenavnNorsk")]
            public string? KommunenavnNorsk { get; set; }

            [JsonPropertyName("kommunenummer")]
            public string? Kommunenummer { get; set; }

            [JsonPropertyName("punktIOmrade")]
            public PunktIOmrade? PunktIOmrade { get; set; }

            [JsonPropertyName("samiskForvaltningsomrade")]
            public bool? SamiskForvaltningsomrade { get; set; }
        }

        public class KommunerResponse
        {
            [JsonPropertyName("antallTreff")]
            public int? AntallTreff { get; set; }

            [JsonPropertyName("kommuner")]
            public List<Kommune>? Kommuner { get; set; }
        }
}
