namespace TaskList
{
	public class TaskListExecutor
	{

		private readonly IDictionary<string, IList<Task>> _tasks = new Dictionary<string, IList<Task>>();
		private long lastId = 0;
		private readonly IConsole _console;

		public TaskListExecutor(IConsole console)
		{
			_console = console;
		}

		public void Execute(string commandLine)
		{
			var commandRest = commandLine.Split(" ".ToCharArray(), 2);
			var command = commandRest[0];
			switch (command) {
			case "show":
				Show();
				break;
			case "add":
				Add(commandRest[1]); // mooi errors afhandelen
				break;
			case "check":
				Check(commandRest[1]); // mooi errors afhandelen
				break;
			case "uncheck":
				Uncheck(commandRest[1]); // mooi errors afhandelen
				break;
			case "help":
				Help();
				break;
			case "deadline":
				Deadline(commandRest[1]);
				break;
			//today
			//deadline
			//group by
			default:
				UnrecognisedCommandError(command);
				break;
			}
		}

		private void Show()
		{
			foreach (var project in _tasks) {
				_console.WriteLine(project.Key);
				foreach (var task in project.Value) {
					_console.WriteLine("    [{0}] {1}: {2}", (task.Done ? 'x' : ' '), task.Id, task.Description);
				}
				_console.WriteLine();
			}
		}

		private void Add(string commandLine)
		{
			//error
			var subcommandRest = commandLine.Split(" ".ToCharArray(), 2);
			var subcommand = subcommandRest[0];
			if (subcommand == "project") {
				AddProject(subcommandRest[1]);
			} else if (subcommand == "task") {
				var projectTask = subcommandRest[1].Split(" ".ToCharArray(), 2);
				AddTask(projectTask[0], projectTask[1]);
			}
		}

		private void AddProject(string name)
		{
			//error
			_tasks[name] = new List<Task>();
		}

		private void AddTask(string project, string description)
		{
			// error
			if (!_tasks.TryGetValue(project, out IList<Task> projectTasks))
			{
				Console.WriteLine("Could not find a project with the name \"{0}\".", project);
				return;
			}
			projectTasks.Add(new Task { Id = NextId(), Description = description, Done = false });
		}

		private void Deadline(string commandLine)
		{
			try {
				var commandRest = commandLine.Split(" ".ToCharArray(), 2);
				long taskId = long.Parse(commandRest[0]);
				string dateString = commandRest[1];
				DateOnly dateOnly = DateOnly.ParseExact(dateString, "d-M-yyyy");

				var task = GetTaskById(taskId);
			} catch {
				//error
			}
		}

		private Task? GetTaskById(long taskId)
		{
			return _tasks
						.Values
						.SelectMany(tasks => tasks) // Flatten all tasks from all projects
						.FirstOrDefault(task => task.Id == taskId); // Find the task by Id
		}

		private void Check(string idString)
		{
			SetDone(idString, true);
		}

		private void Uncheck(string idString)
		{
			SetDone(idString, false);
		}

		private void SetDone(string idString, bool done)
		{
			if (string.IsNullOrEmpty(idString))
			{
				// add something
				return;
			}
			int id = int.Parse(idString);
			var identifiedTask = _tasks
				.Select(project => project.Value.FirstOrDefault(task => task.Id == id))
				.Where(task => task != null)
				.FirstOrDefault();
			if (identifiedTask == null) {
				_console.WriteLine("Could not find a task with an ID of {0}.", id);
				return;
			}

			identifiedTask.Done = done;
		}

		private void Help()
		{
			_console.WriteLine("Commands:");
			_console.WriteLine("  show");
			_console.WriteLine("  add project <project name>");
			_console.WriteLine("  add task <project name> <task description>");
			_console.WriteLine("  check <task ID>");
			_console.WriteLine("  uncheck <task ID>");
			_console.WriteLine();
		}

		private void UnrecognisedCommandError(string command)
		{
			_console.WriteLine("I don't know what the command \"{0}\" is.", command);
		}

		private void FormatError(string command, string format)
		{
			_console.WriteLine($"Please format the {command} command like this: {format}");
		}

		private long NextId()
		{
			return ++lastId;
		}
	}
}