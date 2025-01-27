namespace TaskList
{
	public interface ITaskListService
	{
		IDictionary<string, IList<Task>> GetProjects();
		void PostProject(string name);
		void PostTask(string project, string description);
		void Check(string idString);
		public void UnCheck(string idString);
		void UpdateTaskDeadline(long taskId, DateOnly date);
		IDictionary<string, IList<Task>> GetProjectsByDay(DateOnly day);
		List<(DateOnly, IList<Task>)> GetTasksByDeadline();
		List<(DateOnly, IDictionary<string, IList<Task>>)> GetProjectsByDeadline();
	}

	/* Todo 
		Additional service for a projectService injected into the task service
		Use an orm with a database to store the data
	*/
	public class TaskListService: ITaskListService
	{
		private readonly IDictionary<string, IList<Task>> _projects = new Dictionary<string, IList<Task>>();
		
		private long lastId = 0;

		public IDictionary<string, IList<Task>> GetProjects() => _projects;

		public void PostProject(string name)
		{
			if (_projects.ContainsKey(name))
            	throw new Exception($"Project '{name}' already exists.");
			_projects[name] = new List<Task>();
		}

		public void PostTask(string project, string description)
		{
			if (!_projects.TryGetValue(project, out IList<Task> projectTasks))
				throw new Exception(string.Format("Could not find a project with the name \"{0}\".", project));
			
			projectTasks.Add(new Task { Id = NextId(), Description = description, Done = false });
		}

		public void UpdateTaskDeadline(long taskId, DateOnly date)
		{
			var task = GetTaskById(taskId);
			if (task == null)
				throw new Exception(string.Format("Could not find a task with an ID of {0}.", taskId));

			task.Deadline = date;
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
			if (!int.TryParse(idString, out int id))
				throw new Exception(string.Format("No id was given for command {0}", done? "check" : "uncheck"));

			var identifiedTask = GetTaskById(id);
			if (identifiedTask == null) 
				throw new Exception(string.Format("Could not find a task with an ID of {0}.", id));

			identifiedTask.Done = done;
		}

		private Task? GetTaskById(long taskId)
		{
			return _projects
				.Select(project => project.Value.FirstOrDefault(task => task.Id == taskId))
				.Where(task => task != null)
				.FirstOrDefault();
		}

		public IDictionary<string, IList<Task>> GetProjectsByDay(DateOnly day)
		{
			return _projects
				.Where(project => project.Value.Any(task => task.Deadline == day))
				.ToDictionary(
					kvp => kvp.Key,
					kvp => (IList<Task>)kvp.Value.Where(task => task.Deadline == day).ToList()
				);
		}

		private List<DateOnly> GetDeadLinesList()
		{
			var deadlines = _projects.Values
                          .SelectMany(tasks => tasks)
                          .Where(task => task.Deadline != new DateOnly())
                          .Select(task => task.Deadline)
                          .Distinct()
                          .OrderBy(deadline => deadline)
                          .ToList();

			deadlines.Add(new DateOnly());
			return deadlines;
		}

		public List<(DateOnly, IList<Task>)> GetTasksByDeadline()
		{
			var deadlineTasks = new List<(DateOnly, IList<Task>)>();
			var deadlines = GetDeadLinesList();
			foreach (var deadline in deadlines)
			{
				var projectsByDeadeline = GetProjectsByDay(deadline);
				var tasks = projectsByDeadeline.Values.SelectMany(tasks => tasks).ToList();
				deadlineTasks.Add((deadline, tasks));
			}
			return deadlineTasks;
		}

		public List<(DateOnly, IDictionary<string, IList<Task>>)> GetProjectsByDeadline()
		{
			var deadlineDict = new List<(DateOnly, IDictionary<string, IList<Task>>)>();
			var deadlines = GetDeadLinesList();
			foreach (var deadline in deadlines)
			{
				var deadLineTasks = GetProjectsByDay(deadline);
				deadlineDict.Add((deadline, deadLineTasks));
			}
			return deadlineDict;
		}

		private long NextId()
		{
			return ++lastId;
		}
	}
}