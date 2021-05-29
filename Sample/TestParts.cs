using UnityEngine;
using UnityEngine.UI;
using XdLayout;
namespace Sample
{
    /// <summary>レイアウトのテスト</summary>
    public class TestParts : Layout
    {
        [SerializeField, LayoutPath("Item")] private TestItem Item;
        [SerializeField, LayoutPath("Select")] private Button Choice;
        /// <summary>特別なレイアウト処理を実装する場合の例</summary>
        public override void SetupLayout()
        {
            base.SetupLayout();
            // プレハブのItemのオブジェクト名を変更してみる
            Item.gameObject.name = "new name";
        }
    }
}