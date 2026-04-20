using Godot;
using System;

public partial class 子弹脚本 : Node2D
{
	public class 内部数据类
	{
		private Timer 销毁计时器;

		public Vector2 dir = Vector2.Zero;
		public float 速度;

		public void 初始化销毁计时器的方法(Timer 计时器)
		{
			销毁计时器 = 计时器;
		}

		public Timer 获取销毁计时器的方法()
		{
			return 销毁计时器;
		}
	}
	public 内部数据类 数据 = new();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		数据.初始化销毁计时器的方法(GetNode<Timer>("销毁计时器"));
		数据.获取销毁计时器的方法().Connect("timeout", new Callable(this, nameof(销毁的回调函数)));
	}

	public override void _Process(double delta)
	{
		if (数据.dir != Vector2.Zero)
		{
			GlobalPosition += 数据.速度 * 数据.dir * (float)delta;
			启动销毁计时器的方法();
        }
	}

	private void 销毁的回调函数()
	{
		QueueFree();
	}

	public void 启动销毁计时器的方法()
	{
		if (数据.获取销毁计时器的方法().IsStopped())
		{
			数据.获取销毁计时器的方法().Start();
		}
	}

	public void 发射子弹的方法(Vector2 位置, float 角度, Vector2 方向, float 速度)
	{
        TopLevel = true;

        GlobalPosition = 位置;		
		Rotation = 角度;
        数据.dir = 方向;
        数据.速度 = 速度;
	}
}
