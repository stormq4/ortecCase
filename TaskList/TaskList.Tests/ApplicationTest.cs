using TaskList;

namespace Tasks
{
	[TestFixture]
	
	/* TODO

		seperate test file for the service
		write proper methods to test project dictionary, viewbydeadline, viewprojectsbydeadline
	*/
	public sealed class ApplicationTest
	{
		public const string PROMPT = "> ";

		private FakeConsole console;

		private ITaskListService _taskListService;
		private System.Threading.Thread applicationThread;

		[SetUp]
		public void StartTheApplication()
		{
			this.console = new FakeConsole();
			this._taskListService = new TaskListService();
			var taskList = new TaskList.TaskList(console, _taskListService);
			this.applicationThread = new System.Threading.Thread(() => taskList.Run());
			applicationThread.Start();
			ReadLines(TaskList.TaskList.startupText);
		}

		[TearDown]
		public void KillTheApplication()
		{
			if (applicationThread == null || !applicationThread.IsAlive)
			{
				return;
			}

			// version not supported
			// applicationThread.Abort();
			// throw new Exception("The application is still running.");
		}

		[Test, Timeout(1000)]
		public void ItWorks()
		{
			Execute("show");

			Execute("add project secrets");
			Execute("add task secrets Eat more donuts.");
			Execute("add task secrets Destroy all humans.");

			Execute("show");
			ReadLines(
				"secrets",
				"    [ ] 1: Eat more donuts.",
				"    [ ] 2: Destroy all humans.",
				""
			);

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
			ReadLines(
				"secrets",
				"    [x] 1: Eat more donuts.",
				"    [ ] 2: Destroy all humans.",
				"",
				"training",
				"    [x] 3: Four Elements of Simple Design",
				"    [ ] 4: SOLID",
				"    [x] 5: Coupling and Cohesion",
				"    [x] 6: Primitive Obsession",
				"    [ ] 7: Outside-In TDD",
				"    [ ] 8: Interaction-Driven Design",
				""
			);

			Execute("quit");
		}

		[Test, Timeout(1000)]
		public void Errors()
		{

			Execute("add project secrets");
			Execute("add task asdf asdf");
			ReadLines("Could not find a project with the name \"asdf\".");
			
			Execute("add project secrets");
			ReadLines("Project 'secrets' already exists.");

			Execute("add dd");
			ReadLines("Please format the input like this 'add project <project name>' or 'add task <project name> <task description>'");

			Execute("add project");
			ReadLines("Invalid input for command 'add'. Enter 'help' for more information.");
			Execute("add task");
			ReadLines("Invalid input for command 'add'. Enter 'help' for more information.");
			Execute("add task secrets");
			ReadLines("Invalid input for command 'add'. Enter 'help' for more information.");

			Execute("add dd");
			ReadLines("Please format the input like this 'add project <project name>' or 'add task <project name> <task description>'");

			Execute("check 12");
			ReadLines("Could not find a task with an ID of 12.");
			
			Execute("uncheck 12");
			ReadLines("Could not find a task with an ID of 12.");

			Execute("check a12");
			ReadLines("No id was given for command check");

			Execute("add task secrets yes yes");
			Execute("deadline 1 asdf");
			ReadLines("The given deadline input should be like this: 'deadline <task id> <d-m-yyyy>'");

			Execute("quit");
		}

		[Test, Timeout(1000)]
		public void ServiceTest1()
		{
			_taskListService.PostProject("test");
			_taskListService.PostTask("test", "testing1");
			_taskListService.PostTask("test", "testing2");
			_taskListService.PostTask("test", "testing3");

			_taskListService.UpdateTaskDeadline(1, DateOnly.FromDateTime(DateTime.Now));
			_taskListService.UpdateTaskDeadline(2, DateOnly.ParseExact("20-10-1997", "d-m-yyyy"));
			_taskListService.UpdateTaskDeadline(3, DateOnly.ParseExact("20-10-2006", "d-m-yyyy"));

			var fakeProjects = ServiceTestHelper.GetFakeProjects();
			var realProjects = _taskListService.GetProjects();
			
			Assert.AreEqual(realProjects["test"].Count(), fakeProjects["test"].Count());
			Assert.AreEqual(realProjects["test"].First().Deadline, fakeProjects["test"].First().Deadline);
			Assert.AreEqual(realProjects["test"].Last().Description, fakeProjects["test"].Last().Description);
			Assert.AreEqual(realProjects["test"].Last().Id, fakeProjects["test"].Last().Id);
		}

		[Test, Timeout(1000)]
		public void ServiceTest2()
		{
			_taskListService.PostProject("test");
			_taskListService.PostTask("test", "testing1");
			_taskListService.PostTask("test", "testing2");
			_taskListService.PostTask("test", "testing3");

			_taskListService.UpdateTaskDeadline(1, DateOnly.FromDateTime(DateTime.Now));
			_taskListService.UpdateTaskDeadline(2, DateOnly.ParseExact("20-10-1997", "d-m-yyyy"));
			_taskListService.UpdateTaskDeadline(3, DateOnly.ParseExact("20-10-2006", "d-m-yyyy"));

			_taskListService.PostProject("dance");
			_taskListService.PostTask("dance", "dancing1");
			_taskListService.PostTask("dance", "dancing2");
			_taskListService.PostTask("dance", "dancing3");
			_taskListService.UpdateTaskDeadline(4, DateOnly.ParseExact("20-10-1997", "d-m-yyyy"));

			
			var realDeadlineTasks = _taskListService.GetTasksByDeadline();
			var fakeDeadlineTasks = ServiceTestHelper.GetFakeDeadlineTasks();
			
			Assert.AreEqual(realDeadlineTasks.ToString(), fakeDeadlineTasks.ToString());
			
		}

		private void Execute(string command)
		{
			Read(PROMPT);
			Write(command);
		}

		private void Read(string expectedOutput)
		{
			var length = expectedOutput.Length;
			var actualOutput = console.RetrieveOutput(expectedOutput.Length);
			Assert.AreEqual(expectedOutput, actualOutput);
		}

		private void ReadLines(params string[] expectedOutput)
		{
			foreach (var line in expectedOutput)
			{
				Read(line + Environment.NewLine);
			}
		}

		private void Write(string input)
		{
			console.SendInput(input + Environment.NewLine);
		}
	}
}
