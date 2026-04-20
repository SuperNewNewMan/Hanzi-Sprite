using Godot;
using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public static class 可交互体的通用方法封装类
{
    public static T 智能获取Node的方法<T>(this Node node, string name) where T : Node
    {
        var result = node.GetNodeOrNull<T>($"/root/{name}")
            ?? node.GetNodeOrNull<T>(name)
            ?? node.GetNodeOrNull<T>($"%{name}")
            ?? node.GetTree().CurrentScene.GetNodeOrNull<T>($"%{name}")
            ?? node.GetTree().CurrentScene.GetNodeOrNull<T>($"{name}");

        return result;
    }
}

public partial class 可交互体封装类脚本 : Node
{
    public const string 分割字符 = "_";

    public partial class 可交互体方法参数类 : RefCounted
    {
        private Dictionary<string, bool> 装置启用字典 = new();

        public readonly Godot.Collections.Dictionary<string, Godot.Collections.Dictionary<string, Variant>> 入口事件字典 = new();
        public readonly Godot.Collections.Dictionary<string, Godot.Collections.Dictionary<string, Variant>> 流程细则字典 = new();
        public readonly Godot.Collections.Dictionary<string, Godot.Collections.Dictionary<string, Variant>> 继发细则字典 = new();

        public 可交互体方法参数类() { }

        public 可交互体方法参数类(
            Godot.Collections.Dictionary<string, Godot.Collections.Dictionary<string, Variant>> 入口事件字典形参,
            Godot.Collections.Dictionary<string, Godot.Collections.Dictionary<string, Variant>> 流程细则字典形参,
            Godot.Collections.Dictionary<string, Godot.Collections.Dictionary<string, Variant>> 继发细则字典形参)
        {
            入口事件字典 = 入口事件字典形参;
            流程细则字典 = 流程细则字典形参;
            继发细则字典 = 继发细则字典形参;
        }

        public void 初始化装置启用字典的方法(string 装置名)
        {
            if (!装置启用字典.ContainsKey(装置名))
            {
                装置启用字典[装置名] = false;
            }
        }

        public bool 允许使用所有装置()
        {
            int count = 0;

            foreach (var value in 装置启用字典.Values)
            {
                count += (value ? 1 : 0);
            }

            return (count >= 装置启用字典.Count) ? true : false;
        }

        public bool 有该装置(string 装置名) => 装置启用字典.ContainsKey(装置名);
        public bool 获取装置是否允许启用的方法(string 装置名) => 装置启用字典[装置名];

        public void 更新装置是否允许启用的方法(string 装置名, bool 允许启用装置形参)
        {
            装置启用字典[装置名] = 允许启用装置形参;
        }
    }


    [AttributeUsage(AttributeTargets.Method)]
    public class 可交互体事件Attribute类 : Attribute
    {
        public string 方法名 { get; }

        public 可交互体事件Attribute类(string 方法名形参)
        {
            方法名 = 方法名形参;
        }
    }

    #region 映射层
    public readonly static Dictionary<string, MethodInfo> 可交互体事件的映射字典 = 获取可交互体事件的方法();

    private static Dictionary<string, MethodInfo> 获取可交互体事件的方法()
    {
        Dictionary<string, MethodInfo> dict = new();

        // 获取本脚本的所有方法
        var 所有方法 = typeof(可交互体封装类脚本).GetMethods(
            BindingFlags.Public |
            BindingFlags.NonPublic |
            BindingFlags.Static |
            BindingFlags.Instance
        );

        foreach (var method in 所有方法)
        {
            // 查找所有属于“可交互体事件Attribute类”的方法
            var attrs = method.GetCustomAttributes(typeof(可交互体事件Attribute类), false);

            foreach (可交互体事件Attribute类 attr in attrs)
            {
                dict[attr.方法名] = method;
            }
        }

        return dict;
    }
    #endregion


