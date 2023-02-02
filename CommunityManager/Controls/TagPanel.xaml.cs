using System;
using System.Collections.Generic;
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

namespace CommunityManager.Controls
{
  /// <summary>
  /// Interaction logic for TagPanel.xaml
  /// </summary>
  public partial class TagPanel : UserControl
  {
    public TagPanel()
    {
      InitializeComponent();
    }

    public static readonly DependencyProperty TagsProperty =
      DependencyProperty.Register("Tags", typeof(List<string>), typeof(TagPanel));

    public List<string> Tags
    {
      get => (List<string>)GetValue(TagsProperty);
      set => SetValue(TagsProperty, value);
    }
  }
}
