using Godot;
using System;
using System.Linq;
using System.Threading.Tasks;

public partial class 联机单例脚本 : Node
{
    // 信号
    [Signal]
    public delegate void 联机失败的信号EventHandler(string 标签名, string 错误信息);
    [Signal]
    public delegate void 联机成功的信号EventHandler();
    [Signal]
    public delegate void 服务器断开连接的信号EventHandler(string 场景名);
    [Signal]
    public delegate void 有人加入房间的信号EventHandler();
    [Signal]
    public delegate void 有人离开房间的信号EventHandler(int peerID);

    // 服务器额外数据
    private const int 等待连接秒数 = 2;

    private 配置Json单例脚本 配置Json单例脚本实例;

    // Called when the node enters the scene tree for the first time.
    public override async void _Ready()
    {
        配置Json单例脚本实例 = GetNode<配置Json单例脚本>("/root/配置Json单例脚本");

        GetTree().Connect("scene_changed", new Callable(this, nameof(获取联机界面脚本的方法)));

        await 获取联机界面脚本的方法();
    }

    private async Task 获取联机界面脚本的方法()
    {
        await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);

        联机画布层脚本 联机界面脚本实例 = GetTree().CurrentScene.GetNodeOrNull<联机画布层脚本>("联机画布层");
        联机界面脚本实例?.Connect(nameof(联机界面脚本实例.创建房间的信号), new Callable(this, nameof(创建房间的回调函数)));
        联机界面脚本实例?.Connect(nameof(联机界面脚本实例.加入房间的信号), new Callable(this, nameof(加入房间的回调函数)));
    }

    // 服务端模块
    private void 创建房间的回调函数(int 最大玩家数, int 房间端口)
    {
        ENetMultiplayerPeer peer = new();
        // port为服务器端口号，maxClients为最大连接数       
        try
        {
            Error err = peer.CreateServer(port: 房间端口, maxClients: 最大玩家数);
            if (err == Error.Ok)
            {
                Multiplayer.MultiplayerPeer = peer;

                // 信号绑定必须在设置 MultiplayerPeer 之后
                Multiplayer.Connect("peer_connected", new Callable(this, nameof(有人加入房间的回调函数)));
                Multiplayer.Connect("peer_disconnected", new Callable(this, nameof(有人离开房间的回调函数)));
                EmitSignal(nameof(联机成功的信号));
            }
            else
            {
                EmitSignal(nameof(联机失败的信号), "创建房间界面标题标签", "创建房间失败！");
                重置网络数据的方法();
            }
        }
        catch (Exception e)
        {
            GD.PrintErr(GetType().Name + e);
        }
    }

    // 有玩家加入的逻辑
    private void 有人加入房间的回调函数(long peerId)
    {
        GD.Print($"{GetType().Name} 有人加入房间！Peer为：{peerId}");
        EmitSignal(nameof(有人加入房间的信号));
    }

    // 有玩家离开的逻辑
    private void 有人离开房间的回调函数(long peerId)
    {
        GD.Print($"{GetType().Name} 有人离开房间！");
        EmitSignal(nameof(有人离开房间的信号), peerId);
    }


    // 客户端模块
    private async void 加入房间的回调函数(string 房间IP地址, int 房间端口)
    {
        ENetMultiplayerPeer peer = new();
        // address为服务器IP地址，port为服务器端口号
        try
        {
            string ip = (房间IP地址 == string.Empty) ? 获取IPv4地址的封装体方法() : 房间IP地址;
            Error err = peer.CreateClient(ip, 房间端口);
            if (err == Error.Ok)
            {
                Multiplayer.MultiplayerPeer = peer;

                // 事件绑定必须在设置 MultiplayerPeer 之后
                Multiplayer.Connect("server_disconnected", new Callable(this, nameof(服务器断开连接的回调函数)));         // ServerDisconnected的触发时机是，当客人成功与房主连接后，房主退出游戏、关闭网络……导致服务器消失时触发的。

                GD.Print($"{GetType().Name} 已发起加入房间请求，等待服务器响应...");

                await 检测是否连接到服务器超时的回调函数();
            }
        }
        catch (Exception e)
        {
            GD.PrintErr(GetType().Name + e);
        }
    }

    private string 获取IPv4地址的封装体方法()
    {
        foreach (var ip in IP.GetLocalAddresses())
        {
            if (ip.StartsWith("192.168.") || ip.StartsWith("10.") || ip.StartsWith("172."))
            {
                return ip;
            }
        }

        return "127.0.0.1"; // 兜底
    }

    // 服务器断开连接后，重置服务器，并返回开始页面
    private void 服务器断开连接的回调函数()
    {
        重置网络数据的方法();

        EmitSignal(nameof(服务器断开连接的信号));
        GetTree().ChangeSceneToPacked(配置Json单例脚本实例.场景预制体字典[场景参数类脚本.场景名Enum.开始界面].场景预制体);
        GD.Print(GetType().Name + " 服务器断开连接！");
    }

    private async Task 检测是否连接到服务器超时的回调函数()
    {
        // 等待
        GD.Print(GetType().Name + $" 等待 {等待连接秒数} 秒...");
        await Task.Delay(等待连接秒数 * 1000);

        // 检查当前是否还保持连接
        if (Multiplayer.MultiplayerPeer != null && Multiplayer.MultiplayerPeer.GetConnectionStatus() == MultiplayerPeer.ConnectionStatus.Connected)
        {
            EmitSignal(nameof(联机成功的信号));
            GD.Print(GetType().Name + " 已连接到房主！");
        }
        else
        {
            重置网络数据的方法();
            EmitSignal(nameof(联机失败的信号), "加入房间界面标题标签", "加入房间失败！");
            GD.PrintErr(GetType().Name + " 连接超时！");
        }
    }

    public void 重置网络数据的方法()
    {
        if (Multiplayer.MultiplayerPeer != null)
        {
            // 重置Multiplayer
            Multiplayer.MultiplayerPeer.Close();
            Multiplayer.MultiplayerPeer = new OfflineMultiplayerPeer();

            if (Multiplayer.IsConnected("peer_connected", new Callable(this, nameof(有人加入房间的回调函数))))
            {
                Multiplayer.Disconnect("peer_connected", new Callable(this, nameof(有人加入房间的回调函数)));
            }
            if (Multiplayer.IsConnected("peer_disconnected", new Callable(this, nameof(有人离开房间的回调函数))))
            {
                Multiplayer.Disconnect("peer_disconnected", new Callable(this, nameof(有人离开房间的回调函数)));
            }
            if (Multiplayer.IsConnected("server_disconnected", new Callable(this, nameof(服务器断开连接的回调函数))))
            {
                Multiplayer.Disconnect("server_disconnected", new Callable(this, nameof(服务器断开连接的回调函数)));
            }

            GD.Print($"{GetType().Name} 重置网络数据成功！");
        }
    }
}