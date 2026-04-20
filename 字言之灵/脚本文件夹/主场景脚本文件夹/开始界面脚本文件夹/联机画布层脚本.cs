using Godot;
using System;
using System.Linq;
using System.Text;

public partial class 联机画布层脚本 : CanvasLayer
{
	[Signal]
	public delegate void 创建房间的信号EventHandler(int 最大玩家数, int 房间端口);
	[Signal]
	public delegate void 加入房间的信号EventHandler(string 房间IP地址, int 房间端口);

	private 联机单例脚本 联机单例脚本实例;

	private VBoxContainer 联机方式VBox;
	private Button 关闭联机界面按钮;
	private Button 作为房主按钮;
	private Button 作为客人按钮;

	private VBoxContainer 创建房间面板VBox;
	private Label 创建房间界面标题标签;
	public string 创建房间界面标题标签名;
	private const string 创建房间界面标题 = "创建房间";
	private Button 关闭创建房间界面按钮;
	private LineEdit 创建房间玩家数LE;
	private LineEdit 创建房间端口LE;
	private Button 创建房间按钮;

	private VBoxContainer 加入房间面板VBox;
	private Label 加入房间界面标题标签;
	public string 加入房间界面标题标签名;
	private const string 加入房间界面标题 = "加入房间";
	private Button 关闭加入房间界面按钮;
	private LineEdit 加入房间IP地址LE;
	private LineEdit 加入房间端口LE;
	private Button 加入房间按钮;

	private VBoxContainer 选择方式面板VBox;
	private Button 关闭选择方式界面按钮;
	private Button 创建角色按钮;
	private Button 加载存档按钮;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		联机单例脚本实例 = GetNode<联机单例脚本>("/root/联机单例脚本");
		联机单例脚本实例.Connect(nameof(联机单例脚本实例.联机失败的信号), new Callable(this, nameof(提示信息的方法)));

		联机单例脚本实例.Connect(nameof(联机单例脚本实例.联机成功的信号), new Callable(this, nameof(打开选择方式面板的回调函数)));

		Panel 联机界面 = GetNode<Panel>("联机界面");

		联机方式VBox = 联机界面.GetNode<VBoxContainer>("联机方式VBox");
		关闭联机界面按钮 = 联机方式VBox.GetNode<Button>("%关闭联机界面按钮");
		关闭联机界面按钮.Connect("pressed", new Callable(this, nameof(关闭联机界面的回调函数)));
		作为房主按钮 = 联机方式VBox.GetNode<Button>("作为房主按钮");
		作为房主按钮.Connect("pressed", new Callable(this, nameof(打开创建房间面板的回调函数)));
		作为客人按钮 = 联机方式VBox.GetNode<Button>("作为客人按钮");
		作为客人按钮.Connect("pressed", new Callable(this, nameof(打开加入房间面板的回调函数)));

		创建房间面板VBox = 联机界面.GetNode<VBoxContainer>("创建房间面板VBox");
		创建房间界面标题标签 = 创建房间面板VBox.GetNode<Label>("%创建房间界面标题标签");
		创建房间界面标题标签名 = 创建房间界面标题标签.Name;
		关闭创建房间界面按钮 = 创建房间面板VBox.GetNode<Button>("%关闭创建房间界面按钮");
		关闭创建房间界面按钮.Connect("pressed", new Callable(this, nameof(关闭创建房间面板的回调函数)));
		创建房间玩家数LE = 创建房间面板VBox.GetNode<LineEdit>("%创建房间玩家数LE");
		创建房间玩家数LE.TextChanged += (new_text) => 检测创建房间参数输入的回调函数(创建房间玩家数LE.Name);
		创建房间端口LE = 创建房间面板VBox.GetNode<LineEdit>("%创建房间端口LE");
		创建房间端口LE.TextChanged += (new_text) => 检测创建房间参数输入的回调函数(创建房间端口LE.Name);
		创建房间按钮 = 创建房间面板VBox.GetNode<Button>("创建房间按钮");
		创建房间按钮.Connect("pressed", new Callable(this, nameof(创建房间的回调函数)));

		加入房间面板VBox = 联机界面.GetNode<VBoxContainer>("加入房间面板VBox");
		加入房间界面标题标签 = 加入房间面板VBox.GetNode<Label>("%加入房间界面标题标签");
		加入房间界面标题标签名 = 加入房间界面标题标签.Name;
		关闭加入房间界面按钮 = 加入房间面板VBox.GetNode<Button>("%关闭加入房间界面按钮");
		关闭加入房间界面按钮.Connect("pressed", new Callable(this, nameof(关闭加入房间面板的回调函数)));
		加入房间IP地址LE = 加入房间面板VBox.GetNode<LineEdit>("%加入房间IP地址LE");
		加入房间IP地址LE.TextChanged += (new_text) => 检测加入房间参数输入的回调函数(加入房间IP地址LE.Name);
		加入房间端口LE = 加入房间面板VBox.GetNode<LineEdit>("%加入房间端口LE");
		加入房间端口LE.TextChanged += (new_text) => 检测加入房间参数输入的回调函数(加入房间端口LE.Name);
		加入房间按钮 = 加入房间面板VBox.GetNode<Button>("加入房间按钮");
		加入房间按钮.Connect("pressed", new Callable(this, nameof(加入房间的回调函数)));

