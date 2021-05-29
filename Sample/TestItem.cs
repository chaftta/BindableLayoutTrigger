using UnityEngine;
using UnityEngine.UI;
using XdLayout;
namespace Sample
{
    /// <summary>サブレイアウト(属性のテスト)</summary>
    [BindableLayout]
    public class TestItem : MonoBehaviour
    {
        [SerializeField, LayoutPath("Name")] private Text Name;
        [SerializeField, LayoutPath("Image")] private Image Image;
        [SerializeField, LayoutPath("Rare/Icon*")] private Image[] RateIcons;
    }
}