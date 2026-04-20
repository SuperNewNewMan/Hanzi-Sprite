using Godot;
using Godot.Collections;
using System;
using static 可交互体封装类脚本;

public partial class 场景参数类脚本 : Node
{
    public enum 场景名Enum
    {
        // 标题场景模块
        开场引言界面,
        开始界面,


        // 主要场景模块
        战斗界面,


        // 地图场景模块
        // 文心府地图模块
        政务中心,
    }

    public partial class NPC参数类 : RefCounted
    {
        public PackedScene 场景预制体;
        public Vector2I 位置;

        public NPC参数类() { }

        public NPC参数类(PackedScene 场景预制体形参, Vector2I 位置形参)
        {
            场景预制体 = 场景预制体形参;
            位置 = 位置形参;
        }
    }

    public partial class 场景区块类 : RefCounted
    {
        public 场景名Enum 场景名;
        public string 场景文件路径;
        public PackedScene 场景预制体;

        public string 营业开始时间字符串;
        public string 营业结束时间字符串;
        public TimeOnly 营业开始时间 => TimeOnly.Parse(营业开始时间字符串);
        public TimeOnly 营业结束时间 => TimeOnly.Parse(营业结束时间字符串);
        public GradientTexture1D 昼夜渐变颜色纹理 = GD.Load<GradientTexture1D>($"res://资源文件夹/颜色纹理Tres资源文件夹/昼夜渐变颜色纹理.tres");

        public Dictionary<string, NPC参数类> NPC预制体字典 = new();

        public Dictionary<Vector2I, string> 可交互体的触发瓦片坐标字典 = new();
        public Dictionary<string, 可交互体方法参数类> 可交互体的触发方法字典 = new();

        public 场景区块类() { }

        public 场景区块类(场景名Enum 场景名形参, string 场景文件路径形参, string 开始时间形参, string 结束时间形参,
            Dictionary<string, NPC参数类> NPC预制体字典形参,
            Dictionary<string, Array<Rect2I>> 可交互体触发范围字典形参, Dictionary<string, 可交互体方法参数类> 可交互体的触发方法字典形参)
        {
            场景名 = 场景名形参;
            场景文件路径 = 场景文件路径形参;
            场景预制体 = GD.Load<PackedScene>(场景文件路径);

            营业开始时间字符串 = 开始时间形参;
            营业结束时间字符串 = 结束时间形参;

            NPC预制体字典 = NPC预制体字典形参;

            foreach (var (key, arr) in 可交互体触发范围字典形参)
            {
                foreach (var value in arr)
                {
                    for (int x轴增量 = 0; x轴增量 <= value.Size.X; x轴增量++)
                    {
                        for (int y轴增量 = 0; y轴增量 <= value.Size.Y; y轴增量++)
                        {
                            Vector2I 瓦片坐标 = new(value.Position.X + x轴增量, value.Position.Y + y轴增量);
                            可交互体的触发瓦片坐标字典[瓦片坐标] = key;
                        }
                    }
                }
            }
            可交互体的触发方法字典 = 可交互体的触发方法字典形参;
        }

        #region 内部方法
        public bool 营业中(TimeOnly 本地时刻)
        {
            // 正常区间
            if (营业开始时间 <= 营业结束时间)
            {
                return 本地时刻 >= 营业开始时间 && 本地时刻 <= 营业结束时间;
            }
            // 跨夜区间
            else
            {
                return 本地时刻 >= 营业开始时间 || 本地时刻 <= 营业结束时间;
            }
        }

        public void 昼夜调节的方法(CanvasModulate 昼夜调节器)
        {
            Dictionary dict = Time.GetTimeDictFromSystem();
            TimeOnly 本地时刻 = new((int)dict["hour"], (int)dict["minute"]);

            if (营业中(本地时刻))
            {
                // 开始营业，关闭昼夜系统
                昼夜调节器.Hide();
            }
            else
            {
                // 关闭营业，开启昼夜系统
                float 昼夜颜色偏移量 = (float)(本地时刻.ToTimeSpan().TotalMinutes / 1440);

                昼夜调节器.Show();
                昼夜调节器.Color = 昼夜渐变颜色纹理.Gradient.Sample(昼夜颜色偏移量);
            }
        }
        #endregion
    }

