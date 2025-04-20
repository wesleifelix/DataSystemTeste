using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Domains
{
    public class TaskEvent
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required( AllowEmptyStrings = false,ErrorMessage = "Este campo é obrigatório")]
        [MinLength(3, ErrorMessage = "O título deve contém no mínimo 3 caracteres")]
        [MaxLength(100,ErrorMessage = "O titulo não conter mais de 100 caracteres")]
        public required string Title { get; set; }

        [JsonPropertyName("dateevent")]
        [DataType(DataType.DateTime)]
        public DateTime DateEvent { get; set; } = DateTime.Now;

        [JsonPropertyName("dateconclusion")]
        [DataType(DataType.DateTime)]
        public DateTime? DateConclusion { get; set; }

        [MaxLength(10000)]
        public string Description { get; set; } = String.Empty;

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public EventsStatus Status { get; set; } = EventsStatus.PENDENTE;

        public bool CloseEvent(DateTime dateConclusion) { 
            if(this.DateEvent > dateConclusion)
            {
                return false;
            }
            DateConclusion = dateConclusion;
            Status = EventsStatus.CONCLUIDO;
            return true;
        }
        public bool StartEvent()
        {
            Status = EventsStatus.EMPROGRESSO;
            return true;
        }

         public void Update(TaskEvent newEvent)
        {
            Title = newEvent.Title;
            Description = newEvent.Description;
            DateEvent = newEvent.DateEvent;
            DateConclusion = newEvent.DateConclusion;
            Status = newEvent.Status;
        }
    }

}
