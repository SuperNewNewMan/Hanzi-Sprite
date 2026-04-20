using Godot;
using System;
using System.Drawing;

public partial class 任务指引总节点脚本 : Node2D
{
    public partial class 内部数据类 : RefCounted
    {
        public Camera2D 相机;

        public Sprite2D 圆圈精灵;
        public Sprite2D 箭头精灵;
        private const float 内缩量 = 10;
        public readonly static Vector2 箭头内缩量 = new(内缩量, 内缩量);

        private AnimationPlayer 任务指引动画机;
        private const string 圆圈动画 = "圆圈动画";
        public void 播放动画的方法()
        {
            任务指引动画机.Play(圆圈动画);
        }
        public void 停止动画的方法()
        {
            任务指引动画机.Stop();
        }

        public 内部数据类() { }

        public 内部数据类(Camera2D 相机形参, Sprite2D 圆圈精灵形参, Sprite2D 箭头精灵形参, AnimationPlayer 任务指引动画机形参)
        {
            相机 = 相机形参;
            圆圈精灵 = 圆圈精灵形参;
            箭头精灵 = 箭头精灵形参;
            任务指引动画机 = 任务指引动画机形参;
        }
    }

    public 内部数据类 数据;

    private 玩家数据单例脚本 玩家数据单例脚本实例;

    public override void _Ready()
    {
        数据 = new(GetViewport().GetCamera2D(), GetNode<Sprite2D>("圆圈精灵"), GetNode<Sprite2D>("箭头精灵"), GetNode<AnimationPlayer>("任务指引动画机"));
        玩家数据单例脚本实例 = GetNode<玩家数据单例脚本>("/root/玩家数据单例脚本");
    }

    public override void _Process(double delta)
    {
        if (Multiplayer.GetUniqueId() == 联机权威后端数据单例脚本.服务器网络会话ID)
        {
            追踪任务地点的方法();
        }
    }

    private void 追踪任务地点的方法()
    {
        var 任务地点 = 玩家数据单例脚本实例.玩家游戏数据.任务地点;

        if (!玩家数据单例脚本实例.玩家游戏数据.正在追踪任务)
        {
            Hide();
            return;
        }

        Show();

        var 相机 = 数据.相机;

        // 世界坐标下的屏幕矩形
        var 屏幕大小 = 相机.GetViewportRect().Size / 相机.Zoom;
        var 屏幕原点 = 相机.GetScreenCenterPosition() - (屏幕大小 / 2);
        Rect2 当前屏幕 = new(屏幕原点, 屏幕大小);

        // 如果相机有限制，且超出Limit，则 clamp 屏幕矩形
        if (相机.LimitEnabled)
        {
            Rect2 限定屏幕 = new(
                new Vector2(相机.LimitLeft, 相机.LimitTop),
                new Vector2(
                    相机.LimitRight - 相机.LimitLeft,
                    相机.LimitBottom - 相机.LimitTop
                )
            );

            // 判断是否越界
            bool 越界 =
                当前屏幕.Position.X < 限定屏幕.Position.X ||
                当前屏幕.Position.Y < 限定屏幕.Position.Y ||
                当前屏幕.End.X > 限定屏幕.End.X ||
                当前屏幕.End.Y > 限定屏幕.End.Y;

            if (越界)
            {
                // clamp 屏幕矩形，使其贴边但不跳变
                当前屏幕.Position = new Vector2(
                    Mathf.Clamp(当前屏幕.Position.X, 限定屏幕.Position.X, 限定屏幕.End.X - 当前屏幕.Size.X),
                    Mathf.Clamp(当前屏幕.Position.Y, 限定屏幕.Position.Y, 限定屏幕.End.Y - 当前屏幕.Size.Y)
                );
            }
        }

        var 圆圈 = 数据.圆圈精灵;
        var 箭头 = 数据.箭头精灵;

        // 屏幕内 -> 显示圆圈
        if (当前屏幕.HasPoint(任务地点))
        {
            圆圈.Show();
            箭头.Hide();
            圆圈.Position = 任务地点;
            数据.播放动画的方法();
            return;
        }

        // 屏幕外 -> 显示箭头
        箭头.Show();
        圆圈.Hide();
        数据.停止动画的方法();

        Vector2 dir = (任务地点 - 相机.GlobalPosition).Normalized();
        Vector2 half = 当前屏幕.Size / 2f;

        // 避免除零
        float tx = (Mathf.Abs(dir.X) < 0.0001f) ? float.PositiveInfinity : half.X / Mathf.Abs(dir.X);
        float ty = (Mathf.Abs(dir.Y) < 0.0001f) ? float.PositiveInfinity : half.Y / Mathf.Abs(dir.Y);

        float t = Mathf.Min(tx, ty);

        // 射线交点
        Vector2 hit = 相机.GlobalPosition + dir * t;

        // 更新箭头
        箭头.Position = hit - dir * 内部数据类.箭头内缩量;
        箭头.Rotation = dir.Angle();
    }
}
