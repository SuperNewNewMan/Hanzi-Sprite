using Godot;
using Godot.Collections;
using System;
using System.IO;
using static 玩家数据类脚本;

public partial class 玩家数据单例脚本 : Node
{
	public 玩家存档数据类 玩家存档数据 = new();

	public 玩家游戏数据类 玩家游戏数据 = new();

    public 游戏行为数据类 游戏行为数据 = new();

    private const double 自动存档倒计时总量 = 240;
    private double 当前倒计时 = 自动存档倒计时总量;

    public override void _Ready()
    {
        //玩家存档数据.物品库字典[物品参数类脚本.物品类型Enum.重要物品].Add("身份证", 1);
        玩家存档数据.物品库字典[物品参数类脚本.物品类型Enum.重要物品].Add("柿子", 10);
        玩家存档数据.物品库字典[物品参数类脚本.物品类型Enum.重要物品].Add("铜板", 10);
        //玩家存档数据.字灵字典.Add("勾", 1);
        //玩家存档数据.字灵字典.Add("原", 1);
        //玩家存档数据.字灵字典.Add("弓", 1);
        //玩家存档数据.字灵字典.Add("长", 1);
        //玩家存档数据.字灵字典.Add("张", 1);
        //玩家存档数据.字灵字典.Add("钩", 1);
        //玩家存档数据.字灵字典.Add("源", 1);
        //玩家存档数据.字灵字典.Add("涨", 1);
        //玩家存档数据.字灵字典.Add("柿", 1);
    }

    public override void _Notification(int what)
    {
        if (what == NotificationWMCloseRequest)
        {
            GD.Print("窗口关闭，正在保存游戏…");

            存档的方法();
            GetTree().Quit();
        }
    }

    public override void _Process(double delta)
    {
        当前倒计时 -= (float)delta;

        if (当前倒计时 <= 0f)
        {
            当前倒计时 = 自动存档倒计时总量;
            存档的方法();
        }
    }

    public void 存档的方法()
    {
        if (游戏行为数据.ID == string.Empty)
        {
            GD.Print($"ID无效：{游戏行为数据.ID}，保存失败！");
            return;
        }

        string folderPath = ProjectSettings.GlobalizePath("user://Save//");
        Directory.CreateDirectory(folderPath);

        string 写入path = Path.Combine(folderPath, $"{游戏行为数据.ID}.json");
        var json = Json.Stringify(游戏行为数据.ToGodot(), "\t");
        File.WriteAllText(写入path, json);
    }
}
