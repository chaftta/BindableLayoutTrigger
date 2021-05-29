#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

namespace XdLayout
{
    /// <summary>UIの拡張メソッド</summary>
    public static class Extensions
    {
        /// <summary>コンポーネントを設定する</summary>
        /// <param name="This">設定先のオブジェクト</param>
        /// <param name="Type">設定するコンポーネント</param>
        /// <returns>設定されているコンポーネント</returns>
        public static Component SetComponent(this GameObject This, Type Type)
        {
            var Component = This.GetComponent(Type); // Typeのコンポーネントを取得
            if (Component != null) return Component; // 既にあるので中断
            return This.AddComponent(Type); // 対象にコンポーネントを追加
        }
        /// <summary>コンポーネントを設定する</summary>
        /// <param name="This">設定先のオブジェクト</param>
        /// <returns>設定されているコンポーネント</returns>
        public static Component SetComponent<T>(this GameObject This)
        {
            return This.SetComponent(typeof(T));
        }
        /// <summary>階層子要素を取得する</summary>
        /// <param name="This">親要素</param>
        /// <param name="Path">子要素のパス</param>
        /// <code>var Child = This.GetChild("Child/SubChild");</code>
        /// <returns>子要素</returns>
        public static GameObject GetChild(this GameObject This, string Path)
        {
            return This.transform.Find(Path)?.gameObject;
        }
        /// <inheritdoc cref="GetChild(UnityEngine.GameObject,string)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static GameObject GetChild(this MonoBehaviour This, string Path)
        {
            return This.gameObject.GetChild(Path);
        }
        /// <inheritdoc cref="GetChild(UnityEngine.GameObject,string)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static GameObject GetChild(this Component This, string Path)
        {
            return This.gameObject.GetChild(Path);
        }
        /// <summary>階層子要素のコンポーネントを取得する</summary>
        /// <param name="This">親要素</param>
        /// <param name="Path">子要素のパス</param>
        /// <code>var Child = This.GetChild("Child/SubChild");</code>
        /// <returns>子要素のコンポーネント</returns>
        public static T GetChild<T>(this GameObject This, string Path) where T : class
        {
            var Target = This.GetChild(Path);
            if (Target == null) return null;
            return Target.GetComponent<T>();
        }
        /// <inheritdoc cref="GetChild(UnityEngine.GameObject,string)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T GetChild<T>(this MonoBehaviour This, string Path) where T : class
        {
            return This.gameObject.GetChild<T>(Path);
        }
        /// <summary>子要素の一覧を取得する</summary>
        /// <param name="This">親要素</param>
        /// <param name="Path">取得する要素</param>
        /// <param name="PatternName">要素名(ワイルドカード付き)</param>
        /// <returns></returns>
        public static GameObject[] GetChildren(this GameObject This, string Path = null, string PatternName = null)
        {
            if (string.IsNullOrEmpty(Path) || string.IsNullOrEmpty(PatternName))
            {
                // 全子要素を返す
                return This.transform.Cast<Transform>().Select(T => T.gameObject).ToArray();
            }
            var Regex = new Regex(PatternName.Replace("*", ".*")); // 検索用正規表現(*を正規表現に変換)
            var Current = This.gameObject.GetChild(Path); // パスの要素を取得
            var Children = new List<GameObject>();
            foreach (Transform Child in Current.transform)
            {
                // 正規表現に一致するファイルを一覧に追加する
                if (Regex.IsMatch(Child.name)) Children.Add(Child.gameObject);
            }
            return Children.ToArray();
        }
        /// <inheritdoc cref="GetChildren(UnityEngine.GameObject,string,string)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static GameObject[] GetChildren(this MonoBehaviour This, string Path = null, string PatternName = null)
        {
            return This.gameObject.GetChildren(Path, PatternName);
        }
        /// <inheritdoc cref="GetChildren(UnityEngine.GameObject,string,string)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static GameObject[] GetChildren(this Component This, string Path = null, string PatternName = null)
        {
            return This.gameObject.GetChildren(Path, PatternName);
        }
        /// <summary>GameObjectに親を設定する</summary>
        /// <param name="This">自分</param>
        /// <param name="Parent">親のオブジェクト</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetParent(this GameObject This, GameObject Parent)
        {
            This.transform.SetParent((Parent != null) ? Parent.transform : null);
        }
        /// <summary>GameObjectに親を設定する</summary>
        /// <param name="This">自分</param>
        /// <param name="Parent">親のオブジェクト</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetParent(this GameObject This, Transform Parent)
        {
            This.transform.SetParent(Parent);
        }
        /// <summary>GameObjectに親を設定する</summary>
        /// <param name="This">自分</param>
        /// <param name="Parent">親のオブジェクト</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetParent(this MonoBehaviour This, GameObject Parent)
        {
            This.transform.SetParent((Parent != null) ? Parent.transform : null);
        }
        /// <summary>GameObjectに親を設定する</summary>
        /// <param name="This">自分</param>
        /// <param name="Parent">親のオブジェクト</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetParent(this Component This, GameObject Parent)
        {
            This.transform.SetParent((Parent != null) ? Parent.transform : null);
        }
        /// <summary>GameObjectに親を設定する</summary>
        /// <param name="This">自分</param>
        /// <param name="Parent">親のオブジェクト</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetParent(this MonoBehaviour This, Transform Parent)
        {
            This.transform.SetParent(Parent);
        }
    }
}
#endif
