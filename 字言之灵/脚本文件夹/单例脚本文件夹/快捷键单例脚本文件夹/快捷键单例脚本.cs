using Godot;
using System;

public partial class 快捷键单例脚本 : Node
{
    public override void _Input(InputEvent @event)
    {
        if (Input.IsActionJustPressed("全屏"))
        {
            Window window = GetWindow();
            window.Mode = (window.Mode == Window.ModeEnum.Fullscreen) ? Window.ModeEnum.Windowed : Window.ModeEnum.Fullscreen;
        }
    }
}
