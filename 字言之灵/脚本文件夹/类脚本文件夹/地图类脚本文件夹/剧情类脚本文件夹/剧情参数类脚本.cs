using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.IO;
using static 任务参数类脚本;

public partial class 剧情参数类脚本 : Node
{
    public partial class 剧情数据类 : RefCounted
    {
        public string 总剧情名;
        public string 二级剧情名;
        public string 剧情资源路径;
        public Resource 剧情资源 = new();

        public Godot.Collections.Dictionary<string, 任务数据类> 任务字典 = new();

        private Godot.Collections.Dictionary<string, string> 角色肖像路径字典;
        public Godot.Collections.Dictionary<string, Texture2D> 角色肖像图片字典 = new();

        public Godot.Collections.Dictionary<string, bool> 事件条件字典 = new();

        public bool 可重复进行;

        public 剧情数据类() { }

        public 剧情数据类(string 总剧情名形参, string 二级剧情名形参, string 剧情资源路径形参,
            Godot.Collections.Dictionary<string, 任务数据类> 任务字典形参,
            Godot.Collections.Dictionary<string, string> 角色肖像路径字典形参,
            Godot.Collections.Dictionary<string, bool> 事件条件字典形参,
            bool 可重复进行形参)
        {
            总剧情名 = 总剧情名形参;
            二级剧情名 = 二级剧情名形参;
            剧情资源路径 = 剧情资源路径形参;
            剧情资源 = GD.Load<Resource>(剧情资源路径形参);

            任务字典 = 任务字典形参;

            foreach (var (key, value) in 角色肖像路径字典形参)
            {
                角色肖像图片字典[key] = GD.Load<Texture2D>(value);
            }

            事件条件字典 = 事件条件字典形参;

            可重复进行 = 可重复进行形参;
        }
    }

    private static Godot.Collections.Dictionary<string, string> 构建索引表的方法(string 剧情Json根目录)
    {
        Godot.Collections.Dictionary<string, string> 索引表 = new();
        Queue<string> 待扫描目录 = new();

        待扫描目录.Enqueue(剧情Json根目录);

        while (待扫描目录.Count > 0)
        {
            string 当前目录 = 待扫描目录.Dequeue();
            DirAccess dir = DirAccess.Open(当前目录);

            if (dir == null)
                continue;

            foreach (string 文件名 in dir.GetFiles())
            {
                if (!文件名.EndsWith(".json"))
                    continue;

                string id = Path.GetFileNameWithoutExtension(文件名);
                string 完整路径 = 当前目录.TrimEnd('/') + "/" + 文件名;

                索引表[id] = 完整路径;
            }

            foreach (string 子目录 in dir.GetDirectories())
            {
                string 完整子路径 = 当前目录.TrimEnd('/') + "/" + 子目录;
                待扫描目录.Enqueue(完整子路径);
            }
        }

        return 索引表;
    }

    private static Array<剧情数据类> 序列化剧情数据Json的方法(string Json文件路径)
    {
        var 文件 = Godot.FileAccess.Open(Json文件路径, Godot.FileAccess.ModeFlags.Read);
        string 文本 = 文件.GetAsText();
        文件.Close();

        var json = Json.ParseString(文本).AsGodotArray();

        Array<剧情数据类> 列表 = new();

        foreach (var item in json)
        {
            Dictionary dict = item.AsGodotDictionary();

            var 总剧情名 = (string)dict["总剧情名"];
            var 二级剧情名 = (string)dict["二级剧情名"];
            var 剧情资源路径 = (string)dict["剧情资源路径"];

            var 任务字典 = new Godot.Collections.Dictionary<string, 任务数据类>();
            var 任务字典Json = (Godot.Collections.Dictionary<string, Dictionary>)dict["任务字典"];
            foreach(var (key, value) in 任务字典Json)
            {
                var 任务地点Json = (Dictionary)value["任务地点"];
                任务数据类 任务 = new(
                    (string)value["任务名"],
                    (string)value["任务描述"],
                    (Godot.Collections.Dictionary<string, int>)value["所需物品名字典"],
                    (Godot.Collections.Dictionary<string, int>)value["奖励物品名字典"],
                    new((int)任务地点Json["x"], (int)任务地点Json["y"])
                    );
                任务字典[key] = 任务;
            }            

            var 角色肖像路径字典 = (Godot.Collections.Dictionary<string, string>)dict["角色肖像路径字典"];

            var 事件条件字典 = (Godot.Collections.Dictionary<string, bool>)dict["事件条件字典"];

            var 可重复进行 = (bool)dict["可重复进行"];

            var 实例 = new 剧情数据类(总剧情名, 二级剧情名, 剧情资源路径,
                任务字典,
                角色肖像路径字典,
                事件条件字典,
                可重复进行);
            列表.Add(实例);
        }

        return 列表;
    }

    public static Array<剧情数据类> 按需序列化剧情数据Json的方法(string 剧情Json根目录, string 总剧情名)
    {
        Array<剧情数据类> 总列表 = new();

        // 1. 自动扫描根目录，构建 ID → 路径 的索引表
        var 索引表 = 构建索引表的方法(剧情Json根目录);

        // 2. 查询剧情
        if (!索引表.ContainsKey(总剧情名))
        {
            GD.PrintErr($"剧情参数类脚本：未找到剧情文件 {总剧情名}");
        }

        string 路径 = 索引表[总剧情名];

        // 3. 按需加载 JSON
        Array<剧情数据类> 单文件列表 = 序列化剧情数据Json的方法(路径);

        foreach (var 项 in 单文件列表)
            总列表.Add(项);

        return 总列表;
    }
}