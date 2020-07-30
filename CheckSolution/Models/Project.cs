using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckSolution.Models
{
  class Project
  {
    public string InitGuid { get; set; }
    public string Name { get; set; }
    public string OutputPath { get; set; }
    public string ProjectGuid { get; set; }
    public bool DoesExists { get; set; }

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
        OutputPath = array[1].Trim().Replace("\"", "");
        ProjectGuid = array[2].Trim().Replace("\"", "");
      }
      else
      {
        result = false;
      }

      return result;
    }
  }
}
