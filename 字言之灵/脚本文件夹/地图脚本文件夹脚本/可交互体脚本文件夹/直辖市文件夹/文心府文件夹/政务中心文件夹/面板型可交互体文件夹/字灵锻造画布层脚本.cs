using Godot;
using Godot.Collections;
using Microsoft.International.Converters.PinYinConverter;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Whisper.net;

public partial class 字灵锻造画布层脚本 : CanvasLayer
{
    private Random 随机数生成器 = new();

    private 配置Json单例脚本 配置Json单例脚本单例;
    private 玩家数据单例脚本 玩家数据单例脚本实例;
    private 剧情演绎单例脚本 剧情演绎单例脚本实例;

    private partial class 内部数据类 : RefCounted
    {
        public partial class 笔画按钮数据 : RefCounted
        {
            public string 笔画名称;
            public Button 对应笔画块按钮;
        }

        public const string whisper模型文件名 = "ggml-small.bin";
        public const string whisper模型导出文件夹路径 = $"user://AdditionPlugins/Whisper";
        public const string whisper模型读取路径 = $"res://AddtionPlugins/Whisper/{whisper模型文件名}";

        public int 上一个聚焦字灵索引 = -1;
        public string 已选字灵 = string.Empty;
        public Array<Button> 选择字灵按钮池 = new();

        public Array<笔画按钮数据> 笔画进度按钮池 = new();
        public Array<笔画按钮数据> 笔画块按钮池 = new();

        public string 当前读音;

        public void 重置的方法()
        {
            已选字灵 = string.Empty;

            if (上一个聚焦字灵索引 >= 0 && 上一个聚焦字灵索引 < 选择字灵按钮池.Count)
            {
                选择字灵按钮池[上一个聚焦字灵索引].ButtonPressed = false;
            }

            上一个聚焦字灵索引 = -1;
        }
    }
    private 内部数据类 数据 = new();

    private MarginContainer 选择字灵MC;
    private PackedScene 选择字灵按钮预制体 = GD.Load<PackedScene>("res://场景文件夹/地图场景文件夹/直辖市文件夹/文心府文件夹/政务中心文件夹/设施场景文件夹/字灵锻造面板/选择字灵按钮.tscn");
    private GridContainer 选择字灵GC;
    private Button 选择字灵下一步按钮;

    private MarginContainer 基石构建MC;
    private Label 基石构建选字标签;
    private PackedScene 笔画块按钮预制体 = GD.Load<PackedScene>("res://场景文件夹/地图场景文件夹/直辖市文件夹/文心府文件夹/政务中心文件夹/设施场景文件夹/字灵锻造面板/笔画块按钮.tscn");
    private PackedScene 笔画进度块按钮预制体 = GD.Load<PackedScene>("res://场景文件夹/地图场景文件夹/直辖市文件夹/文心府文件夹/政务中心文件夹/设施场景文件夹/字灵锻造面板/笔画进度块按钮.tscn");
    private GridContainer 笔画块GC;
    private GridContainer 笔画进度GC;
    private Button 基石构建按钮;
    private Button 基石构建重选按钮;

    private bool 允许录音 = false;
    private MarginContainer 言灵锚定MC;
    private AudioStreamPlayer 拼音播放器;
    private AudioStreamPlayer 录音器;
    private AudioEffectRecord 录音数据接收器;
    private Button 言灵锚定拼音按钮;
    private Label 言灵锚定选字标签;
    private Button 言灵锚定录音按钮;
    private LineEdit 言灵锚定拼音LE;
    private Button 言灵锚定停录按钮;
    private Button 言灵锚定重选按钮;

    private MarginContainer 意义封神MC;
    private Label 意义封神选字标签;

    private MarginContainer 誓言命定MC;
    private Label 誓言命定选字标签;
    private LineEdit 誓言命定LE;
    private Button 誓言命定按钮;

