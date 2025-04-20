using Infrastructure;
using Domains;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Data.Common;

namespace ApplicationServices
{
    public class  EventsRepository : ITasksInterface{
        private readonly ContextData _context;
        public Dictionary<String,String> ListErrors;
        public EventsRepository(ContextData context)
        {
            _context = context;
        }

        public async Task<List<TaskEvent>> GetAllAsync()
        {
            var _events = await _context.TaskEvents
                .AsNoTracking()
                .ToListAsync();            
            return _events;
        }

        public List<TaskEvent> GetAll()
        {
             var _events = _context.TaskEvents
                .AsNoTracking()
                .ToList();            
            return _events;
        }

        public TaskEvent GetById(int Id)
        {
            var _events =  _context.TaskEvents
                .AsNoTracking()
                .FirstOrDefault(e=>e.Id == Id);            
            return _events;
        }
        public async Task<TaskEvent> GetByIdAsync(int Id)
        {
            var _events = await _context.TaskEvents
                .AsNoTracking()
                .FirstOrDefaultAsync(e=>e.Id == Id);            
            return _events;
        }

        public async Task<List<TaskEvent>> GetByStatusAsync(EventsStatus status)
        {
            var _events = await _context.TaskEvents
                .AsNoTracking()
                .Where(e=>e.Status == status).ToListAsync(); 

            return _events;
        }

        public async Task<List<TaskEvent>> GetAllPendentsAsyncAsync()
        {
            var _events = await _context.TaskEvents
                .AsNoTracking()
                .Where(e=>e.Status == EventsStatus.PENDENTE)
                .ToListAsync();            
            return _events;
        }

        public async Task<List<TaskEvent>> GetAllClosedsAsyncAsync()
        {
            var _events = await _context.TaskEvents
                .AsNoTracking()
                .Where(e=>e.Status == EventsStatus.CONCLUIDO)
                .ToListAsync();            
            return _events;
        }
        public async Task<List<TaskEvent>> GetAllExcecutionsAsyncAsync()
        {
            var _events = await _context.TaskEvents
                .AsNoTracking()
                .Where(e=>e.Status == EventsStatus.EMPROGRESSO)
                .ToListAsync();            
            return _events;
        }

        public async Task<bool> TasksExistsAsync(int Id)
        {      
            return await _context.TaskEvents
                .AsNoTracking()
                .AnyAsync(e=>e.Id == Id);
        }

         public bool TasksExists(int Id)
        {      
            return _context.TaskEvents
                .AsNoTracking()
                .Any(e=>e.Id == Id);
        }
        public async Task<TaskEvent> CreateAsync(TaskEvent _event)
        {
            _context.Database.BeginTransaction();
            try{
                TaskEventCreate(_event);
                await _context.SaveChangesAsync();
                _context.Database.CurrentTransaction.Commit();
            }catch{
                _context.Database.CurrentTransaction.Rollback();
                return null;
            }
            return _event;
        }

        public async Task<TaskEvent> UpdateAsync(TaskEvent _event)
        {
            _context.Database.BeginTransaction();
            try{

                TaskEvent currentEvent = await GetByIdAsync(_event.Id);
                currentEvent.Update(_event);

                ListErrors = new Dictionary<string, string>();

                if(_event.DateConclusion < _event.DateEvent)
                    ListErrors.Add("DateConclusion", "Data Inferiror a Data da Tarefa");
                if(_event.DateConclusion.HasValue){
                    if(_event.DateConclusion.Value.Date > DateTime.Now.Date)
                        ListErrors.Add("DateConclusion", "Data Superiror a Data da atual");
                }
                if(ListErrors.Count > 0){
                    _context.Database.CurrentTransaction.Rollback();
                    return null;
                }

                _context.TaskEvents.Attach(currentEvent).State = EntityState.Modified;
               
                await _context.SaveChangesAsync();
                await _context.Database.CurrentTransaction.CommitAsync();

            }catch(DbException ex){
                Console.WriteLine(ex.Message);
                await _context.Database.CurrentTransaction.RollbackAsync();
                return null;
            }
            return _event;
        }

        

        private  TaskEvent  TaskEventCreate(TaskEvent _event){
            _event.Status = EventsStatus.PENDENTE;
            _event.DateConclusion = null;
            _context.TaskEvents.Add(_event);
            return _event;
        }

        public async Task<bool> DeleteByIdAsync(int Id)
        {
            _context.Database.BeginTransaction();
            try{
                var _event = await _context.TaskEvents.FindAsync(Id);
                if(_event == null)
                    return false;
                _context.TaskEvents.Remove(_event);
                _context.SaveChanges();
                _context.Database.CurrentTransaction.Commit();
            }catch{
                _context.Database.CurrentTransaction.Rollback();
                return false;
            }
            return true;
        }

        public Dictionary<string,string> GetErrors(){
            return ListErrors;
        }

    }
}