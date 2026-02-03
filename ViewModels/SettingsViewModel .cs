using Starter.Infrastructure;

namespace Starter.ViewModels
{
    public class SettingsViewModel : ViewModelBase
    {
        public object CurrentView { get; private set; }

        public SettingsViewModel() : base(null)
        {

            ShowGeneral();
        }

        public void ShowGeneral()
        {
            CurrentView = new GeneralSettingsViewModel();
            OnPropertyChanged(nameof(CurrentView));
        }

        public void ShowStyle()
        {
            CurrentView = new StyleSettingsViewModel();
            OnPropertyChanged(nameof(CurrentView));
        }
    }

}
