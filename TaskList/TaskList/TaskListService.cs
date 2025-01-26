namespace TaskList
{
	public interface ITaskListService
	{
		void AddProject(string name);
		void AddTask(string project, string description);
		void Check(string idString);
		public void UnCheck(string idString);
		Task? GetTaskById(long taskId);
		IDictionary<string, IList<Task>> GetTasksByDay(DateOnly day);
		List<DateOnly> GetDeadLinesList();
	}

	public class TaskListService: ITaskListService
	{
		private readonly IDictionary<string, IList<Task>> _taskLists = new Dictionary<string, IList<Task>>();
		
		private long lastId = 0;

		public TaskListService() {}

		public void AddProject(string name)
		{
			//error
			_taskLists[name] = new List<Task>();
		}

		public void AddTask(string project, string description)
		{
			// error
			if (!_taskLists.TryGetValue(project, out IList<Task> projectTasks))
				throw new Exception(string.Format("Could not find a project with the name \"{0}\".", project));
			
			projectTasks.Add(new Task { Id = NextId(), Description = description, Done = false });
		}

		public void Check(string idString)
		{
			SetDone(idString, true);
		}

		public void UnCheck(string idString)
		{
			SetDone(idString, false);
		}

		private void SetDone(string idString, bool done)
		{
			if (string.IsNullOrEmpty(idString))
			{
				throw new Exception("No id was given");
			}
			int id = int.Parse(idString);
			var identifiedTask = GetTaskById(id);
			
			if (identifiedTask == null) 
			{
				throw new Exception(string.Format("Could not find a task with an ID of {0}.", id));
			}

			identifiedTask.Done = done;
		}

		public Task? GetTaskById(long taskId)
		{
			return _taskLists
				.Select(project => project.Value.FirstOrDefault(task => task.Id == taskId))
				.Where(task => task != null)
				.FirstOrDefault();
		}

		public IDictionary<string, IList<Task>> GetTasksByDay(DateOnly day)
		{
			return _taskLists
				.Where(project => project.Value.Any(task => task.Deadline == day))
				.ToDictionary(
					kvp => kvp.Key,
					kvp => (IList<Task>)kvp.Value.Where(task => task.Deadline == day).ToList()
				);
		}

		public List<DateOnly> GetDeadLinesList()
		{
			var deadlines = _taskLists.Values
                          .SelectMany(tasks => tasks)
                          .Where(task => task.Deadline != new DateOnly())
                          .Select(task => task.Deadline)
                          .Distinct()
                          .OrderBy(deadline => deadline)
                          .ToList();

			deadlines.Add(new DateOnly());
			return deadlines;
		}

		private long NextId()
		{
			return ++lastId;
		}
	}
}