using Starter.Infrastructure;
using System.Collections.ObjectModel;

namespace Starter.ViewModels
{
    public class TabBarViewModel : ViewModelBase
    {
        private int _selectedIndex;

        public ObservableCollection<TabItemViewModel> Tabs { get; } =
            new ObservableCollection<TabItemViewModel>();

        public int SelectedIndex
        {
            get => _selectedIndex;
            set
            {
                if (_selectedIndex == value) return;
                _selectedIndex = value;
                OnPropertyChanged(nameof(SelectedIndex));
            }
        }

        public TabBarViewModel() : base(null)
        {
            Tabs.Add(new TabItemViewModel("启动"));
            Tabs.Add(new TabItemViewModel("工具"));
            Tabs.Add(new TabItemViewModel("设置"));
            Tabs.Add(new TabItemViewModel("关于"));

            SelectedIndex = 0;
        }
    }
}
