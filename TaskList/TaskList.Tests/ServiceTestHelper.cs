using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tasks
{
    public static class ServiceTestHelper
    {
        
        public static  Dictionary<string, IList<TaskList.Task>> GetFakeProjects()
        {
            var fakeProjects = new Dictionary<string, IList<TaskList.Task>>();
			var testTasks = new List<TaskList.Task>
            {
                new TaskList.Task { Id = 1, Description = "testing1", Done = false, Deadline = DateOnly.FromDateTime(DateTime.Now) },
                new TaskList.Task { Id = 2, Description = "testing2", Done = false, Deadline = new DateOnly(1997, 10, 20) },
                new TaskList.Task { Id = 3, Description = "testing3", Done = false, Deadline = new DateOnly(2006, 10, 20) }
            };
			fakeProjects.Add("test", testTasks);
            return fakeProjects;
        }

        public  static List<(DateOnly, IList<TaskList.Task>)> GetFakeDeadlineTasks() => 
        new List<(DateOnly, IList<TaskList.Task>)>
        {
            (
                new DateOnly(1997, 10, 20),
                new List<TaskList.Task>
                {
                    new TaskList.Task { Id = 2, Description = "testing2", Done = false, Deadline = new DateOnly(1997, 10, 20) }
                }
            ),
            (
                new DateOnly(2006, 10, 20),
                new List<TaskList.Task>
                {
                    new TaskList.Task { Id = 3, Description = "testing3", Done = false, Deadline = new DateOnly(2006, 10, 20) }
                }
            ),
            (
                new DateOnly(2016, 10, 20),
                new List<TaskList.Task>
                {
                    new TaskList.Task { Id = 4, Description = "dancing1", Done = false, Deadline = new DateOnly(2016, 10, 20) }
                }
            ),
            (
                new DateOnly(2025, 1, 27),
                new List<TaskList.Task>
                {
                    new TaskList.Task { Id = 1, Description = "testing1", Done = false, Deadline = new DateOnly(2025, 1, 27) }
                }
            ),
            (
            new DateOnly(),
                new List<TaskList.Task>
                {
                    new TaskList.Task { Id = 5, Description = "dancing2", Done = false, Deadline = new DateOnly() },
                    new TaskList.Task { Id = 6, Description = "dancing3", Done = false, Deadline = new DateOnly() },
                }
            )
        };



    }
}