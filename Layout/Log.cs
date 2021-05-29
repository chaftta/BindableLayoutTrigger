using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;
/// <summary>ログクラス(DEBUG時以外は出力されない)</summary>
public class Log {
	/// <summary>デバッグログを出力する</summary>
	/// <param name="Message">ログメッセージ</param>
	[Conditional("DEBUG"), MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void Debug(string Message) {
		LogMessage(LogType.Log, Message);
	}
	/// <summary>ワーニングログを出力する</summary>
	/// <param name="Message">ログメッセージ</param>
	[Conditional("DEBUG"), MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void Warning(string Message) {
		LogMessage(LogType.Warning, Message);
	}
	/// <summary>エラーログを出力する</summary>
	/// <param name="Message">ログメッセージ</param>
	[Conditional("DEBUG"), MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void Error(string Message) {
		LogMessage(LogType.Error, Message);
	}
	/// <summary>ログを出力する</summary>
	/// <param name="Type">ログタイプ</param>
	/// <param name="Message">ログメッセージ</param>
	[Conditional("DEBUG"), MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void LogMessage(LogType Type, string Message) => UnityEngine.Debug.unityLogger.Log(Type, Message);
}
