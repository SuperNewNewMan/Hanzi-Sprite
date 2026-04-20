using Godot;
using System;

public partial class 身份证画布层 : CanvasLayer
{
	private partial class 内部数据类
	{
		public Label 姓名标签;
		public Label 性别标签;
		public Label 种族标签;
		public Label 年标签;
		public Label 月标签;
		public Label 日标签;
		public Label 住址标签;
		public Label 字灵证标签;
		public Label 学术头衔标签;
		public Label 身份证号标签;

		public void 设置身份证信息的方法(string 姓名, string 性别, string 种族, DateOnly 出生, string 住址, string 字灵证, string 学术头衔, string 身份证号)
		{
			姓名标签.Text = 姓名;
			性别标签.Text = 性别;
			种族标签.Text = 种族;
			年标签.Text = 出生.Year.ToString();
			月标签.Text = 出生.Month.ToString();
			日标签.Text = 出生.Day.ToString();
			住址标签.Text = 住址;
			字灵证标签.Text = 字灵证;
			学术头衔标签.Text = 学术头衔;
			身份证号标签.Text = 身份证号;
		}
	}
	private 内部数据类 数据类实例 = new();

	private 玩家数据单例脚本 玩家数据单例脚本实例;

	private Button 身份证关闭按钮;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		数据类实例.姓名标签 = GetNode<Label>("%姓名标签");
		数据类实例.性别标签 = GetNode<Label>("%性别标签");
		数据类实例.种族标签 = GetNode<Label>("%种族标签");
		数据类实例.年标签 = GetNode<Label>("%年标签");
		数据类实例.月标签 = GetNode<Label>("%月标签");
		数据类实例.日标签 = GetNode<Label>("%日标签");
		数据类实例.住址标签 = GetNode<Label>("%住址标签");
		数据类实例.字灵证标签 = GetNode<Label>("%字灵证标签");
		数据类实例.学术头衔标签 = GetNode<Label>("%学术头衔标签");
		数据类实例.身份证号标签 = GetNode<Label>("%身份证号标签");

		玩家数据单例脚本实例 = GetNode<玩家数据单例脚本>("/root/玩家数据单例脚本");

		身份证关闭按钮 = GetNode<Button>("%身份证关闭按钮");
		身份证关闭按钮.Pressed += Hide;

		Connect("visibility_changed", new Callable(this, nameof(刷新身份证信息的回调函数)));
	}

	private void 刷新身份证信息的回调函数()
	{
		if (Visible)
		{
			数据类实例.设置身份证信息的方法(玩家数据单例脚本实例.玩家存档数据.姓名,
				玩家数据单例脚本实例.玩家存档数据.性别, 玩家数据单例脚本实例.玩家存档数据.种族,
				玩家数据单例脚本实例.玩家存档数据.出生,
				玩家数据单例脚本实例.玩家存档数据.住址,
				玩家数据单例脚本实例.玩家存档数据.字灵证,
				玩家数据单例脚本实例.玩家存档数据.学术头衔,
				玩家数据单例脚本实例.玩家存档数据.身份号码);
		}
	}
}