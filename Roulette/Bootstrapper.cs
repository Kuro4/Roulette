using Prism.Unity;
using Roulette.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Roulette
{
    class Bootstrapper: UnityBootstrapper
    {
        protected override DependencyObject CreateShell()
        {
            // this.ContainerでUnityのコンテナが取得できるので
            // そこからShellを作成する
            return this.Container.TryResolve<Shell>();
        }

        protected override void InitializeShell()
        {
            // Shellを表示する
            ((Window)this.Shell).Show();
        }
    }
}
