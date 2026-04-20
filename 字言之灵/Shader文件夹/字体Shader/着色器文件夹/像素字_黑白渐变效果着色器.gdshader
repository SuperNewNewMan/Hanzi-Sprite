shader_type canvas_item;

// 描边大小
uniform float outline_size = 2.0;
uniform float cutoff = 0.5;
uniform float time_speed = 1.0;

uniform vec4 color_a : source_color = vec4(1.0, 1.0, 1.0, 1.0); // 默认白
uniform vec4 color_b : source_color = vec4(0.0, 0.0, 0.0, 1.0); // 默认黑

void fragment() {
    vec4 tex = texture(TEXTURE, UV);

    // 硬边透明
    float alpha = tex.a >= cutoff ? 1.0 : 0.0;

    // 主体像素
    if (alpha > 0.5) {
        COLOR = vec4(tex.rgb, 1.0);
    } else {
        // 检测周围是否有主体像素
        float outline = 0.0;
        for (int x = -1; x <= 1; x++) {
            for (int y = -1; y <= 1; y++) {
                vec2 offset = vec2(float(x), float(y)) * outline_size * TEXTURE_PIXEL_SIZE;
                float a = texture(TEXTURE, UV + offset).a;
                if (a >= cutoff) {
                    outline = 1.0;
                }
            }
        }

        if (outline > 0.5) {
            float t = TIME * time_speed;
            float wave = (sin(t + UV.y * 10.0) + 1.0) * 0.5;
            vec3 col = mix(color_a.rgb, color_b.rgb, wave);
            COLOR = vec4(col, 1.0);
        } else {
            COLOR = vec4(0.0);
        }
    }
}