    private Panel 提醒Panel;
    private Button 确认退出按钮;
    private Button 返回测试按钮;

    private Button 关闭面板按钮;

    private Panel 遮挡Panel;

    private WhisperFactory whisperFactory;
    private WhisperProcessor whisperProcessor;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        配置Json单例脚本单例 = GetNode<配置Json单例脚本>("/root/配置Json单例脚本");
        玩家数据单例脚本实例 = GetNode<玩家数据单例脚本>("/root/玩家数据单例脚本");
        剧情演绎单例脚本实例 = GetNode<剧情演绎单例脚本>("/root/剧情演绎单例脚本");

        选择字灵MC = GetNode<MarginContainer>("%选择字灵MC");
        选择字灵MC.Connect("visibility_changed", new Callable(this, nameof(刷新选择字灵模块的回调函数)));
        选择字灵GC = GetNode<GridContainer>("%选择字灵GC");
        选择字灵下一步按钮 = GetNode<Button>("%选择字灵下一步按钮");
        选择字灵下一步按钮.Connect("pressed", new Callable(this, nameof(选择字灵下一步按钮被按下的回调函数)));

        基石构建MC = GetNode<MarginContainer>("%基石构建MC");
        基石构建MC.Connect("visibility_changed", new Callable(this, nameof(刷新基石构建模块的回调函数)));
        基石构建选字标签 = GetNode<Label>("%基石构建选字标签");
        笔画块GC = GetNode<GridContainer>("%笔画块GC");
        笔画进度GC = GetNode<GridContainer>("%笔画进度GC");
        基石构建按钮 = GetNode<Button>("%基石构建按钮");
        基石构建按钮.Connect("pressed", new Callable(this, nameof(基石构建按钮被按下的回调函数)));
        基石构建重选按钮 = GetNode<Button>("%基石构建重选按钮");
        基石构建重选按钮.Pressed += () => 重选的回调函数(基石构建MC);

        言灵锚定MC = GetNode<MarginContainer>("%言灵锚定MC");
        言灵锚定MC.Connect("visibility_changed", new Callable(this, nameof(刷新言灵锚定模块的回调函数)));
        拼音播放器 = GetNode<AudioStreamPlayer>("%拼音播放器");
        录音器 = GetNode<AudioStreamPlayer>("%录音器");
        录音数据接收器 = (AudioEffectRecord)AudioServer.GetBusEffect(AudioServer.GetBusIndex("Record"), 0);
        言灵锚定拼音按钮 = GetNode<Button>("%言灵锚定拼音按钮");
        言灵锚定拼音按钮.Pressed += () => 拼音播放器.Play();
        言灵锚定选字标签 = GetNode<Label>("%言灵锚定选字标签");
        言灵锚定录音按钮 = GetNode<Button>("%言灵锚定录音按钮");
        言灵锚定录音按钮.Connect("pressed", new Callable(this, nameof(开始录音的方法)));
        言灵锚定拼音LE = GetNode<LineEdit>("%言灵锚定拼音LE");
        言灵锚定停录按钮 = GetNode<Button>("%言灵锚定停录按钮");
        言灵锚定停录按钮.Connect("pressed", new Callable(this, nameof(停止录音的方法)));
        言灵锚定重选按钮 = GetNode<Button>("%言灵锚定重选按钮");
        言灵锚定重选按钮.Pressed += () => 重选的回调函数(言灵锚定MC);

        意义封神MC = GetNode<MarginContainer>("%意义封神MC");
        意义封神MC.VisibilityChanged += async () => await 刷新意义封神模块的回调函数();
        意义封神选字标签 = GetNode<Label>("%意义封神选字标签");

