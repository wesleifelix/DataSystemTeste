using Domains;
using Infrastructure;
using ApplicationServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace DataSystem.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class EventsController : ControllerBase
    {
        private readonly ILogger<EventsController> _logger;
        private readonly ITasksInterface _eventsRepository;
        public EventsController(ILogger<EventsController> logger, ITasksInterface eventsRepository)
        {
            _logger = logger;
            _eventsRepository = eventsRepository;
        }

        [HttpGet()]
        public async Task<ActionResult> Get(int? id, EventsStatus? status)
        {
            try{
                if(id.HasValue){
                    return Ok(await _eventsRepository.GetByIdAsync(id.Value));
                }
                else if(status.HasValue){
                    return Ok(await _eventsRepository.GetByStatusAsync(status.Value));
                }
                else{
                    var tarefas = await _eventsRepository.GetAllAsync();
                    return Ok(tarefas);
                }
            }catch{
                return BadRequest(
                    new { message = "Erro ao encontrar o Tarefas." }
                );
            }
        }

        [HttpGet()]
        [ActionName("Pendents")]
        [Route("api/events/v1/Pendents")]
        public async Task<ActionResult> GetPendents()
        {
            var tarefas = await _eventsRepository.GetAllPendentsAsyncAsync();
            return Ok(tarefas);
        }

        [HttpGet()]
        [ActionName("Excecutions")]
        [Route("api/events/v1/Excecutions")]
        public async Task<ActionResult> GetExcecutions()
        {
            var tarefas = await _eventsRepository.GetAllExcecutionsAsyncAsync();
            return Ok(tarefas);
        }

        [ActionName("Closeds")]
        [HttpGet()]
        [Route("api/events/v1/Closeds")]
        public async Task<ActionResult> GetCloseds()
        {
            var tarefas = await _eventsRepository.GetAllClosedsAsyncAsync();
            return Ok(tarefas);
        }
        [HttpPost]
        public async Task<ActionResult> Create(TaskEvent tarefa)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState.Values.SelectMany(e => e.Errors));

            try{
                tarefa.DateConclusion = null;
                await _eventsRepository.CreateAsync(tarefa);
                return Created("GetEvents", tarefa);
            }
            catch{
                 return BadRequest(
                    new { message = "Erro ao criar o Tarefa." }
                );
            }
        }

        [HttpPut()]
        [ActionName("Update")]
        [Route("api/events/v1/Update")]
        public async Task<ActionResult> Update([FromHeader] int id,[FromBody] TaskEvent tarefa)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState.Values.SelectMany(e => e.Errors));
            if(tarefa.Id != id)
                return BadRequest("ID inválida");
            try{
                if(await _eventsRepository.UpdateAsync(tarefa) == null)
                    return BadRequest(ListErrors());

                return NoContent();
            }
            catch(Exception ex){
                Console.WriteLine(ex.Message);
                 return BadRequest(
                    new { message = "Erro ao editar o Tarefa." }
                );
            }
        }

        [HttpPut()]
        [ActionName("StartEvent")]
        [Route("api/events/v1/Start/{id}")]
        public async Task<ActionResult> StartEvent([FromRoute] int id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState.Values.SelectMany(e => e.Errors));
            var tarefa = await _eventsRepository.GetByIdAsync(id);
            if(tarefa == null)
                return NotFound();
            else if(tarefa.Status != EventsStatus.PENDENTE)
                return BadRequest("Tarefa já iniciada");
            try{
                tarefa.StartEvent();
                await _eventsRepository.UpdateAsync(tarefa);
                return NoContent();
            }
            catch{
                 return BadRequest(
                    new { message = "Erro ao Atualizar o Tarefa." }
                );
            }
        }

        [HttpPut()]
        [ActionName("Conclusion")]
        [Route("api/events/v1/Conclusion")]
        public async Task<ActionResult> ConclusionEvent([FromHeader]int id, [FromBody] DateTime DateConclusion)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState.Values.SelectMany(e => e.Errors));
            var tarefa = await _eventsRepository.GetByIdAsync(id);
            if(tarefa == null)
                return NotFound();

            if(DateConclusion < tarefa.DateEvent)
                return BadRequest(
                        new { message = "A data de conclusão não pode ser inferior a data da Tarefa" }
                    );
            try{
                if(tarefa.CloseEvent(DateConclusion)){
                   if(await _eventsRepository.UpdateAsync(tarefa) == null){
                        return BadRequest(ListErrors().Select(e=>e));
                   }
                   return NoContent();
                }
                else
                {
                   return BadRequest(ListErrors().Select(e=>e));
                }
                return NoContent();
            }
            catch{
                 return BadRequest(
                    new { message = "Erro ao concluir o Tarefa." }
                );
            }
        }

        [HttpDelete("{id}")]
        [ActionName("Delete")]
        public async Task<ActionResult> DeleteEvent([FromHeader]int id)
        {
            try{
                if(!_eventsRepository.TasksExists(id)){
                    return NotFound();
                }
                if(!await _eventsRepository.DeleteByIdAsync(id)){
                    return BadRequest(ListErrors().Select(e=>e));
                }
                return Ok();
            }
            catch{
                 return BadRequest(
                    new { message = "Erro ao remover o Tarefa." }
                );
            }
        }

        private List<string> ListErrors(){
            List<string> Errors = [];
            var lstErros = _eventsRepository.GetErrors();
            foreach (var item in lstErros)
            {
                Errors.Add( $"{item.Key}: {item.Value}");
            }

            return Errors;
        }
    }

}
