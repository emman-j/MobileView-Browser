using MobileView.Core;
using MobileView.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobileView.Winforms.Utilities
{
    public class SettingsManager
    {
        public string UserAgent { get => Properties.Settings.Default.UserAgent; set => Properties.Settings.Default.UserAgent = value; }
        public string ProfileName { get => Properties.Settings.Default.ProfileName; set => Properties.Settings.Default.ProfileName = value; }
        public SettingsManager() { }

        public void Save()
        { 
            Properties.Settings.Default.Save();
        }
    }
}
