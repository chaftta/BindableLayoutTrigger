using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEditor;

namespace XdLayout.Editor
{
    /// <summary>タイプの取得を行う</summary>
    public static class ScriptType
    {
        /// <summary>タイプ名とタイプ型のマップ(検索用)</summary>
        private static Dictionary<string, Type> Types;

        /// <summary>クラス名からタイプを取得する</summary>
        public static Type GetType(string ClassName)
        {
            // ClassTypeの辞書を作成
            if (Types == null) CreateIndex();
            // クラスが存在
            if (Types != null && Types.ContainsKey(ClassName))
            {
                return Types[ClassName];
            }
            // クラスが存在しない場合
            return null;
        }
        /// <summary>Type辞書の作成</summary>
        private static void CreateIndex()
        {
            // クラスタイプの辞書を作成
            Types = Resources.FindObjectsOfTypeAll<MonoScript>()
                .Where(script => script != null)
                .Select(script => script.GetClass())
                .Where(classType => !string.IsNullOrEmpty(classType?.Name))
                .Distinct()
                .ToDictionary(T => T.FullName, T => T);
        }
    }
}