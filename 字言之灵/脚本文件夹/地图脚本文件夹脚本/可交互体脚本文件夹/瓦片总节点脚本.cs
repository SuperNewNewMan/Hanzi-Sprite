using Godot;
using System;

public partial class 瓦片总节点脚本 : Node2D
{
    [Signal]
    public delegate void 触发可交互体的信号EventHandler(string 可交互体的名称);

    private 玩家数据单例脚本 玩家数据单例脚本实例;
    private 配置Json单例脚本 配置Json单例脚本实例;

    private 玩家总节点脚本 玩家总节点脚本实例;
    private bool 已生成玩家;

    private TileMapLayer 地基瓦片层;

    public override void _Ready()
    {
        玩家数据单例脚本实例 = GetNode<玩家数据单例脚本>("/root/玩家数据单例脚本");
        配置Json单例脚本实例 = GetNode<配置Json单例脚本>("/root/配置Json单例脚本");

        玩家总节点脚本实例 = GetTree().CurrentScene.GetNode<玩家总节点脚本>("玩家总节点");
        玩家总节点脚本实例.Connect(nameof(玩家总节点脚本实例.已生成玩家的信号), new Callable(this, nameof(获取本地玩家的回调函数)));

        地基瓦片层 = GetNode<TileMapLayer>("地基瓦片层");
    }

    public override void _Process(double delta)
    {
        if (已生成玩家 && 玩家总节点脚本实例.已生成的玩家字典.ContainsKey(Multiplayer.GetUniqueId()))
        {
            CharacterBody2D 本地玩家 = 玩家总节点脚本实例.已生成的玩家字典[Multiplayer.GetUniqueId()];
            提示按钮总节点脚本 提示按钮脚本实例 = 本地玩家.GetNode<提示按钮总节点脚本>("%提示按钮总节点");

            if (玩家数据单例脚本实例.玩家游戏数据.玩家拥有控制权())
            {
                var 触发坐标dict = 配置Json单例脚本实例.场景预制体字典[场景参数类脚本.场景名Enum.政务中心].可交互体的触发瓦片坐标字典;
                var 玩家所在的瓦片坐标 = 地基瓦片层.LocalToMap(本地玩家.Position);
                if (触发坐标dict.ContainsKey(玩家所在的瓦片坐标))
                {
                    提示按钮脚本实例.启用提示的方法();
                    if (Input.IsActionJustPressed("交互"))
                    {
                        EmitSignal(nameof(触发可交互体的信号), 触发坐标dict[玩家所在的瓦片坐标]);                        
                    }

                    return;
                }                
            }

            提示按钮脚本实例.停用提示的方法();
        }
    }

    private void 获取本地玩家的回调函数()
    {
        已生成玩家 = true;
    }
}