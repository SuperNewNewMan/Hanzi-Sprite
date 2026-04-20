using Godot;
using Godot.Collections;
using System;

public partial class 图鉴画布层脚本 : CanvasLayer
{
    public static PackedScene 槽位预制体 = GD.Load<PackedScene>("res://场景文件夹/角色场景文件夹/玩家场景文件夹/图鉴场景文件夹/字灵槽位.tscn");

	private 玩家数据单例脚本 玩家数据单例脚本实例;

    private class 内部数据类
	{
		public Array<MarginContainer> 槽位池 = new();
        public GridContainer 槽位GC;

        public Label 字义标签;

        public Button 关闭面板按钮;

    }
	private 内部数据类 数据 = new();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		玩家数据单例脚本实例 = GetNode<玩家数据单例脚本>("/root/玩家数据单例脚本");

        数据.槽位GC = GetNode<GridContainer>("%槽位GC");
        数据.字义标签 = GetNode<Label>("%字义标签");
        数据.关闭面板按钮 = GetNode<Button>("%关闭面板按钮");
		数据.关闭面板按钮.Connect("pressed", new Callable(this, nameof(关闭面板被按下的回调函数)));

        Connect("visibility_changed", new Callable(this, nameof(刷新当前槽位的封装体方法)));
    }

    private void 刷新当前槽位的封装体方法()
    {
        if (Visible)
        {
            int i = -1;

            var 智能字典 = 玩家数据单例脚本实例.玩家存档数据.字灵字典;
            var 槽位池 = 数据.槽位池;
            foreach (var (字灵, 数量) in 智能字典)
            {
                i++;

                MarginContainer 槽位;
                if (i >= 槽位池.Count)
                {
                    槽位 = 槽位预制体.Instantiate<MarginContainer>();
                    槽位池.Add(槽位);
                    数据.槽位GC.AddChild(槽位);                   
                }
                else
                {
                    槽位 = 槽位池[i];
                }

                槽位.Show();
                Button 字灵按钮 = 槽位.GetNode<Button>("字灵按钮");
                字灵按钮.Text = 字灵;
                Label 数量标签 = 字灵按钮.GetNode<Label>("数量标签");
                数量标签.Text = 数量.ToString();
            }

            while (i + 1 < 槽位池.Count)
            {
                i++;

                var 槽位 = 槽位池[i];
                槽位.Hide();
            }
        }
    }

    private void 关闭面板被按下的回调函数()
	{
		Hide();
	}
}