    #region 调用
    public static Task 执行可交互体事件的方法(Node 调用该方法的脚本, string 方法名, Godot.Collections.Dictionary<string, Variant> 参数字典, 可交互体方法参数类 方法实例, Dictionary<string, object> 附加数据)
    {
        if (!可交互体事件的映射字典.TryGetValue(方法名, out MethodInfo method))
        {
            GD.PrintErr($"{typeof(可交互体封装类脚本)} 未找到方法：{方法名}");
        }

        var result = method.Invoke(null, [调用该方法的脚本, 方法名, 参数字典, 方法实例, 附加数据]);

        // 如果方法返回 Task，则等待它
        if (result is Task task)
            return task;

        return Task.CompletedTask;
    }

    #endregion

    #region 入口方法
    [可交互体事件Attribute类("启用NPC的入口方法")]
    private static async void 启用NPC的入口方法(Node 调用该方法的脚本, string 方法名, Godot.Collections.Dictionary<string, Variant> 参数字典, 可交互体方法参数类 方法实例, Dictionary<string, object> 附加数据)
    {
        var 入口 = 方法实例.入口事件字典[方法名];

        string 临时的方法名 = string.Empty;
        Dictionary<string, object> 临时的附加数据 = new();

        Func<string, Task> 调用func = (string 事件名) =>
        {
            if (入口.ContainsKey(事件名))
            {
                临时的方法名 = 事件名;
                return 执行可交互体事件的方法(调用该方法的脚本, 临时的方法名, 方法实例.流程细则字典[临时的方法名], 方法实例, 临时的附加数据);
            }

            return Task.CompletedTask;
        };

        // 使玩家处于交互状态的方法
        _ = 调用func("使玩家处于交互状态的方法");
        // 注册装置的方法
        _ = 调用func("注册装置的方法");

        // 启用剧情的方法
        await 调用func("启用剧情的方法");

        if (方法实例.允许使用所有装置())
        {
            if (入口.ContainsKey("启用面板的方法"))
            {
                // 启用面板的方法
                await 调用func("启用面板的方法");
            }
        }

        // 使玩家结束交互状态的方法
        _ = 调用func("使玩家结束交互状态的方法");
    }

    [可交互体事件Attribute类("启用面板型可交互体的入口方法")]
    private static async void 启用面板型可交互体的入口方法(Node 调用该方法的脚本, string 方法名, Godot.Collections.Dictionary<string, Variant> 参数字典, 可交互体方法参数类 方法实例, Dictionary<string, object> 附加数据)
    {
        var 入口 = 方法实例.入口事件字典[方法名];

        string 临时的方法名 = string.Empty;
        Dictionary<string, object> 临时的附加数据 = new();

        Func<string, Task> 调用func = (string 事件名) =>
        {
            if (入口.ContainsKey(事件名))
            {
                临时的方法名 = 事件名;
                return 执行可交互体事件的方法(调用该方法的脚本, 临时的方法名, 方法实例.流程细则字典[临时的方法名], 方法实例, 临时的附加数据);
            }

            return Task.CompletedTask;
        };

        // 使玩家处于交互状态的方法
        _ = 调用func("使玩家处于交互状态的方法");

        // 注册装置的方法
        _ = 调用func("注册装置的方法");
        // 播放动画的方法
        await 调用func("播放动画的方法");
        // 启用剧情的方法
        _ = 调用func("启用剧情的方法");

        // 启用面板的方法
        await 调用func("启用面板的方法");
        // 倒放动画的方法
        await 调用func("倒放动画的方法");
        // 使玩家结束交互状态的方法
        _ = 调用func("使玩家结束交互状态的方法");
    }

