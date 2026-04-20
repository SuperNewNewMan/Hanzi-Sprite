using Godot;
using System;

public partial class 槽位脚本 : MarginContainer
{
    [Signal]
    public delegate void 已选中物品的信号EventHandler();
    [Signal]
    public delegate void 取消选中物品的信号EventHandler();

    public class 槽位内部类
    {
        public Button 物品按钮;
        private Label 物品数量标签;
        public Node2D 物品描述节点;
        private Label 物品描述标签;        

        public 槽位内部类(Button 物品按钮形参, Label 物品数量标签形参, Node2D 物品描述节点形参, Label 物品描述标签形参)
        {
            物品按钮 = 物品按钮形参;
            物品数量标签 = 物品数量标签形参;
            物品描述节点 = 物品描述节点形参;
            物品描述标签 = 物品描述标签形参;            
        }

        public void 重置槽位信息的方法()
        {
            物品按钮.Icon = null;
            物品按钮.ButtonPressed = false;
            物品数量标签.Text = string.Empty;
            物品描述标签.Text = string.Empty;            
        }

        public void 设置物品信息的方法(Texture2D 物品图片, int 物品数量, string 物品描述)
        {
            物品按钮.Icon = 物品图片;
            物品数量标签.Text = 物品数量.ToString();
            物品描述标签.Text = 物品描述;
        }

        public void 禁用槽位的方法(槽位脚本 槽位本身)
        {
            槽位本身.MouseFilter = MouseFilterEnum.Ignore;

            物品按钮.MouseFilter = MouseFilterEnum.Ignore;
            物品按钮.MouseDefaultCursorShape = CursorShape.Arrow;
        }
    }

    public 槽位内部类 槽位内部类实例;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        槽位内部类实例 = new 槽位内部类(GetNode<Button>("%物品按钮"), GetNode<Label>("物品数量标签"), GetNode<Node2D>("物品描述节点"), GetNode<Label>("%物品描述标签"));
        槽位内部类实例.物品按钮.ButtonDown += () => { EmitSignal(nameof(已选中物品的信号)); };
        槽位内部类实例.物品按钮.ButtonUp += () => { EmitSignal(nameof(取消选中物品的信号)); };

        Connect("mouse_entered", new Callable(this, nameof(聚焦的回调函数)));
        Connect("mouse_exited", new Callable(this, nameof(失焦的回调函数)));
    }

    private void 聚焦的回调函数()
    {
        槽位内部类实例.物品描述节点.Show();
    }

    private void 失焦的回调函数()
    {
        槽位内部类实例.物品描述节点.Hide();
    }
}