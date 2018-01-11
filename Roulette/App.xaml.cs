using RouletteApp.Properties;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace RouletteApp
{
    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            // 前バージョンからのUpgradeを実行していないときは、Upgradeを実施する
            if (Settings.Default.IsUpgrade == false)
            {
                Settings.Default.Upgrade();
                Settings.Default.IsUpgrade = true;
                Settings.Default.Save();
            }
            // Bootstrapperを起動する
            new Bootstrapper().Run();
        }
    }
}
