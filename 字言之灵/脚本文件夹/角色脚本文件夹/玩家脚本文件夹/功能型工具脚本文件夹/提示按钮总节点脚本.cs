using Godot;
using System;

public partial class 提示按钮总节点脚本 : Node2D
{
    private 玩家数据单例脚本 玩家数据单例脚本实例;

    private AnimationPlayer 提示按钮动画机;
    private 玩家场景数据节点脚本 玩家场景数据节点脚本实例;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        玩家数据单例脚本实例 = GetNode<玩家数据单例脚本>("/root/玩家数据单例脚本");

        提示按钮动画机 = GetNode<AnimationPlayer>("提示按钮动画机");
        玩家场景数据节点脚本实例 = GetNode<玩家场景数据节点脚本>("%玩家场景数据节点");
    }

    public void 启用提示的方法()
    {
        if (玩家数据单例脚本实例.玩家游戏数据.玩家拥有控制权())
        {
            Show();
            if(!提示按钮动画机.IsPlaying())
            {
                提示按钮动画机.Play("提示按钮动画");
            }
        }
    }

    public void 停用提示的方法()
    {
        Hide();
        if (提示按钮动画机.IsPlaying())
        {
            提示按钮动画机.Stop();
        }
    }
}