using Microsoft.AspNetCore.Http.Connections;

namespace TaskList
{
	public class TaskListExecutor
	{
		private readonly ITaskListService _taskListService;
		private readonly IConsole _console;

		public TaskListExecutor(IConsole console, ITaskListService taskListService)
		{
			_console = console;
			_taskListService = taskListService;
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
			_console.WriteLine("--------show----------");
			Execute("show");
			_console.WriteLine("--------view-by-deadline----------");
			Execute("view-by-deadline");
			_console.WriteLine("--------show---------");
			Execute("show");
			_console.WriteLine("--------view-project-by-deadline----------");
			Execute("view-project-by-deadline");
			_console.WriteLine("--------show----------");
			Execute("show");
		}

		public void Execute(string commandLine)
		{
			var commandRest = commandLine.Split(" ".ToCharArray(), 2);
			var command = commandRest[0];
			try 
			{
				ExecuteSwitch(command, commandRest);
			} 
			catch (IndexOutOfRangeException ex)
			{
				_console.WriteLine(string.Format("Invalid input for command '{0}'. Enter 'help' for more information.", command));
			}
			catch (Exception ex)
			{
				_console.WriteLine(ex.Message);
			}
		}

		private void ExecuteSwitch(string command, string[] commandRest)
		{
			switch (command) {
				case "show":
					ShowProjects();
					break;
				case "add":
					Add(commandRest[1]);
					break;
				case "check":
					Check(commandRest[1]);
					break;
				case "uncheck":
					UnCheck(commandRest[1]);
					break;
				case "help":
					Help();
					break;
				case "deadline":
					AddDeadline(commandRest[1]);
					break;
				case "today":
					ViewTodaysProjects();
					break;
				case "view-by-deadline":
					ViewDeadlineByTasks();
					break;
				case "view-project-by-deadline":
					ViewByDeadlinesGroupByProject();
					break;
				default:
					UnrecognisedCommandError(command);
					break;
			}
		}

		private void Check (string idString) => _taskListService.Check(idString);
		private void UnCheck (string idString) => _taskListService.UnCheck(idString);
		private void ShowProjects() => Show(_taskListService.GetProjects());

		private void Show(IDictionary<string, IList<Task>> projects)
		{
			foreach (var project in projects) {
				_console.WriteLine(project.Key);
				foreach (var task in project.Value) {
					_console.WriteLine("    [{0}] {1}: {2}", (task.Done ? 'x' : ' '), task.Id, task.Description);
				}
				_console.WriteLine();
			}
		}

		private void ShowTasksByDeadline(DateOnly deadline, IList<Task> tasks )
		{
			if (deadline == new DateOnly())
				_console.WriteLine("No deadline:");
			else
				_console.WriteLine("{0}:", deadline);

			foreach(var task in tasks)
			{
				_console.WriteLine("    {0}: {1}", task.Id, task.Description);
			}
			_console.WriteLine();
		}

		private void ShowDeadlineByProject( DateOnly deadline,IDictionary<string, IList<Task>> projects)
		{
			if (deadline == new DateOnly())
				_console.WriteLine("No deadline:");
			else
				_console.WriteLine("{0}:", deadline);

			foreach (var project in projects) 
			{
				_console.WriteLine("    " + project.Key + ":");
				foreach(var task in project.Value)
					_console.WriteLine("        {0}: {1}", task.Id, task.Description);
			}
			_console.WriteLine();
		}

		private void ViewDeadlineByTasks()
		{
			var deadlinesTasks = _taskListService.GetTasksByDeadline();
			foreach (var deadlineTask in deadlinesTasks)
			{
				ShowTasksByDeadline(deadlineTask.Item1, deadlineTask.Item2);
			}
		}

		private void ViewByDeadlinesGroupByProject()
		{
			var projectDeadlines = _taskListService.GetProjectsByDeadline();

			foreach(var deadLineProject in projectDeadlines)
			{
				ShowDeadlineByProject(deadLineProject.Item1, deadLineProject.Item2);
			}
		}

		private void Add(string commandLine)
		{
			var subcommandRest = commandLine.Split(" ".ToCharArray(), 2);
			var subcommand = subcommandRest[0];
			if (subcommand == "project") {
				_taskListService.PostProject(subcommandRest[1]);
			} else if (subcommand == "task") {
				var projectTask = subcommandRest[1].Split(" ".ToCharArray(), 2);
				_taskListService.PostTask(projectTask[0], projectTask[1]);
			}
			else
				throw new Exception("Please format the input like this 'add project <project name>' or 'add task <project name> <task description>'");
		}

		private void AddDeadline(string commandLine)
		{
			var commandRest = commandLine.Split(" ".ToCharArray(), 2);
			if (!long.TryParse(commandRest[0], out long taskId))
				throw new Exception("The given deadline input should be like this: 'deadline <task id> <d-m-yyyy>'");

			string dateString = commandRest[1];
			if (!DateOnly.TryParseExact(dateString, "d-M-yyyy", out DateOnly date))
				throw new Exception("The given deadline input should be like this: 'deadline <task id> <d-m-yyyy>'");

			_taskListService.UpdateTaskDeadline(taskId, date);
	
		}

		private void ViewTodaysProjects()
		{
			DateOnly today = DateOnly.FromDateTime(DateTime.Now);
			IDictionary<string, IList<Task>> todayTasks = _taskListService.GetProjectsByDay(today);
			
			Show(todayTasks);
		}

		private void Help()
		{
			_console.WriteLine("Commands:");
			_console.WriteLine("  show");
			_console.WriteLine("  add project <project name>");
			_console.WriteLine("  add task <project name> <task description>");
			_console.WriteLine("  check <task ID>");
			_console.WriteLine("  uncheck <task ID>");
			_console.WriteLine("  deadline <task ID> <d-m-yyyy>");
			_console.WriteLine("  today");
			_console.WriteLine("  view-by-deadline");
			_console.WriteLine("  view-project-by-deadline");
			_console.WriteLine();
		}

		private void UnrecognisedCommandError(string command)
		{
			_console.WriteLine("I don't know what the command \"{0}\" is.", command);
		}
	}
}