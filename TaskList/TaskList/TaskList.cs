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

		public static void Main(string[] args)
		{
			new TaskList(new RealConsole()).Run();
		}

		public TaskList(IConsole console)
		{
			this._console = console;
		}

		public void Run() // apparte klasse voor run
		{
			var executor =new TaskListExecutor(_console, new TaskListService());
			_console.WriteLine(startupText);
			while (true) {
				_console.Write("> ");
				var command = _console.ReadLine();
				if (command == QUIT) {
					break;
				}
				executor.Execute(command); // apparte klasse --> met interface
			}
		}
	}
}
