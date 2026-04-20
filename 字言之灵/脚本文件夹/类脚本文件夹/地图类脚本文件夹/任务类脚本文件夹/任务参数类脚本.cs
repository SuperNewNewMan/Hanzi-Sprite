using Godot;
using Godot.Collections;
using System;
using static 物品参数类脚本;

public partial class 任务参数类脚本 : Node
{
	public partial class 任务数据类 : RefCounted
	{
		public string 任务名;
		public string 任务描述;
		public Dictionary<string, int> 所需物品名字典 = new();
		public Dictionary<string, int> 奖励物品名字典 = new();

		public Vector2I 任务地点;

		public 任务数据类() { }

		public 任务数据类(string 任务名形参, string 任务描述形参, Dictionary<string, int> 所需物品名字典形参, Dictionary<string, int> 奖励物品名字典形参, Vector2I 任务地点形参)
		{
			任务名 = 任务名形参;
			任务描述 = 任务描述形参;
			所需物品名字典 = 所需物品名字典形参;
			奖励物品名字典 = 奖励物品名字典形参;

			任务地点 = 任务地点形参;
		}

		public Dictionary<物品数据类, int> 寻找所需物品的方法(Dictionary<string, 物品数据类> 物品映射字典)
		{
			Dictionary<物品数据类, int> dict = new();
			foreach (var (key, value) in 所需物品名字典)
			{
				dict[物品映射字典[key]] = value;
			}

			return dict;
		}

		public Dictionary<物品数据类, int> 寻找奖励物品的方法(Dictionary<string, 物品数据类> 物品映射字典)
		{
			Dictionary<物品数据类, int> dict = new();
			foreach (var (key, value) in 奖励物品名字典)
			{
                dict[物品映射字典[key]] = value;
            }

			return dict;
		}
	}
}
