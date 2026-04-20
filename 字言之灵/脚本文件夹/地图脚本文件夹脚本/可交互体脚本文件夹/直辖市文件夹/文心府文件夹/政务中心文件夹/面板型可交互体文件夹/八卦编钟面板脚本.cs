using Godot;
using Godot.Collections;
using System;
using System.Threading.Tasks;
using static 字灵参数类脚本;

public partial class 八卦编钟面板脚本 : CanvasLayer
{
    private static PackedScene 按钮预制体 = GD.Load<PackedScene>("res://场景文件夹/地图场景文件夹/直辖市文件夹/文心府文件夹/政务中心文件夹/设施场景文件夹/八卦编钟装置/字灵按钮.tscn");
    private readonly static Dictionary<字灵属性Enum, Dictionary<string, string>> 暂时的硬编码配方字典 = new()
    {
        { 字灵属性Enum.金,
            new(){
                { "勾", "钩" },
            }
        },
        { 字灵属性Enum.木,
            new(){
                { "市", "柿" },
            }
        },
        { 字灵属性Enum.水,
            new(){
                { "原", "源" },
                { "张", "涨" },
            }
        },
    };

    private 玩家数据单例脚本 玩家数据单例脚本实例;
    private 剧情演绎单例脚本 剧情演绎单例脚本实例;
    private 配置Json单例脚本 配置Json单例脚本实例;

    private TextureRect 造字图片;
    private Button 钟槌按钮;
    private HBoxContainer 字灵按钮HBox;
    private Label 形旁标签;
    private Label 声旁标签;
    private Button 教程按钮;
    private AnimationPlayer 八卦编钟动画机;
    public Button 关闭面板按钮;

    private class 内部数据类
    {
        public 字灵属性Enum 当前所选的形旁;
        public string 当前所选的声旁;

        public Array<Button> 按钮池 = new();

        public void 重置的方法()
        {
            当前所选的形旁 = 字灵属性Enum.无;
            当前所选的声旁 = string.Empty;
        }
    }

    private 内部数据类 数据 = new 内部数据类();

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        玩家数据单例脚本实例 = GetNode<玩家数据单例脚本>("/root/玩家数据单例脚本");
        剧情演绎单例脚本实例 = GetNode<剧情演绎单例脚本>("/root/剧情演绎单例脚本");
        配置Json单例脚本实例 = GetNode<配置Json单例脚本>("/root/配置Json单例脚本");

        造字图片 = GetNode<TextureRect>("%造字图片");
        钟槌按钮 = GetNode<Button>("%钟槌按钮");
        钟槌按钮.Connect("pressed", new Callable(this, nameof(钟槌按钮被按下的回调函数)));
        字灵按钮HBox = GetNode<HBoxContainer>("%字灵按钮HBox");
        形旁标签 = GetNode<Label>("%形旁标签");
        声旁标签 = GetNode<Label>("%声旁标签");
        教程按钮 = GetNode<Button>("%教程按钮");
        教程按钮.Connect("pressed", new Callable(this, nameof(教程按钮被按下的回调函数)));
        八卦编钟动画机 = GetNode<AnimationPlayer>("%八卦编钟动画机");
        关闭面板按钮 = GetNode<Button>("%关闭面板按钮");
        关闭面板按钮.Connect("pressed", new Callable(this, nameof(关闭面板的回调函数)));

        Control 形旁按钮总控件 = GetNode<Control>("%形旁按钮总控件");
        foreach (var item in Enum.GetValues<字灵属性Enum>())
        {
             var btn = 形旁按钮总控件.GetNodeOrNull<Button>(item.ToString());
            if (btn != null)
            {
                btn.Pressed += () => 形旁按钮被按下的回调函数(item);
            }
        }

        Connect("visibility_changed", new Callable(this, nameof(刷新字灵库存的回调函数)));
        // 初始化
        刷新字灵库存的回调函数();
    }

    private void 钟槌按钮被按下的回调函数()
    {
        if (!暂时的硬编码配方字典.ContainsKey(数据.当前所选的形旁) || !暂时的硬编码配方字典[数据.当前所选的形旁].ContainsKey(数据.当前所选的声旁))
        {
            玩家数据单例脚本实例.游戏行为数据.造字失败次数++;
            return;
        }

        string 产物 = 暂时的硬编码配方字典[数据.当前所选的形旁][数据.当前所选的声旁];
        玩家数据单例脚本实例.游戏行为数据.造字完成时间[产物] = DateTime.Now.ToString("HH:mm:ss");
        var 字典 = 玩家数据单例脚本实例.玩家存档数据.字灵字典;
        if (字典.ContainsKey(产物))
        {
            字典[产物]++;
        }
        else
        {
            字典[产物] = 1;
        }

        var 配置 = 配置Json单例脚本实例.物品字典;
        造字图片.Texture = 配置[产物].加载物品纹理的方法();
        八卦编钟动画机.Play("造字冲击波动画");

        刷新字灵库存的回调函数();
    }

    private void 教程按钮被按下的回调函数()
    {
        _ = 剧情演绎单例脚本实例.播放剧情的方法("八卦编钟装置教程", "八卦编钟装置教程_白天", string.Empty, null);
    }

    private void 关闭面板的回调函数()
    {
        Hide();

        形旁标签.Text = string.Empty;
        声旁标签.Text = string.Empty;
        数据.重置的方法();
    }

    private void 形旁按钮被按下的回调函数(字灵属性Enum 属性)
    {
        数据.当前所选的形旁 = 属性;
        形旁标签.Text = 属性.ToString();
    }

    private void 刷新字灵库存的回调函数()
    {
        if (!Visible) return;

        var 字典 = 玩家数据单例脚本实例.玩家存档数据.字灵字典;
        int i = 0;
        foreach (var item in 字典.Keys)
        {
            if (i >= 数据.按钮池.Count)
            {
                Button 新btn = 按钮预制体.Instantiate<Button>();
                新btn.Text = item;
                数据.按钮池.Add(新btn);
                字灵按钮HBox.AddChild(新btn);

                新btn.Pressed += () =>
                {
                    声旁标签.Text = 新btn.Text;
                    数据.当前所选的声旁 = 新btn.Text;
                };
            }
            else
            {
                数据.按钮池[i].Text = item;
                数据.按钮池[i].Show();
            }

            i++;
        }

        while (i < 数据.按钮池.Count)
        {
            数据.按钮池[i].Hide();
            i++;
        }
    }
}
