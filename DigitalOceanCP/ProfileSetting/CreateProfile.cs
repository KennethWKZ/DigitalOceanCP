using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace DigitalOceanCP.ProfileSetting
{
    internal sealed class ProfileLists : ApplicationSettingsBase
    {
        // If you used [ApplicationScopedSetting()] instead of [UserScopedSetting()],
        // you would NOT be able to persist any data changes!
        [UserScopedSetting()]
        [SettingsSerializeAs(System.Configuration.SettingsSerializeAs.Binary)]
        [DefaultSettingValue("")]
        public SortedList<string, string> ProfileList
        {
            get
            {
                return ((SortedList<string, string>)this["ProfileList"]);
            }
            set
            {
                this["ProfileList"] = (SortedList<string, string>)value;
            }
        }
    }
}
