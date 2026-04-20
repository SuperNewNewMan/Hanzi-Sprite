using Godot;
using System;

public partial class NPC总节点脚本 : Node2D
{
	private 配置Json单例脚本 配置Json单例脚本实例;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        配置Json单例脚本实例 = GetNode<配置Json单例脚本>("/root/配置Json单例脚本");

        var 快照 = 配置Json单例脚本实例.场景预制体字典[Enum.Parse<场景参数类脚本.场景名Enum>($"{GetTree().CurrentScene.Name}")].NPC预制体字典;
        foreach(var (key, value) in 快照)
        {
            Node2D npc = value.场景预制体.Instantiate() as Node2D;
            npc.Position = value.位置;
            AddChild(npc);
        }
    }
}