    #region 序列化/反序列化
    public static Array<场景区块类> 序列化场景预制体Json的方法(string Json文件路径)
    {
        var 文件 = FileAccess.Open(Json文件路径, FileAccess.ModeFlags.Read);
        string 文本 = 文件.GetAsText();
        文件.Close();

        var json = Json.ParseString(文本).AsGodotArray();

        Array<场景区块类> 列表 = new();

        foreach (var item in json)
        {
            Dictionary dict = item.AsGodotDictionary();

            // 场景本体模块
            var 场景名 = Enum.Parse<场景名Enum>((string)dict["场景名"]);
            var 场景文件路径 = (string)dict["场景文件路径"];
            var 营业开始时间字符串 = (string)dict["营业开始时间字符串"];
            var 营业结束时间字符串 = (string)dict["营业结束时间字符串"];

            // NPC预制体模块
            var NPC预制体字典Json = (Dictionary<string, Dictionary>)dict["NPC预制体字典"];
            var NPC预制体字典 = new Dictionary<string, NPC参数类>();
            foreach (var (key, value) in NPC预制体字典Json)
            {
                var 位置Json = (Dictionary)value["位置"];
                var 参数类实例 = new NPC参数类(
                    GD.Load<PackedScene>(value["场景预制体路径"].ToString()),
                    new((int)位置Json["x"], (int)位置Json["y"])
                    );
                NPC预制体字典[key] = 参数类实例;
            }

            // 触发范围模块
            var 可交互体触发范围字典Json = (Dictionary)dict["可交互体触发范围字典"];
            var 可交互体触发范围字典 = new Dictionary<string, Array<Rect2I>>();
            foreach (string key in 可交互体触发范围字典Json.Keys)
            {
                var value = (Array<Dictionary>)可交互体触发范围字典Json[key];
                可交互体触发范围字典[key] = new();

                // 如果存在范围的数据，则添加到字典中
                if (value != null)
                {
                    foreach (var rect_dict in value)
                    {
                        var rect = new Rect2I((int)rect_dict["x"], (int)rect_dict["y"], (int)rect_dict["w"], (int)rect_dict["h"]);
                        可交互体触发范围字典[key].Add(rect);
                    }
                }
            }

            // 触发方法模块
            var 触发方法字典Json = (Dictionary<string, Dictionary>)dict["可交互体的触发方法字典"];

            var 可交互体的触发方法字典 = new Dictionary<string, 可交互体方法参数类>();

            foreach (var (可交互体名, value) in 触发方法字典Json)
            {
                可交互体方法参数类 参数类实例 = new(
                    (Dictionary<string, Dictionary<string, Variant>>)value["入口事件字典"],
                    (Dictionary<string, Dictionary<string, Variant>>)value["流程细则字典"],
                    (Dictionary<string, Dictionary<string, Variant>>)value["继发细则字典"]);

                可交互体的触发方法字典[可交互体名] = 参数类实例;
            }

            var 实例 = new 场景区块类(场景名, 场景文件路径, 营业开始时间字符串, 营业结束时间字符串,
                NPC预制体字典,
                可交互体触发范围字典, 可交互体的触发方法字典);
            列表.Add(实例);
        }

        return 列表;
    }

    public static Array<场景区块类> 加载文件夹下所有场景预制体Json的方法(string 场景预制体Json根目录)
    {
        Array<场景区块类> 总列表 = new();

        DirAccess dir = DirAccess.Open(场景预制体Json根目录);
        if (dir == null)
        {
            GD.PrintErr($"无法打开文件夹: {场景预制体Json根目录}");
            return 总列表;
        }

        foreach (string 文件名 in dir.GetFiles())
        {
            // 只处理 .json 或 .txt
            if (!文件名.EndsWith(".json") && !文件名.EndsWith(".txt"))
                continue;

            string 完整路径 = 场景预制体Json根目录.TrimEnd('/') + "/" + 文件名;

            Array<场景区块类> 单文件列表 = 序列化场景预制体Json的方法(完整路径);

            // 合并到总列表
            foreach (var 项 in 单文件列表)
                总列表.Add(项);
        }

        return 总列表;
    }
    #endregion
}
