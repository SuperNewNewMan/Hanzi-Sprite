using Godot;
using Godot.Collections;
using System;
using static Godot.MultiplayerApi;

public partial class 玩家总节点脚本 : Node2D
{
    [Signal]
    public delegate void 已生成玩家的信号EventHandler();

    private PackedScene 玩家场景预制体 = GD.Load<PackedScene>($"res://场景文件夹/角色场景文件夹/玩家场景文件夹/玩家.tscn");
    public Dictionary<int, CharacterBody2D> 已生成的玩家字典
    {
        get;
        private set;
    } = new();

    private 联机权威后端数据单例脚本 联机权威后端数据单例脚本实例;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        联机权威后端数据单例脚本实例 = GetNode<联机权威后端数据单例脚本>("/root/联机权威后端数据单例脚本");
        联机权威后端数据单例脚本实例.Connect(nameof(联机权威后端数据单例脚本实例.某玩家所在场景发生改变的信号), new Callable(this, nameof(生成玩家的回调函数)));
        联机权威后端数据单例脚本实例.Connect(nameof(联机权威后端数据单例脚本实例.有人离开房间的信号), new Callable(this, nameof(有人离开房间的回调函数)));
    }

    private void 生成玩家的回调函数(int peerID, int 场景枚举值)
    {
        foreach (var (key, value) in 联机权威后端数据单例脚本实例.各玩家所在的场景字典)
        {
            // 通知所有在当前场景的玩家，需要生成玩家
            if (value == (场景参数类脚本.场景名Enum)场景枚举值)
            {
                RpcId(key, nameof(Rpc_生成玩家的封装体方法), 场景枚举值);
            }
        }
    }

    [Rpc(RpcMode.AnyPeer, CallLocal = true)]
    private void Rpc_生成玩家的封装体方法(int 场景枚举值)
    {
        // 生成所有在当前场景，但还没有生成的玩家
        foreach (var (key, value) in 联机权威后端数据单例脚本实例.各玩家所在的场景字典)
        {
            if (!已生成的玩家字典.ContainsKey(key) && value == (场景参数类脚本.场景名Enum)场景枚举值)
            {
                CharacterBody2D 未生成的玩家 = 玩家场景预制体.Instantiate() as CharacterBody2D;
                未生成的玩家.Name = key.ToString();
                玩家场景数据节点脚本 漏掉的场景数据脚本实例 = 未生成的玩家.GetNode<玩家场景数据节点脚本>("%玩家场景数据节点");
                漏掉的场景数据脚本实例.本地网络ID = key;

                已生成的玩家字典[key] = 未生成的玩家;
                AddChild(未生成的玩家);
            }
        }

        EmitSignal(nameof(已生成玩家的信号));
    }

    private void 有人离开房间的回调函数(int peerID)
    {
        if (已生成的玩家字典.ContainsKey(peerID))
        {
            CharacterBody2D 被移除的玩家 = 已生成的玩家字典[peerID];
            RemoveChild(被移除的玩家);
            被移除的玩家.QueueFree();
            已生成的玩家字典.Remove(peerID);
        }
    }

    public override void _Process(double delta)
    {
        // 向相同场景下的玩家同步本地的位置
        foreach (var (key, value) in 已生成的玩家字典)
        {
            // 忽略本地
            if (key != Multiplayer.GetUniqueId())
            {
                RpcId(key, nameof(Rpc_同步某玩家的位置), Multiplayer.GetUniqueId(), 已生成的玩家字典[Multiplayer.GetUniqueId()].Position);

                AnimationTree 动画树 = 已生成的玩家字典[Multiplayer.GetUniqueId()].GetNode<AnimationTree>("玩家动画树");
                float 移动系统混合值 = (float)动画树.Get("parameters/移动系统B2/blend_amount");
                Vector2 朝向 = (Vector2)动画树.Get("parameters/移动BS2D/blend_position");
                RpcId(key, nameof(Rpc_同步某玩家的移动动画), Multiplayer.GetUniqueId(), 移动系统混合值, 朝向);
            }
        }
    }

    [Rpc(RpcMode.AnyPeer)]
    private void Rpc_同步某玩家的位置(int peerID, Vector2 位置)
    {
        已生成的玩家字典[peerID].Position = 位置;
    }

    [Rpc(RpcMode.AnyPeer)]
    private void Rpc_同步某玩家的移动动画(int peerID, float 移动系统混合值, Vector2 朝向)
    {
        AnimationTree 动画树 = 已生成的玩家字典[peerID].GetNode<AnimationTree>("玩家动画树");
        动画树.Set("parameters/移动系统B2/blend_amount", 移动系统混合值);
        动画树.Set("parameters/待机BS2D/blend_position", 朝向);
        动画树.Set("parameters/移动BS2D/blend_position", 朝向);
    }
}