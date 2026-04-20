using Godot;
using System;

public partial class 菜单栏画布层脚本 : CanvasLayer
{
	private 玩家数据单例脚本 玩家数据单例脚本实例;
	private 配置Json单例脚本 配置Json单例脚本实例;

    private Button 返回游戏按钮;
	private Button 保存游戏按钮;
	private Button 退出游戏按钮;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		玩家数据单例脚本实例 = GetNode<玩家数据单例脚本>("/root/玩家数据单例脚本");
		配置Json单例脚本实例 = GetNode<配置Json单例脚本>("/root/配置Json单例脚本");

        返回游戏按钮 = GetNode<Button>("%返回游戏按钮");
		返回游戏按钮.Pressed += Hide;
        保存游戏按钮 = GetNode<Button>("%保存游戏按钮");
		保存游戏按钮.Connect("pressed", new Callable(this, nameof(保存游戏按钮被按下的回调函数)));
		退出游戏按钮 = GetNode<Button>("%退出游戏按钮");
		退出游戏按钮.Connect("pressed", new Callable(this, nameof(退出游戏按钮被按下的回调函数)));
    }

	private void 保存游戏按钮被按下的回调函数()
	{
		玩家数据单例脚本实例.存档的方法();
    }

	private void 退出游戏按钮被按下的回调函数()
	{
        玩家数据单例脚本实例.存档的方法();

        CallDeferred(nameof(退出游戏的封装体方法));
    }

	private void 退出游戏的封装体方法()
	{
        GetTree().Quit();
    }
}