    [可交互体事件Attribute类("启用一般装置型交互体的入口方法")]
    private static async void 启用一般装置型交互体的入口方法(Node 调用该方法的脚本, string 方法名, Godot.Collections.Dictionary<string, Variant> 参数字典, 可交互体方法参数类 方法实例, Dictionary<string, object> 附加数据)
    {
        var 入口 = 方法实例.入口事件字典[方法名];

        string 临时的方法名 = string.Empty;
        Dictionary<string, object> 临时的附加数据 = new();

        Func<string, Task> 调用func = (string 事件名) =>
        {
            if (入口.ContainsKey(事件名))
            {
                临时的方法名 = 事件名;
                return 执行可交互体事件的方法(调用该方法的脚本, 临时的方法名, 方法实例.流程细则字典[临时的方法名], 方法实例, 临时的附加数据);
            }

            return Task.CompletedTask;
        };

        // 注册装置的方法
        _ = 调用func("注册装置的方法");

        // 使玩家处于交互状态的方法
        _ = 调用func("使玩家处于交互状态的方法");

        if (!方法实例.允许使用所有装置())
        {
            // 启用剧情的方法
            await 调用func("启用剧情的方法");

            // 播放启用装置动画的方法
            await 调用func("播放启用装置动画的方法");
            // 播放字灵显圣动画的方法
            _ = 调用func("播放字灵显圣动画的方法");
            // 获得字灵的方法
            _ = 调用func("获得字灵的方法");

            if (方法实例.允许使用所有装置())
            {
                // 监测区域信号绑定的方法
                _ = 调用func("监测区域信号绑定的方法");
            }
        }

        // 使玩家结束交互状态的方法
        _ = 调用func("使玩家结束交互状态的方法");
    }

    [可交互体事件Attribute类("启用炮塔型交互体的入口方法")]
    private static async void 启用炮塔型交互体入口的方法(Node 调用该方法的脚本, string 方法名, Godot.Collections.Dictionary<string, Variant> 参数字典, 可交互体方法参数类 方法实例, Dictionary<string, object> 附加数据)
    {
        var 入口 = 方法实例.入口事件字典[方法名];

        string 临时的方法名 = string.Empty;
        Dictionary<string, object> 临时的附加数据 = new()
        {
            { "退出交互体", false },
            { "当前角速度", 0f },
            { "当前冷却时间", 0f }
        };

        Func<string, Task> 调用func = (string 事件名) =>
        {
            if (入口.ContainsKey(事件名))
            {
                临时的方法名 = 事件名;
                return 执行可交互体事件的方法(调用该方法的脚本, 临时的方法名, 方法实例.流程细则字典[临时的方法名], 方法实例, 临时的附加数据);
            }

            return Task.CompletedTask;
        };

        // 注册装置的方法
        _ = 调用func("注册装置的方法");

        // 使玩家处于交互状态的方法
        _ = 调用func("使玩家处于交互状态的方法");

        if (!方法实例.允许使用所有装置())
        {
            // 启用剧情的方法
            await 调用func("启用剧情的方法");
            // 播放启用装置动画的方法
            await 调用func("播放启用装置动画的方法");
            // 播放字灵显圣动画的方法
            await 调用func("播放字灵显圣动画的方法");
            // 获得字灵的方法
            _ = 调用func("获得字灵的方法");
        }


        if (方法实例.允许使用所有装置())
        {
            // 手动控制相机的方法
            _ = 调用func("手动控制相机的方法");
            // 启用退出非面板型交互体的方法
            _ = 调用func("启用退出非面板型交互体的方法");

            // 循环部分
            while ((bool)临时的附加数据["退出交互体"] == false)
            {
                // 机械跟随鼠标旋转的方法
                _ = 调用func("机械跟随鼠标旋转的方法");
                // 生成的方法
                await 调用func("生成的方法");
                // 发射子弹的方法
                await 调用func("发射子弹的方法");

                await 调用该方法的脚本.ToSignal(调用该方法的脚本.GetTree(), SceneTree.SignalName.ProcessFrame);
            }
        }

        // 完成任务的方法
        _ = 调用func("完成任务的方法");
        // 使玩家结束交互状态的方法
        _ = 调用func("使玩家结束交互状态的方法");

        // 清除子弹
        if (临时的附加数据.ContainsKey("子弹") && 临时的附加数据["子弹"] != null)
        {
            var 子弹 = (Node)临时的附加数据["子弹"];
            子弹.QueueFree();
        }
    }
    #endregion


    #region 工具方法
    [可交互体事件Attribute类("使玩家处于交互状态的方法")]
    private static void 使玩家处于交互状态的方法(Node 调用该方法的脚本, string 方法名, Godot.Collections.Dictionary<string, Variant> 参数字典, 可交互体方法参数类 方法实例, Dictionary<string, object> 附加数据)
    {
        玩家数据单例脚本 脚本实例 = 调用该方法的脚本.GetNode<玩家数据单例脚本>("/root/玩家数据单例脚本");
        脚本实例.玩家游戏数据.正在交互 = true;

        Node 玩家总节点 = 调用该方法的脚本.智能获取Node的方法<Node>("玩家总节点");
        玩家移动脚本 本地玩家 = 玩家总节点.GetNode<玩家移动脚本>($"{调用该方法的脚本.Multiplayer.GetUniqueId()}");
        本地玩家.移速增幅倍率 = 0;
    }

