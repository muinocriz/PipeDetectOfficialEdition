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
                if (main2.GetValue(key) == null)
                {
                    //如果没有值就设置新值，如果有值就跳过
                    main2.SetValue(key, value, RegistryValueKind.String);
                }
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
        public static bool SetStRegedit(string key, string value)
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
            try
            {
                RegistryKey regkey = Registry.CurrentUser;
                RegistryKey software = regkey.OpenSubKey("Software");
                RegistryKey main1 = software.OpenSubKey("PDS");
                RegistryKey main2 = main1.OpenSubKey("PDSD");
                object o = main2.GetValue(key);
                //如果得到空值，就返回null，否则返回o.toString()
                return o?.ToString();
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

        public static int CheckRegedit()
        {
            string stateBase64 = GetRegedit("St");
            if(string.IsNullOrEmpty(stateBase64))
            {
                return -1;
            }
            string state = Base64Helper.DecodeBase64("utf-8", stateBase64);
            if(state== "warning")
            {
                return -1;
            }

            string todayString = Base64Helper.GetTime();
            DateTime today = DateTime.Parse(todayString);
            string targetDayBase64 = GetRegedit("sd");
            string targetDayString = Base64Helper.DecodeBase64("utf-8", targetDayBase64);
            DateTime dt = DateTime.Parse(targetDayString);
            //int result = DateTime.Compare(dt, DateTime.Now.AddDays(100));
            int result = DateTime.Compare(dt, today);
            if (result>=0)
            {
                return 1;
            }
            else
            {
                SetStRegedit("St", "d2FybmluZw==");
                return -1;
            }
        }
    }
}
