using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.IO;

public partial class 物品参数类脚本 : Node
{
    public enum 物品类型Enum
    {
        杂物,
        重要物品,
        卡牌,
    }

    public partial class 物品数据类 : RefCounted
    {
        public string 物品名;
        public 物品类型Enum 物品类型;
        public int 物品价值;
        public string 物品描述;
        private string 物品纹理路径;
        public Texture2D 物品纹理;

        public 物品数据类() { }

        public 物品数据类(string 物品名形参, 物品类型Enum 物品类型形参, int 物品价值形参, string 物品描述形参, string 物品纹理路径形参)
        {
            物品名 = 物品名形参;
            物品类型 = 物品类型形参;
            物品价值 = 物品价值形参;
            物品描述 = 物品描述形参;
            物品纹理路径 = 物品纹理路径形参;
        }

        public Texture2D 加载物品纹理的方法()
        {
            if (物品纹理 == null)
            {
                物品纹理 = GD.Load(物品纹理路径) as Texture2D;
            }

            return 物品纹理;
        }
    }

    private static Godot.Collections.Dictionary<string, string> 构建物品索引表的方法(string 物品Json根目录)
    {
        Godot.Collections.Dictionary<string, string> 索引表 = new();
        Queue<string> 待扫描目录 = new();

        待扫描目录.Enqueue(物品Json根目录);

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

    private static Array<物品数据类> 序列化物品数据Json的封装体方法(string Json文件路径)
    {
        var 文件 = Godot.FileAccess.Open(Json文件路径, Godot.FileAccess.ModeFlags.Read);
        string 文本 = 文件.GetAsText();
        文件.Close();

        var json = Json.ParseString(文本).AsGodotArray();

        Array<物品数据类> 列表 = new();

        foreach (var item in json)
        {
            Dictionary dict = item.AsGodotDictionary();

            var 物品名 = (string)dict["物品名"];
            var 物品类型 = Enum.Parse<物品类型Enum>((string)dict["物品类型"]);
            var 物品价值 = (int)dict["物品价值"];
            var 物品描述 = (string)dict["物品描述"];
            var 物品纹理路径 = (string)dict["物品纹理路径"];

            var 实例 = new 物品数据类(物品名, 物品类型, 物品价值, 物品描述, 物品纹理路径);
            列表.Add(实例);
        }

        return 列表;
    }

    public static Array<物品数据类> 按总类名序列化物品数据Json的方法(string 物品Json根目录, string 物品总类名)
    {
        Array<物品数据类> 总列表 = new();

        // 1. 自动扫描根目录，构建 ID → 路径 的索引表
        var 索引表 = 构建物品索引表的方法(物品Json根目录);

        // 2. 查询物品
        if (!索引表.ContainsKey(物品总类名))
        {
            GD.PrintErr($"物品参数类脚本：未找到物品文件 {物品总类名}");
        }

        string 路径 = 索引表[物品总类名];

        // 3. 按需加载 JSON
        Array<物品数据类> 单文件列表 = 序列化物品数据Json的封装体方法(路径);

        foreach (var 项 in 单文件列表)
            总列表.Add(项);

        return 总列表;
    }
}
