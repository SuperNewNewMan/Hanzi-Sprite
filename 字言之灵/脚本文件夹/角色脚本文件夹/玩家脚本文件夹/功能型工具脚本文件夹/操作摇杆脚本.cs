using Godot;
using Godot.Collections;
using System;

public partial class 操作摇杆脚本 : CanvasLayer
{
    private Sprite2D 移动摇杆背景精灵;
    private Sprite2D 移动摇杆精灵;

    private int 手指索引 = -1;
    private float 移动摇杆半径;
    private float 移动摇杆移动阈值;

    public enum 按钮类型Enum
    {
        交互,
        跑,
        菜单栏,
        任务,
        背包,
        身份证,
        图鉴
    }

    private const float 输入映射保留时间 = 5;
    private Dictionary<按钮类型Enum, bool> 输入映射聚焦字典 = new();
    private Dictionary<按钮类型Enum, SceneTreeTimer> 输入映射计时器字典 = new();
    private Dictionary<按钮类型Enum, Button> 按钮映射字典 = new();

    public override void _Ready()
    {
        移动摇杆背景精灵 = GetNode<Sprite2D>("%移动摇杆背景精灵");
        移动摇杆精灵 = 移动摇杆背景精灵.GetNode<Sprite2D>("移动摇杆精灵");

        移动摇杆半径 = 移动摇杆背景精灵.Texture.GetWidth() * 移动摇杆背景精灵.Scale.X / 2;
        移动摇杆移动阈值 = 移动摇杆背景精灵.Texture.GetWidth() / 2;

        Button F按钮 = GetNode<Button>("%F按钮");
        Button Shift按钮 = GetNode<Button>("%Shift按钮");
        Button Esc按钮 = GetNode<Button>("%Esc按钮");
        Button 任务按钮 = GetNode<Button>("%任务按钮");
        Button 背包按钮 = GetNode<Button>("%背包按钮");
        Button 身份证按钮 = GetNode<Button>("%身份证按钮");
        Button 图鉴按钮 = GetNode<Button>("%图鉴按钮");
        按钮映射字典[按钮类型Enum.交互] = F按钮;
        按钮映射字典[按钮类型Enum.跑] = Shift按钮;
        按钮映射字典[按钮类型Enum.菜单栏] = Esc按钮;
        按钮映射字典[按钮类型Enum.任务] = 任务按钮;
        按钮映射字典[按钮类型Enum.背包] = 背包按钮;
        按钮映射字典[按钮类型Enum.身份证] = 身份证按钮;
        按钮映射字典[按钮类型Enum.图鉴] = 图鉴按钮;
        foreach (var (枚举值, button) in 按钮映射字典)
        {
            按钮映射字典[枚举值] = button;
            输入映射聚焦字典[枚举值] = false;

            button.ButtonDown += () => 获得输入映射的回调函数(枚举值);
            button.ButtonUp += () => 释放输入映射的回调函数(枚举值);
        }
    }

    private async void 获得输入映射的回调函数(按钮类型Enum 枚举值)
    {
        string action = 枚举值.ToString();

        // 如果已经按着，就不重复触发
        if (输入映射聚焦字典[枚举值])
            return;

        输入映射聚焦字典[枚举值] = true;

        // 按下 → press
        Input.ActionPress(action);

        // 创建计时器
        var timer = GetTree().CreateTimer(输入映射保留时间);
        输入映射计时器字典[枚举值] = timer;

        await ToSignal(timer, "timeout");

        // 如果仍然按着，说明 ButtonUp 丢失 → 自动 release
        if (输入映射聚焦字典[枚举值])
        {
            Input.ActionRelease(action);
            输入映射聚焦字典[枚举值] = false;
        }

        timer = null;
        输入映射计时器字典.Remove(枚举值);
    }

    private void 释放输入映射的回调函数(按钮类型Enum 枚举值)
    {
        string action = 枚举值.ToString();

        // 松开 → release
        Input.ActionRelease(action);

        输入映射聚焦字典[枚举值] = false;

        // 停止计时器
        if (输入映射计时器字典.TryGetValue(枚举值, out var timer))
        {
            timer = null;
            输入映射计时器字典.Remove(枚举值);
        }
    }

    public override void _ExitTree()
    {
        // 场景退出兜底释放
        foreach (var item in Enum.GetValues(typeof(按钮类型Enum)))
        {
            Input.ActionRelease(item.ToString());
        }
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventScreenTouch touchEvent && touchEvent.Pressed)
        {
            float 距离 = touchEvent.Position.DistanceTo(移动摇杆背景精灵.Position);
            if (距离 <= 移动摇杆半径)
            {
                手指索引 = touchEvent.Index;
                移动摇杆精灵.GlobalPosition = touchEvent.Position;
            }
        }
        else if (@event is InputEventScreenDrag dragEvent)
        {
            float 距离 = dragEvent.Position.DistanceTo(移动摇杆背景精灵.Position);
            if (手指索引 == dragEvent.Index)
            {
                if (距离 <= 移动摇杆半径)
                {
                    移动摇杆精灵.GlobalPosition = dragEvent.Position;
                }
                else
                {
                    Vector2 方向 = (dragEvent.Position - 移动摇杆背景精灵.Position).Normalized();
                    移动摇杆精灵.Position = 方向 * 移动摇杆移动阈值;
                }
            }
        }

        if (@event is InputEventScreenTouch && !@event.IsPressed())
        {
            手指索引 = -1;
            移动摇杆精灵.Position = Vector2.Zero;
        }
    }
}