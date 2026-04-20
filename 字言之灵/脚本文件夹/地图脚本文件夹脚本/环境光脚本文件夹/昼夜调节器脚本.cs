using Godot;
using Godot.Collections;
using System;

public partial class 昼夜调节器脚本 : CanvasModulate
{
    private 配置Json单例脚本 配置Json单例脚本实例;

    private Timer 记录本地时间计时器;
    private const float 等待秒数 = 600;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        配置Json单例脚本实例 = GetNode<配置Json单例脚本>("/root/配置Json单例脚本");
        场景参数类脚本.场景区块类 场景预制体类实例 = null;
        if (Enum.TryParse<场景参数类脚本.场景名Enum>(GetTree().CurrentScene.Name, out var 场景名))
        {
            场景预制体类实例 = 配置Json单例脚本实例.场景预制体字典[场景名];
        }

        记录本地时间计时器 = GetNode<Timer>("记录本地时间计时器");
        记录本地时间计时器.WaitTime = 等待秒数;
        记录本地时间计时器.Connect("timeout", new Callable(this, nameof(场景预制体类实例.昼夜调节的方法)));

        // 初始化本地时间和颜色
        //场景预制体类实例.昼夜调节的方法(this);
    }
}