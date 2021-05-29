using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
using UnityEngine;
namespace XdLayout.Editor
{
    /// <summary>エディタ用メニュー</summary>
    public static class EditorMenu
    {
        /// <summary>選択中のヒエラルキーのレイアウトパスをクリップボードにコピーする</summary>
        [MenuItem("GameObject/レイアウトパスのコピー", false, int.MinValue)]
        private static void CopyLayoutPath()
        {
            const string Indent = "        ";
            var Names = GetSelectedPaths();
            // コードの生成
            var Code = Names.Select(N =>
            {
                var Name = Path.GetFileName(N);
                return $"{Indent}[SerializeField, LayoutPath(\"{N}\")] private GameObject {Name};";
            });
            GUIUtility.systemCopyBuffer = string.Join("\n", Code) + "\n";
        }
        /// <summary>選択しているオブジェクトのパス一覧を取得する</summary>
        /// <returns>選択しているオブジェクトのパス一覧</returns>
        private static IEnumerable<string> GetSelectedPaths()
        {
            if (Selection.gameObjects == null || Selection.gameObjects.Length == 0) return null;
            GameObject Root = null;
            // プレハブモードの場合
            var Stage = PrefabStageUtility.GetCurrentPrefabStage();
            if (Stage != null && Stage.prefabContentsRoot != null)
            {
                Root = Stage.prefabContentsRoot;
            }
            // 選択されているオブジェクト毎のパスを取得
            return Selection.gameObjects.Select(G => GetPathName(Root, G.transform));
        }
        /// <summary>ルートからのパスを取得する</summary>
        /// <param name="Root">ルートオブジェクト(判定用)</param>
        /// <param name="Current">パスを取得するオブジェクト</param>
        /// <returns>ルートからのパス</returns>
        private static string GetPathName(GameObject Root, Transform Current)
        {
            var Builder = new StringBuilder(Current.name);  // 自分の名前を最初に追加
            Current = Current.parent;                       // 自分の親を開始オブジェクトにする
            // ルートまで親を辿ってパスを作成する
            while (Current != null && Root != Current.gameObject)
            {
                // 親のパスをバッファの先頭に挿入する
                Builder.Insert(0, Current.name + "/");
                // 親を操作対象にする
                Current = Current.parent;
            }
            return Builder.ToString();
        }
    }
}