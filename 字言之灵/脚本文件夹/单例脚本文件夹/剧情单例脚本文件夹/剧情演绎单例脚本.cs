using DialogueManagerRuntime;
using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static 可交互体封装类脚本;

public partial class 剧情演绎单例脚本 : Node
{
    private readonly static Vector2 上vec2 = new(0, -1);
    private readonly static Vector2 下vec2 = new(0, 1);
    private readonly static Vector2 左vec2 = new(-1.1f, 0);
    private readonly static Vector2 右vec2 = new(1.1f, 0);
    private readonly CanvasLayer 面板教程画布层 = GD.Load<PackedScene>("res://场景文件夹/主场景文件夹/功能型工具场景文件夹/面板教程画布层.tscn").Instantiate() as CanvasLayer;

    public string 当前总剧情名;
    public string 当前二级剧情名;
    private 可交互体方法参数类 当前可交互体数据实例;

    private 玩家数据单例脚本 玩家数据单例脚本实例;
    private 配置Json单例脚本 配置Json单例脚本实例;

    public override void _Ready()
    {
        玩家数据单例脚本实例 = GetNode<玩家数据单例脚本>("/root/玩家数据单例脚本");
        配置Json单例脚本实例 = GetNode<配置Json单例脚本>("/root/配置Json单例脚本");
    }

    #region 播放剧情部分
    public async Task 播放剧情的方法(string 总剧情名, string 二级剧情名, string 标题名, 可交互体方法参数类 可交互体数据实例形参)
    {
        配置Json单例脚本实例.配置剧情字典的方法(总剧情名);

        var Json快照 = 配置Json单例脚本实例.剧情字典[总剧情名][二级剧情名];
        var Data快照 = 玩家数据单例脚本实例.玩家存档数据.记录总剧情的字典;
        if (!Data快照.ContainsKey(总剧情名) || !Data快照[总剧情名].Contains(二级剧情名) || Json快照.可重复进行)
        {
            DialogueManager.ShowDialogueBalloon(Json快照.剧情资源, 标题名);
            当前总剧情名 = 总剧情名;
            当前二级剧情名 = 二级剧情名;
            当前可交互体数据实例 = 可交互体数据实例形参;
            await ToSignal(DialogueManager.Instance, "dialogue_ended");
        }
    }
    #endregion

    #region 通用部分
    public void 展示角色立绘的方法(TextureRect Portrait, RichTextLabel CharacterLabel)
    {
        var 快照 = 配置Json单例脚本实例.剧情字典[当前总剧情名];
        Portrait.Texture = 快照[当前二级剧情名].角色肖像图片字典.ContainsKey(CharacterLabel.Text) ? 快照[当前二级剧情名].角色肖像图片字典[CharacterLabel.Text] : null;
    }

    public bool 检查是否拥有该物品的方法(string 物品名)
    {
        var 物品 = 配置Json单例脚本实例.物品字典[物品名];
        var 智能字典 = 玩家数据单例脚本实例.玩家存档数据.物品库字典[物品.物品类型];

        return 智能字典.ContainsKey(物品名);
    }

    public void 添加物品的方法(string 物品名, int 数量)
    {
        var 物品 = 配置Json单例脚本实例.物品字典[物品名];
        var 智能字典 = 玩家数据单例脚本实例.玩家存档数据.物品库字典[物品.物品类型];
        if(智能字典.ContainsKey(物品名))
        {
            智能字典[物品名] += 数量;
        }
        else
        {
            智能字典[物品名] = 数量;
        }
    }

    public void 减少物品的方法(string 物品名, int 数量)
    {
        var 物品 = 配置Json单例脚本实例.物品字典[物品名];
        var 智能字典 = 玩家数据单例脚本实例.玩家存档数据.物品库字典[物品.物品类型];
        if (智能字典.ContainsKey(物品名))
        {
            智能字典[物品名] -= 数量;
        }
    }

    public bool 检查字灵库存是否存在该字灵的方法(string 字灵名)
    {
        var 字灵字典 = 玩家数据单例脚本实例.玩家存档数据.字灵字典;
        return 字灵字典.ContainsKey(字灵名);
    }

    public void 修改剧情条件的方法(string 条件名, bool 真假值)
    {
        var 快照 = 配置Json单例脚本实例.剧情条件字典;
        快照[条件名] = 真假值;
    }

    public bool 获取剧情条件的方法(string 条件名)
    {
        var 快照 = 配置Json单例脚本实例.剧情条件字典;
        return 快照[条件名];
    }