    [可交互体事件Attribute类("使玩家结束交互状态的方法")]
    private static void 使玩家结束交互状态的方法(Node 调用该方法的脚本, string 方法名, Godot.Collections.Dictionary<string, Variant> 参数字典, 可交互体方法参数类 方法实例, Dictionary<string, object> 附加数据)
    {
        玩家数据单例脚本 脚本实例 = 调用该方法的脚本.GetNode<玩家数据单例脚本>("/root/玩家数据单例脚本");
        脚本实例.玩家游戏数据.重置交互数据的方法();
    }

    [可交互体事件Attribute类("注册装置的方法")]
    private static void 注册装置的方法(Node 调用该方法的脚本, string 方法名, Godot.Collections.Dictionary<string, Variant> 参数字典, 可交互体方法参数类 方法实例, Dictionary<string, object> 附加数据)
    {
        var 装置列表 = (Godot.Collections.Array<string>)参数字典["装置名列表"];
        foreach (var 装置名 in 装置列表)
        {
            方法实例.初始化装置启用字典的方法(装置名);
        }
    }

    [可交互体事件Attribute类("启用剧情的方法")]
    private static async Task 启用剧情的方法(Node 调用该方法的脚本, string 方法名, Godot.Collections.Dictionary<string, Variant> 参数字典, 可交互体方法参数类 方法实例, Dictionary<string, object> 附加数据)
    {
        if(参数字典.ContainsKey("单次触发"))
        {
            if((bool)参数字典["单次触发"])
            {
                return;
            }
            else
            {
                参数字典["单次触发"] = true;
            }
        }

        剧情演绎单例脚本 脚本实例 = 调用该方法的脚本.GetNode<剧情演绎单例脚本>("/root/剧情演绎单例脚本");
        string 标题名 = 参数字典.ContainsKey("标题名") ? $"{参数字典["标题名"]}" : string.Empty;
        await 脚本实例.播放剧情的方法($"{参数字典["一级剧情名"]}", $"{参数字典["二级剧情名"]}", 标题名, 方法实例);
    }

    [可交互体事件Attribute类("播放动画的方法")]
    private static async Task 播放动画的方法(Node 调用该方法的脚本, string 方法名, Godot.Collections.Dictionary<string, Variant> 参数字典, 可交互体方法参数类 方法实例, Dictionary<string, object> 附加数据)
    {
        if(参数字典.ContainsKey("单次触发"))
        {
            if ((bool)参数字典["单次触发"])
            {
                return;
            }
            else
            {
                参数字典["单次触发"] = true;
            }
        }

        var 持有动画机的节点 = 调用该方法的脚本.智能获取Node的方法<Node>($"{参数字典["持有动画机的节点名"]}");
        var 动画机 = 持有动画机的节点.智能获取Node的方法<AnimationPlayer>($"{参数字典["动画机名"]}");
        var 动画名 = (string)参数字典["动画名"];

        if (动画机 == null)
        {
            GD.PrintErr($"{typeof(可交互体封装类脚本)} 获取节点失败：{参数字典["动画机名"]} 不存在");
            return;
        }
        if (!动画机.HasAnimation(动画名))
        {
            GD.PrintErr($"{typeof(可交互体封装类脚本)} 播放动画失败：{参数字典["动画名"]} 不存在动画 {动画名}");
            return;
        }

        if (!动画机.IsPlaying())
        {
            动画机.Play(动画名);
        }

        await 动画机.ToSignal(动画机, "animation_finished");
    }
    [可交互体事件Attribute类("倒放动画的方法")]
    private static async Task 倒放动画的方法(Node 调用该方法的脚本, string 方法名, Godot.Collections.Dictionary<string, Variant> 参数字典, 可交互体方法参数类 方法实例, Dictionary<string, object> 附加数据)
    {
        if (参数字典.ContainsKey("单次触发"))
        {
            if ((bool)参数字典["单次触发"])
            {
                return;
            }
            else
            {
                参数字典["单次触发"] = true;
            }
        }

        var 持有动画机的节点 = 调用该方法的脚本.智能获取Node的方法<Node>($"{参数字典["持有动画机的节点名"]}");
        var 动画机 = 持有动画机的节点.智能获取Node的方法<AnimationPlayer>($"{参数字典["动画机名"]}");
        var 动画名 = (string)参数字典["动画名"];

        if (动画机 == null)
        {
            GD.PrintErr($"{typeof(可交互体封装类脚本)} 获取节点失败：{参数字典["动画机名"]} 不存在");
            return;
        }
        if (!动画机.HasAnimation(动画名))
        {
            GD.PrintErr($"{typeof(可交互体封装类脚本)} 播放动画失败：{参数字典["动画名"]} 不存在动画 {动画名}");
            return;
        }

        if (!动画机.IsPlaying())
        {
            动画机.PlayBackwards(动画名);
        }

        await 动画机.ToSignal(动画机, "animation_finished");
    }