        誓言命定MC = GetNode<MarginContainer>("%誓言命定MC");
        誓言命定MC.Connect("visibility_changed", new Callable(this, nameof(刷新誓言命定模块的回调函数)));
        誓言命定选字标签 = GetNode<Label>("%誓言命定选字标签");
        誓言命定LE = GetNode<LineEdit>("%誓言命定LE");
        誓言命定按钮 = GetNode<Button>("%誓言命定按钮");
        誓言命定按钮.Connect("pressed", new Callable(this, nameof(誓言命定按钮被按下的回调函数)));

        提醒Panel = GetNode<Panel>("%提醒Panel");
        确认退出按钮 = GetNode<Button>("%确认退出按钮");
        确认退出按钮.Connect("pressed", new Callable(this, nameof(关闭面板的回调函数)));
        返回测试按钮 = GetNode<Button>("%返回测试按钮");
        返回测试按钮.Pressed += 提醒Panel.Hide;

        关闭面板按钮 = GetNode<Button>("%关闭面板按钮");
        关闭面板按钮.Pressed += 提醒Panel.Show;

        遮挡Panel = GetNode<Panel>("%遮挡Panel");

        VisibilityChanged += 基本初始化的方法;

        // 初始化
        基本初始化的方法();
        刷新选择字灵模块的回调函数();
        初始化笔画块按钮池();

        var devices = AudioServer.GetInputDeviceList();
        GD.Print("检测到的设备数量: ", devices.Length);
        foreach (var d in devices)
            GD.Print("设备: ", d);

