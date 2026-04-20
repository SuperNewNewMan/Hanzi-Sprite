using Godot;
using Godot.Collections;
using System;

public partial class 战斗_棋盘前端数据脚本 : Control
{
	// 信号
	[Signal]
	public delegate void 通知战斗权威端初始化棋盘完毕的信号EventHandler(int 本地网络会话ID);


    // 场景预制体
	private static PackedScene 卡牌预制体 = GD.Load<PackedScene>("res://场景文件夹/卡牌场景文件夹/卡牌MC.tscn");

	private class 内部数据类
	{
		public Dictionary<int, MarginContainer> 基础属性控件映射字典 = new();
		public MarginContainer 可控手牌区;
		private Array<MarginContainer> 手牌池 = new();

		public void 渲染手牌UI的方法(Array<string> 手牌数组)
		{
			HBoxContainer 手牌区HBox = 可控手牌区.GetNode<HBoxContainer>("%手牌区HBox");
			var 字灵字典 = 配置Json单例脚本实例.字灵字典;
            var 纹理字典 = 配置Json单例脚本实例.卡牌纹理映射字典;

            for (int i = 0; i < 手牌数组.Count; i++)
			{
				if (i >= 手牌池.Count)
				{
					string 字灵 = 手牌数组[i];

                    var 手牌 = 卡牌预制体.Instantiate<MarginContainer>();
					手牌池.Add(手牌);
					手牌区HBox.AddChild(手牌);

					var 字灵数据 = 字灵字典[字灵];
                }
            }
        }
	}

	// 场景树节点实例
	private 玩家数据单例脚本 玩家数据单例脚本实例;
	private static 配置Json单例脚本 配置Json单例脚本实例;
    private 联机权威后端数据单例脚本 联机权威后端数据单例脚本;

    private Timer 回合倒计时Timer;
	private Label 回合倒计时Label;
    private HBoxContainer 合纵方出牌区HBox;
	private HBoxContainer 连横方出牌区HBox;
	private HBoxContainer 合纵方手牌区总HBox;
    private HBoxContainer 连横方手牌区总HBox;
	private VBoxContainer 合纵方基础属性控件总VBox;
	private VBoxContainer 连横方基础属性控件总VBox;

	private Button 临阵脱逃按钮;
	private Button 结束回合按钮;
	private Panel 逃跑提示Panel;
	private Button 逃跑确认按钮;
	private Button 逃跑取消按钮;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		玩家数据单例脚本实例 = GetNode<玩家数据单例脚本>("/root/玩家数据单例脚本");
		配置Json单例脚本实例 = GetNode<配置Json单例脚本>("/root/配置Json单例脚本");
        联机权威后端数据单例脚本 = GetNode<联机权威后端数据单例脚本>("/root/联机权威后端数据单例脚本");

        回合倒计时Timer = GetNode<Timer>("回合倒计时Timer");
		回合倒计时Label = 回合倒计时Timer.GetNode<Label>("回合倒计时Label");
		合纵方出牌区HBox = GetNode<HBoxContainer>("%合纵方出牌区HBox");
		连横方出牌区HBox = GetNode<HBoxContainer>("%连横方出牌区HBox");
        合纵方手牌区总HBox = GetNode<HBoxContainer>("合纵方手牌区总HBox");
        连横方手牌区总HBox = GetNode<HBoxContainer>("连横方手牌区总HBox");
        合纵方基础属性控件总VBox = GetNode<VBoxContainer>("合纵方基础属性控件总VBox");
        连横方基础属性控件总VBox = GetNode<VBoxContainer>("连横方基础属性控件总VBox");

		临阵脱逃按钮 = GetNode<Button>("临阵脱逃按钮");
		临阵脱逃按钮.Connect("pressed", new Callable(this, nameof(临阵脱逃按钮被按下的回调函数)));
		结束回合按钮 = GetNode<Button>("结束回合按钮");
		逃跑提示Panel = GetNode<Panel>("逃跑提示Panel");
		逃跑确认按钮 = GetNode<Button>("%逃跑确认按钮");
		逃跑确认按钮.Connect("pressed", new Callable(this, nameof(逃跑确认按钮被按下的回调函数)));
        逃跑取消按钮 = GetNode<Button>("%逃跑取消按钮");
		逃跑取消按钮.Connect("pressed", new Callable(this, nameof(逃跑取消按钮被按下的回调函数)));

		// 初始化
        初始化棋盘控件的方法();
	}

	public override void _Process(double delta)
	{
		回合倒计时Label.Text = (回合倒计时Timer.TimeLeft < 10) ? 回合倒计时Label.Text = 回合倒计时Timer.TimeLeft.ToString("F1") : Mathf.Ceil((float)回合倒计时Timer.TimeLeft).ToString();
	}

	private void 临阵脱逃按钮被按下的回调函数()
	{
		逃跑提示Panel.Show();
    }

	private void 逃跑确认按钮被按下的回调函数()
	{
        GetTree().ChangeSceneToPacked(配置Json单例脚本实例.场景预制体字典[场景参数类脚本.场景名Enum.政务中心].场景预制体);
    }


    private void 逃跑取消按钮被按下的回调函数()
	{
		逃跑提示Panel.Hide();
    }

    private void 初始化棋盘控件的方法()
	{
		int 发起方ID = 联机权威后端数据单例脚本.战斗发起方ID;
		if (!联机权威后端数据单例脚本.连横方人数字典.ContainsKey(发起方ID))
		{
			GD.PrintErr($"{GetType().Name}：没有发起方的ID {发起方ID}，请检查哪里出现了问题！");
			return;
		}

		var 连横array = 联机权威后端数据单例脚本.连横方人数字典[发起方ID];
		var 合纵array = 联机权威后端数据单例脚本.合纵方人数字典[发起方ID];

		foreach (var check in 连横array)
		{


			if (check == 发起方ID)
			{

			}
		}
    }

	private void 出牌按钮被按下的回调函数()
	{
		
    }
}