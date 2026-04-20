using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public partial class 字灵参数类脚本 : Node
{
    public enum 字灵属性Enum
    {
        // 默认
        无,

        // 显然属性
        金,
        木,
        水,
        火,
        土,
        风,
        雷,
        暗,

        // 本然属性
        相能,
        场力,
        时弦,
        化形,

        // 延展属性
        自然,
        生灵,
        创造,
        玄术,
        人文,
        神话,
        思理
    }

    public static readonly Godot.Collections.Dictionary<char, string[]> 声调映射字典 = new()
    {
        { 'a', new[] { "a", "ā", "á", "ǎ", "à" } },
        { 'e', new[] { "e", "ē", "é", "ě", "è" } },
        { 'i', new[] { "i", "ī", "í", "ǐ", "ì" } },
        { 'o', new[] { "o", "ō", "ó", "ǒ", "ò" } },
        { 'u', new[] { "u", "ū", "ú", "ǔ", "ù" } },
        { 'v', new[] { "ü", "ǖ", "ǘ", "ǚ", "ǜ" } },
        { 'ü', new[] { "ü", "ǖ", "ǘ", "ǚ", "ǜ" } },
    };

    public static string 获取注音韵母的方法(string 韵母, string 声调)
    {
        string result = 韵母;

        if (声调 == string.Empty) return result;

        int i = int.Parse(声调);
        // 特例：iu / ui
        if (韵母 == "iu")
            return "i" + 声调映射字典['u'][i];
        if (韵母 == "ui")
            return "u" + 声调映射字典['i'][i];

        // 一般规则：a > o > e
        foreach (char c in new[] { 'a', 'o', 'e' })
        {
            int idx = 韵母.IndexOf(c);
            if (idx >= 0)
                return 韵母[..idx] + 声调映射字典[c][i] + 韵母[(idx + 1)..];
        }

        // 其他元音：i u ü
        foreach (char c in 韵母)
        {
            if (声调映射字典.ContainsKey(c))
                return 韵母.Replace(c.ToString(), 声调映射字典[c][i]);
        }

        return result;
    }


    public partial class 笔画映射类 : RefCounted
    {
        public string 笔画名;
        public string 笔画书写;

        public 笔画映射类() { }

        public 笔画映射类(string 笔画名形参, string 笔画书写形参)
        {
            笔画名 = 笔画名形参;
            笔画书写 = 笔画书写形参;
        }
    }

    public partial class 卡牌纹理映射类 : RefCounted
    {
        public string 卡牌纹理路径;
        public Texture2D 卡牌纹理;

        public 卡牌纹理映射类() { }

        public 卡牌纹理映射类(string 卡牌纹理路径形参)
        {
            卡牌纹理路径 = 卡牌纹理路径形参;
        }

        public Texture2D 加载卡牌纹理的方法()
        {
            if (卡牌纹理 == null)
            {
                卡牌纹理 = GD.Load(卡牌纹理路径) as Texture2D;
            }

            return 卡牌纹理;
        }
    }

    public partial class 字灵数据类 : RefCounted
    {
        // 基础板块
        public string 汉字;
        public Array<string> 笔画;
        public string 字义;
        public string 读音;
        public string 拼音;
        public string 声母;
        public string 韵母;
        public string 声调;

        // 战斗板块
        public int 笔画数;
        public 字灵属性Enum 第一属性;
        public 字灵属性Enum 第二属性;

        // 其他板块
        public string 字灵纹理路径;
        public Texture2D 字灵纹理;

        public 字灵数据类() { }

        public 字灵数据类(string 汉字形参, Array<string> 笔画形参, string 字义形参, string 声母形参, string 韵母形参, string 声调形参,
            int 笔画数形参, 字灵属性Enum 第一属性形参, 字灵属性Enum 第二属性形参,
            string 字灵纹理路径形参)
        {
            汉字 = 汉字形参;
            笔画 = 笔画形参;
            字义 = 字义形参;
            读音 = 声母形参 + 韵母形参 + 声调形参;
            拼音 = 声母形参 + 获取注音韵母的方法(韵母形参, 声调形参);
            声母 = 声母形参;
            韵母 = 韵母形参;
            声调 = 声调形参;

            笔画数 = 笔画数形参;
            第一属性 = 第一属性形参;
            第二属性 = 第二属性形参;

            字灵纹理路径 = 字灵纹理路径形参;
        }

        public Texture2D 加载字灵纹理的方法()
        {
            if (字灵纹理 == null)
            {
                字灵纹理 = GD.Load(字灵纹理路径) as Texture2D;
            }

            return 字灵纹理;
        }
    }

    private static Godot.Collections.Dictionary<string, string> 构建字灵相关索引表的方法(string 相关Json根目录)
    {
        Godot.Collections.Dictionary<string, string> 索引表 = new();
        Queue<string> 待扫描目录 = new();

        待扫描目录.Enqueue(相关Json根目录);

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

    public static Godot.Collections.Dictionary<string, 字灵数据类> 序列化字灵数据Json的方法(string Json文件路径)
    {
        var 文件 = Godot.FileAccess.Open(Json文件路径, Godot.FileAccess.ModeFlags.Read);
        string 文本 = 文件.GetAsText();
        文件.Close();

        var json = Json.ParseString(文本).AsGodotArray();

        Godot.Collections.Dictionary<string, 字灵数据类> 字典 = new();

        foreach (var item in json)
        {
            Dictionary dict = item.AsGodotDictionary();

            var 汉字 = (string)dict["汉字"];
            var 笔画 = (Array<string>)dict["笔画"];
            var 字义 = (string)dict["字义"];
            var 声母 = (string)dict["声母"];
            var 韵母 = (string)dict["韵母"];
            var 声调 = (string)dict["声调"];
            var 笔画数 = (int)dict["笔画数"];

            var 原始属性 = (Array<string>)dict["属性"];
            字灵属性Enum 第一属性 = 字灵属性Enum.无;
            字灵属性Enum 第二属性 = 字灵属性Enum.无;
            foreach (var 属性 in 原始属性)
            {
                第一属性 = Enum.Parse<字灵属性Enum>(原始属性.First());
                第二属性 = Enum.Parse<字灵属性Enum>(原始属性.Last());
            }
            var 字灵纹理路径 = (string)dict["字灵纹理路径"];

            var 数据 = new 字灵数据类(汉字, 笔画, 字义, 声母, 韵母, 声调, 笔画数, 第一属性, 第二属性, 字灵纹理路径);
            字典[汉字] = 数据;
        }

        return 字典;
    }

    public static Godot.Collections.Dictionary<string, 笔画映射类> 序列化笔画映射Json的方法(string Json文件路径)
    {
        var 文件 = Godot.FileAccess.Open(Json文件路径, Godot.FileAccess.ModeFlags.Read);
        string 文本 = 文件.GetAsText();
        文件.Close();

        var json = Json.ParseString(文本).AsGodotArray();

        Godot.Collections.Dictionary<string, 笔画映射类> 字典 = new();

        foreach (var item in json)
        {
            Dictionary dict = item.AsGodotDictionary();

            var 笔画名 = (string)dict["笔画名"];
            var 笔画书写 = (string)dict["笔画书写"];
            var 映射 = new 笔画映射类(笔画名, 笔画书写);

            字典[笔画名] = 映射;
        }

        return 字典;
    }

    public static Godot.Collections.Dictionary<string, 字灵数据类> 加载文件夹下所有字灵Json的方法(string 字灵Json根目录)
    {
        Godot.Collections.Dictionary<string, 字灵数据类> 字典 = new();

        DirAccess dir = DirAccess.Open(字灵Json根目录);
        if (dir == null)
        {
            GD.PrintErr($"无法打开文件夹: {字灵Json根目录}");
            return 字典;
        }

        foreach (string 文件名 in dir.GetFiles())
        {
            // 只处理 .json 或 .txt
            if (!文件名.EndsWith(".json") && !文件名.EndsWith(".txt"))
                continue;

            string 完整路径 = 字灵Json根目录.TrimEnd('/') + "/" + 文件名;

            字典 = 序列化字灵数据Json的方法(完整路径);
        }

        return 字典;
    }

    public static Godot.Collections.Dictionary<string, 笔画映射类> 加载文件夹下所有笔画Json的方法(string 笔画Json根目录)
    {
        Godot.Collections.Dictionary<string, 笔画映射类> 字典 = new();

        DirAccess dir = DirAccess.Open(笔画Json根目录);
        if (dir == null)
        {
            GD.PrintErr($"无法打开文件夹: {笔画Json根目录}");
            return 字典;
        }

        foreach (string 文件名 in dir.GetFiles())
        {
            // 只处理 .json 或 .txt
            if (!文件名.EndsWith(".json") && !文件名.EndsWith(".txt"))
                continue;

            string 完整路径 = 笔画Json根目录.TrimEnd('/') + "/" + 文件名;

            字典 = 序列化笔画映射Json的方法(完整路径);
        }

        return 字典;
    }

    public static AudioStreamMP3 按需加载拼音音频的方法(string 根目录路径, string 读音)
    {
        AudioStreamMP3 读音音频 = null;

        string 后缀 = $".mp3";
        读音音频 = GD.Load<AudioStreamMP3>(根目录路径 + 读音 + 后缀);

        return 读音音频;
    }
}
