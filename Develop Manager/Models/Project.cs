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
    public string InitGuid { get; set; }
    public string Name { get; set; }
    public string ProjectPath { get; set; }
    public string ProjectFile { get; set; }
    public string ProjectFullName { get; set; }
    public string ProjectGuid { get; set; }
    public string OutputType { get; set; }
    public bool DoesExists { get; set; }
    public bool HasReadMe { get; set; }
    public string HistoryFile { get; set; }

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
        ProjectGuid = array[2].Trim().Replace("\"", "");
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
      //XmlDocument xml = new XmlDocument();
      //xml.Load(fileName);

      //var nodeList = xml.DocumentElement.SelectNodes("Project");

      //OutputType = node.Attributes["OutputType"].ToString();

      /* https://stackoverflow.com/questions/1191151/reading-the-list-of-references-from-csproj-files
      XNamespace msbuild = "http://schemas.microsoft.com/developer/msbuild/2003";
      XDocument projDefinition = XDocument.Load(fileName);
      IEnumerable<string> references = projDefinition
          .Element(msbuild + "Project")
          .Elements(msbuild + "ItemGroup")
          .Elements(msbuild + "Reference")
          .Select(refElem => refElem.Value);
      */

      /* https://stackoverflow.com/questions/4171451/selectsinglenode-returns-null-when-tag-contains-xmlnamespace/4171468#4171468
      XmlNamespaceManager ns = new XmlNamespaceManager(xml.NameTable);
      ns.AddNamespace("msbld", "http://schemas.microsoft.com/developer/msbuild/2003");
      XmlNode node = xml.SelectSingleNode("//msbld:Project", ns);
      */

      string project;

      using (StreamReader stream = new StreamReader(fileName))
      {
        project = stream.ReadToEnd();
      }
    }
  }
}
