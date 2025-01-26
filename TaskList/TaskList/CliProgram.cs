using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TaskList
{
    public static class CliProgram
    {
        
		public static void Main(string[] args)
		{
			new TaskList(new RealConsole(), new TaskListService()).Run();
		}
    }
}