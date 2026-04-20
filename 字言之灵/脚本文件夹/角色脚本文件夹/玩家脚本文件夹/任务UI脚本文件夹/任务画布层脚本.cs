using Godot;
using Godot.Collections;
using System;
using static 任务参数类脚本;
using static 物品参数类脚本;

public partial class 任务画布层脚本 : CanvasLayer
{
    private static PackedScene 任务卡预制体 = GD.Load<PackedScene>("res://场景文件夹/角色场景文件夹/玩家场景文件夹/任务场景文件夹/任务卡.tscn");

    private 玩家数据单例脚本 玩家数据单例脚本实例;
    private 配置Json单例脚本 配置Json单例脚本实例;

    private partial class 内部数据类 : RefCounted
    {
        public VBoxContainer 任务卡总VBox;
        public Array<任务卡脚本> 任务卡池 = new();
        public Button 任务关闭按钮;

        public 内部数据类() { }

        public 内部数据类(VBoxContainer 任务卡总VBox形参, Button 任务关闭按钮形参)
        {
            任务卡总VBox = 任务卡总VBox形参;
            任务关闭按钮 = 任务关闭按钮形参;
        }       
    }
    private 内部数据类 数据;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        玩家数据单例脚本实例 = GetNode<玩家数据单例脚本>("/root/玩家数据单例脚本");
        配置Json单例脚本实例 = GetNode<配置Json单例脚本>("/root/配置Json单例脚本");

        数据 = new(GetNode<VBoxContainer>("%任务卡总VBox"), GetNode<Button>("%任务关闭按钮"));
        数据.任务关闭按钮.Pressed += Hide;

        Connect("visibility_changed", new Callable(this, nameof(更新任务面板UI的方法)));
    }

    private void 更新任务面板UI的方法()
    {
        Array<任务数据类> arr = new();
        var 存档 = 玩家数据单例脚本实例.玩家存档数据.正在进行任务的字典;
        var 剧情 = 配置Json单例脚本实例.剧情字典;

        foreach (var (总剧情名, 细节dict) in 存档)
        {
            foreach (var (二级剧情名, 任务数组) in 细节dict)
            {
                var 任务库 = 配置Json单例脚本实例.剧情字典[总剧情名][二级剧情名].任务字典;
                foreach (var name in 任务数组)
                {
                    if (任务库.ContainsKey(name))
                    {
                        arr.Add(任务库[name]);
                    }
                }
            }
        }

        更新任务的方法(arr, 配置Json单例脚本实例.物品字典);
    }

    private void 更新任务的方法(Array<任务数据类> 任务数组, Dictionary<string, 物品数据类> 物品映射字典)
    {
        int i = 0;
        var list = 数据.任务卡池;
        foreach (var item in 任务数组)
        {
            if (i < list.Count)
            {
                list[i].内部数据.更新任务卡的方法(item, 物品映射字典);
                list[i].Show();
            }
            else
            {
                任务卡脚本 任务卡预制体实例 = 任务卡预制体.Instantiate() as 任务卡脚本;
                list.Add(任务卡预制体实例);
                数据.任务卡总VBox.AddChild(任务卡预制体实例);

                int index = i;
                任务卡预制体实例.内部数据.追踪按钮.Pressed += () => 追踪的方法(index);
                任务卡预制体实例.内部数据.搁置按钮.Pressed += () => 搁置的方法(index);

                任务卡预制体实例.内部数据.更新任务卡的方法(item, 物品映射字典);
            }

            i++;
        }

        for (int left = i; left < list.Count; left++)
        {
            list[left].Hide();
        }
    }

    private void 追踪的方法(int index)
    {
        var 任务卡 = 数据.任务卡池[index];
        玩家数据单例脚本实例.玩家游戏数据.任务地点 = 任务卡.内部数据.任务地点;
        玩家数据单例脚本实例.玩家游戏数据.正在追踪任务 = true;
    }

    private void 搁置的方法(int index)
    {
        var 任务卡 = 数据.任务卡池[index];
        if (任务卡.内部数据.任务地点 == 玩家数据单例脚本实例.玩家游戏数据.任务地点)
        {
            玩家数据单例脚本实例.玩家游戏数据.正在追踪任务 = false;
        }
    }
}
