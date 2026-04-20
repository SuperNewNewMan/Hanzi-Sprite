using Godot;
using System;

public partial class 玩家移动脚本 : CharacterBody2D
{
	public Vector2 朝向;
	private const float 移速 = 40;
	private bool 启用奔跑;
	public const int 默认移动增幅倍率 = 1;
	public const int 奔跑移动增幅倍率 = 2;
	public int 移速增幅倍率 = 1;
	public const float 移动混合值 = 1;
	public const float 待机混合值 = 0;

	private 玩家数据单例脚本 玩家数据单例脚本实例;

	private Sprite2D 移动摇杆背景精灵;
	private Sprite2D 移动摇杆精灵;
	private AnimationTree 玩家动画树;
	private 玩家场景数据节点脚本 玩家场景数据节点脚本实例;

	public override void _Ready()
	{
		玩家数据单例脚本实例 = GetNode<玩家数据单例脚本>("/root/玩家数据单例脚本");

		Node2D UI面板总节点 = GetNode<Node2D>("UI面板总节点");
		CanvasLayer 操作摇杆画布层 = UI面板总节点.GetNode<CanvasLayer>("操作摇杆画布层");
		Node2D 摇杆总节点 = 操作摇杆画布层.GetNode<Node2D>("摇杆总节点");
		移动摇杆背景精灵 = 摇杆总节点.GetNode<Sprite2D>("%移动摇杆背景精灵");
		移动摇杆精灵 = 移动摇杆背景精灵.GetNode<Sprite2D>("移动摇杆精灵");
		玩家动画树 = GetNode<AnimationTree>("玩家动画树");
		玩家场景数据节点脚本实例 = GetNode<玩家场景数据节点脚本>("%玩家场景数据节点");
	}

	public override void _PhysicsProcess(double delta)
	{
		// 控制权
		if (玩家场景数据节点脚本实例.本地网络ID == Multiplayer.GetUniqueId())
		{
			if (玩家数据单例脚本实例.玩家游戏数据.玩家拥有控制权())
			{
				Vector2 输入 = new Vector2(Input.GetActionStrength("右") - Input.GetActionStrength("左"), Input.GetActionStrength("下") - Input.GetActionStrength("上")).Normalized();
				朝向 = (输入 != Vector2.Zero) ? 输入 : 朝向;
				if (Input.IsActionJustPressed("跑"))
				{
					启用奔跑 = !启用奔跑;
				}
				移速增幅倍率 = (输入 != Vector2.Zero) ? 启用奔跑 ? 奔跑移动增幅倍率 : 默认移动增幅倍率 : 0;

				//if (移动摇杆精灵.Position != Vector2.Zero)
				//{
				//    朝向 = 移动摇杆精灵.Position.Normalized();
				//    Velocity = 朝向 * 移速;
				//}
				//else
				//{
				//    Velocity = Vector2.Zero;
				//}
			}

			// 计算Velocity
			移动的封装体方法(朝向);

			// 移动动画
			if (Velocity != Vector2.Zero)
			{
				玩家动画树.Set("parameters/移速TS/scale", 移速增幅倍率);
				玩家动画树.Set("parameters/移动BS2D/blend_position", 朝向);
				玩家动画树.Set("parameters/移动系统B2/blend_amount", 移动混合值);
			}
			else
			{
				玩家动画树.Set("parameters/待机BS2D/blend_position", 朝向);
				玩家动画树.Set("parameters/移动系统B2/blend_amount", 待机混合值);
			}

			// 进行移动
			MoveAndSlide();


			// 剧情模块
			玩家数据单例脚本实例.玩家游戏数据.检测玩家已到达剧情移动目标点的方法(this);
		}
	}

	public void 移动的封装体方法(Vector2 朝向)
	{
		Velocity = (朝向 != Vector2.Zero) ? (朝向 * 移速 * 移速增幅倍率) : Vector2.Zero;
	}
}