    [可交互体事件Attribute类("播放启用装置动画的方法")]
    private static async Task 播放启用装置动画的方法(Node 调用该方法的脚本, string 方法名, Godot.Collections.Dictionary<string, Variant> 参数字典, 可交互体方法参数类 方法实例, Dictionary<string, object> 附加数据)
    {
        var 持有动画机的节点 = 调用该方法的脚本.智能获取Node的方法<Node>($"{参数字典["持有动画机的节点名"]}");
        AnimationPlayer 动画机 = 持有动画机的节点.智能获取Node的方法<AnimationPlayer>($"{参数字典["动画机名"]}");

        var 动画名字典 = (Godot.Collections.Dictionary<string, Godot.Collections.Dictionary<string, bool>>)参数字典["动画名字典"];
        foreach (var (装置名, 详细字典) in 动画名字典)
        {
            if (方法实例.获取装置是否允许启用的方法(装置名))
            {
                foreach (var (动画名, 已禁止) in 详细字典)
                {
                    if (!已禁止)
                    {
                        if (动画机 == null)
                        {
                            GD.PrintErr($"{typeof(可交互体封装类脚本)} 获取节点失败：{参数字典["动画机名"]} 不存在");
                            return;
                        }
                        if (!动画机.HasAnimation(动画名))
                        {
                            GD.PrintErr($"{typeof(可交互体封装类脚本)} 播放动画失败：{参数字典["动画名"]} 不存在动画 {动画名}");
                            return;
                        }

                        if (!动画机.IsPlaying())
                        {
                            动画机.Play(动画名);
                        }

                        await 动画机.ToSignal(动画机, "animation_finished");

                        详细字典[动画名] = true;
                    }
                }
            }
        }
    }

    [可交互体事件Attribute类("播放字灵显圣动画的方法")]
    private static async Task 播放字灵显圣动画的方法(Node 调用该方法的脚本, string 方法名, Godot.Collections.Dictionary<string, Variant> 参数字典, 可交互体方法参数类 方法实例, Dictionary<string, object> 附加数据)
    {
        var 持有动画机的节点 = 调用该方法的脚本.智能获取Node的方法<Node>($"{参数字典["持有动画机的节点名"]}");
        AnimationPlayer 动画机 = 持有动画机的节点.智能获取Node的方法<AnimationPlayer>($"{参数字典["动画机名"]}");

        var 动画名字典 = (Godot.Collections.Dictionary<string, Godot.Collections.Dictionary<string, bool>>)参数字典["动画名字典"];
        foreach (var (装置名, 详细字典) in 动画名字典)
        {
            if (方法实例.获取装置是否允许启用的方法(装置名))
            {
                foreach (var (动画名, 已禁止) in 详细字典)
                {
                    if (!已禁止)
                    {
                        if (动画机 == null)
                        {
                            GD.PrintErr($"{typeof(可交互体封装类脚本)} 获取节点失败：{参数字典["动画机名"]} 不存在");
                            return;
                        }
                        if (!动画机.HasAnimation(动画名))
                        {
                            GD.PrintErr($"{typeof(可交互体封装类脚本)} 播放动画失败：{参数字典["动画名"]} 不存在动画 {动画名}");
                            return;
                        }

                        if (!动画机.IsPlaying())
                        {
                            动画机.Play(动画名);
                        }

                        await 动画机.ToSignal(动画机, "animation_finished");

                        详细字典[动画名] = true;
                    }
                }
            }
        }
    }

