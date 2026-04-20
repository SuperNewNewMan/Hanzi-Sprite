shader_type canvas_item;

uniform float cutoff = 0.5;

// 灵气最大扩散范围（像素）
uniform float aura_range = 4.0;

// 七彩速度
uniform float rainbow_speed = 0.6;

// 灵气波动速度
uniform float aura_speed = 2.0;

// 灵气强度
uniform float aura_strength = 0.4;

// HSV → RGB
vec3 hsv2rgb(vec3 c) {
    vec3 rgb = clamp(abs(mod(c.x * 6.0 + vec3(0.0,4.0,2.0), 6.0) - 3.0) - 1.0,
                     0.0,
                     1.0);
    return c.z * mix(vec3(1.0), rgb, c.y);
}

void fragment() {
    vec4 tex = texture(TEXTURE, UV);
    float alpha = tex.a >= cutoff ? 1.0 : 0.0;

    vec4 out_color = vec4(0.0);

    // 主体像素
    if (alpha > 0.5) {
        out_color = vec4(tex.rgb, 1.0);
    } else {
        float min_dist = 999.0;

        // 扩散 aura：检测 aura_range 范围内的所有像素
        for (int x = -8; x <= 8; x++) {
            for (int y = -8; y <= 8; y++) {
                vec2 offset = vec2(float(x), float(y)) * TEXTURE_PIXEL_SIZE;
                float a = texture(TEXTURE, UV + offset).a;

                if (a >= cutoff) {
                    float d = length(vec2(float(x), float(y)));
                    min_dist = min(min_dist, d);
                }
            }
        }

        // 如果在 aura 范围内
        if (min_dist <= aura_range) {

            // 七彩色相
            float hue = mod(TIME * rainbow_speed + min_dist * 0.1, 1.0);

            // 灵气波动
            float wave = sin(TIME * aura_speed + min_dist * 2.0) * 0.3;

            // **外溢逻辑：越远越亮**
            float fade = max(0.0, (min_dist + wave) / aura_range);

            vec3 rgb = hsv2rgb(vec3(hue, 1.0, 1.0));
            out_color = vec4(rgb, fade * aura_strength);
        }
    }

    COLOR = out_color;
}