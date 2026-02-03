using Starter.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace Starter.Views
{
    /// <summary>
    /// SettingsWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
            this.DataContext = new SettingsViewModel();
        }

        private SettingsViewModel VM => DataContext as SettingsViewModel;

        private void CategoryList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (VM == null) return;

            switch (((ListBox)sender).SelectedIndex)
            {
                case 0:
                    VM.ShowGeneral();
                    break;
                case 1:
                    VM.ShowStyle();
                    break;
            }
        }


    }
}
