namespace TaskList
{
	public class TaskListService
	{
		private readonly IDictionary<string, IList<Task>> _tasks = new Dictionary<string, IList<Task>>();
		
		private long lastId = 0;

		public TaskListService() {}
	}
}