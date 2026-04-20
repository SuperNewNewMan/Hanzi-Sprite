using Godot;
using System;

public partial class 管理剧情总节点脚本 : Node
{    
    protected 剧情演绎单例脚本 剧情演绎单例脚本实例;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        剧情演绎单例脚本实例 = GetNode<剧情演绎单例脚本>("/root/剧情演绎单例脚本");

        _ = 剧情演绎单例脚本实例.播放剧情的方法("新手教程", "新手教程_白天_1", string.Empty, null);
    }
}