    [可交互体事件Attribute类("启用面板的方法")]
    private static async Task 启用面板的方法(Node 调用该方法的脚本, string 方法名, Godot.Collections.Dictionary<string, Variant> 参数字典, 可交互体方法参数类 方法实例, Dictionary<string, object> 附加数据)
    {
        var 挂载该面板的节点 = 调用该方法的脚本.智能获取Node的方法<Node>($"{参数字典["挂载该面板的节点名"]}");
        var 面板 = 挂载该面板的节点.智能获取Node的方法<CanvasLayer>($"{参数字典["面板的名称"]}");
        if (面板 != null)
        {
            面板.Show();
        }
        else
        {
            面板 = GD.Load<PackedScene>($"{参数字典["面板的场景路径"]}").Instantiate() as CanvasLayer;
            面板.Name = $"{参数字典["面板的名称"]}";
            挂载该面板的节点.AddChild(面板);
        }      

        Button 关闭面板按钮 = 面板.智能获取Node的方法<Button>($"{参数字典["关闭面板按钮的名称"]}");
        await 关闭面板按钮.ToSignal(关闭面板按钮, "pressed");
    }

    [可交互体事件Attribute类("手动控制相机的方法")]
    private static void 手动控制相机的方法(Node 调用该方法的脚本, string 方法名, Godot.Collections.Dictionary<string, Variant> 参数字典, 可交互体方法参数类 方法实例, Dictionary<string, object> 附加数据)
    {
        // 手动控制相机
        相机总节点脚本 相机脚本 = 调用该方法的脚本.智能获取Node的方法<相机总节点脚本>($"{参数字典["当前场景的相机总节点名"]}");
        相机脚本.内部数据.更新相机状态的方法(相机总节点脚本.相机状态Enum.手动控制);
    }

    [可交互体事件Attribute类("启用退出非面板型交互体的方法")]
    private static void 启用退出非面板型交互体的方法(Node 调用该方法的脚本, string 方法名, Godot.Collections.Dictionary<string, Variant> 参数字典, 可交互体方法参数类 方法实例, Dictionary<string, object> 附加数据)
    {
        var 挂载该场景的节点 = 调用该方法的脚本.智能获取Node的方法<Node>($"{参数字典["挂载该场景的节点名"]}");
        var 画布层 = 挂载该场景的节点.智能获取Node的方法<CanvasLayer>($"{参数字典["退出非面板型交互体画布层的名称"]}");
        if (画布层 != null)
        {
            画布层.Show();
        }
        else
        {
            画布层 = GD.Load<PackedScene>($"{参数字典["退出非面板型交互体画布层的场景路径"]}").Instantiate() as CanvasLayer;
            画布层.Name = $"{参数字典["退出非面板型交互体画布层的名称"]}";
            挂载该场景的节点.AddChild(画布层);
        }

        Button 退出非面板型交互体按钮 = 画布层.智能获取Node的方法<Button>($"{参数字典["退出非面板型交互体按钮的名称"]}");
        Action 退出炮塔Action = null;

        退出炮塔Action = () =>
        {
            相机总节点脚本 相机脚本 = 调用该方法的脚本.智能获取Node的方法<相机总节点脚本>($"{参数字典["当前场景的相机总节点名"]}");
            相机脚本.内部数据.更新相机状态的方法(相机总节点脚本.相机状态Enum.跟随玩家);

            画布层.Hide();
            附加数据["退出交互体"] = true;
            退出非面板型交互体按钮.Pressed -= 退出炮塔Action;
        };

        退出非面板型交互体按钮.Pressed += 退出炮塔Action;
    }

