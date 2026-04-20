using Godot;
using Godot.Collections;
using System;

public partial class 开始界面_按钮总容器脚本 : MarginContainer
{
    [Signal]
    public delegate void 切换到创建角色界面场景的信号EventHandler(string 场景名);

    private VBoxContainer 按钮VBox;
    private Array<Button> 按钮数组;

    private CanvasLayer 创建角色画布层;
    private CanvasLayer 设置画布层;
    private CanvasLayer 联机画布层;
    private CanvasLayer 存档画布层;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        按钮VBox = GetNode<VBoxContainer>("VBoxContainer");
        foreach (var child in 按钮VBox.GetChildren())
        {
            if (child is Button button)
            {
                if (button.Name == "开始按钮")
                {
                    // 开始游戏
                    button.Connect("pressed", new Callable(this, nameof(显示创建角色界面的回调函数)));
                }
                else if (button.Name == "读档按钮")
                {
                    // 读档
                    button.Connect("pressed", new Callable(this, nameof(显示存档界面的回调函数)));
                }
                else if (button.Name == "联机按钮")
                {
                    // 联机
                    button.Connect("pressed", new Callable(this, nameof(显示联机面板的回调函数)));
                }
                else if (button.Name == "设置按钮")
                {
                    // 显示设置面板
                    button.Connect("pressed", new Callable(this, nameof(显示设置面板的回调函数)));
                }
                else if (button.Name == "退出按钮")
                {
                    // 退出游戏
                    button.Connect("pressed", new Callable(this, nameof(退出游戏的回调函数)));
                }
            }
        }

        创建角色画布层 = GetTree().CurrentScene.GetNode<CanvasLayer>("创建角色画布层");
        设置画布层 = GetTree().CurrentScene.GetNode<CanvasLayer>("设置画布层");
        联机画布层 = GetTree().CurrentScene.GetNode<CanvasLayer>("联机画布层");
        存档画布层 = GetTree().CurrentScene.GetNode<CanvasLayer>("存档画布层");
    }

    private void 显示创建角色界面的回调函数()
    {
        创建角色画布层.Show();
    }

    private void 显示设置面板的回调函数()
    {
        设置画布层.Show();
    }

    private void 显示联机面板的回调函数()
    {
        联机画布层.Show();
    }

    private void 显示存档界面的回调函数()
    {
        存档画布层.Show();
    }

    private void 退出游戏的回调函数()
    {
        GetTree().Quit();
    }
}