using Godot;
using System;
using static 物品参数类脚本;

public partial class 创建角色画布层文件夹 : CanvasLayer
{
	private static class 固定数据类
	{
		public const string 默认姓名 = "鼠鼠";
		public const string 默认性别 = "男";
		public const string 默认种族 = "家鼠";
		public const string 默认住址 = "文心府龙游梅区冬明街道107号4单元4101室";
		public const string 默认字灵证 = "未开通";
		public const string 默认学术头衔 = "无";

		public const string 默认行政区划代码 = "100101";
		public static readonly Random 随机数生成器 = new Random();

		public const string 电子档证件照 = "电子档证件照";
		public const string 户口复印件 = "户口复印件";
	}

	private 配置Json单例脚本 配置Json单例脚本实例;
	private 玩家数据单例脚本 玩家数据单例脚本实例;

	private Button 创建按钮;
	private Button 返回按钮;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		配置Json单例脚本实例 = GetNode<配置Json单例脚本>("/root/配置Json单例脚本");
		玩家数据单例脚本实例 = GetNode<玩家数据单例脚本>("/root/玩家数据单例脚本");

		创建按钮 = GetNode<Button>("%创建按钮");
		创建按钮.Connect("pressed", new Callable(this, nameof(创建角色按钮被按下的回调函数)));
		返回按钮 = GetNode<Button>("%返回按钮");
		返回按钮.Connect("pressed", new Callable(this, nameof(返回按钮被按下的回调函数)));
	}

	private void 创建角色按钮被按下的回调函数()
	{
        // 记录游戏的开始时间
        玩家数据单例脚本实例.游戏行为数据.开始游戏的时间 = DateTime.Now.ToString("HH:mm:ss");

        // 创建角色信息
        玩家数据单例脚本实例.玩家存档数据.姓名 = 固定数据类.默认姓名;
		玩家数据单例脚本实例.玩家存档数据.性别 = 固定数据类.默认性别;
		玩家数据单例脚本实例.玩家存档数据.种族 = 固定数据类.默认种族;
		玩家数据单例脚本实例.玩家存档数据.住址 = 固定数据类.默认住址;
		玩家数据单例脚本实例.玩家存档数据.字灵证 = 固定数据类.默认字灵证;
		玩家数据单例脚本实例.玩家存档数据.学术头衔 = 固定数据类.默认学术头衔;

		获取出生和身份证号的方法(玩家数据单例脚本实例.玩家存档数据);

		// 创建默认获得的物品
		物品数据类 电子档证件照 = 配置Json单例脚本实例.物品字典[固定数据类.电子档证件照];
		物品数据类 户口复印件 = 配置Json单例脚本实例.物品字典[固定数据类.户口复印件];
		var 快照 = 玩家数据单例脚本实例.玩家存档数据.物品库字典[电子档证件照.物品类型];
		快照[电子档证件照.物品名] = 1;
		快照[户口复印件.物品名] = 1;

		GetTree().ChangeSceneToPacked(配置Json单例脚本实例.场景预制体字典[场景参数类脚本.场景名Enum.政务中心].场景预制体);
	}

	private void 返回按钮被按下的回调函数()
	{
		Hide();
	}

	public static void 获取出生和身份证号的方法(玩家数据类脚本.玩家存档数据类 玩家存档数据)
	{
		// 获取现在的时间
		DateTime 出生日期 = DateTime.Now;
		玩家存档数据.出生 = new(出生日期.Year, 出生日期.Month, 出生日期.Day);

		// 现在时间的总秒数
		int 总秒数 = (int)(出生日期 - DateTime.Today).TotalSeconds;
		int 映射 = 总秒数 * 10000 / 86400;
		// 随机取性别号。男奇数，女偶数
		int 性别码 = (玩家存档数据.性别 == "男") ? 固定数据类.随机数生成器.Next(0, 5) * 2 + 1 : 固定数据类.随机数生成器.Next(0, 5) * 2;

		玩家存档数据.身份号码 = $"{固定数据类.默认行政区划代码}{出生日期.Year}{出生日期.Month:D2}{出生日期.Day:D2}{映射:D4}{性别码}";
	}
}