using Godot;
using Godot.Collections;
using System;
using static 物品参数类脚本;

public partial class 玩家数据类脚本 : Node
{
    public partial class 玩家存档数据类 : RefCounted
    {
        #region 信息数据
        public string 姓名;
        public string 性别;
        public string 种族;
        public DateOnly 出生;
        public string 住址;
        public string 字灵证;
        public string 学术头衔;
        public string 身份号码;
        #endregion

        #region 能力数据
        public int 字言等级;
        public int 文墨点数;        // 技能点
        public int 精神力上限;       // 血量
        public int 灵感上限;        // 能量
        #endregion

        #region 库存数据
        public partial class 智能字典 : RefCounted
        {
            private Dictionary<string, int> 内部字典 = new();

            public int this[string key]
            {
                get => 内部字典.ContainsKey(key) ? 内部字典[key] : 0;
                set
                {
                    if (value <= 0)
                    {
                        内部字典.Remove(key);
                        return;
                    }
                    else
                    {
                        内部字典[key] = value;
                    }
                }
            }

            public System.Collections.Generic.ICollection<string> Keys => 内部字典.Keys;
            public System.Collections.Generic.ICollection<int> Values => 内部字典.Values;
            public int Count => 内部字典.Count;

            public bool ContainsKey(string key) => 内部字典.ContainsKey(key);

            public System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<string, int>> GetEnumerator()
            {
                return 内部字典.GetEnumerator();
            }

            public void Add(string key, int value)
            {
                this[key] = value;
            }

            public 智能字典() { }
        }
        public Dictionary<物品类型Enum, 智能字典> 物品库字典 = new()
        {
            { 物品类型Enum.杂物, new() },
            { 物品类型Enum.重要物品, new() },
            { 物品类型Enum.卡牌, new() },
        };
        public 智能字典 字灵字典 = new();
        #endregion

        #region 外貌数据
        public string 帽子图片路径;
        public Texture2D 帽子图片;
        public string 眼镜图片路径;
        public Texture2D 眼镜图片;
        public string 胡须图片路径;
        public Texture2D 胡须图片;
        public string 上衣图片路径;
        public Texture2D 上衣图片;
        public string 裤子图片路径;
        public Texture2D 裤子图片;
        public string 鞋子图片路径;
        public Texture2D 鞋子图片;
        #endregion

        #region 剧情数据
        public partial class 转义哈希表 : RefCounted
        {
            private System.Collections.Generic.HashSet<string> 内部哈希表 = new();

            public void Add(string key) => 内部哈希表.Add(key);
            public bool Contains(string key) => 内部哈希表.Contains(key);
            public void Remove(string key) => 内部哈希表.Remove(key);

            public 转义哈希表() { }
        }
        public Dictionary<string, 转义哈希表> 记录总剧情的字典 = new();

        // 任务数据
        public Dictionary<string, Dictionary<string, Array<string>>> 已完成任务的字典 = new();     //前者为总剧情名，后者为二级剧情名和任务列表
        public Dictionary<string, Dictionary<string, Array<string>>> 正在进行任务的字典 = new();
        #endregion


        public 玩家存档数据类() { }

        public 玩家存档数据类(string 姓名形参, string 性别形参, string 种族形参, DateOnly 出生形参, string 住址形参, string 字灵证形参, string 学术头衔形参, string 身份号码形参,
            int 字言等级形参, int 精神力上限形参, int 灵感上限形参, int 文墨点数形参,
            Dictionary<物品类型Enum, 智能字典> 物品库字典形参, 智能字典 字灵字典形参,
            string 帽子图片路径形参, string 眼镜图片路径形参, string 胡须图片路径形参, string 上衣图片路径形参, string 裤子图片路径形参, string 鞋子图片路径形参,
            Dictionary<string, 转义哈希表> 已完成剧情的字典形参, Dictionary<string, Dictionary<string, Array<string>>> 正在进行任务的字典形参)
        {

            // 信息数据
            姓名 = 姓名形参;
            性别 = 性别形参;
            出生 = 出生形参;
            种族 = 种族形参;
            住址 = 住址形参;
            字灵证 = 字灵证形参;
            学术头衔 = 学术头衔形参;
            身份号码 = 身份号码形参;

            // 能力数据            
            字言等级 = 字言等级形参;
            文墨点数 = 文墨点数形参;
            精神力上限 = 精神力上限形参;
            灵感上限 = 灵感上限形参;

            // 库存数据
            物品库字典 = 物品库字典形参;
            字灵字典 = 字灵字典形参;

            // 外貌数据
            帽子图片路径 = 帽子图片路径形参;
            帽子图片 = GD.Load<Texture2D>(帽子图片路径);
            眼镜图片路径 = 眼镜图片路径形参;
            眼镜图片 = GD.Load<Texture2D>(眼镜图片路径);
            胡须图片路径 = 胡须图片路径形参;
            胡须图片 = GD.Load<Texture2D>(胡须图片路径);
            上衣图片路径 = 上衣图片路径形参;
            上衣图片 = GD.Load<Texture2D>(上衣图片路径);
            裤子图片路径 = 裤子图片路径形参;
            裤子图片 = GD.Load<Texture2D>(裤子图片路径);
            鞋子图片路径 = 鞋子图片路径形参;
            鞋子图片 = GD.Load<Texture2D>(鞋子图片路径);

            // 剧情数据
            记录总剧情的字典 = 已完成剧情的字典形参;
            已完成任务的字典 = 正在进行任务的字典形参;
            正在进行任务的字典 = 正在进行任务的字典形参;
        }
    }

