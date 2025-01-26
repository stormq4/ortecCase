using Microsoft.AspNetCore.Mvc;

namespace TaskList
{
    [ApiController]
    [Route("tasks")]
    public class TasksController : ControllerBase
    {
        private readonly ITaskListService _taskListService;

        public TasksController(ITaskListService taskListService)
        {
            _taskListService = taskListService;
        }

        [HttpGet]
        public IDictionary<string, IList<Task>> GetTasks()
        {
            // var tasks = Enumerable.Range(1, 5).Select(index => "task" + index).ToArray();
            var tasks =  _taskListService.GetProjects();
            return tasks;
        }
    }

    [ApiController]
    [Route("projects")]
    public class ProjectsController : ControllerBase
    {
        private readonly ITaskListService _taskListService;

        public ProjectsController(ITaskListService taskListService)
        {
            _taskListService = taskListService;
        }

        [HttpGet]
        public IDictionary<string, IList<Task>> GetTasks()
        {
            // var tasks = Enumerable.Range(1, 5).Select(index => "task" + index).ToArray();
            var tasks =  _taskListService.GetProjects();
            return tasks;
        }
    }
}