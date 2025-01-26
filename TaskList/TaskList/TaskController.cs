using System.Reflection.Metadata.Ecma335;
using Microsoft.AspNetCore.Http.HttpResults;
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
        public IActionResult GetTasks()
        {
            // var tasks = Enumerable.Range(1, 5).Select(index => "task" + index).ToArray();
            var tasks =  _taskListService.GetProjects();
            return Ok(tasks);
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

        [HttpPost]
        public IActionResult PostProject([FromBody] string projectName)
        {
            try 
            {
                _taskListService.PostProject(projectName);
                return Created();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("{projectName}/tasks")]
        public IActionResult PostTask(string projectName, [FromBody] string taskName)
        {
            try {
                _taskListService.PostTask(projectName, taskName);
                return Created();
            } catch (Exception ex){
                if (ex.Message.Equals(string.Format("Could not find a project with the name \"{0}\".", projectName)))
                    return NotFound(ex.Message);
                return BadRequest(ex.Message); 
            }
        }

        [HttpPut("/projects/tasks/{task_id}")] // removed {projectId}
        public IActionResult UpdateTaskDeadline(string project_id, long task_id, [FromQuery] string deadline)
        {
            try
            {
                if (!DateOnly.TryParseExact(deadline, "d-M-yyyy", out DateOnly date))
                    throw new Exception("Please format deadline like this d-m-yyyy'");
                _taskListService.UpdateTaskDeadline(task_id, date);
                return Ok();
            }
            catch (Exception ex)
            {
                if (ex.Message.Equals(string.Format("Could not find a task with an ID of {0}.", task_id)))
                    return NotFound(ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("/view_by_deadline")]
        public IActionResult GetTasksByDeadline()
        {
            try 
            {
                return Ok(_taskListService.GetTasksByDeadline());
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("/view-project-by-deadline")]
        public IActionResult GetProjectsByDeadline()
        {
            try 
            {
                return Ok(_taskListService.GetProjectsByDeadline());
            } 
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}