    [可交互体事件Attribute类("机械跟随鼠标旋转的方法")]
    private static void 机械跟随鼠标旋转的方法(Node 调用该方法的脚本, string 方法名, Godot.Collections.Dictionary<string, Variant> 参数字典, 可交互体方法参数类 方法实例, Dictionary<string, object> 附加数据)
    {
        Node2D 装置 = 调用该方法的脚本.智能获取Node的方法<Node2D>($"{参数字典["装置名"]}");

        // 1. 计算方向
        Vector2 dir = (装置.GetGlobalMousePosition() - 装置.GlobalPosition).Normalized();
        float 目标角度 = dir.Angle();
        float 当前角度 = 装置.Rotation;

        float 角度差 = Mathf.AngleDifference(当前角度, 目标角度);

        float 当前角速度 = (float)附加数据["当前角速度"];

        float 旋转加速度 = (float)参数字典["旋转加速度"];
        当前角速度 += 角度差 * 旋转加速度 * (float)调用该方法的脚本.GetProcessDeltaTime();

        float 最大角速度 = (float)参数字典["最大角速度"];
        当前角速度 = Mathf.Clamp(当前角速度, -最大角速度, 最大角速度);
        float 惯性阻尼 = (float)参数字典["惯性阻尼"];
        当前角速度 *= (1f - 惯性阻尼);
        // 更新角速度
        附加数据["当前角速度"] = 当前角速度;

        // 7. 更新旋转
        装置.Rotation += 当前角速度 * (float)调用该方法的脚本.GetProcessDeltaTime();
    }

    [可交互体事件Attribute类("生成的方法")]
    private static async Task 生成的方法(Node 调用该方法的脚本, string 方法名, Godot.Collections.Dictionary<string, Variant> 参数字典, 可交互体方法参数类 方法实例, Dictionary<string, object> 附加数据)
    {
        float 总冷却 = (float)参数字典["总冷却时间"];
        float 当前冷却 = (float)附加数据["当前冷却时间"];
        string 生成物的名称 = (string)参数字典["生成物的名称"];

        if (当前冷却 <= 0)
        {
            Node 挂载该生成器的节点 = 调用该方法的脚本.智能获取Node的方法<Node>($"{参数字典["挂载该生成器的节点名"]}");
            Node2D 生成器 = 挂载该生成器的节点.智能获取Node的方法<Node2D>($"{参数字典["生成器的名称"]}");

            PackedScene 预制体 = GD.Load<PackedScene>($"{参数字典["生成物的场景路径"]}");
            var 生成物 = 预制体.Instantiate();
            生成器.AddChild(生成物);

            附加数据[生成物的名称] = 生成物;
            附加数据["当前冷却时间"] = 总冷却;

            var 继发事件字典 = (Godot.Collections.Dictionary<string, string>)参数字典["继发事件"];
            string 临时的方法名 = string.Empty;

            foreach (var (原始方法名, value) in 继发事件字典)
            {
                var split = 原始方法名.Split(分割字符);
                临时的方法名 = split.Last();
                await 执行可交互体事件的方法(调用该方法的脚本, 临时的方法名, 方法实例.继发细则字典[原始方法名], 方法实例, 附加数据);
            }
        }

        if (附加数据[生成物的名称] == null)
        {
            当前冷却 -= (float)调用该方法的脚本.GetProcessDeltaTime();
            附加数据["当前冷却时间"] = 当前冷却;
        }
    }

    [可交互体事件Attribute类("发射子弹的方法")]
    private static async Task 发射子弹的方法(Node 调用该方法的脚本, string 方法名, Godot.Collections.Dictionary<string, Variant> 参数字典, 可交互体方法参数类 方法实例, Dictionary<string, object> 附加数据)
    {
        var 子弹 = (子弹脚本)附加数据["子弹"];
        if (子弹 != null && Input.IsActionJustPressed("点击"))
        {
            var 装置 = 调用该方法的脚本.智能获取Node的方法<Node2D>($"{参数字典["发射器的名称"]}");

            子弹.发射子弹的方法(装置.GlobalPosition, 装置.GlobalRotation, 装置.GlobalTransform.X.Normalized(), (float)参数字典["子弹速度"]);

            附加数据["子弹"] = null;

            var 继发事件字典 = (Godot.Collections.Dictionary<string, string>)参数字典["继发事件"];
            string 临时的方法名 = string.Empty;

            foreach (var (原始方法名, value) in 继发事件字典)
            {
                var split = 原始方法名.Split(分割字符);
                临时的方法名 = split.Last();
                await 执行可交互体事件的方法(调用该方法的脚本, 临时的方法名, 方法实例.继发细则字典[原始方法名], 方法实例, 附加数据);
            }
        }
    }

