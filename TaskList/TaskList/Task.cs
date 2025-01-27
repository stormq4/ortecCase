using System;
using System.Collections.Generic;

namespace TaskList
{

	// todo: new project class with a one to many relation to task class + ORM 
	// nullable deadline, description
	public class Task
	{
		public long Id { get; set; }

		public string Description { get; set; }

		public bool Done { get; set; }

		public DateOnly Deadline {get; set; }
	}
}
