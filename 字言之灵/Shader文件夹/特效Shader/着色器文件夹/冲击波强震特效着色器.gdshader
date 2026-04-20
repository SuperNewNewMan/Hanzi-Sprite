shader_type canvas_item;

uniform sampler2D screen_tex : hint_screen_texture;

uniform vec2 center = vec2(0.5, 0.5);

// 只需要一个 progress（0 → 1）
uniform float progress = 0.0;

// 波纹宽度
uniform float width = 0.1;
// 扭曲强度
uniform float strength = 0.05;
// 拖尾时长
uniform float trail = 0.15;
// 衰减强度
uniform float falloff = 1.4;

// 屏幕震动强度（0为无震动，1为正常，2为强震，以此类推……）
uniform float shake_strength = 1.0;

void fragment() {
    vec2 uv = SCREEN_UV;
    vec2 dir = uv - center;
	// 当前像素到中心的距离
    float dist = length(dir);

    // radius 由 progress 推导
    float radius = progress * 1.2;

    // 主波前
    float front = 1.0 - abs(dist - radius) / width;
    front = clamp(front, 0.0, 1.0);

    // 拖尾（修正顺序）
    float tail = smoothstep(radius - trail, radius, dist);

    float wave = front + tail * 0.3;

    // 距离衰减
    wave *= exp(-dist * falloff);

    // 时间淡出（关键）
    float fade = smoothstep(1.0, 0.7, progress);
    wave *= fade;
	
	// 屏幕震动
	// 波前越强，震动越强
	float shake = pow(front, 2.0) * 0.02 * shake_strength;
	float t = progress * 200.0;
	uv += vec2(sin(t + dist * 20.0), cos(t + dist * 25.0)) * shake;

    // 扭曲
    if (dist > 0.0001) {
        uv += normalize(dir) * wave * strength;
    }

    COLOR = texture(screen_tex, uv);
}
