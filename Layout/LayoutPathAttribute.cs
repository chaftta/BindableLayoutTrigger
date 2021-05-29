using System;
using System.Diagnostics;
using System.Linq;
namespace XdLayout
{
    /// <summary>取得元のレイアウトパスの設定</summary>
    [Conditional("UNITY_EDITOR")]
    public class LayoutPathAttribute : Attribute
    {
        /// <summary>レイアウトパス</summary>
        public string Path;
        /// <summary>コンストラクタ</summary>
        /// <param name="Path">レイアウトパス</param>
        public LayoutPathAttribute(string Path)
        {
            this.Path = Path;
        }
        /// <summary>ファイルパターンのパスかどうか</summary>
        /// <returns>true:ファイルパターン false:固定パス</returns>
        public bool IsFilePattern()
        {
            return Path.Contains("*");
        }
        /// <summary>パスを含まない名前を取得する</summary>
        /// <returns>パスを含まない名前</returns>
        public string GetName()
        {
            return Path.Split('/').Last();
        }
        /// <summary>親のパスを取得する</summary>
        /// <returns>親のパス</returns>
        public (string Parent, string Name) GetPathInfo()
        {
            var Nodes = Path.Split('/');
            return (string.Join("/", Nodes.Take(Nodes.Length - 1)), Nodes.Last());
        }
    }
}