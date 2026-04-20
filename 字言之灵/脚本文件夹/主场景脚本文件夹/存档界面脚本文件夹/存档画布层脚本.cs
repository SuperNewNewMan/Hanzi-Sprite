using Godot;
using System;

public partial class 存档画布层脚本 : CanvasLayer
{
	private Button 关闭存档界面按钮;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		关闭存档界面按钮 = GetNode<Button>("%关闭存档界面按钮");
		关闭存档界面按钮.Pressed += Hide;
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
