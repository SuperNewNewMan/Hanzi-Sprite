using Godot;
using System;
using static 物品参数类脚本;

public partial class 自助办理机面板画布层脚本 : CanvasLayer
{
    private 玩家数据单例脚本 玩家数据单例脚本实例;
    private 配置Json单例脚本 配置Json单例脚本实例;

    // 自助办理机的内部数据
    private partial class 固定的数据类 : RefCounted
    {
        // 动画
        public const string 重置动画 = "RESET";
        public const string 证件口提示动画 = "证件口提示动画";
        public const string 提交面板动画 = "提交面板动画";

        public const string 电子档证件照 = "电子档证件照";
        public const string 户口复印件 = "户口复印件";
        public const string 身份证 = "身份证";

        public 物品数据类 物品快照;

        public bool 已放入证件照;
        public bool 已放入户口复印件;
        public bool 已按下手掌;
        public bool 已刷身份证;
        // 提示图片
        public TextureRect 照片已提交图片;
        public TextureRect 户口复印件已提交图片;
        public TextureRect 手掌已按下图片;
        public TextureRect 身份证已刷图片;


        public void 重置数据的方法()
        {
            物品快照 = null;

            已放入证件照 = false;
            已放入户口复印件 = false;
            已按下手掌 = false;
            已刷身份证 = false;
            // 提示图片
            照片已提交图片.Hide();
            户口复印件已提交图片.Hide();
            手掌已按下图片.Hide();
            身份证已刷图片.Hide();
        }

        public void 放入的物品的方法()
        {
            if (物品快照 != null)
            {
                switch (物品快照.物品名)
                {
                    case 电子档证件照:
                        已放入证件照 = true;
                        照片已提交图片.Show();
                        break;
                    case 户口复印件:
                        已放入户口复印件 = true;
                        户口复印件已提交图片.Show();
                        break;
                    default:
                        break;
                }
            }
        }
    }
    private 固定的数据类 固定的数据类实例 = new();

    // 开始面板
    private MarginContainer 开始面板MC;
    private Button 进入身份证面板按钮;
    private Button 进入字灵证面板按钮;

    // 身份证面板
    private MarginContainer 办理身份证面板MC;
    private Button 手机按钮;
    private Button 办理身份证按钮;
    private Button 身份证面板返回按钮;

    // 字灵证面板
    private MarginContainer 办理字灵证面板MC;
    private Button 办理字灵证按钮;
    private Button 字灵证面板返回按钮;

    // 底部按钮
    private Button 取号口按钮;
    private Button 证件口按钮;
    private Button 复印件提交口按钮;
    private Button 手掌按钮;
    private Button 身份证按钮;

    // 提交面板
    private MarginContainer 自助办理机槽位;
    private Button 物品按钮;
    private 槽位脚本 槽位脚本实例;
    private Button 提交物品按钮;
    private Button 关闭提交面板按钮;

    // 关闭
    private Button 关闭面板按钮;

    // 动画
    private AnimationPlayer 自助办理机动画机;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        玩家数据单例脚本实例 = GetNode<玩家数据单例脚本>("/root/玩家数据单例脚本");
        配置Json单例脚本实例 = GetNode<配置Json单例脚本>("/root/配置Json单例脚本");

        开始面板MC = GetNode<MarginContainer>("%开始面板MC");
        进入身份证面板按钮 = GetNode<Button>("%进入身份证面板按钮");
        进入字灵证面板按钮 = GetNode<Button>("%进入字灵证面板按钮");

        办理身份证面板MC = GetNode<MarginContainer>("%办理身份证面板MC");
        进入身份证面板按钮.Pressed += 办理身份证面板MC.Show;
        手机按钮 = GetNode<Button>("%手机按钮");
        手机按钮.Pressed += () =>
        {
            固定的数据类实例.物品快照 = 更新提交面板UI的方法(固定的数据类.电子档证件照);
        };
        办理身份证按钮 = GetNode<Button>("%办理身份证按钮");
        办理身份证按钮.Pressed += () =>
        {
            if (固定的数据类实例.已放入证件照 && 固定的数据类实例.已放入户口复印件)
            {
                固定的数据类实例.重置数据的方法();

                var 身份证数据 = 配置Json单例脚本实例.物品字典[固定的数据类.身份证];
                var 重要物品库 = 玩家数据单例脚本实例.玩家存档数据.物品库字典[物品类型Enum.重要物品];
                重要物品库[固定的数据类.电子档证件照] -= 1;
                重要物品库[固定的数据类.户口复印件] -= 1;
                重要物品库[固定的数据类.身份证] = 1;

                自助办理机动画机.Play(固定的数据类.证件口提示动画);
            }
        };
        身份证面板返回按钮 = GetNode<Button>("%身份证面板返回按钮");
        身份证面板返回按钮.Pressed += () => { 办理身份证面板MC.Hide(); 固定的数据类实例.重置数据的方法(); 槽位脚本实例.槽位内部类实例.重置槽位信息的方法(); };

