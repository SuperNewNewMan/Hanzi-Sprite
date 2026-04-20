using Godot;
using Godot.Collections;
using System;
using static 剧情参数类脚本;
using static 物品参数类脚本;
using static 场景参数类脚本;
using static 字灵参数类脚本;

public partial class 配置Json单例脚本 : Node
{
    private const string 剧情Json根目录 = $"res://Json文件夹/剧情Json文件夹/";
    private const string 场景预制体Json根目录 = $"res://Json文件夹/地图Json文件夹/场景预制体Json文件夹/";
    private const string 物品Json根目录 = "res://Json文件夹/物品Json文件夹/";

    private const string 字灵Json根目录 = "res://Json文件夹/字灵相关Json文件夹/字灵Json文件夹/";
    private const string 笔画Json根目录 = "res://Json文件夹/字灵相关Json文件夹/笔画Json文件夹/";
    private const string 卡牌Json根目录 = "res://Json文件夹/字灵相关Json文件夹/卡牌Json文件夹/";
    private const string 拼音音频资源根目录 = "res://资源文件夹/拼音音频资源文件夹/";

    public Dictionary<string, Dictionary<string, 剧情数据类>> 剧情字典
    {
        get;
        private set;
    } = new();
    public Dictionary<string, bool> 剧情条件字典
    {
        get;
        private set;
    } = new();

    public Dictionary<string, 物品数据类> 物品字典
    {
        get;
        private set;
    } = new();

    public Dictionary<场景名Enum, 场景区块类> 场景预制体字典
    {
        get;
        private set;
    } = new();

    public Dictionary<string, 字灵数据类> 字灵字典
    {
        get;
        private set;
    } = new();

    public Dictionary<string, 笔画映射类> 笔画映射字典
    {
        get;
        private set;
    } = new();

    public System.Collections.Generic.Dictionary<System.Collections.Generic.HashSet<字灵属性Enum>, 卡牌纹理映射类> 卡牌纹理映射字典
    {
        get;
        private set;
    } = new();

    public Dictionary<string, AudioStreamMP3> 拼音音频映射字典
    {
        get;
        private set;
    } = new();

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        foreach (var 枚举 in Enum.GetValues<物品类型Enum>())
        {
            foreach (var 物品 in 按总类名序列化物品数据Json的方法(物品Json根目录, 枚举.ToString()))
            {
                物品字典[物品.物品名] = 物品;
            }
        }

        var 场景预制体array = 加载文件夹下所有场景预制体Json的方法(场景预制体Json根目录);
        foreach (var item in 场景预制体array)
        {
            场景预制体字典[item.场景名] = item;
        }

        字灵字典 = 加载文件夹下所有字灵Json的方法(字灵Json根目录);
        笔画映射字典 = 加载文件夹下所有笔画Json的方法(笔画Json根目录);
    }

    public void 配置剧情字典的方法(string 总剧情名)
    {
        剧情字典[总剧情名] = new();
        var 快照 = 剧情字典[总剧情名];
        foreach (var item in 按需序列化剧情数据Json的方法(剧情Json根目录, 总剧情名))
        {
            快照[item.二级剧情名] = item;

            foreach(var key in item.事件条件字典.Keys)
            {
                if(!剧情条件字典.ContainsKey(key))
                {
                    剧情条件字典[key] = false;
                }
            }
        }
    }

    public AudioStreamMP3 加载拼音音频的方法(string 读音)
    {
        if (拼音音频映射字典.ContainsKey(读音))
        {
            return 拼音音频映射字典[读音];
        }

        AudioStreamMP3 音频 = 按需加载拼音音频的方法(拼音音频资源根目录, 读音);
        拼音音频映射字典[读音] = 音频;
        return 音频;
    }
}
