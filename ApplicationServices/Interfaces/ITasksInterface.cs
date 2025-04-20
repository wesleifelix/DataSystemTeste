using Domains;

namespace ApplicationServices{
    public interface ITasksInterface
    {

        public Task<List<TaskEvent>> GetAllAsync();
        public Task<List<TaskEvent>> GetAllPendentsAsyncAsync();
        public Task<List<TaskEvent>> GetAllClosedsAsyncAsync();
        public Task<List<TaskEvent>> GetAllExcecutionsAsyncAsync();
        public TaskEvent GetById(int Id);
        public Task<TaskEvent> GetByIdAsync(int Id);
        public Task<List<TaskEvent>> GetByStatusAsync(EventsStatus satus);
        public Task<TaskEvent> CreateAsync(TaskEvent _event);
        public Task<TaskEvent> UpdateAsync(TaskEvent _event);
        public Task<bool> DeleteByIdAsync(int Id);
        public bool TasksExists(int Id);
        public Dictionary<string,string> GetErrors();
    }
}