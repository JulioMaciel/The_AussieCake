using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace AussieCake.Util.WPF
{
  public static class HelperWPF
  {
    public static void CleanStackChildren(StackPanel stackFather)
    {
      foreach (var paragraph in stackFather.Children)
      {
        if (paragraph is StackPanel)
        {
          var stackParagraph = (StackPanel)paragraph;
          foreach (var line in stackParagraph.Children)
          {
            if (line is StackPanel)
            {
              var stackLine = (StackPanel)line;

              for (int i = 0; i < stackLine.Children.Count; i++)
              {
                var item = stackLine.Children[i];
                if (item is TextBox)
                {
                  stackLine.Children.Remove(item as TextBox);
                  i--;
                }
                else if (item is TextBlock)
                {
                  stackLine.Children.Remove(item as TextBlock);
                  i--;
                }
              }
            }
          }
          stackParagraph.Visibility = Visibility.Collapsed;
        }
      }
    }
  }
}
