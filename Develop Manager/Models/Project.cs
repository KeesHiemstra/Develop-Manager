using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Develop_Manager.Models
{
  class Project
  {
    public string InitGuid { get; set; } //From solution
    public string Name { get; set; } //From solution
    public string ProjectPath { get; set; } //From solution
    public string ProjectFile { get; set; } //From solution
    public string ProjectFullName { get; set; } //From solution
    public string SolutionProjectGuid { get; set; } //From solution
    public string ProjectGuid { get; set; } //From project
    public string OutputType { get; set; } //From project
    public string RootNamespace { get; set; } //From project
    public string AssemblyName { get; set; } //From project
    public string TargetFrameworkVersion { get; set; } //From project
		public string LangVersion { get; set; }
		public bool DoesExists { get; set; } //Extern
    public bool HasReadMe { get; set; } //Extern
    public string HistoryFile { get; set; } //Extern

    /// <summary>
    /// Information from Solution file.
    /// </summary>
    /// <param name="line"></param>
    /// <returns></returns>
    public bool ReadSolutionProjects(string line)
    {
      //Project("{<Guid>}") = "<Name>", "<File>", "<Guid>"
      bool result = true;
      int pos;

      if (line.StartsWith("Project(\""))
      {
        line = line.Replace("Project(\"", "");
        pos = line.IndexOf("\"");
        InitGuid = line.Substring(0, pos);
        pos = line.IndexOf("=") + 1;
        line = line.Substring(pos).Trim();
        string[] array = line.Split(',');
        if (array.Length != 3)
        {
          return false;
        }
        Name = array[0].Trim().Replace("\"", "");
        ProjectFile = array[1].Trim().Replace("\"", "");
        SolutionProjectGuid = array[2].Trim().Replace("\"", "");
      }
      else
      {
        result = false;
      }

      return result;
    }

    /// <summary>
    /// Information from directories.
    /// </summary>
    /// <param name="folder"></param>
    internal void GetProjectDetails(string folder)
    {
      ProjectFullName = $"{folder}\\{ProjectFile}";
      FileInfo info = new FileInfo(ProjectFullName);
      ProjectPath = info.DirectoryName;
      HasReadMe = File.Exists($"{ProjectPath}\\README.md");
      try
      {
        IEnumerable<string> history =
          Directory.EnumerateFiles(ProjectPath, "History.txt", SearchOption.AllDirectories);
        if (!(history.Count() == 0))
        {
          HistoryFile = history.First();
        }
      }
      catch { }

      ReadProjectFile(ProjectFullName);
    }

    private void ReadProjectFile(string fileName)
    {
      string project;

      if (File.Exists(fileName))
      {
        using (StreamReader stream = new StreamReader(fileName))
        {
          project = stream.ReadToEnd();
        }

        ProjectGuid = ReadElement(project, "ProjectGuid");
        OutputType = ReadElement(project, "OutputType");
        RootNamespace = ReadElement(project, "RootNamespace");
        AssemblyName = ReadElement(project, "AssemblyName");
        TargetFrameworkVersion = ReadElement(project, "TargetFrameworkVersion");
        LangVersion = ReadElement(project, "LangVersion");
      }
    }

    private string ReadElement(string project, string element)
    {
      int pos1 = project.IndexOf($"<{element}>");
      if (pos1 < 0) { return string.Empty; }
      pos1 += element.Length + 2;
      int pos2 = project.IndexOf($"</{element}>", pos1);
      return project.Substring(pos1, pos2 - pos1);
    }
  }
}
