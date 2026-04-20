using Godot;
using Godot.Collections;
using System;
using static 任务参数类脚本;
using static 物品参数类脚本;

public partial class 任务卡脚本 : MarginContainer
{
	private static PackedScene 槽位预制体 = GD.Load<PackedScene>("res://场景文件夹/角色场景文件夹/玩家场景文件夹/背包场景文件夹/槽位.tscn");

	public partial class 内部数据类 : RefCounted
	{
		public Vector2 任务地点;

		private Label 任务名标签;
		private HBoxContainer 所需物品槽位HBox;
		private Array<槽位脚本> 所需物品槽位池 = new();
		private TextureRect 红箭头图片;
		private HBoxContainer 奖励物品槽位HBox;
		private Array<槽位脚本> 奖励物品槽位池 = new();

		public Button 追踪按钮;
		public Button 搁置按钮;

		public Node2D 任务描述节点;
		private Label 任务描述标签;

		public 内部数据类() { }

		public 内部数据类(Label 任务名标签形参, HBoxContainer 所需物品槽位HBox形参, TextureRect 红箭头图片形参, HBoxContainer 奖励物品槽位HBox形参, Button 追踪按钮形参, Button 搁置按钮形参, Node2D 任务描述节点形参, Label 任务描述标签形参)
		{
			任务名标签 = 任务名标签形参;
			所需物品槽位HBox = 所需物品槽位HBox形参;
			红箭头图片 = 红箭头图片形参;
			奖励物品槽位HBox = 奖励物品槽位HBox形参;

			追踪按钮 = 追踪按钮形参;
			搁置按钮 = 搁置按钮形参;

			任务描述节点 = 任务描述节点形参;
			任务描述标签 = 任务描述标签形参;
		}

		public void 更新任务卡的方法(任务数据类 任务, Dictionary<string, 物品数据类> 物品映射字典)
		{
			更新物品槽位的封装体方法(任务.寻找所需物品的方法(物品映射字典), 所需物品槽位池, 所需物品槽位HBox);
			更新物品槽位的封装体方法(任务.寻找奖励物品的方法(物品映射字典), 奖励物品槽位池, 奖励物品槽位HBox);

            任务地点 = 任务.任务地点;
            任务名标签.Text = 任务.任务名;
            任务描述标签.Text = 任务.任务描述;

			if(任务.所需物品名字典.Count <= 0 || 任务.奖励物品名字典.Count <= 0)
			{
                红箭头图片.Hide();
            }
            else
			{
				红箭头图片.Show();
			}
        }

		private void 更新物品槽位的封装体方法(Dictionary<物品数据类, int> dict, Array<槽位脚本> arr, HBoxContainer hbox)
		{
			int i = 0;

			foreach (var (key, value) in dict)
			{
				if (i < arr.Count)
				{
					arr[i].槽位内部类实例.设置物品信息的方法(key.加载物品纹理的方法(), value, key.物品描述);
					arr[i].Show();
				}
				else
				{
					var 槽位 = 槽位预制体.Instantiate() as 槽位脚本;
                    arr.Add(槽位);
					hbox.AddChild(槽位);
					槽位.槽位内部类实例.禁用槽位的方法(槽位);

                    槽位.槽位内部类实例.设置物品信息的方法(key.加载物品纹理的方法(), value, key.物品描述);
                }

				i++;
			}

			for (int left = i; left < arr.Count; left++)
			{
				arr[left].Hide();
			}
		}
	}

	public 内部数据类 内部数据;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		内部数据 = new(GetNode<Label>("%任务名标签"), GetNode<HBoxContainer>("%所需物品槽位HBox"), GetNode<TextureRect>("%红箭头图片"), GetNode<HBoxContainer>("%奖励物品槽位HBox"),
			GetNode<Button>("%追踪按钮"), GetNode<Button>("%搁置按钮"),
			GetNode<Node2D>("%任务描述节点"), GetNode<Label>("%任务描述标签")
			);

		MouseEntered += 内部数据.任务描述节点.Show;
		MouseExited += 内部数据.任务描述节点.Hide;
	}
}
