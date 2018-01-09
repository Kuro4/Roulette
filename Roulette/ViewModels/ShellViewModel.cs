using Microsoft.Practices.Unity;
using Prism.Mvvm;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Roulette.ViewModels
{
    public class ShellViewModel:BindableBase
    {
        public ReactiveCollection<string> Data1 { get; private set; } = new ReactiveCollection<string>();

        private string result1;
        public string Result1
        {
            get { return this.result1; }
            private set {this.SetProperty(ref this.result1, value); }
        }

        private Models.Roulette Roulette1 = new Models.Roulette();
        private Models.Roulette Roulette2 = new Models.Roulette();
        private Models.Roulette Roulette3 = new Models.Roulette();
        public ReactiveCommand SpinRoulette1 { get; private set; } = new ReactiveCommand();
        public ReactiveCommand StopRoulette1 { get; private set; } = new ReactiveCommand();

        public ShellViewModel()
        {
            Data1.ObserveAddChanged()
                .Subscribe(x => 
                {
                    this.Roulette1.Data.Add(x);
                });
            Data1.ObserveRemoveChanged()
                .Subscribe(x =>
                {
                    this.Roulette1.Data.Add(x);
                });


            this.Roulette1.Data.Add("test1");
            this.Roulette1.Data.Add("test2");
            this.Roulette1.Data.Add("test3");
            this.Roulette1.Data.Add("test4");

            this.SpinRoulette1 = this.Roulette1
                .ObserveProperty(x => x.IsSpin)
                .ToReactiveProperty()
                .Select(x => !x)
                .ToReactiveCommand();


            this.SpinRoulette1.Subscribe(_ =>
            {
                Task.Run(async () => 
                {
                    foreach (var res in this.Roulette1.Spin())
                    {
                        await Task.Delay(100);
                        this.Result1 = res;
                    }
                });
            });
            this.StopRoulette1.Subscribe(_ => this.Roulette1.Stop());
        }
    }
}
