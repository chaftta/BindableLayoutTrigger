using System;
using System.Diagnostics;
using System.Linq;
namespace XdLayout
{
    /// <summary>バインド可能なレイアウト属性</summary>
    /// <remarks>Layoutを継承したくない場合、クラスにこの属性を設定することでバインドが実行される</remarks>
    [Conditional("UNITY_EDITOR"), AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
    public class BindableLayoutAttribute : Attribute {}
}