    [可交互体事件Attribute类("监测区域信号绑定的方法")]
    private static void 监测区域信号绑定的方法(Node 调用该方法的脚本, string 方法名, Godot.Collections.Dictionary<string, Variant> 参数字典, 可交互体方法参数类 方法实例, Dictionary<string, object> 附加数据)
    {
        var 持有监测范围的节点 = 调用该方法的脚本.智能获取Node的方法<Node>($"{参数字典["持有监测区域的节点名"]}");
        Area2D 监测范围 = 持有监测范围的节点.智能获取Node的方法<Area2D>($"{参数字典["监测区域的名称"]}");

        Area2D.AreaEnteredEventHandler 区域进入事件 = null;
        区域进入事件 = async (area) =>
        {
            if (area.IsInGroup($"{参数字典["被监测对象的所属组名"]}"))
            {
                var 继发事件字典 = (Godot.Collections.Dictionary<string, string>)参数字典["继发事件"];
                string 临时的方法名 = string.Empty;

                foreach (var (原始方法名, value) in 继发事件字典)
                {
                    var split = 原始方法名.Split(分割字符);
                    临时的方法名 = split.Last();
                    await 执行可交互体事件的方法(调用该方法的脚本, 临时的方法名, 方法实例.继发细则字典[原始方法名], 方法实例, 附加数据);
                }
            }
        };
        监测范围.AreaEntered += 区域进入事件;
    }

    [可交互体事件Attribute类("获得字灵的方法")]
    private static void 获得字灵的方法(Node 调用该方法的脚本, string 方法名, Godot.Collections.Dictionary<string, Variant> 参数字典, 可交互体方法参数类 方法实例, Dictionary<string, object> 附加数据)
    {
        玩家数据单例脚本 数据单例 = 调用该方法的脚本.GetNode<玩家数据单例脚本>("/root/玩家数据单例脚本");
        var 存档字灵字典 = 数据单例.玩家存档数据.字灵字典;

        Godot.Collections.Dictionary<string, Godot.Collections.Dictionary<string, bool>> 字灵字典 = (Godot.Collections.Dictionary<string, Godot.Collections.Dictionary<string, bool>>)参数字典["字灵字典"];

        foreach (var (装置名, dict) in 字灵字典)
        {
            if (!方法实例.获取装置是否允许启用的方法(装置名)) continue;

            foreach (var (字灵, 已获得) in dict)
            {
                if (已获得)
                {
                    continue;
                }

                if (!存档字灵字典.ContainsKey(字灵))
                {
                    存档字灵字典[字灵] = 1;
                }
                else
                {
                    存档字灵字典[字灵]++;
                }

                dict[字灵] = true;
            }            
        }

    }

    [可交互体事件Attribute类("外部触发装置的方法")]
    private static void 外部触发装置的方法(Node 调用该方法的脚本, string 方法名, Godot.Collections.Dictionary<string, Variant> 参数字典, 可交互体方法参数类 方法实例, Dictionary<string, object> 附加数据)
    {
        if (参数字典.ContainsKey("单次触发"))
        {
            if ((bool)参数字典["单次触发"])
            {
                return;
            }
            else
            {
                参数字典["单次触发"] = true;
            }
        }

        配置Json单例脚本 单例 = 调用该方法的脚本.GetNode<配置Json单例脚本>("/root/配置Json单例脚本");
        var data = 单例.场景预制体字典[Enum.Parse<场景参数类脚本.场景名Enum>($"{调用该方法的脚本.GetTree().CurrentScene.Name}")];
        var 实例 = data.可交互体的触发方法字典[(string)参数字典["被触发的可交互体名"]];

        实例.更新装置是否允许启用的方法((string)参数字典["内部装置名"], true);
    }
    #endregion
}
