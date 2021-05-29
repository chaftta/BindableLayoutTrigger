using System;
using AkyuiUnity.Editor.ScriptableObject;
using AkyuiUnity.Loader;
using UnityEditor;
using UnityEngine;

namespace XdLayout.Editor
{
    /// <summary>スクリプトを追加するトリガー</summary>
    [CreateAssetMenu(menuName = "Akyui/Triggers/Layout", fileName = nameof(LayoutTrigger))]
    public class LayoutTrigger : AkyuiImportTrigger
    {
        /// <summary>追加するスクリプトクラス名のフォーマット(完全修飾名)</summary>
        /// <remarks>コントローラ等を付けたい場合は、ClassNameにxxx.{name}Controllerを設定すれば良い</remarks>
        [SerializeField]
        private string ClassName;
        /// <summary>プレハブ作成後処理</summary>
        /// <param name="Loader">ローダー</param>
        /// <param name="Prefab">作成されるプレハブのインスタンス</param>
        public override void OnPostprocessPrefab(IAkyuiLoader Loader, ref GameObject Prefab)
        {
            // クラスが設定されていない場合は何もしない
            if (string.IsNullOrEmpty(ClassName)) return;
            // 追加するコンポーネント名を作成する
            var Name = ClassName.Replace("{name}", Prefab.name);
            // アセンブリからクラスタイプを取得する
            var ComponentType = ScriptType.GetType(Name);
            if (ComponentType == null)
            {
                Log.Debug($"コンポーネント({Name})が見つかりません");
                return;
            }
            // コンポーネントを追加する
            var Script = Prefab.AddComponent(ComponentType);
            try
            {
                if (BindLayout.IsBindable(ComponentType))
                {
                    // BindableLayout属性があるか場合はバインドを実行する
                    BindLayout.Bind(Script as MonoBehaviour);
                }
                else if (Script is Layout Layout)
                {
                    // レイアウトの処理を行う
                    Layout.SetupLayout();
                }
                else
                {
                    Log.Warning($"{Name}は、レイアウト処理ができません");
                }
            } catch (Exception E)
            {
                Log.Error(E.Message + $"({Name})");
            }
        }
    }
    /// <summary>トリガーの補足説明表示用</summary>
    [CustomEditor(typeof(LayoutTrigger))]
    public class LayoutTriggerInspector : UnityEditor.Editor
    {
        /// <summary>ヘルプ</summary>
        private static string Tips = 
@"このトリガーは、スクリプトを追加するトリガーです
ClassNameは、追加したいスクリプト名(完全修飾名)を指定してください
{name}はアートボード名に置換されます 

アートボード名+Controllerのスクリプトを追加する例

ClassName:Game.UI.{name}Controller";
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.BeginVertical(GUI.skin.box);
            {
                EditorGUILayout.HelpBox(Tips, MessageType.Info);
            }
            EditorGUILayout.EndVertical();
        }
    }
    
}