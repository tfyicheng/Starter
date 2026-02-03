using Starter.Infrastructure;
using Starter.Models;

namespace Starter.ViewModels
{
    public class GeneralSettingsViewModel : ViewModelBase
    {
        private readonly SettingsManager _settings = new SettingsManager();

        private readonly GeneralSettings _bg;
        public GeneralSettingsViewModel() : base(null)
        {
            _bg = _settings.LoadGeneral();
        }


        public bool OpenTray
        {
            get => _bg.OpenTray;
            set
            {
                if (_bg.OpenTray == value) return;
                _bg.OpenTray = value;
                SaveAndNotify();

                OnPropertyChanged(nameof(OpenTray));

                if (value)
                {
                    Helper.Common.ic.ShowTray();
                }
                else
                {
                    Helper.Common.ic.HideTray();
                }
            }
        }

        private void SaveAndNotify()
        {
            _settings.SaveGeneral(_bg);
        }

    }
}
