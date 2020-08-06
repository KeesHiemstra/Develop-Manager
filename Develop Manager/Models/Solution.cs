using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Develop_Manager.Models
{
  class Solution
  {
    //Solution name
    public string Name { get; set; }
    //File path
    public string SolutionPath { get; set; }
    //File name (without path, including extension)
    public string SolutionFile { get; set; }
    public string SolutionFull { get { return $"{SolutionPath}\\{SolutionFile}"; } }
    public string SolutionGuid { get; set; }
    public bool HasGit { get; set; }
    public List<string> Folders { get; set; } = new List<string>();
    public List<Project> Projects { get; set; } = new List<Project>();
    public List<string> MissedProjectLinks { get; set; } = new List<string>();

    public Solution(string solutionPath)
    {
      DirectoryInfo info = new DirectoryInfo(solutionPath);
      SolutionFile = info.Name;
      Name = SolutionFile.Replace(".sln", "");
      SolutionPath = info.Parent.FullName;
      CollectProjectsFromSolution();
      CollectFolders();
      CheckProjectFolders();
    }

    private void CollectProjectsFromSolution()
    {
      string line;
      using (StreamReader stream = File.OpenText(SolutionFull))
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
      IEnumerable<string> folders = Directory.EnumerateDirectories(SolutionPath);
      foreach (string item in folders)
      {
        DirectoryInfo info = new DirectoryInfo(item);
        string folderName = info.Name;
        if (folderName == ".git")
        {
          HasGit = true;
        }
        else if (folderName == ".vs") { }
        else if (folderName == "packages") { }
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
        item.DoesExists = Folders.Exists(x => x.ToLower() == item.Name.ToLower());
        item.GetProjectDetails(SolutionPath);
      }

      foreach (string item in Folders)
      {
        if (Projects.Where(x => x.Name.ToLower() == item.ToLower()).Count() == 0)
        {
          MissedProjectLinks.Add(item);
        }
      }
    }

  }
}
