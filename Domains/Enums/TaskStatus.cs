using System.Text.Json.Serialization;

namespace Domains
{

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum EventsStatus
    {
        PENDENTE,
        EMPROGRESSO,
        CONCLUIDO
    }
}