        if (devices.Length < 2)
        {
            允许录音 = false;
            return;
        }
        允许录音 = true;
        // Whisper 初始化（v1.9 API）
        string realPath = 拷贝模型的方法(内部数据类.whisper模型文件名, 内部数据类.whisper模型导出文件夹路径, 内部数据类.whisper模型读取路径);
        whisperFactory = WhisperFactory.FromPath(realPath);
        whisperProcessor = whisperFactory.CreateBuilder().WithLanguage("zh").Build();
    }

    public override void _ExitTree()
    {
        whisperProcessor?.Dispose();
        whisperFactory?.Dispose();
    }

    private void 基本初始化的方法()
    {
        选择字灵MC.Show();
        基石构建MC.Hide();
        言灵锚定MC.Hide();
        意义封神MC.Hide();
        誓言命定MC.Hide();
    }

    private void 选择字灵下一步按钮被按下的回调函数()
    {
        if (数据.已选字灵 == string.Empty) return;

        选择字灵MC.Hide();
        基石构建MC.Show();
    }

    private void 关闭面板的回调函数()
    {
        Hide();
        选择字灵MC.Hide();
        基石构建MC.Hide();
        言灵锚定MC.Hide();
        意义封神MC.Hide();
        誓言命定MC.Hide();
        提醒Panel.Hide();

        数据.重置的方法();
    }

    private void 重选的回调函数(MarginContainer 被关闭的MC)
    {
        数据.重置的方法();

        被关闭的MC.Hide();
        选择字灵MC.Show();
    }

    private void 刷新选择字灵模块的回调函数()
    {
        if (Visible)
        {
            int i = -1;

            var 智能字典 = 玩家数据单例脚本实例.玩家存档数据.字灵字典;
            var 按钮池 = 数据.选择字灵按钮池;
            foreach (var (字灵, 数量) in 智能字典)
            {
                i++;

                Button 按钮;
                if (i >= 按钮池.Count)
                {
                    var 索引 = i;
                    按钮 = 选择字灵按钮预制体.Instantiate<Button>();
                    按钮池.Add(按钮);
                    选择字灵GC.AddChild(按钮);
                    按钮.Pressed += () =>
                    {
                        if (数据.上一个聚焦字灵索引 >= 0 && 数据.上一个聚焦字灵索引 < 按钮池.Count)
                        {
                            按钮池[数据.上一个聚焦字灵索引].ButtonPressed = false;
                        }

                        数据.上一个聚焦字灵索引 = 索引;
                        按钮.ButtonPressed = true;
                        数据.已选字灵 = 按钮.Text;
                    };
                }
                else
                {
                    按钮 = 按钮池[i];
                }

                按钮.Show();
                按钮.Text = 字灵;
                Label 数量标签 = 按钮.GetNode<Label>("数量标签");
                数量标签.Text = 数量.ToString();
            }

            while (i + 1 < 按钮池.Count)
            {
                i++;

                var 按钮 = 按钮池[i];
                按钮.Hide();
            }
        }
    }

    private void 初始化笔画块按钮池()
    {
        var 块池 = 数据.笔画块按钮池;

        // 假设最大笔画数不会超过 30
        int 最大笔画数 = 30;

        for (int i = 0; i < 最大笔画数; i++)
        {
            Button 块 = 笔画块按钮预制体.Instantiate<Button>();
            笔画块GC.AddChild(块);

            // 创建数据对象
            var data = new 内部数据类.笔画按钮数据();
            data.对应笔画块按钮 = 块;
            块.SetMeta("data", data);

            // ⭐ 绑定一次事件（永远不再重复绑定）
            块.Pressed += () =>
            {
                var d = (内部数据类.笔画按钮数据)块.GetMeta("data");
                string 当前笔画 = d.笔画名称;

                块.Hide();

                // 创建进度按钮
                Button 新进度 = 笔画进度块按钮预制体.Instantiate<Button>();
                新进度.Text = 配置Json单例脚本单例.笔画映射字典[当前笔画].笔画书写;

                数据.笔画进度按钮池.Add(d);
                笔画进度GC.AddChild(新进度);

                // 点击进度按钮 → 删除自己 + 恢复笔画块
                新进度.Pressed += () =>
                {
                    新进度.QueueFree();
                    数据.笔画进度按钮池.Remove(d);
                    块.Show();
                };
            };

            块池.Add(data);
        }
    }

    private void 刷新基石构建模块的回调函数()
    {
        if (!基石构建MC.Visible) return;

        string 已选字灵 = 数据.已选字灵;
        var 映射 = 配置Json单例脚本单例.笔画映射字典;

        // 复制原数组
        var 随机笔画 = 配置Json单例脚本单例.字灵字典[已选字灵].笔画.ToList();

        // 洗牌
        for (int i = 随机笔画.Count - 1; i > 0; i--)
        {
            int swapIndex = 随机数生成器.Next(i + 1);
            (随机笔画[i], 随机笔画[swapIndex]) = (随机笔画[swapIndex], 随机笔画[i]);
        }

        基石构建选字标签.Text = 已选字灵;

        var 块池 = 数据.笔画块按钮池;

        // 清空进度区
        foreach (var d in 数据.笔画进度按钮池)
        {
            d.对应笔画块按钮.Show();
        }
        数据.笔画进度按钮池.Clear();
        foreach (Node child in 笔画进度GC.GetChildren())
            child.QueueFree();

        // 更新笔画块
        for (int i = 0; i < 块池.Count; i++)
        {
            var data = 块池[i];
            Button 块按钮 = data.对应笔画块按钮;

            if (i < 随机笔画.Count)
            {
                string 当前笔画 = 随机笔画[i];
                data.笔画名称 = 当前笔画;

                块按钮.Text = 映射[当前笔画].笔画书写;
                块按钮.Show();
            }
            else
            {
                块按钮.Hide();
            }
        }
    }

    private async void 基石构建按钮被按下的回调函数()
    {
        string 已选字灵 = 数据.已选字灵;
        Array<string> 笔画 = 配置Json单例脚本单例.字灵字典[已选字灵].笔画;

        if (数据.笔画进度按钮池.Count < 笔画.Count)
        {
            _ = 剧情演绎单例脚本实例.播放剧情的方法("琥珀阿比猫_对话", "琥珀阿比猫_对话_基石构建", "错误", null);
            return;
        }

        for (int i = 0; i < 数据.笔画进度按钮池.Count; i++)
        {
            if (数据.笔画进度按钮池[i].笔画名称 != 笔画[i])
            {
                _ = 剧情演绎单例脚本实例.播放剧情的方法("琥珀阿比猫_对话", "琥珀阿比猫_对话_基石构建", "错误", null);
                return;
            }
        }

        await 剧情演绎单例脚本实例.播放剧情的方法("琥珀阿比猫_对话", "琥珀阿比猫_对话_基石构建", "正确", null);

        基石构建MC.Hide();
        言灵锚定MC.Show();
    }

    private async void 刷新言灵锚定模块的回调函数()
    {
        if (!言灵锚定MC.Visible) return;

        言灵锚定选字标签.Text = 数据.已选字灵;
        言灵锚定拼音LE.Text = string.Empty;

        if (!允许录音)
        {
            await 剧情演绎单例脚本实例.播放剧情的方法("琥珀阿比猫_对话", "琥珀阿比猫_对话_言灵锚定", "没有麦克风", null);

            言灵锚定MC.Hide();
            意义封神MC.Show();
            return;
        }
       
        言灵锚定拼音按钮.Text = 配置Json单例脚本单例.字灵字典[数据.已选字灵].拼音;

        var 字灵 = 配置Json单例脚本单例.字灵字典[数据.已选字灵];
        拼音播放器.Stream = 配置Json单例脚本单例.加载拼音音频的方法(字灵.读音);
    }

    private void 开始录音的方法()
    {
        if (!允许录音)
            return;

        录音器.Play();
        录音数据接收器.SetRecordingActive(true);

        言灵锚定录音按钮.Disabled = true;
        言灵锚定停录按钮.Disabled = false;
        关闭面板按钮.Disabled = true;
    }

    private async void 停止录音的方法()
    {
        if (!允许录音)
            return;

        遮挡Panel.Show();

        录音器.Stop();
        录音数据接收器.SetRecordingActive(false);

        言灵锚定录音按钮.Disabled = false;
        言灵锚定停录按钮.Disabled = true;
        关闭面板按钮.Disabled = false;

        AudioStreamWav wav = 录音数据接收器.GetRecording();

        byte[] pcm = wav.Data;
        int sampleRate = wav.MixRate;
        int channels = wav.Stereo ? 2 : 1;

        // 1. Godot PCM → RawSourceWaveStream
        using var rawStream = new MemoryStream(pcm);
        var rawWave = new RawSourceWaveStream(
            rawStream,
            new WaveFormat(sampleRate, 16, channels)
        );

        // 2. 重采样到 16kHz
        var resampler = new WdlResamplingSampleProvider(
            rawWave.ToSampleProvider(),
            16000
        );

        // 3. 用 WaveFileWriter 写到 MemoryStream（正确方式）
        byte[] wav16Bytes;
        using (var ms = new MemoryStream())
        {
            using (var writer = new WaveFileWriter(ms, new WaveFormat(16000, 16, channels)))
            {
                float[] buffer = new float[16000];
                int read;

                while ((read = resampler.Read(buffer, 0, buffer.Length)) > 0)
                {
                    writer.WriteSamples(buffer, 0, read);
                }
            }

            wav16Bytes = ms.ToArray();
        }

        // 4. 喂给 Whisper
        using var ms2 = new MemoryStream(wav16Bytes);

        string finalText = "";
        await foreach (var seg in whisperProcessor.ProcessAsync(ms2))
        {
            finalText += seg.Text;
        }

        if (ChineseChar.IsValidChar(finalText[0]))
        {
            ChineseChar check = new(finalText[0]);
            ChineseChar target = new(数据.已选字灵[0]);
            string 小写 = check.Pinyins[0].ToLower();
            string 拼音 = new(小写.Where(char.IsLetter).ToArray());
            string 声调 = new(小写.Where(char.IsDigit).ToArray());
            言灵锚定拼音LE.Text = 字灵参数类脚本.获取注音韵母的方法(拼音, 声调);
            if (check.Pinyins[0] == target.Pinyins[0])
            {
                await 剧情演绎单例脚本实例.播放剧情的方法("琥珀阿比猫_对话", "琥珀阿比猫_对话_言灵锚定", "正确", null);

                言灵锚定MC.Hide();
                意义封神MC.Show();
            }
            else
            {
                _ = 剧情演绎单例脚本实例.播放剧情的方法("琥珀阿比猫_对话", "琥珀阿比猫_对话_言灵锚定", "错误", null);
            }
        }
        else
        {
            _ = 剧情演绎单例脚本实例.播放剧情的方法("琥珀阿比猫_对话", "琥珀阿比猫_对话_言灵锚定", "没说清楚", null);
        }

        遮挡Panel.Hide();
    }

    private string 拷贝模型的方法(string 模型名, string 导出文件夹路径, string 读取路径)
    {
        string userPath = ProjectSettings.GlobalizePath(导出文件夹路径);

        // 检查文件夹
        if (!Directory.Exists(userPath))
        {
            Directory.CreateDirectory(userPath);
        }

        // 拼接完整路径
        string fullPath = Path.Combine(userPath, 模型名);

        // 如果已经存在，就不重复写
        if (File.Exists(fullPath))
        {
            return fullPath;
        }

        // 从PCK读取
        byte[] data = Godot.FileAccess.GetFileAsBytes(读取路径);
        if (data == null || data.Length == 0)
            throw new Exception($"模型文件不存在：{读取路径}");

        // 写入 user://
        File.WriteAllBytes(fullPath, data);
        return fullPath;
    }

    private async Task 刷新意义封神模块的回调函数()
    {
        if (!意义封神MC.Visible) return;

        意义封神选字标签.Text = 数据.已选字灵;
        await 剧情演绎单例脚本实例.播放剧情的方法("琥珀阿比猫_对话", $"琥珀阿比猫_对话_意义封神_{数据.已选字灵}", string.Empty, null);

        意义封神MC.Hide();
        誓言命定MC.Show();
    }

    private void 刷新誓言命定模块的回调函数()
    {
        if (!誓言命定MC.Visible) return;

        誓言命定选字标签.Text = 数据.已选字灵;
        誓言命定LE.Text = string.Empty;
    }

    private async void 誓言命定按钮被按下的回调函数()
    {
        string result = 誓言命定LE.Text;
        if (result.Contains(数据.已选字灵))
        {
            await 剧情演绎单例脚本实例.播放剧情的方法("琥珀阿比猫_对话", "琥珀阿比猫_对话_誓言命定", "正确", null);

            生成卡牌道具的封装体方法();
            记录游戏行为数据的方法();
            重选的回调函数(誓言命定MC);
        }
        else
        {
            _ = 剧情演绎单例脚本实例.播放剧情的方法("琥珀阿比猫_对话", "琥珀阿比猫_对话_誓言命定", "错误", null);
        }
    }

    private void 生成卡牌道具的封装体方法()
    {
        var 卡牌库存 = 玩家数据单例脚本实例.玩家存档数据.物品库字典[物品参数类脚本.物品类型Enum.卡牌];
        var 字灵库存 = 玩家数据单例脚本实例.玩家存档数据.字灵字典;
        if (卡牌库存.ContainsKey(数据.已选字灵))
        {
            卡牌库存[数据.已选字灵]++;
        }
        else
        {
            卡牌库存[数据.已选字灵] = 1;
        }

        字灵库存[数据.已选字灵]--;
    }

    private void 记录游戏行为数据的方法()
    {
        var 游戏行为 = 玩家数据单例脚本实例.游戏行为数据;

        游戏行为.锻造完成时间[数据.已选字灵] = DateTime.Now.ToString("HH:mm:ss");
    }
}
