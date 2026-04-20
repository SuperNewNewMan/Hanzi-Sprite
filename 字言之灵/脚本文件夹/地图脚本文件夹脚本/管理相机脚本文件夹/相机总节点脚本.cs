using Godot;
using System;

public partial class 相机总节点脚本 : Node2D
{
    public enum 相机状态Enum
    {
        跟随玩家,
        手动控制,
    }
    public class 内部数据类
    {
        private 相机状态Enum 相机状态;
        public const float 手动控制移速 = 400;

        public 相机状态Enum 读取相机状态的方法()
        {
            return 相机状态;
        }

        public void 更新相机状态的方法(相机状态Enum 相机状态形参)
        {
            相机状态 = 相机状态形参;
        }
    }
    public 内部数据类 内部数据 = new 内部数据类();

    // 单例模块
    private 玩家数据单例脚本 玩家数据单例脚本实例;

    // 场景对象模块
    private 玩家总节点脚本 玩家总节点脚本实例;
    private Camera2D 地图场景相机;
    private CharacterBody2D 本地玩家;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        玩家数据单例脚本实例 = GetNode<玩家数据单例脚本>("/root/玩家数据单例脚本");

        玩家总节点脚本实例 = GetTree().CurrentScene.GetNode<玩家总节点脚本>("玩家总节点");
        玩家总节点脚本实例.Connect(nameof(玩家总节点脚本实例.已生成玩家的信号), new Callable(this, nameof(寻找本地玩家的回调函数)));
        地图场景相机 = GetNode<Camera2D>("地图场景相机");
    }

    private void 寻找本地玩家的回调函数()
    {
        if (本地玩家 == null)
        {
            foreach (var child in 玩家总节点脚本实例.GetChildren())
            {
                玩家场景数据节点脚本 场景数据脚本实例 = child.GetNode<玩家场景数据节点脚本>("%玩家场景数据节点");
                if (场景数据脚本实例.本地网络ID == Multiplayer.GetUniqueId())
                {
                    本地玩家 = child as CharacterBody2D;
                    break;
                }
            }
        }
    }

    public override void _Process(double delta)
    {
        switch (内部数据.读取相机状态的方法())
        {
            case 相机状态Enum.跟随玩家:
                if (本地玩家 != null)
                {
                    地图场景相机.GlobalPosition = 本地玩家.GlobalPosition;
                }
                break;
            case 相机状态Enum.手动控制:
                Vector2 输入 = new Vector2(Input.GetActionStrength("右") - Input.GetActionStrength("左"), Input.GetActionStrength("下") - Input.GetActionStrength("上")).Normalized();
                if (地图场景相机.LimitEnabled)
                {
                    Vector2 半屏 = (GetViewport().GetVisibleRect().Size / 地图场景相机.Zoom) / 2;
                    Vector2 pos = 地图场景相机.GlobalPosition;

                    // --- 修正相机位置 ---
                    float left = 地图场景相机.LimitLeft + 半屏.X;
                    float right = 地图场景相机.LimitRight - 半屏.X;
                    float top = 地图场景相机.LimitTop + 半屏.Y;
                    float bottom = 地图场景相机.LimitBottom - 半屏.Y;

                    pos.X = Mathf.Clamp(pos.X, left, right);
                    pos.Y = Mathf.Clamp(pos.Y, top, bottom);

                    地图场景相机.GlobalPosition = pos;

                    // --- 修正输入方向 ---
                    if (pos.X < left || pos.X > right)
                        输入 = new Vector2(0, 输入.Y);

                    if (pos.Y < top || pos.Y > bottom)
                        输入 = new Vector2(输入.X, 0);
                }
                地图场景相机.GlobalPosition += 输入 * (内部数据类.手动控制移速 * (float)delta);
                break;

            default:
                GD.PrintErr($"{GetType().Name}：未知相机状态！");
                break;
        }
    }
}
