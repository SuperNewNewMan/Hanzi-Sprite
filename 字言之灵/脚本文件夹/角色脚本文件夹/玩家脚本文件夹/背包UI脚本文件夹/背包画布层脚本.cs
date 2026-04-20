using Godot;
using Godot.Collections;
using System;
using static 物品参数类脚本;

public partial class 背包画布层脚本 : CanvasLayer
{
    private readonly static Color 聚焦颜色 = new("FFFFFF");
    private readonly static Color 失焦颜色 = new("737373");
    private PackedScene 槽位预制体 = GD.Load<PackedScene>("res://场景文件夹/角色场景文件夹/玩家场景文件夹/背包场景文件夹/槽位.tscn");

    private 玩家数据单例脚本 玩家数据单例脚本实例;
    private 配置Json单例脚本 配置Json单例脚本实例;

    private Dictionary<物品类型Enum, TextureButton> 背包按钮字典 = new();
    private 物品类型Enum 当前选中类型 = 物品类型Enum.杂物;

    private LineEdit 搜索输入栏;
    private Button 搜索按钮;
    private GridContainer 槽位GC;
    private Array<MarginContainer> 槽位池 = new();
    private Button 背包关闭按钮;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        玩家数据单例脚本实例 = GetNode<玩家数据单例脚本>("/root/玩家数据单例脚本");
        配置Json单例脚本实例 = GetNode<配置Json单例脚本>("/root/配置Json单例脚本");

        VBoxContainer 背包按钮VBox = GetNode<VBoxContainer>("%背包按钮VBox");
        for (int i = 0; i < 背包按钮VBox.GetChildCount(); i++)
        {
            物品类型Enum 按钮索引 = (物品类型Enum)i;
            背包按钮字典[按钮索引] = 背包按钮VBox.GetChild(i) as TextureButton;
            背包按钮字典[按钮索引].Pressed += () => 背包按钮被按下的回调函数(按钮索引);
        }
        // 初始化默认选中第一个按钮
        背包按钮被按下的回调函数(物品类型Enum.杂物);

        搜索输入栏 = GetNode<LineEdit>("%搜索输入栏");
        搜索按钮 = GetNode<Button>("%搜索按钮");
        搜索按钮.Connect("pressed", new Callable(this, nameof(搜索按钮被按下的回调函数)));
        槽位GC = GetNode<GridContainer>("%槽位GC");
        背包关闭按钮 = GetNode<Button>("%背包关闭按钮");
        背包关闭按钮.Connect("pressed", new Callable(this, nameof(背包关闭按钮被按下的回调函数)));

        Connect("visibility_changed", new Callable(this, nameof(刷新当前类型槽位的封装体方法)));
    }

    private void 背包按钮被按下的回调函数(物品类型Enum 按钮索引)
    {
        背包按钮字典[当前选中类型].Modulate = 失焦颜色;
        背包按钮字典[按钮索引].Modulate = 聚焦颜色;

        当前选中类型 = 按钮索引;

        刷新当前类型槽位的封装体方法();
    }

    private void 刷新当前类型槽位的封装体方法()
    {
        if (Visible)
        {
            int i = -1;
            var 物品字典 = 配置Json单例脚本实例.物品字典;

            var 智能字典 = 玩家数据单例脚本实例.玩家存档数据.物品库字典[当前选中类型];
            foreach (var (物品名, 物品数量) in 智能字典)
            {
                i++;

                MarginContainer 槽位;
                if (i >= 槽位池.Count)
                {
                    槽位 = 槽位预制体.Instantiate<MarginContainer>();
                    槽位池.Add(槽位);
                    槽位GC.AddChild(槽位);
                }
                else
                {
                    槽位 = 槽位池[i];
                }

                槽位脚本 脚本 = 槽位 as 槽位脚本;
                脚本.槽位内部类实例.重置槽位信息的方法();
                脚本.槽位内部类实例.设置物品信息的方法(物品字典[物品名].加载物品纹理的方法(), 物品数量, 物品字典[物品名].物品描述);

                槽位.Show();
            }

            while (i + 1 < 槽位池.Count)
            {
                i++;

                var 槽位 = 槽位池[i];
                var 脚本 = 槽位 as 槽位脚本;

                脚本.槽位内部类实例.重置槽位信息的方法();
                槽位.Hide();
            }
        }
    }

    private void 搜索按钮被按下的回调函数()
    {
        string 搜索词 = 搜索输入栏.Text;
    }

    private void 背包关闭按钮被按下的回调函数()
    {
        foreach (var 槽位 in 槽位池)
        {
            槽位脚本 脚本 = 槽位 as 槽位脚本;
            脚本.槽位内部类实例.重置槽位信息的方法();
            槽位.Hide();
        }

        Hide();
    }
}
