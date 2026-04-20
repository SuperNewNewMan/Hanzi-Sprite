using Godot;
using System;

public partial class 面板教程画布层脚本 : CanvasLayer
{
    private bool 启动;

    private ColorRect 面板教程遮罩;
    private TextureRect 面板教程洞口;
    private Node2D 面板教程光圈节点;
    private AnimationPlayer 面板教程动画机;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        面板教程遮罩 = GetNode<ColorRect>("面板教程遮罩");
        面板教程洞口 = 面板教程遮罩.GetNode<TextureRect>("面板教程洞口");
        面板教程光圈节点 = GetNode<Node2D>("面板教程光圈节点");
        面板教程动画机 = GetNode<AnimationPlayer>("面板教程动画机");
    }

    public override void _Input(InputEvent @event)
    {
        if (启动)
        {
            Vector2 鼠标位置 = GetViewport().GetMousePosition();
            Rect2 洞口 = new(面板教程洞口.Position, 面板教程洞口.Size);
            if (洞口.HasPoint(鼠标位置))
            {
                面板教程遮罩.MouseFilter = Control.MouseFilterEnum.Ignore;
            }
            else
            {
                面板教程遮罩.MouseFilter = Control.MouseFilterEnum.Stop;
            }
        }
    }

    public void 调整面板教程洞口的方法(Vector2 新尺寸, Vector2 新位置, Texture2D 新图片)
    {
        启动 = true;

        面板教程洞口.Size = 新尺寸;
        面板教程洞口.Position = 新位置;
        面板教程洞口.Texture = 新图片;

        面板教程光圈节点.Position = new(新位置.X + 新尺寸.X / 2, 新位置.Y + 新尺寸.Y / 2);

        面板教程动画机.Stop();
        面板教程动画机.Play("光圈动画");
    }

    public void 复原的方法()
    {
        启动 = false;

        面板教程动画机.Play("RESET");
        面板教程洞口.Texture = null;
    }
}