		选择方式面板VBox = 联机界面.GetNode<VBoxContainer>("选择方式面板VBox");
		关闭选择方式界面按钮 = 选择方式面板VBox.GetNode<Button>("%关闭选择方式界面按钮");
		关闭选择方式界面按钮.Connect("pressed", new Callable(this, nameof(关闭选择方式面板的回调函数)));
		CanvasLayer 创建角色画布层 = GetTree().CurrentScene.GetNode<CanvasLayer>("创建角色画布层");
		CanvasLayer 存档画布层 = GetTree().CurrentScene.GetNode<CanvasLayer>("存档画布层");
		创建角色按钮 = 选择方式面板VBox.GetNode<Button>("创建角色按钮");
		创建角色按钮.Pressed += 创建角色画布层.Show;
		加载存档按钮 = 选择方式面板VBox.GetNode<Button>("加载存档按钮");
		加载存档按钮.Pressed += 存档画布层.Show;
	}

	private void 关闭联机界面的回调函数()
	{
		Hide();
	}

	// 两种操作面板的显示与隐藏
	private void 打开创建房间面板的回调函数()
	{
		联机方式VBox.Hide();
		创建房间面板VBox.Show();
	}

	private void 关闭创建房间面板的回调函数()
	{
		联机方式VBox.Show();
		创建房间面板VBox.Hide();
		创建房间界面标题标签.Text = 创建房间界面标题;
	}

	private void 打开加入房间面板的回调函数()
	{
		联机方式VBox.Hide();
		加入房间面板VBox.Show();
	}

	private void 关闭加入房间面板的回调函数()
	{
		联机方式VBox.Show();
		加入房间面板VBox.Hide();
		加入房间界面标题标签.Text = 加入房间界面标题;
	}

	// 联机成功后的回调函数
	private void 打开选择方式面板的回调函数()
	{
		创建房间面板VBox.Hide();
		加入房间面板VBox.Hide();

		选择方式面板VBox.Show();
	}

	private void 关闭选择方式面板的回调函数()
	{
		联机方式VBox.Show();
		选择方式面板VBox.Hide();

		联机单例脚本实例.重置网络数据的方法();
	}


	// 创建房间和加入房间的参数检测，以及发送信号
	private void 检测创建房间参数输入的回调函数(string LineEdit名)
	{
		switch (LineEdit名)
		{
			case "创建房间玩家数LE":
				if (!int.TryParse(创建房间玩家数LE.Text, out int 加入玩家数) || 加入玩家数 < 1 || 加入玩家数 > 8)
				{
					创建房间玩家数LE.Text = string.Empty;
				}
				break;
			case "创建房间端口LE":
				// 过滤掉非数字字符
				string 端口digitsOnly = new string(创建房间端口LE.Text.Where(char.IsDigit).ToArray());
				创建房间端口LE.Text = 端口digitsOnly;
				// 把光标移到最后
				创建房间端口LE.CaretColumn = 创建房间端口LE.Text.Length;
				break;
			default:
				GD.Print(GetType().Name + " 检测到未知的LineEdit！");
				break;
		}
	}

	private void 检测加入房间参数输入的回调函数(string LineEdit名)
	{
		switch (LineEdit名)
		{
			case "加入房间IP地址LE":
				// 只保留数字和点
				var filtered = new StringBuilder();
				foreach (var ch in 加入房间IP地址LE.Text)
				{
					if (char.IsDigit(ch) || ch == '.')
						filtered.Append(ch);
				}
				if (filtered.ToString() != 加入房间IP地址LE.Text)
				{
					加入房间IP地址LE.Text = filtered.ToString();
					加入房间IP地址LE.CaretColumn = 加入房间IP地址LE.Text.Length;
				}
				// 把光标移到最后
				加入房间端口LE.CaretColumn = 加入房间IP地址LE.Text.Length;
				break;
			case "加入房间端口LE":
				// 过滤掉非数字字符
				string 端口digitsOnly = new string(加入房间端口LE.Text.Where(char.IsDigit).ToArray());
				加入房间端口LE.Text = 端口digitsOnly;
				// 把光标移到最后
				加入房间端口LE.CaretColumn = 加入房间端口LE.Text.Length;
				break;
			default:
				GD.Print(GetType().Name + " 检测到未知的LineEdit！");
				break;
		}
	}

	private void 创建房间的回调函数()
	{
		if (创建房间玩家数LE.Text == string.Empty)
		{
			提示信息的方法(创建房间界面标题标签名, "：请输入玩家数！");
			return;
		}
		if (创建房间端口LE.Text == string.Empty || int.Parse(创建房间端口LE.Text) < 1024 || int.Parse(创建房间端口LE.Text) > 49151)
		{
			提示信息的方法(创建房间界面标题标签名, "：房间端口有误！");
			return;
		}

		int 最大玩家数 = int.Parse(创建房间玩家数LE.Text);
		int 房间端口 = int.Parse(创建房间端口LE.Text);

		提示信息的方法(创建房间界面标题标签名, "：正在创建房间...");
		EmitSignal(nameof(创建房间的信号), 最大玩家数, 房间端口);
	}

	private void 加入房间的回调函数()
	{
		if (加入房间端口LE.Text == string.Empty)
		{
			提示信息的方法(加入房间界面标题标签名, "：请输入房间端口！");
			return;
		}

		int 房间端口 = int.Parse(加入房间端口LE.Text);

		提示信息的方法(加入房间界面标题标签名, "：正在加入房间...");
		EmitSignal(nameof(加入房间的信号), 加入房间IP地址LE.Text, 房间端口);
	}

	// 显示提示信息
	private void 提示信息的方法(string 标签名, string 提示文本)
	{
		switch (标签名)
		{
			case "创建房间界面标题标签":
				创建房间界面标题标签.Text = $"{创建房间界面标题}：{提示文本}";
				break;
			case "加入房间界面标题标签":
				加入房间界面标题标签.Text = $"{加入房间界面标题}：{提示文本}";
				break;

			default:
				GD.Print(GetType().Name + " 显示提示信息时发生错误，不存在 " + 标签名 + " 该Label！");
				break;
		}
	}
}