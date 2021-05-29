using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;
namespace XdLayout
{
    /// <summary>レイアウトをバインドするクラス</summary>
    public class BindLayout
    {
        private static Type BaseType = typeof(GameObject);
        /// <summary>レイアウトがバインドできるか取得する</summary>
        /// <param name="Type">データ型</param>
        /// <returns>true:バインド可能 false:バインド不可</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsBindable(Type Type) => Attribute.IsDefined(Type, typeof(BindableLayoutAttribute));
        /// <summary>レイアウトをフィールドにバインドする</summary>
        /// <remarks>
        /// 大まかな処理の流れ<br />
        ///
        /// 実装例<br />
        /// [SerializeField, LayoutPath("PathA/Item*")] Button[] ItemButtons;	// ワイルドカードのパスの例<br />
        /// [SerializeField, LayoutPath("PathB/Item)] Button ItemButton;		// 通常の単体パスの例<br />
        ///
        /// 1. インスタンスフィールドでLayoutPath属性が設定されているフィールドを検索<br />
        /// 2. LayoutPath属性のパスがワイルドカードでフィールド型が配列だった場合は3-5を繰り返す<br />
        /// 3. LayoutPath属性に設定されているオブジェクトを取得<br />
        /// 4. 取得オブジェクトのフィールド型のコンポーネントを取得<br />
        /// 5. コンポーネントが無い場合は、フィールドにコンポーネントを追加<br />
        /// 6. フィールドのインスタンスにコンポーネントを設定する<br />
        ///
        /// ※配列に設定されるインスタンは、オブジェクト名でソートされた順で保持される<br />
        /// </remarks>
        [Conditional("UNITY_EDITOR")]
        public static void Bind(MonoBehaviour This)
        {
            var Name = This.gameObject.name;
            // LayoutPath属性のフィールド一覧を取得する
            var FieldFilter = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            var Fields = This.GetType().GetFields(FieldFilter);
            foreach (var FieldInfo in Fields)
            {
                // LayoutPath属性のフィールドを探す
                var Layout = FieldInfo.GetCustomAttribute<LayoutPathAttribute>();
                if (Layout == null) continue;
                if (Layout.Path == null)
                {
                    // ルートオブジェクトの場合
                    SetValue(FieldInfo, This, This.gameObject);
                }
                else if (Layout.IsFilePattern())
                {
                    // 複数レイヤーの処理
                    if (!FieldInfo.FieldType.IsArray)
                    {
                        Log.Error($"ワイルドカード({Layout.Path})で指定されたフィールドは配列で宣言する必要があります");
                        continue;
                    }
                    // 配列の型(XXX[]のXXXの型)を取得
                    var ArrayType = FieldInfo.FieldType.GetElementType();
                    if (ArrayType == null)
                    {
                        Log.Error($"指定({Layout.Path})されたレイアウトの配列型の取得に失敗しました");
                        continue;
                    }
                    // パスを取得
                    var (ParentPath, NamePattern) = Layout.GetPathInfo();
                    // Name条件に合う子要素を取得
                    var Children = This.GetChildren(ParentPath, NamePattern);
                    // ==============================
                    // フィールド型の配列を作成する
                    // ==============================
                    var i = 0;
                    // 配列型がGameObjectかどうか
                    if (ArrayType == BaseType)
                    {
                        // 配列を作成(Items = new ArrayType[Components.Length])
                        var Items = Array.CreateInstance(ArrayType, Children.Length);
                        foreach (var Component in Children)
                        {
                            // 配列にコンポーネントを設定する(Items[i] = Component;)
                            Items.SetValue(Component, i++);
                        }
                        // クラスフィールドに配列を設定(This.Field = Items;)
                        FieldInfo.SetValue(This, Items);
                    }
                    else
                    {
                        // コンポーネントの一覧を取得
                        var Components = Children
                            .Select(Child =>
                            {
                                var Component = Child.SetComponent(ArrayType);
                                if (Component == null)
                                {
                                    Log.Error($"{Name}の{Layout.Path}のオブジェクトが見つかりません");
                                }
                                else
                                {
                                    // コンポーネントがレイアウトの場合は再帰的に処理する
                                    if (IsBindable(ArrayType) && Component is MonoBehaviour Behaviour)
                                    {
                                        // BindableLayoutなのでレイアウトを設定
                                        Bind(Behaviour);
                                    }
                                    else
                                    {
                                        // フィールドがサブレイアウトの場合はレイアウトを設定する
                                        if (Component is Layout SubLayout) SubLayout.SetupLayout();
                                    }

                                }
                                return Component;
                            })
                            .Where(Child => Child != null)
                            .ToArray();
                        // 配列を作成(Items = new ArrayType[Components.Length])
                        var Items = Array.CreateInstance(ArrayType, Components.Length);
                        foreach (var Component in Components)
                        {
                            // 配列にコンポーネントを設定する(Items[i] = Component;)
                            Items.SetValue(Component, i++);
                        }
                        // クラスフィールドに配列を設定(This.Field = Items;)
                        FieldInfo.SetValue(This, Items);
                    }
                }
                else
                {
                    // 単一レイヤーの処理
                    var Child = This.GetChild(Layout.Path);
                    if (Child == null)
                    {
                        Log.Error($"{Name}の{Layout.Path}のオブジェクトが見つかりません");
                        continue;
                    }
                    SetValue(FieldInfo, This, Child);
                }
            }
        }
        [Conditional("UNITY_EDITOR")]
        private static void SetValue(FieldInfo FieldInfo, MonoBehaviour This, GameObject Child)
        {
            if (FieldInfo.FieldType == BaseType)
            {
                // GameObjectの変数をフィールドに設定
                FieldInfo.SetValue(This, Child);
            }
            else
            {
                // コンポーネントの変数フィールドに設定
                var Component = Child.SetComponent(FieldInfo.FieldType);
                FieldInfo.SetValue(This, Component);
                // コンポーネントがレイアウトの場合は再帰的に処理する
                if (IsBindable(FieldInfo.FieldType) && Component is MonoBehaviour Behaviour)
                {
                    // BindableLayoutなのでレイアウトを設定
                    Bind(Behaviour);
                }
                else
                {
                    // フィールドがサブレイアウトの場合はレイアウトを設定する
                    if (Component is Layout SubLayout) SubLayout.SetupLayout();
                }
            }
        }
    }
}