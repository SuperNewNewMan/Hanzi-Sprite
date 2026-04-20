using Godot;
using System;
using System.Collections.Generic;

public partial class 开场引言界面脚本 : Control
{
    private 配置Json单例脚本 配置Json单例脚本实例;

    private Random rd = new Random();
    private Dictionary<string, string> 名人名言字典 = new Dictionary<string, string>()
    {
        { "名无固宜，约之以命，约定俗成谓之宜。", "——《荀子·正名》" },
        { "名不正，则言不顺；言不顺，则事不成。", "——《论语·子路》" },
        { "礼，失则求诸野。", "——《礼记·檀弓上》" },
    };
    private Label 开场白引用标签;
    private Label 开场白出处标签;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        配置Json单例脚本实例 = GetNode<配置Json单例脚本>("/root/配置Json单例脚本");

        VBoxContainer 开场白VBox = GetNode<VBoxContainer>("开场白VBox");
        开场白引用标签 = 开场白VBox.GetNode<Label>("开场白引用标签");
        开场白出处标签 = 开场白VBox.GetNode<Label>("开场白出处标签");
        int idx = rd.Next(名人名言字典.Count);
        var element = new List<KeyValuePair<string, string>>(名人名言字典)[idx];
        开场白引用标签.Text = element.Key;
        开场白出处标签.Text = element.Value;
    }

    public void 切换到开始界面的方法()
    {
        GetTree().ChangeSceneToPacked(配置Json单例脚本实例.场景预制体字典[场景参数类脚本.场景名Enum.开始界面].场景预制体);
    }
}
