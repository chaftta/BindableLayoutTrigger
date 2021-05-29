using System.Diagnostics;
using UnityEngine;
namespace XdLayout {
	/// <summary>レイアウトクラス</summary>
	public class Layout : MonoBehaviour {
		/// <summary>レイアウトを設定する</summary>
		/// <remarks>個別のレイアウト処理を行いたい場合は、派生で実装する</remarks>
		[Conditional("UNITY_EDITOR")]
		public virtual void SetupLayout() {
			BindLayout.Bind(this);
		}
	}
}