    public partial class 玩家游戏数据类 : GodotObject
    {
        // 剧情数据
        [Signal]
        public delegate void 玩家已到达剧情移动目标点的信号EventHandler();
        public bool 在剧情中;
        public string 获得部分控制权;
        public const float 目标点范围的允许误差值 = 1;
        public Vector2 剧情移动目标点;

        // 任务数据
        public bool 正在追踪任务;
        public Vector2 任务地点 = new();

        // 交互数据
        public bool 正在交互;

        public void 检测玩家已到达剧情移动目标点的方法(CharacterBody2D 玩家)
        {
            if (剧情移动目标点 != Vector2.Zero)
            {
                Rect2 允许误差范围 = new(new(剧情移动目标点.X - 目标点范围的允许误差值, 剧情移动目标点.Y - 目标点范围的允许误差值), new(目标点范围的允许误差值 * 2, 目标点范围的允许误差值 * 2));
                if (允许误差范围.HasPoint(玩家.Position))
                {
                    EmitSignal(nameof(玩家已到达剧情移动目标点的信号));
                    剧情移动目标点 = Vector2.Zero;
                }
            }
        }

        public bool 玩家拥有控制权()
        {
            return (!在剧情中 && !正在交互);
        }

        public void 重置剧情数据的方法()
        {
            在剧情中 = false;
            获得部分控制权 = string.Empty;
            剧情移动目标点 = Vector2.Zero;
        }

        public void 重置交互数据的方法()
        {
            正在交互 = false;
        }
    }

    public partial class 游戏行为数据类 : RefCounted
    {
        public string ID = string.Empty;
        public string 开始游戏的时间 = string.Empty;

        // 推理部分
        public Dictionary<string, string> 装置完成时间 = new()
        {
            { "勾装置", string.Empty },
            { "原装置", string.Empty },
            { "市装置", string.Empty },
            { "张装置", string.Empty },
            { "钩装置", string.Empty },
            { "源装置", string.Empty },
            { "涨装置", string.Empty },
            { "柿装置", string.Empty },
        };

        public Dictionary<string, int> 推理错误次数 = new()
        {
            { "勾装置", 0 },
            { "原装置", 0 },
            { "市装置", 0 },
            { "张装置", 0 },
        };

        // 造字部分
        public Dictionary<string, string> 造字完成时间 = new()
        {
            { "钩", string.Empty },
            { "源", string.Empty },
            { "涨", string.Empty },
            { "柿", string.Empty },
        };
        public int 造字失败次数 = 0;

        // 锻造部分
        public Dictionary<string, string> 锻造完成时间 = new()
        {
            { "勾", string.Empty },
            { "原", string.Empty },
            { "市", string.Empty },
            { "弓", string.Empty },
            { "长", string.Empty },
            { "张", string.Empty },
            { "钩", string.Empty },
            { "源", string.Empty },
            { "涨", string.Empty },
            { "柿", string.Empty },
        };

        public 游戏行为数据类() { }

        public Dictionary<string, Variant> ToGodot()
        {
            return new Dictionary<string, Variant>
            {
                { "ID", ID },
                { "开始游戏的时间", 开始游戏的时间 },
                { "装置完成时间", new Dictionary<string, string>(装置完成时间) },
                { "推理错误次数", new Dictionary<string, int>(推理错误次数) },
                { "造字完成时间", new Dictionary<string, string>(造字完成时间) },
                { "造字失败次数", 造字失败次数 },
                { "锻造完成时间", new Dictionary<string, string>(锻造完成时间) },
            };
        }
    }
}
