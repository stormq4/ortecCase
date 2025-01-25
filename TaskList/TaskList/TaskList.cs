using System;
using System.Collections.Generic;
using System.Linq;

namespace TaskList
{
	public sealed class TaskList
	{
		private const string QUIT = "quit";
		public static readonly string startupText = "Welcome to TaskList! Type 'help' for available commands.";

		private readonly IDictionary<string, IList<Task>> _tasks = new Dictionary<string, IList<Task>>();
		private readonly IConsole _console;

		private long lastId = 0;

		public static void Main(string[] args)
		{
			new TaskList(new RealConsole()).Run();
		}

		public TaskList(IConsole console)
		{
			this._console = console;
		}

		public void Run() // apparte klasse voor console
		{
			_console.WriteLine(startupText);
			while (true) {
				_console.Write("> ");
				var command = _console.ReadLine();
				if (command == QUIT) {
					break;
				}
				Execute(command); // apparte klasse --> met interface
			}
		}

		private void Execute(string commandLine)
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
			//deadline
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

		private long NextId()
		{
			return ++lastId;
		}
	}
}
