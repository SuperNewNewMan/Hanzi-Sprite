using Godot;
using Godot.Collections;
using System;

public partial class 设施总节点脚本 : Node2D
{
    private 配置Json单例脚本 配置Json单例脚本实例;

    private 瓦片总节点脚本 瓦片总节点脚本实例;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        配置Json单例脚本实例 = GetNode<配置Json单例脚本>("/root/配置Json单例脚本");

        瓦片总节点脚本实例 = GetTree().CurrentScene.GetNode<瓦片总节点脚本>("瓦片总节点");
        瓦片总节点脚本实例.Connect(nameof(瓦片总节点脚本实例.触发可交互体的信号), new Callable(this, nameof(触发可交互体的回调函数)));
    }

    private void 触发可交互体的回调函数(string 可交互体的名称)
    {
        var data = 配置Json单例脚本实例.场景预制体字典[Enum.Parse<场景参数类脚本.场景名Enum>($"{GetTree().CurrentScene.Name}")];
        var 实例 = data.可交互体的触发方法字典[可交互体的名称];

        foreach (var (方法名, 字典) in 实例.入口事件字典)
        {
            可交互体封装类脚本.执行可交互体事件的方法(this, 方法名, 字典, 实例, null);
        }
    }
}
