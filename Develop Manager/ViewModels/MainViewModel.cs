using Develop_Manager.Models;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Develop_Manager.ViewModels
{
  class MainViewModel
  {
    MainWindow MainV;
    Solution Solution;
    Project Project;

    public string RootFolder { get; set; } = 
      Environment.GetEnvironmentVariable("GitDev").Replace("\\", "\\\\");

    public MainViewModel(MainWindow mainView)
    {
      MainV = mainView;
    }

    public void InitializeFolderTree()
    {
      CollectRootFolders(MainV.SolutionTreeView, RootFolder);
    }

    public void SelectSolution(string solutionName)
    {
      //Hide Project
      MainV.ProjectTreeView.Visibility = Visibility.Hidden;
      MainV.ProjectStackPanel.Visibility = Visibility.Hidden;

      //Remove ProjectTreeView items
      MainV.ProjectTreeView.Items.Clear();

      //Remove Solution details
      MainV.SolutionStackPanel.Children.Clear();
      MainV.ProjectStackPanel.Children.Clear();

      Solution = (Solution)((TreeViewItem)MainV.SolutionTreeView.SelectedItem).Tag;
      DisplaySolutionDetails();
      DisplayProjectTreeView();
    }

    public void SelectProject(string projectName)
    {
      Project = (Project)((TreeViewItem)MainV.ProjectTreeView.SelectedItem).Tag;
      DisplayProjectDetails();
    }

    #region Solution TreeView

    private void CollectRootFolders(TreeView root, string path)
    {
      string[] folders = Directory.GetDirectories(path);

      foreach (string folder in folders)
      {
        TreeViewItem item = new TreeViewItem()
        {
          Header = folder.Replace($"{path}\\", ""),
          Margin = new Thickness(0, 0, 5, 0),
        };
        root.Items.Add(item);
        if (CheckSolutionFile(folder))
        {
          item.Foreground = Brushes.Black;
          item.Tag = Solution;
        }
        else
        {
          item.Foreground = Brushes.Gray;
          CollectSubFolders(item, folder);
        }
      }
    }

    private bool CheckSolutionFile(string path)
    {
      DirectoryInfo info = new DirectoryInfo(path);
      string[] files = Directory.GetFiles(path);

      foreach (string file in files)
      {
        if ($"{path}\\{info.Name}.sln" == file)
        {
          Solution = new Solution(file);
          return true;
        }
      }

      return false;
    }

    private void CollectSubFolders(TreeViewItem currentItem, string path)
    {
      string[] folders = Directory.GetDirectories(path);

      foreach (string folder in folders)
      {
        DirectoryInfo info = new DirectoryInfo(folder);
        if (info.Name == ".git" || info.Name == ".vs")
        {
          continue;
        }

        TreeViewItem item = new TreeViewItem()
        {
          Header = folder.Replace($"{path}\\", ""),
          Margin = new Thickness(0, 0, 5, 0),
        };
        currentItem.Items.Add(item);
        if (CheckSolutionFile(folder))
        {
          item.Foreground = Brushes.Black;
          item.Tag = Solution;
        }
        else
        {
          item.Foreground = Brushes.Gray;
          CollectSubFolders(item, folder);
        }
      }
    }

    #endregion

    #region Solution details

    private void DisplaySolutionDetails()
    {
      MainV.SolutionStackPanel.Visibility = Visibility.Visible;
      StackPanel solutionDetails = new StackPanel();

      solutionDetails.Children.Add(LabelBlock("", $"Solution {Solution.Name}"));
      solutionDetails.Children.Add(LabelBlock("File", Solution.SolutionFull));
      solutionDetails.Children.Add(LabelBlock("Folder", Solution.SolutionPath));
      solutionDetails.Children.Add(LabelBlock("Guid", Solution.SolutionGuid));
      solutionDetails.Children.Add(LabelBlock("Git", 
        Solution.HasGit ? "Initialized" : "This solution is not added to Source Control"));

      Border border = new Border()
      {
        BorderBrush = Brushes.DarkGray,
        BorderThickness = new Thickness(1),
        CornerRadius = new CornerRadius(5),
        Margin = new Thickness(5, 0, 5, 5),
        Padding = new Thickness(5),
        Child = solutionDetails,
      };

      MainV.SolutionStackPanel.Children.Add(border);
    }

    #endregion

    private void DisplayProjectTreeView()
    {
      foreach (Project project in Solution.Projects)
      {
        TreeViewItem item = new TreeViewItem()
        {
          Header = project.Name,
          Foreground = Brushes.Black,
          Margin = new Thickness(0, 0, 5, 0),
        };
        if (project.DoesExists)
        {
          item.Tag = project;
        }
        else
        {
          item.Foreground = Brushes.Gray;
        }
        MainV.ProjectTreeView.Items.Add(item);
      }

      foreach (string folder in Solution.MissedProjectLinks)
      {
        TreeViewItem item = new TreeViewItem()
        {
          Header = folder,
          Margin = new Thickness(0, 0, 5, 0),
          Foreground = Brushes.Blue,
        };
        MainV.ProjectTreeView.Items.Add(item);
      }

      MainV.ProjectTreeView.Visibility = Visibility.Visible;
    }

    private void DisplayProjectDetails()
    {
      MainV.ProjectStackPanel.Visibility = Visibility.Visible;

      StackPanel projectDetails = new StackPanel();

      projectDetails.Children.Add(LabelBlock("", $"Project {Project.Name}"));
      projectDetails.Children.Add(LabelBlock("Initial guid", Project.InitGuid));
      projectDetails.Children.Add(LabelBlock("Project guid", Project.ProjectGuid));
      projectDetails.Children.Add(LabelBlock("Project file", Project.ProjectFile));
      projectDetails.Children.Add(LabelBlock("README.md", Project.HasReadMe ?
        "Exists" : "Doesn't exists"));
      projectDetails.Children.Add(LabelBlock("History file", Project.HistoryFile ?? 
        "Doesn't exist"));

      Border border = new Border()
      {
        BorderBrush = Brushes.DarkGray,
        BorderThickness = new Thickness(1),
        CornerRadius = new CornerRadius(5),
        Margin = new Thickness(5, 0, 5, 5),
        Padding = new Thickness(5),
        Child = projectDetails,
      };

      MainV.ProjectStackPanel.Children.Add(border);
    }

    private StackPanel LabelBlock(string label, string content)
    {
      TextBlock text;
      StackPanel result = new StackPanel()
      {
        Orientation = Orientation.Horizontal,
      };

      if (!string.IsNullOrEmpty(label))
      {
        text = new TextBlock()
        {
          Text = $"{label}:",
          Width = 100,
        };
        result.Children.Add(text);
      }

      text = new TextBlock()
      {
        Text = content,
      };
      if (string.IsNullOrEmpty(label))
      {
        text.FontSize = 16;
        text.FontWeight = FontWeights.Bold;
      }
      result.Children.Add(text);

      return result;
    }
  }
}
