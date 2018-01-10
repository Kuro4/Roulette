using Microsoft.Practices.Unity;
using Prism.Mvvm;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Roulette.ViewModels
{
    public class ShellViewModel:BindableBase
    {
        #region Property
        public ReactiveProperty<int> Speed { get; private set; } = new ReactiveProperty<int>(100);
        public ReactiveProperty<bool> AutoStop { get; private set; } = new ReactiveProperty<bool>(true);
        public ReactiveProperty<bool> CanAutoStopChange { get; private set; } = new ReactiveProperty<bool>(true);
        public ReactiveCollection<string> Data1 { get; private set; } = new ReactiveCollection<string>();
        public ReactiveCollection<string> Data2 { get; private set; } = new ReactiveCollection<string>();
        public ReactiveCollection<string> Data3 { get; private set; } = new ReactiveCollection<string>();
        public ReactiveCollection<string> Data4 { get; private set; } = new ReactiveCollection<string>();
        public ReactiveCollection<string> Data5 { get; private set; } = new ReactiveCollection<string>();
        public ReactiveProperty<string> Edit1 { get; private set; } = new ReactiveProperty<string>();
        public ReactiveProperty<string> Edit2 { get; private set; } = new ReactiveProperty<string>();
        public ReactiveProperty<string> Edit3 { get; private set; } = new ReactiveProperty<string>();
        public ReactiveProperty<string> Edit4 { get; private set; } = new ReactiveProperty<string>();
        public ReactiveProperty<string> Edit5 { get; private set; } = new ReactiveProperty<string>();
        public ReactiveProperty<string> Result1 { get; private set; } = new ReactiveProperty<string>();
        public ReactiveProperty<string> Result2 { get; private set; } = new ReactiveProperty<string>();
        public ReactiveProperty<string> Result3 { get; private set; } = new ReactiveProperty<string>();
        public ReactiveProperty<string> Result4 { get; private set; } = new ReactiveProperty<string>();
        public ReactiveProperty<string> Result5 { get; private set; } = new ReactiveProperty<string>();
        private Models.Roulette Roulette1 = new Models.Roulette();
        private Models.Roulette Roulette2 = new Models.Roulette();
        private Models.Roulette Roulette3 = new Models.Roulette();
        private Models.Roulette Roulette4 = new Models.Roulette();
        private Models.Roulette Roulette5 = new Models.Roulette();
        public ReactiveCommand SpinRoulette1 { get; private set; } = new ReactiveCommand();
        public ReactiveCommand SpinRoulette2 { get; private set; } = new ReactiveCommand();
        public ReactiveCommand SpinRoulette3 { get; private set; } = new ReactiveCommand();
        public ReactiveCommand SpinRoulette4 { get; private set; } = new ReactiveCommand();
        public ReactiveCommand SpinRoulette5 { get; private set; } = new ReactiveCommand();
        public ReactiveCommand StopRoulette1 { get; private set; } = new ReactiveCommand();
        public ReactiveCommand StopRoulette2 { get; private set; } = new ReactiveCommand();
        public ReactiveCommand StopRoulette3 { get; private set; } = new ReactiveCommand();
        public ReactiveCommand StopRoulette4 { get; private set; } = new ReactiveCommand();
        public ReactiveCommand StopRoulette5 { get; private set; } = new ReactiveCommand();
        public ReactiveCommand ChangeContent1 { get; private set; } = new ReactiveCommand();
        public ReactiveCommand ChangeContent2 { get; private set; } = new ReactiveCommand();
        public ReactiveCommand ChangeContent3 { get; private set; } = new ReactiveCommand();
        public ReactiveCommand ChangeContent4 { get; private set; } = new ReactiveCommand();
        public ReactiveCommand ChangeContent5 { get; private set; } = new ReactiveCommand();
        public ReactiveCommand LoadFromCSV1 { get; private set; } = new ReactiveCommand();
        public ReactiveCommand LoadFromCSV2 { get; private set; } = new ReactiveCommand();
        public ReactiveCommand LoadFromCSV3 { get; private set; } = new ReactiveCommand();
        public ReactiveCommand LoadFromCSV4 { get; private set; } = new ReactiveCommand();
        public ReactiveCommand LoadFromCSV5 { get; private set; } = new ReactiveCommand();
        #endregion
        public ShellViewModel()
        {
            #region Roulette1
            //ルーレットを回せる条件設定
            this.SpinRoulette1 = this.Roulette1
                .ObserveProperty(x => x.IsSpin)
                .ToReactiveProperty()
                .Select(x => !x)
                .ToReactiveCommand();
            //ルーレットを回す処理
            this.SpinRoulette1.Subscribe(_ => SpinRoulette(this.Roulette1,this.Result1));
            //ルーレットを止める条件設定
            this.StopRoulette1 = this.Roulette1
                .ObserveProperty(x => x.IsSpin)
                .ToReactiveProperty()
                .Select(x => x)
                .CombineLatest(this.AutoStop, (x, y) => x && !y)
                .ToReactiveCommand();
            //ルーレットを止める処理
            this.StopRoulette1.Subscribe(_ =>this.StopRoulette(this.Roulette1));
            //コレクション追加時の監視
            this.Data1.ObserveAddChanged()
                .Subscribe(x => this.Roulette1.Data.Add(x));
            //コレクション削除時の監視
            this.Data1.ObserveRemoveChanged()
                .Subscribe(x => this.Roulette1.Data.Remove(x));
            //コレクションリセット時の監視
            this.Data1.ObserveResetChanged()
                .Subscribe(x => this.Roulette1.Data.Clear());
            this.ChangeContent1 = this.Roulette1
                .ObserveProperty(x => x.IsSpin)
                .ToReactiveProperty()
                .Select(x => !x)
                .ToReactiveCommand();
            //コレクションへの変更処理
            this.ChangeContent1.Subscribe((x => StringSplit((string)x,this.Data1)));
            #endregion
        }
        #region Method
        /// <summary>
        /// ルーレットを回す
        /// </summary>
        /// <param name="roulette">回すルーレット</param>
        /// <param name="result">結果を入れるReactiveProperty</param>
        private void SpinRoulette(Models.Roulette roulette,ReactiveProperty<string> result)
        {
            this.CanAutoStopChange.Value = false;
            Task.Run(async () =>
            {
                var random = new Random();
                //1秒～10秒範囲
                var randomTime = random.Next(1000, 10000);
                var stopWach = new Stopwatch();
                stopWach.Reset();
                stopWach.Start();
                foreach (var res in roulette.Spin())
                {
                    await Task.Delay(Speed.Value);
                    result.Value = res;
                    if (this.AutoStop.Value && stopWach.ElapsedMilliseconds >= randomTime)
                    {
                        stopWach.Stop();
                        this.StopRoulette(roulette);
                    }
                }
            });
        }
        /// <summary>
        /// ルーレットを止める
        /// </summary>
        /// <param name="roulette">止めるルーレット</param>
        private void StopRoulette(Models.Roulette roulette)
        {
            roulette.Stop();
            this.CanAutoStopChange.Value = true;
        }
        /// <summary>
        /// カンマ(,)区切りで文字列をコレクションにする
        /// </summary>
        /// <param name="source">コレクションにする文字列</param>
        /// <param name="collection">変更するコレクション</param>
        public void StringSplit(string source,ICollection<string> collection)
        {
            collection.Clear();
            foreach(var str in source.Split(','))
            {
                collection.Add(str);
            }
        }
        #endregion
    }
}
