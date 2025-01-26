using System;
using System.Collections.Generic;
using System.Linq;

namespace TaskList
{
	public sealed class TaskList
	{
		private const string QUIT = "quit";
		public static readonly string startupText = "Welcome to TaskList! Type 'help' for available commands.";


		private readonly IConsole _console;
		private readonly ITaskListService _taskListService;


		public TaskList(IConsole console, ITaskListService taskListService)
		{
			this._console = console;
			this._taskListService = taskListService;
		}

		public void Run() // apparte klasse voor run
		{
			var executor = new TaskListExecutor(_console, _taskListService);
			_console.WriteLine(startupText);
			while (true) {
				_console.Write("> ");
				var command = _console.ReadLine();
				if (command == QUIT) {
					break;
				}
				executor.Execute(command);
			}
		}
	}
}
