using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace CheckSolution.Models
{
  class Solution
  {
    //Solution name
    public string Name { get; set; }
    //File path
    public string Folder { get; set; }
    //File name (without path, including extension)
    public string FileName { get; set; }
    public string FileFullName { get { return $"{Folder}\\{FileName}"; } }
    public string SolutionGuid { get; set; }
    public bool HasGit { get; set; }
    public List<string> Folders { get; set; } = new List<string>();
    public List<Project> Projects { get; set; } = new List<Project>();
    public List<string> MissedProjectLinks { get; set; } = new List<string>();

    public Solution(string solutionPath)
    {
      DirectoryInfo info = new DirectoryInfo(solutionPath);
      FileName = info.Name;
      Name = FileName.Replace(".sln", "");
      Folder = info.Parent.FullName;
      CollectProjectsFromSolution();
      CollectFolders();
    }

    private void CollectProjectsFromSolution()
    {
      string line;
      using (StreamReader stream = File.OpenText(FileFullName))
      {
        while (!stream.EndOfStream)
        {
          line = stream.ReadLine().Trim();
          if (line.StartsWith("Project(\""))
          {
            Project project = new Project();
            if (project.ReadSolutionProjects(line))
            {
              Projects.Add(project);
            }
          }

          if (line.StartsWith("SolutionGuid"))
          {
            string[] array = line.Split('=');
            SolutionGuid = array[1].Trim();
          }
        }
      }
    }

    private void CollectFolders()
    {
      IEnumerable<string> folders = Directory.EnumerateDirectories(Folder);
      foreach (string item in folders)
      {
        DirectoryInfo info = new DirectoryInfo(item);
        string folderName = info.Name;
        if (folderName == ".git")
        {
          HasGit = true;
        }
        else if (folderName == ".vs") { }
        else
        {
          Folders.Add(folderName);
        }
      }
    }

    public void CheckProjectFolders()
    {
      foreach (Project item in Projects)
      {
        item.DoesExists = Folders.Contains(item.Name);
      }

      foreach (string item in Folders)
      {
        if (Projects.Where(x => x.Name == item).Count() == 0)
        {
          MissedProjectLinks.Add(item);
        }
      }
    }
  }
}
