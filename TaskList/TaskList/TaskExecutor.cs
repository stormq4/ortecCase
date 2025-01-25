using Microsoft.AspNetCore.Http.Connections;

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
			TestData();
		}

		private void TestData()
		{
			Execute("show");

			Execute("add project secrets");
			Execute("add project secrets");
			Execute("add task secrets Eat more donuts.");
			Execute("add task secrets Destroy all humans.");

			Execute("show");
			Execute("add project training");
			Execute("add task training Four Elements of Simple Design");
			Execute("add task training SOLID");
			Execute("add task training Coupling and Cohesion");
			Execute("add task training Primitive Obsession");
			Execute("add task training Outside-In TDD");
			Execute("add task training Interaction-Driven Design");

			Execute("check 1");
			Execute("check 3");
			Execute("check 5");
			Execute("check 6");

			Execute("show");

			Execute("deadline 6 25-1-2025");
			Execute("deadline 4 25-1-2025");
			Execute("deadline 8 27-1-2025");
			Execute("deadline 3 25-1-2025");
			Execute("deadline 1 25-1-2025");
			Execute("deadline 2 20-6-2006");
			Execute("show");

			_console.WriteLine("--------today----------");
			Execute("today");
			_console.WriteLine("--------today----------");
			Execute("show");
			_console.WriteLine("--------view-by-deadline----------");
			Execute("view-by-deadline");
			_console.WriteLine("--------view-by-deadline----------");
			Execute("show");
			_console.WriteLine("--------view-project-by-deadline----------");
			Execute("view-project-by-deadline");
			_console.WriteLine("--------view-project-by-deadline----------");
			Execute("show");
		}

		public void Execute(string commandLine)
		{
			var commandRest = commandLine.Split(" ".ToCharArray(), 2);
			var command = commandRest[0];
			switch (command) {
				case "show":
					Show(_tasks);
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
					AddDeadline(commandRest[1]);
					break;
				case "today":
					ShowTodaysProjects();
					break;
				case "view-by-deadline":
					ViewByDeadlines();
					break;
				case "view-project-by-deadline":
					ViewByDeadlinesGroupByProject();
					break;
				default:
					UnrecognisedCommandError(command);
					break;
			}
		}

		// private void Show(IDictionary<string, IList<Task>> tasks)
		// {
		// 	foreach (var project in tasks) {
		// 		_console.WriteLine(project.Key);
		// 		foreach (var task in project.Value) {
		// 			_console.WriteLine("    [{0}] {1}: {2}", (task.Done ? 'x' : ' '), task.Id, task.Description);
		// 		}
		// 		_console.WriteLine();
		// 	}
		// }

		private void Show(IDictionary<string, IList<Task>> tasks)
		{
			foreach (var project in tasks) {
				_console.WriteLine(project.Key);
				foreach (var task in project.Value) {
					_console.WriteLine("    [{0}] {1}: {2} : deadline-->    {3}", (task.Done ? 'x' : ' '), task.Id, task.Description, task.Deadline);
				}
				_console.WriteLine();
			}
		}

		private void ShowByDeadline(IDictionary<string, IList<Task>> projects, DateOnly deadline)
		{
			if (deadline == new DateOnly())
				_console.WriteLine("No deadline:");
			else
				_console.WriteLine("{0}:", deadline);

			foreach (var project in projects) 
			{
				foreach(var task in project.Value)
					_console.WriteLine("    {0}: {1} : deadline", task.Id, task.Description);
			}
			_console.WriteLine();
		}

		private void ShowByDeadlineByGroup(IDictionary<string, IList<Task>> projects, DateOnly deadline)
		{
			if (deadline == new DateOnly())
				_console.WriteLine("No deadline:");
			else
				_console.WriteLine("{0}:", deadline);

			foreach (var project in projects) 
			{
				_console.WriteLine("    " + project.Key + ":");
				foreach(var task in project.Value)
					_console.WriteLine("        {0}: {1} : deadline", task.Id, task.Description);
			}
			_console.WriteLine();
		}

		private void ViewByDeadlines()
		{
			var deadlines = GetDeadLinesList();
			foreach (var deadline in deadlines)
			{
				var deadLineTasks = GetTasksByDay(deadline);
				ShowByDeadline(deadLineTasks, deadline);
			}
		}

		private void ViewByDeadlinesGroupByProject()
		{
			var deadlines = GetDeadLinesList();
			foreach (var deadline in deadlines)
			{
				var deadLineTasks = GetTasksByDay(deadline);

				ShowByDeadlineByGroup(deadLineTasks, deadline);
			}
		}



		private List<DateOnly> GetDeadLinesList()
		{
			var deadlines = _tasks.Values
                          .SelectMany(tasks => tasks)
                          .Where(task => task.Deadline != new DateOnly())
                          .Select(task => task.Deadline)
                          .Distinct()
                          .OrderBy(deadline => deadline)
                          .ToList();

			deadlines.Add(new DateOnly());
			return deadlines;
		}

		private void Add(string commandLine)
		{
			try {
				var subcommandRest = commandLine.Split(" ".ToCharArray(), 2);
				var subcommand = subcommandRest[0];
				if (subcommand == "project") {
					AddProject(subcommandRest[1]);
				} else if (subcommand == "task") {
					var projectTask = subcommandRest[1].Split(" ".ToCharArray(), 2);
					AddTask(projectTask[0], projectTask[1]);
				}
				else
					throw (new Exception());
			} catch
			{
				_console.WriteLine("Please format the input like this add project <project name> or add task <project name> <task description");
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

		private void AddDeadline(string commandLine)
		{
			try {
				var commandRest = commandLine.Split(" ".ToCharArray(), 2);
				long taskId = long.Parse(commandRest[0]);
				string dateString = commandRest[1];
				DateOnly dateOnly = DateOnly.ParseExact(dateString, "d-M-yyyy");

				var task = GetTaskById(taskId);
				if (task == null)
				{
					_console.WriteLine("Could not find a task with an ID of {0}.", taskId);
					return;
				}
				task.Deadline = dateOnly;
			} 
			catch {
				_console.WriteLine("The given deadline input should be like this: 'deadline <task id> <d-m-yyyy>'");
			}
		}

		private Task? GetTaskById(long taskId)
		{
			return _tasks
				.Select(project => project.Value.FirstOrDefault(task => task.Id == taskId))
				.Where(task => task != null)
				.FirstOrDefault();
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
			var identifiedTask = GetTaskById(id);
			if (identifiedTask == null) {
				_console.WriteLine("Could not find a task with an ID of {0}.", id);
				return;
			}

			identifiedTask.Done = done;
		}

		private void ShowTodaysProjects()
		{
				DateOnly today = DateOnly.FromDateTime(DateTime.Now);
				// Filter tasks and reconstruct the dictionary
				IDictionary<string, IList<Task>> todayTasks = GetTasksByDay(today);
				
				Show(todayTasks);
		}

		private IDictionary<string, IList<Task>> GetTasksByDay(DateOnly day)
		{
			return _tasks
				.Where(project => project.Value.Any(task => task.Deadline == day))
				.ToDictionary(
					kvp => kvp.Key,
					kvp => (IList<Task>)kvp.Value.Where(task => task.Deadline == day).ToList()
				);
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

		private long NextId()
		{
			return ++lastId;
		}
	}
}