    public async Task 等待动画播放完毕的方法(string 节点路径)
    {
        AnimationPlayer 动画机 = 解析节点路径的封装体方法(节点路径) as AnimationPlayer;
        await ToSignal(动画机, "animation_finished");
    }

    public void 创建面板教程画布层的方法()
    {
        GetTree().CurrentScene.AddChild(面板教程画布层);
    }

    public void 创建面板洞口的方法(string 节点路径)
    {
        if (解析节点路径的封装体方法(节点路径) is Button 按钮)
        {
            面板教程画布层脚本 面板教程画布层脚本实例 = 面板教程画布层 as 面板教程画布层脚本;
            面板教程画布层脚本实例.调整面板教程洞口的方法(按钮.Size, 按钮.GlobalPosition, 按钮.Icon);
        }
    }

    public void 显隐面板教程画布层的方法(bool 显示)
    {
        面板教程画布层.Visible = 显示;
    }

    public async Task 等待按钮被按下的方法(string 节点路径)
    {
        Button 按钮 = 解析节点路径的封装体方法(节点路径) as Button;
        await ToSignal(按钮, "pressed");
    }

    public void 寻找玩家UI面板按钮并创建洞口的方法(string 节点路径)
    {
        Button 按钮 = 解析节点路径的封装体方法(节点路径) as Button;
        面板教程画布层脚本 面板教程画布层脚本实例 = 面板教程画布层 as 面板教程画布层脚本;
        面板教程画布层脚本实例.调整面板教程洞口的方法(按钮.Size, 按钮.GlobalPosition, 按钮.Icon);
    }

    public async Task 等待玩家UI面板按钮被按下的方法(string 节点路径)
    {
        Button 按钮 = 解析节点路径的封装体方法(节点路径) as Button;
        await ToSignal(按钮, "pressed");
    }

    public void 移除面板教程画布层的方法()
    {
        面板教程画布层脚本 面板教程画布层脚本实例 = 面板教程画布层 as 面板教程画布层脚本;
        面板教程画布层脚本实例.复原的方法();

        GetTree().CurrentScene.RemoveChild(面板教程画布层);
    }

    private Node 解析节点路径的封装体方法(string 节点路径)
    {
        // 从当前场景开始查
        string[] list = 节点路径.Split(',').Select(s => s.Trim()).ToArray();
        var 解析后的节点名 = 解析节点名的封装体方法(list[0]);
        var 容错 = GetTree().CurrentScene.GetNodeOrNull($"%{解析后的节点名}");

        Node 目标节点 = (容错 == null) ? GetTree().CurrentScene.GetNode($"{解析后的节点名}") : 容错;

        // 如果有多个节点，则继续从第二个节点开始查
        for (int i = 1; i < list.Length; i++)
        {
            解析后的节点名 = 解析节点名的封装体方法(list[i]);
            容错 = 目标节点.GetNodeOrNull($"%{解析后的节点名}");

            目标节点 = (容错 == null) ? 目标节点.GetNode($"{解析后的节点名}") : 容错;
        }

        return 目标节点;
    }

    private string 解析节点名的封装体方法(string 节点名)
    {
        if (节点名.StartsWith("{") && 节点名.EndsWith("}"))
        {
            switch (节点名)
            {
                case "{本地玩家}":
                    return Multiplayer.GetUniqueId().ToString();
                default:
                    break;
            }
        }

        return 节点名;
    }
    #endregion

    #region 装置部分
    public bool 允许启用装置()
    {
        if (当前可交互体数据实例 != null)
        {
            return 当前可交互体数据实例.允许使用所有装置();
        }

        GD.PrintErr($"{GetType().Name}：当前可交互体数据实例为空，请检查调用了播放剧情的方法时，是否传入了可交互体数据实例");
        return false;
    }

    public bool 检查某个装置是否已启用的方法(string 装置名)
    {
        if (当前可交互体数据实例 != null)
        {
            return 当前可交互体数据实例.获取装置是否允许启用的方法(装置名);
        }

        GD.PrintErr($"{GetType().Name}：当前可交互体数据实例为空，请检查调用了播放剧情的方法时，是否传入了可交互体数据实例");
        return false;
    }

    public void 更新是否允许启用装置的方法(string 装置名, bool 目标值)
    {
        if (当前可交互体数据实例 != null)
        {
            当前可交互体数据实例.更新装置是否允许启用的方法(装置名, 目标值);
            return;
        }

        GD.PrintErr($"{GetType().Name}：当前可交互体数据实例为空，请检查调用了播放剧情的方法时，是否传入了可交互体数据实例");
    }

