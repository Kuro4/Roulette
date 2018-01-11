using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RouletteApp.Models
{
    public class FileIOHelper
    {
        /// <summary>
        /// テキストを指定パスに保存する
        /// 同名ファイルがあるなら上書きする
        /// </summary>
        /// <param name="Text">保存するテキスト</param>
        /// <param name="filePath">保存するパス</param>
        /// <param name="encoding">エンコード(省略するとUTF-8)</param>
        public static void SaveText(string Text,string filePath,Encoding encoding = null)
        {
            if (encoding == null) encoding = Encoding.UTF8;
            if (string.IsNullOrWhiteSpace(Text)) return;
            //同名ファイルがあれば上書き、文字コードはUTF-8
            File.WriteAllText(filePath, Text, encoding);
        }
        /// <summary>
        /// 指定パスのテキストファイルを読み込んで返す
        /// </summary>
        /// <param name="filePath">読み込むファイル</param>
        /// <returns></returns>
        public static string LoadText(string filePath)
        {
            if (!File.Exists(filePath)) throw new FileNotFoundException();
            return File.ReadAllText(filePath);
        }
    }
}
