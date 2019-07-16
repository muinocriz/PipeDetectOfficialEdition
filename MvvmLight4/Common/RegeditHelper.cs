using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
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
        public static bool SetRegedit(string key, string value)
        {
            RegistryKey hklm = Registry.CurrentUser;
            try
            {
                RegistryKey software = hklm.OpenSubKey("Software", true);
                RegistryKey main1 = software.CreateSubKey("PDS");
                RegistryKey main2 = main1.CreateSubKey("PDSD");
                main2.SetValue(key, value, RegistryValueKind.String);
                return true;
            }
            catch (ArgumentNullException)
            {
                Console.WriteLine("name 为 null");
                return false;
            }
            catch (SecurityException)
            {
                Console.WriteLine("用户没有访问指定的模式中的注册表项所需的权限");
                return false;
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine("用户没有访问指定的模式中的注册表项所需的权限");
                return false;
            }
            catch (Exception e)
            {
                Console.WriteLine("发生异常：" + e.ToString());
                return false;
            }
        }

        public static string GetRegedit(string key)
        {
            string s = "";
            try
            {
                RegistryKey regkey = Registry.CurrentUser;
                RegistryKey software = regkey.OpenSubKey("Software");
                RegistryKey main1 = software.OpenSubKey("PDS");
                RegistryKey main2 = main1.OpenSubKey("PDSD");
                object o = main2.GetValue(key);
                s = o.ToString();
                return s;
            }
            catch (ArgumentNullException)
            {
                Console.WriteLine("name 为 null");
                return null;
            }
            catch (SecurityException)
            {
                Console.WriteLine("用户没有访问指定的模式中的注册表项所需的权限");
                return null;
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine("用户没有访问指定的模式中的注册表项所需的权限");
                return null;
            }
            catch (Exception e)
            {
                Console.WriteLine("发生异常：" + e.ToString());
                return null;
            }
        }
    }
}