    public void 外部触发装置的方法(string 被触发的可交互体名, string 内部装置名, bool 目标值)
    {
        var data = 配置Json单例脚本实例.场景预制体字典[Enum.Parse<场景参数类脚本.场景名Enum>($"{GetTree().CurrentScene.Name}")];
        var 实例 = data.可交互体的触发方法字典[被触发的可交互体名];

        实例.更新装置是否允许启用的方法(内部装置名, 目标值);
    }

    public void 记录装置通关的方法(string 装置名)
    {
        var 游戏行为 = 玩家数据单例脚本实例.游戏行为数据;

        游戏行为.装置完成时间[装置名] = DateTime.Now.ToString("HH:mm:ss");
    }

    public void 记录错误次数的方法(string 装置名)
    {
        var 游戏行为 = 玩家数据单例脚本实例.游戏行为数据;

        游戏行为.推理错误次数[装置名]++;
    }
    #endregion

    #region 控制权部分
    public void 单机_掌管玩家控制权的方法()
    {
        玩家数据单例脚本实例.玩家游戏数据.在剧情中 = true;

        // 控制玩家停下
        玩家移动脚本 本地玩家 = 单机_获取本地玩家的方法() as 玩家移动脚本;
        本地玩家.移速增幅倍率 = 0;
    }

    public void 单机_恢复玩家控制权的方法()
    {
        var 字典 = 玩家数据单例脚本实例.玩家存档数据.记录总剧情的字典;

        if (!字典.TryGetValue(当前总剧情名, out var 集合))
        {
            集合 = new();
            字典[当前总剧情名] = 集合;
        }

        集合.Add(当前二级剧情名);

        当前总剧情名 = string.Empty;
        当前二级剧情名 = string.Empty;
        玩家数据单例脚本实例.玩家游戏数据.重置剧情数据的方法();
    }

    public void 单机_给予玩家部分控制权的方法(string 输入名)
    {
        玩家数据单例脚本实例.玩家游戏数据.获得部分控制权 = 输入名;
    }

    public void 单机_取消玩家部分控制权的方法()
    {
        玩家数据单例脚本实例.玩家游戏数据.获得部分控制权 = string.Empty;
    }

    private CharacterBody2D 单机_获取本地玩家的方法()
    {
        Node 玩家总节点 = GetTree().CurrentScene.GetNode("玩家总节点");
        CharacterBody2D 本地玩家 = 玩家总节点.GetNode<CharacterBody2D>($"{Multiplayer.GetUniqueId()}");
        return 本地玩家;
    }
    #endregion

    #region 剧情操控玩家行为部分
    public void 单机_重新定位角色至路径起点上的方法(Line2D 路径)
    {
        CharacterBody2D 本地玩家 = 单机_获取本地玩家的方法();
        本地玩家.Position = 路径.Points[0];
    }

    public Line2D 单机_获取剧情路径的方法(string 路径节点名称)
    {
        Line2D 路径 = GetTree().CurrentScene.GetNode<Line2D>($"%{路径节点名称}");
        return 路径;
    }

    public void 单机_角色原地转向的方法(string 朝向的字符串)
    {
        Vector2 朝向 = 转换朝向的封装体方法(朝向的字符串);

        玩家移动脚本 本地玩家的移动脚本 = 单机_获取本地玩家的方法() as 玩家移动脚本;
        本地玩家的移动脚本.朝向 = 朝向;
        本地玩家的移动脚本.移速增幅倍率 = 0;
    }

    private Vector2 转换朝向的封装体方法(string 朝向)
    {
        switch (朝向)
        {
            case "上":
                return 上vec2;

            case "下":
                return 下vec2;

            case "左":
                return 左vec2;

            case "右":
                return 右vec2;

            default:
                GD.PrintErr($"{GetType().Name}：所输入的朝向 {朝向} 不正确");
                return Vector2.Zero;
        }
    }


    // 移动部分
    public async Task 单机_角色在路径上移动的方法(Line2D 路径, int 起点, int 终点)
    {
        Vector2 朝向 = (路径.Points[终点] - 路径.Points[起点]).Normalized();

        玩家移动脚本 本地玩家的移动脚本 = 单机_获取本地玩家的方法() as 玩家移动脚本;
        玩家数据单例脚本实例.玩家游戏数据.剧情移动目标点 = 路径.Points[终点];
        本地玩家的移动脚本.朝向 = 朝向;
        本地玩家的移动脚本.移速增幅倍率 = 玩家移动脚本.默认移动增幅倍率;

        await ToSignal(玩家数据单例脚本实例.玩家游戏数据, nameof(玩家数据单例脚本实例.玩家游戏数据.玩家已到达剧情移动目标点的信号));
    }
}
#endregion
