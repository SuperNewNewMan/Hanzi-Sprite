using Godot;
using System;
using System.Threading.Tasks;

public partial class UI面板总节点脚本 : Node2D
{
    private 玩家数据单例脚本 玩家数据单例脚本实例;

    CanvasLayer 操作摇杆画布层;
    private Node2D 摇杆总节点;
    private CanvasLayer 任务画布层;
    private CanvasLayer 背包画布层;
    private CanvasLayer 身份证画布层;
    private CanvasLayer 图鉴画布层;
    private CanvasLayer 菜单栏画布层;
    private 玩家场景数据节点脚本 玩家场景数据节点脚本实例;

    private Tween tween;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        玩家数据单例脚本实例 = GetNode<玩家数据单例脚本>("/root/玩家数据单例脚本");

        操作摇杆画布层 = GetNode<CanvasLayer>("操作摇杆画布层");
        摇杆总节点 = 操作摇杆画布层.GetNode<Node2D>("摇杆总节点");
        任务画布层 = GetNode<CanvasLayer>("任务画布层");
        背包画布层 = GetNode<CanvasLayer>("背包画布层");
        身份证画布层 = GetNode<CanvasLayer>("身份证画布层");
        图鉴画布层 = GetNode<CanvasLayer>("图鉴画布层");
        菜单栏画布层 = GetNode<CanvasLayer>("菜单栏画布层");
        玩家场景数据节点脚本实例 = GetNode<玩家场景数据节点脚本>("%玩家场景数据节点");

        if (玩家场景数据节点脚本实例.本地网络ID == Multiplayer.GetUniqueId())
        {
            操作摇杆画布层.Show();
        }

        // 显示ID
        Label ID标签 = GetNode<Label>("%ID标签");
        ID标签.Text = 玩家数据单例脚本实例.游戏行为数据.ID;
    }

    public override void _Process(double delta)
    {
        if (玩家场景数据节点脚本实例.本地网络ID == Multiplayer.GetUniqueId())
        {
            剧情控制摇杆显隐的方法();

            if (玩家数据单例脚本实例.玩家游戏数据.玩家拥有控制权())
            {
                if(Input.IsActionJustPressed("任务"))
                {
                    任务画布层.Visible = !任务画布层.Visible;
                }
                if (Input.IsActionJustPressed("背包"))
                {
                    背包画布层.Visible = !背包画布层.Visible;
                }
                if (Input.IsActionJustPressed("身份证") && 玩家数据单例脚本实例.玩家存档数据.物品库字典[物品参数类脚本.物品类型Enum.重要物品].ContainsKey("身份证"))
                {
                    身份证画布层.Visible = !身份证画布层.Visible;
                }
                if (Input.IsActionJustPressed("图鉴"))
                {
                    图鉴画布层.Visible = !图鉴画布层.Visible;
                }
                if (Input.IsActionJustPressed("菜单栏"))
                {
                    菜单栏画布层.Visible = !菜单栏画布层.Visible;
                }
            }
            else if (玩家数据单例脚本实例.玩家游戏数据.获得部分控制权 != string.Empty)
            {
                switch (玩家数据单例脚本实例.玩家游戏数据.获得部分控制权)
                {
                    case "背包":
                        if (Input.IsActionJustPressed("背包"))
                        {
                            背包画布层.Visible = !背包画布层.Visible;
                        }
                        break;
                    case "身份证":
                        if (Input.IsActionJustPressed("身份证") && 玩家数据单例脚本实例.玩家存档数据.物品库字典[物品参数类脚本.物品类型Enum.重要物品].ContainsKey("身份证"))
                        {
                            身份证画布层.Visible = true;
                        }
                        break;
                    case "图鉴":
                        if (Input.IsActionJustPressed("图鉴"))
                        {
                            图鉴画布层.Visible = !图鉴画布层.Visible;
                        }
                        break;
                    default:
                        break;
                }
            }
        }
    }

    private void 剧情控制摇杆显隐的方法()
    {
        if (玩家数据单例脚本实例.玩家游戏数据.玩家拥有控制权() || !string.IsNullOrEmpty(玩家数据单例脚本实例.玩家游戏数据.获得部分控制权))
        {
            if ((tween == null || !tween.IsRunning()) && 摇杆总节点.Modulate.A != 1)
            {
                tween = GetTree().CreateTween();
                tween.TweenProperty(摇杆总节点, "modulate", new Color(1, 1, 1, 1), 0.2f);
            }
        }
        else
        {
            if ((tween == null || !tween.IsRunning()) && 摇杆总节点.Modulate.A != 0)
            {
                tween = GetTree().CreateTween();
                tween.TweenProperty(摇杆总节点, "modulate", new Color(1, 1, 1, 0), 0.2f);
            }
        }
    }
}