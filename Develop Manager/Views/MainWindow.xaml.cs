using Develop_Manager.ViewModels;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Develop_Manager
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    MainViewModel MainVM;
    public MainWindow()
    {
      InitializeComponent();

      MainVM = new MainViewModel(this);
      MainVM.InitializeFolderTree();

      SolutionTreeView.SelectedItemChanged += SolutionTreeView_SelectedItemChanged;
      ProjectTreeView.SelectedItemChanged += ProjectTreeView_SelectedItemChanged;
    }

    private void SolutionTreeView_SelectedItemChanged(object sender, 
      RoutedPropertyChangedEventArgs<object> e)
    {
      if (!(((TreeViewItem)e.NewValue).Foreground == Brushes.Black)) { return; }

      MainVM.SelectSolution(((TreeViewItem)e.NewValue).Header.ToString());
    }

    private void ProjectTreeView_SelectedItemChanged(object sender,
      RoutedPropertyChangedEventArgs<object> e)
    {
      if ((TreeViewItem)e.NewValue == null) { return; }
      if (((((TreeViewItem)e.NewValue).Tag) == null)) { return; }

      MainVM.SelectProject(((TreeViewItem)e.NewValue).Header.ToString());
    }

  }
}
