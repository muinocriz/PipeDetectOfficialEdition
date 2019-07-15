using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmLight4.Common
{
    public static class RegeditHelper
    {
        /// <summary>
        /// 新建注册表
        /// </summary>
        /// <param name="date"></param>
        public static void SetRegedit(string key, string value)
        {
            RegistryKey hklm = Registry.CurrentUser;
            RegistryKey software = hklm.OpenSubKey("Software", true);
            RegistryKey main1 = software.CreateSubKey("pds");
            RegistryKey main2 = main1.CreateSubKey("pdsd");
            main2.SetValue(key, value);
        }

        public static string GetRegedit(string key)
        {
            RegistryKey regkey = Registry.CurrentUser;
            RegistryKey software = regkey.OpenSubKey("Software");
            RegistryKey main1 = software.OpenSubKey("pds");
            RegistryKey main2 = main1.OpenSubKey("pdsd");
            return main2.GetValue(key).ToString();
        }
    }
}
