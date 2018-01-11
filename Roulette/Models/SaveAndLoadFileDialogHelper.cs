using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RouletteApp.Models
{
    public class SaveAndLoadFileDialogHelper
    {
        /// <summary>
        /// ファイル保存ダイアログを表示して入力されたファイルのフルパスを返す
        /// また、キャンセルされたらnullを返す
        /// </summary>
        /// <param name="filter">フィルター</param>
        /// <param name="initialDirectory">初期表示ディレクトリ(省略するとアプリケーションの保存場所)</param>
        /// <returns></returns>
        public static string ShowSaveDialog(string filter,string initialDirectory = null)
        {
            if (string.IsNullOrWhiteSpace(initialDirectory)) initialDirectory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            var saveDialog = new SaveFileDialog()
            {
                Filter = filter,
                InitialDirectory = initialDirectory,
            };
            var res = saveDialog.ShowDialog();
            if (!res.HasValue) return null;
            if (res.Value) return saveDialog.FileName;
            else return null;
        }
        /// <summary>
        /// ファイル読込ダイアログを表示して選択されたファイルのフルパスを返す
        /// また、キャンセルされたらnullを返す
        /// </summary>
        /// <param name="filter">フィルター</param>
        /// <param name="initialDirectory">初期表示ディレクトリ(省略するとアプリケーションの保存場所)</param>
        /// <returns></returns>
        public static string ShowLoadDialog(string filter,string initialDirectory = null)
        {
            if (string.IsNullOrWhiteSpace(initialDirectory)) initialDirectory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            var loadDialog = new OpenFileDialog()
            {
                Filter = filter,
                InitialDirectory = initialDirectory,
                Multiselect = false,
            };
            var res = loadDialog.ShowDialog();
            if (!res.HasValue) return null;
            if (res.Value) return loadDialog.FileName;
            else return null;
        }
    }
}
