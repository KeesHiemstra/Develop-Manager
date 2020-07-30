using CheckSolution.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckSolution
{
	class Program
	{
		static Solution Solution;

		static void Main(string[] args)
		{
			ReportSolution(@"Z:\Dev\Experiments\Scratch\Scratch.sln");
			Console.WriteLine("----");
			//ReportSolution(@"Z:\Dev\Develop Manager\Develop Manager.sln");


			Console.Write("\nPress any key...");
			Console.ReadKey();
		}

		private static void ReportSolution(string solutionPath)
		{
			Solution = new Solution(solutionPath);
			Solution.CheckProjectFolders();


			Console.WriteLine(Solution.Name);
			Console.WriteLine(Solution.FileName);
			Console.WriteLine(Solution.Folder);
			if (Solution.HasGit)
			{
				Console.WriteLine("Git does exists");
			}

			Console.WriteLine("\nFolders:");
			foreach (string item in Solution.Folders)
			{
				Console.WriteLine($" * {item}");
			}

			Console.WriteLine("\nExisting projects:");
			foreach (Project item in Solution.Projects.Where(x => x.DoesExists))
			{
				Console.WriteLine($" * {item.Name}");
			}

			Console.WriteLine("\nUnlink the following folder(s):");
			foreach (Project item in Solution.Projects.Where(x => !x.DoesExists))
			{
				Console.WriteLine($" * {item.Name}");
			}

			Console.WriteLine("\nLink the following folder(s):");
			foreach (string item in Solution.MissedProjectLinks)
			{
				Console.WriteLine($" * {item}");
			}
		}

	}
}
