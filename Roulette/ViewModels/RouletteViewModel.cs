using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using RouletteApp.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RouletteApp.ViewModels
{
    public class RouletteViewModel
    {
        #region Property
        private ReactiveProperty<int> Speed;
        private ReactiveProperty<bool> AutoStop;
        public ReactiveProperty<bool> IsSpin { get; private set; }

        public ReactiveCollection<string> Data { get; private set; } = new ReactiveCollection<string>();
        public ReactiveProperty<string> Edit { get; private set; } = new ReactiveProperty<string>();
        public ReactiveProperty<string> Result { get; private set; } = new ReactiveProperty<string>();
        public ReactiveProperty<bool> AutoUpdate { get; private set; } = new ReactiveProperty<bool>(true);
        public ReactiveProperty<bool> IsAllSpin { get; private set; } = new ReactiveProperty<bool>(true);
        public ReactiveProperty<bool> CanEdit { get; private set; } = new ReactiveProperty<bool>(true);
        public ReactiveCommand C_SpinRoulette { get; private set; } = new ReactiveCommand();
        public ReactiveCommand C_StopRoulette { get; private set; } = new ReactiveCommand();
        public ReactiveCommand C_ChangeContent { get; private set; } = new ReactiveCommand();
        public ReactiveCommand C_SaveToCSV { get; private set; } = new ReactiveCommand();
        public ReactiveCommand C_LoadFromCSV { get; private set; } = new ReactiveCommand();
        private Roulette Roulette = new Roulette();
        #endregion
        public RouletteViewModel(ReactiveProperty<int> speed, ReactiveProperty<bool> autoStop)
        {
            this.Speed = speed;
            this.AutoStop = autoStop;
            this.IsSpin = this.Roulette.ObserveProperty(x => x.IsSpin).ToReactiveProperty();

            //ルーレットを回せる条件設定
            this.C_SpinRoulette = IsSpin
                .Select(x => !x)
                .ToReactiveCommand();
            //ルーレットを回す処理
            this.C_SpinRoulette.Subscribe(_ => SpinRoulette(this.Roulette, this.Result));
            //ルーレットを止める条件設定
            this.C_StopRoulette = IsSpin
                .CombineLatest(this.AutoStop, (x, y) => x && !y)
                .ToReactiveCommand();
            //ルーレットを止める処理
            this.C_StopRoulette.Subscribe(_ => this.StopRoulette(this.Roulette));
            //コレクション追加時の監視
            this.Data.ObserveAddChanged()
                .Subscribe(x => this.Roulette.Data.Add(x));
            //コレクション削除時の監視
            this.Data.ObserveRemoveChanged()
                .Subscribe(x => this.Roulette.Data.Remove(x));
            //コレクションリセット時の監視
            this.Data.ObserveResetChanged()
                .Subscribe(x => this.Roulette.Data.Clear());
            //コレクションの変更可能条件の設定
            this.C_ChangeContent = IsSpin
                .Select(x => !x)
                .ToReactiveCommand();
            //コレクションへの変更処理
            this.C_ChangeContent.Subscribe((x => StringSplit((string)x)));
            //自動更新がTrueならEdit変更時にコレクションへ追加する
            Edit.Subscribe(x =>
            {
                if (this.AutoUpdate.Value) this.StringSplit(x);
            });
            //Edit可否の設定
            this.CanEdit = IsSpin
                .Select(x => !x)
                .ToReactiveProperty();
            //CSV保存
            this.C_SaveToCSV.Subscribe(_ => this.SaveToCSV(this.Edit.Value));
            //CSVが読み込める条件設定
            this.C_LoadFromCSV =IsSpin
                .Select(x => !x)
                .ToReactiveCommand();
            //CSV読込
            this.C_LoadFromCSV.Subscribe(_ =>
            {
                this.Edit.Value = this.LoadFromCSV();
                this.StringSplit(this.Edit.Value);
            });
        }
        #region Method
        /// <summary>
        /// ルーレットを回す
        /// </summary>
        /// <param name="roulette">回すルーレット</param>
        /// <param name="result">結果を入れるReactiveProperty</param>
        private void SpinRoulette(Roulette roulette, ReactiveProperty<string> result)
        {
            if (this.Roulette.Data.Count == 0) return;
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
        }
        /// <summary>
        /// カンマ(,)区切りで文字列をコレクションにする
        /// </summary>
        /// <param name="source">コレクションにする文字列</param>
        /// <param name="collection">変更するコレクション</param>
        public void StringSplit(string source)
        {
            this.Data.Clear();
            if (string.IsNullOrWhiteSpace(source)) return;
            foreach (var str in source.Split(','))
            {
                this.Data.Add(str);
            }
        }
        /// <summary>
        /// Saveダイアログを表示して指定したテキストをcsvで保存する
        /// </summary>
        /// <param name="text">保存するテキスト</param>
        private void SaveToCSV(string text)
        {
            var filePath = SaveAndLoadFileDialogHelper.ShowSaveDialog("CSV(カンマ区切り)(*.csv) | *.csv");
            if (!string.IsNullOrWhiteSpace(filePath)) FileIOHelper.SaveText(text, filePath);
        }
        /// <summary>
        /// Loadダイアログを表示して選択したcsvファイルを読み込んで返す
        /// </summary>
        /// <param name="edit"></param>
        /// <returns></returns>
        private string LoadFromCSV()
        {
            var filePath = SaveAndLoadFileDialogHelper.ShowLoadDialog("CSV(カンマ区切り)(*.csv) | *.csv");
            return FileIOHelper.LoadText(filePath);
        }
        #endregion
    }
}
