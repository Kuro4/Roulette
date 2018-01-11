using Microsoft.Practices.Unity;
using Prism.Mvvm;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using RouletteApp.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RouletteApp.ViewModels
{
    public class ShellViewModel:BindableBase
    {
        #region Property
        public ReactiveProperty<int> Speed { get; private set; } = new ReactiveProperty<int>(100);
        public ReactiveProperty<bool> AutoStop { get; private set; } = new ReactiveProperty<bool>(true);
        public ReactiveProperty<bool> CanAutoStopChange { get; private set; } = new ReactiveProperty<bool>(true);
        public RouletteViewModel Roulette1 { get; }
        public RouletteViewModel Roulette2 { get; }
        public RouletteViewModel Roulette3 { get; }
        public RouletteViewModel Roulette4 { get; }
        public RouletteViewModel Roulette5 { get; }
        public ReactiveCommand C_AllSpin { get; private set; } = new ReactiveCommand();
        public ReactiveCommand C_AllStop { get; private set; } = new ReactiveCommand();
        #endregion
        public ShellViewModel()
        {
            this.Roulette1 = new RouletteViewModel(this.Speed, this.AutoStop);
            this.Roulette2 = new RouletteViewModel(this.Speed, this.AutoStop);
            this.Roulette3 = new RouletteViewModel(this.Speed, this.AutoStop);
            this.Roulette4 = new RouletteViewModel(this.Speed, this.AutoStop);
            this.Roulette5 = new RouletteViewModel(this.Speed, this.AutoStop);
            //どれか一つでも回っていれば自動停止の変更を不能にする
            this.CanAutoStopChange = this.Roulette1.IsSpin
                .CombineLatest(this.Roulette2.IsSpin, (x, y) => !x && !y)
                .CombineLatest(this.Roulette3.IsSpin, (x, y) => x && !y)
                .CombineLatest(this.Roulette4.IsSpin, (x, y) => x && !y)
                .CombineLatest(this.Roulette5.IsSpin, (x, y) => x && !y)
                .ToReactiveProperty();
            //全て回っていなければ全て回すコマンドを活性化
            C_AllSpin = this.Roulette1.IsSpin
                .CombineLatest(this.Roulette2.IsSpin, (x, y) => !x && !y)
                .CombineLatest(this.Roulette3.IsSpin, (x, y) => x && !y)
                .CombineLatest(this.Roulette4.IsSpin, (x, y) => x && !y)
                .CombineLatest(this.Roulette5.IsSpin, (x, y) => x && !y)
                .ToReactiveCommand();
            //全て回す
            C_AllSpin.Subscribe(_ => 
            {
                if(this.Roulette1.IsAllSpin.Value) this.Roulette1.C_SpinRoulette.Execute();
                if (this.Roulette2.IsAllSpin.Value) this.Roulette2.C_SpinRoulette.Execute();
                if (this.Roulette3.IsAllSpin.Value) this.Roulette3.C_SpinRoulette.Execute();
                if (this.Roulette4.IsAllSpin.Value) this.Roulette4.C_SpinRoulette.Execute();
                if (this.Roulette5.IsAllSpin.Value) this.Roulette5.C_SpinRoulette.Execute();
            });
            //自動停止がオフかつどれか一つでも回っていれば全て止めるコマンドを活性化
            C_AllStop = this.Roulette1.IsSpin
                .CombineLatest(this.Roulette2.IsSpin, (x, y) => x || y)
                .CombineLatest(this.Roulette3.IsSpin, (x, y) => x || y)
                .CombineLatest(this.Roulette4.IsSpin, (x, y) => x || y)
                .CombineLatest(this.Roulette5.IsSpin, (x, y) => x || y)
                .CombineLatest(this.AutoStop,(x,y)=> x && !y)
                .ToReactiveCommand();
            //全て止める
            C_AllStop.Subscribe(_ =>
            {
                this.Roulette1.C_StopRoulette.Execute();
                this.Roulette2.C_StopRoulette.Execute();
                this.Roulette3.C_StopRoulette.Execute();
                this.Roulette4.C_StopRoulette.Execute();
                this.Roulette5.C_StopRoulette.Execute();
            });
        }
        #region Method
        #endregion
    }
}