        办理字灵证面板MC = GetNode<MarginContainer>("%办理字灵证面板MC");
        进入字灵证面板按钮.Pressed += 办理字灵证面板MC.Show;
        办理字灵证按钮 = GetNode<Button>("%办理字灵证按钮");
        字灵证面板返回按钮 = GetNode<Button>("%字灵证面板返回按钮");
        字灵证面板返回按钮.Pressed += () => { 办理字灵证面板MC.Hide(); 固定的数据类实例.重置数据的方法(); 槽位脚本实例.槽位内部类实例.重置槽位信息的方法(); };

        取号口按钮 = GetNode<Button>("%取号口按钮");
        取号口按钮.Pressed += () => { 自助办理机动画机.Play(固定的数据类.重置动画); };
        证件口按钮 = GetNode<Button>("%证件口按钮");
        证件口按钮.Pressed += () => { 自助办理机动画机.Play(固定的数据类.重置动画); };
        复印件提交口按钮 = GetNode<Button>("%复印件提交口按钮");
        复印件提交口按钮.Pressed += () =>
        {
            固定的数据类实例.物品快照 = 更新提交面板UI的方法(固定的数据类.户口复印件);
        };
        手掌按钮 = GetNode<Button>("%手掌按钮");
        身份证按钮 = GetNode<Button>("%身份证按钮");

        自助办理机槽位 = GetNode<MarginContainer>("%自助办理机槽位");
        槽位脚本实例 = 自助办理机槽位 as 槽位脚本;
        物品按钮 = 自助办理机槽位.GetNode<Button>("%物品按钮");
        提交物品按钮 = GetNode<Button>("%提交物品按钮");
        提交物品按钮.Pressed += () => { 物品按钮.ButtonPressed = false; 固定的数据类实例.放入的物品的方法(); };
        关闭提交面板按钮 = GetNode<Button>("%关闭提交面板按钮");
        关闭提交面板按钮.Pressed += () => { 自助办理机动画机.PlayBackwards(固定的数据类.提交面板动画); 固定的数据类实例.物品快照 = null; };

        固定的数据类实例.照片已提交图片 = GetNode<TextureRect>("%照片已提交图片");
        固定的数据类实例.户口复印件已提交图片 = GetNode<TextureRect>("%户口复印件已提交图片");
        固定的数据类实例.手掌已按下图片 = GetNode<TextureRect>("%手掌已按下图片");
        固定的数据类实例.身份证已刷图片 = GetNode<TextureRect>("%身份证已刷图片");

        关闭面板按钮 = GetNode<Button>("%关闭面板按钮");
        关闭面板按钮.Connect("pressed", new Callable(this, nameof(关闭面板按钮被按下的回调函数)));

        自助办理机动画机 = GetNode<AnimationPlayer>("自助办理机动画机");
    }

    private void 关闭面板按钮被按下的回调函数()
    {
        // 恢复默认设置
        Hide();
        办理身份证面板MC.Hide();
        办理字灵证面板MC.Hide();

        自助办理机动画机.Play(固定的数据类.重置动画);
        固定的数据类实例.重置数据的方法();
        槽位脚本实例.槽位内部类实例.重置槽位信息的方法();
    }

    private 物品数据类 更新提交面板UI的方法(string 物品名)
    {
        自助办理机动画机.Play(固定的数据类.提交面板动画);
        var 重要物品库 = 玩家数据单例脚本实例.玩家存档数据.物品库字典[物品类型Enum.重要物品];
        物品数据类 物品数据 = null;

        槽位脚本实例.槽位内部类实例.重置槽位信息的方法();
        if (重要物品库.ContainsKey(物品名))
        {
            物品数据 = 配置Json单例脚本实例.物品字典[物品名];
            槽位脚本实例.槽位内部类实例.设置物品信息的方法(物品数据.加载物品纹理的方法(), 重要物品库[物品名], 物品数据.物品描述);
        }

        return 物品数据;
    }
}