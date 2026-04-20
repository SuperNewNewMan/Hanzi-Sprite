using Godot;
using System;

public partial class 开始界面背景脚本 : Node2D
{
	[Export]
	public float 视差强度 = 20f;
	
	private Sprite2D 大幅度动态部分;
	private Sprite2D 小幅度动态部分;

	private Rect2 视窗;
	private Vector2 屏幕中心;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		大幅度动态部分 = GetNode<Sprite2D>("大幅度动态部分");
		小幅度动态部分 = GetNode<Sprite2D>("小幅度动态部分");

		视窗 = GetViewportRect();
		屏幕中心 = 视窗.Size / 2f;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Vector2 鼠标位置 = GetViewport().GetMousePosition();

		// 鼠标相对中心的偏移（-1 ~ 1）
		Vector2 偏移量 = (鼠标位置 - 屏幕中心) / 屏幕中心;       

		// 视差效果
		if(视窗.HasPoint(鼠标位置))
		{
			大幅度动态部分.Position = 偏移量 * (视差强度 * 0.6f);
			小幅度动态部分.Position = 偏移量 * (视差强度 * 0.3f);
		}       
	}
}
