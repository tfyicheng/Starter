using Starter.Infrastructure;

namespace Starter.ViewModels
{
    public class TabItemViewModel : ViewModelBase
    {
        private string _title;

        public string Title
        {
            get => _title;
            set { _title = value; OnPropertyChanged(nameof(Title)); }
        }

        public TabItemViewModel(string title) : base(null)
        {
            _title = title;
        }


    }
}
