using Godot;
using Godot.Collections;
using System;

public partial class 室内光调节器脚本 : DirectionalLight2D
{
    private Dictionary 本地时间字典 = new Dictionary();
    private Timer 记录本地时间计时器;
    private const float 等待秒数 = 600;

    private const float 默认光强 = 0;
    private const float 灯照光强 = 1;
    private const int 开灯时间 = 18;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        记录本地时间计时器 = GetNode<Timer>("记录本地时间计时器");
        记录本地时间计时器.WaitTime = 等待秒数;
        记录本地时间计时器.Connect("timeout", new Callable(this, nameof(记录本地时间的方法)));

        // 初始化本地时间和灯光强度
        记录本地时间的方法();
    }

    // 每10分钟（即600秒）调用一次
    public void 记录本地时间的方法()
    {
        本地时间字典 = Time.GetTimeDictFromSystem();
        //int 小时 = (int)本地时间字典["hour"];
        int 小时 = 18;

        if (小时 >= 开灯时间)
        {
            Energy = 0.6f;
        }
        else
        {
            Energy = 默认光强;
        }
    }
}