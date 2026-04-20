using Godot;
using Godot.Collections;
using static Godot.MultiplayerApi;
using System;

public partial class 联机权威后端数据单例脚本 : Node
{
    // 网络模块
    private 联机单例脚本 联机单例脚本实例;
    public const int 服务器网络会话ID = 1;

    // 泛用模块
    [Signal]
    public delegate void 某玩家所在场景发生改变的信号EventHandler(int peerID, int 场景枚举值);
    [Signal]
    public delegate void 有人离开房间的信号EventHandler(int peerID);
    public Dictionary<int, 场景参数类脚本.场景名Enum> 各玩家所在的场景字典
    {
        get;
        private set;
    } = new();

    // 战斗模块
    public int 战斗发起方ID = 1;
    public Dictionary<int, Array<int>> 连横方人数字典 = new()      // 前者为战斗场次ID（跟战斗发起方有关，如果为单机则为1），后者为玩家（电脑）ID列表
    { 
        { 1, new() }
    };
    public Dictionary<int, Array<int>> 合纵方人数字典 = new()
    { 
        { 1, new() }
    };
    public override void _Ready()
    {
        联机单例脚本实例 = GetNode<联机单例脚本>("/root/联机单例脚本");
        联机单例脚本实例.Connect(nameof(联机单例脚本实例.服务器断开连接的信号), new Callable(this, nameof(服务器断开连接的回调函数)));
        联机单例脚本实例.Connect(nameof(联机单例脚本实例.有人加入房间的信号), new Callable(this, nameof(有人加入房间的回调函数)));
        联机单例脚本实例.Connect(nameof(联机单例脚本实例.有人离开房间的信号), new Callable(this, nameof(有人离开房间的回调函数)));

        GetTree().Connect("scene_changed", new Callable(this, nameof(本人所在场景发生改变的回调函数)));
    }

    private void 服务器断开连接的回调函数()
    {
        各玩家所在的场景字典.Clear();
    }

    private void 有人加入房间的回调函数()
    {
        // 向房主发送自己的当前peer和场景
        RpcId(服务器网络会话ID, nameof(Rpc_房主靶向_更新各玩家所在的场景字典封装体方法), Multiplayer.GetUniqueId(), (int)Enum.Parse<场景参数类脚本.场景名Enum>(GetTree().CurrentScene.Name));
    }

    [Rpc(RpcMode.AnyPeer, CallLocal = true)]
    private void Rpc_房主靶向_更新各玩家所在的场景字典封装体方法(int peerID, int 场景枚举值)
    {
        各玩家所在的场景字典[peerID] = (场景参数类脚本.场景名Enum)场景枚举值;
        if(各玩家所在的场景字典.Count >= Multiplayer.GetPeers().Length)
        {
            // 房主已更新完毕字典，发送数据给所有人
            Rpc(nameof(Rpc_面向全体_广播更新各玩家所在的场景字典封装体方法), 各玩家所在的场景字典);
        }
    }

    [Rpc]
    private void Rpc_面向全体_广播更新各玩家所在的场景字典封装体方法(Dictionary<int, 场景参数类脚本.场景名Enum> 各玩家所在的场景字典形参)
    {
        各玩家所在的场景字典 = 各玩家所在的场景字典形参;
    }


    private void 有人离开房间的回调函数(int peerID)
    {
        // 通知所有人，有人离开了房间
        Rpc(nameof(Rpc_面向全体_广播有人离开房间的封装体方法), peerID);
    }

    [Rpc(RpcMode.AnyPeer, CallLocal = true)]
    private void Rpc_面向全体_广播有人离开房间的封装体方法(int peerID)
    {
        各玩家所在的场景字典.Remove(peerID);
        EmitSignal(nameof(有人离开房间的信号), peerID);
    }

    private void 本人所在场景发生改变的回调函数()
    {
        // 通知房主，本人的所在场景发生了改变
        RpcId(服务器网络会话ID, nameof(Rpc_房主靶向_通知本人所在场景发生改变的封装体方法), Multiplayer.GetUniqueId(), (int)Enum.Parse<场景参数类脚本.场景名Enum>(GetTree().CurrentScene.Name));
    }

    [Rpc(RpcMode.AnyPeer, CallLocal = true)]
    private void Rpc_房主靶向_通知本人所在场景发生改变的封装体方法(int peerID, int 场景枚举值)
    {
        if (Multiplayer.IsServer())
        {
            Rpc(nameof(Rpc_面向全体_广播某玩家场景发生改变的封装体方法), peerID, 场景枚举值);
        }
    }

    [Rpc(CallLocal = true)]
    private void Rpc_面向全体_广播某玩家场景发生改变的封装体方法(int peerID, int 场景枚举值)
    {
        各玩家所在的场景字典[peerID] = (场景参数类脚本.场景名Enum)场景枚举值;

        EmitSignal(nameof(某玩家所在场景发生改变的信号), peerID, 场景枚举值);
    }
}