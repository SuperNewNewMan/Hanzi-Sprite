using Godot;
using System;
using System.Collections.Generic;

public partial class 临时的登录系统画布层脚本 : CanvasLayer
{
	private 玩家数据单例脚本 玩家数据单例脚本实例;

    private LineEdit 账号LE;
	private Button 登录按钮;

	private HashSet<string> 账号库 = new HashSet<string>();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		玩家数据单例脚本实例 = GetNode<玩家数据单例脚本>("/root/玩家数据单例脚本");

		for (int i = 1; i < 31; i++)
		{
			string id = i.ToString();
            string result = (id.Length < 2) ? $"0{id}" : id;
            账号库.Add(result);
        }

        账号LE = GetNode<LineEdit>("%账号LE");
		登录按钮 = GetNode<Button>("%登录按钮");
		登录按钮.Connect("pressed", new Callable(this, nameof(登录按钮被按下的回调函数)));
    }

	private void 登录按钮被按下的回调函数()
	{
		if (账号库.Contains(账号LE.Text))
		{
			玩家数据单例脚本实例.游戏行为数据.ID = 账号LE.Text;

            Hide();
		}
	}
}
