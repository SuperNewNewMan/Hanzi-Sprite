	shader_type canvas_item;

	uniform int mode = 1;                           // 0=出现, 1=消散, 2=周期性
	uniform float progress = 0.0;                   // 0~1 动画进度（外部驱动）
	uniform vec4 ColorParameter : source_color = vec4(1.0);
	uniform float noise_offset = 0.01;
	uniform float grid_size = 25.0;
	uniform float edge_power = 15.0;
	uniform sampler2D PerlinNoise;

	void fragment() {

	vec4 base_color = texture(TEXTURE, UV);
	float is_transparent = step(base_color.a, 0.0);

	// --- Quantized UV (grid snapping) ---
	vec2 uv_q = ceil((UV + noise_offset) * grid_size) / grid_size;
	vec2 uv_q2 = ceil(UV * grid_size) / grid_size;

	// Combine both quantized UVs
	vec2 noise_uv = (uv_q - uv_q2) * vec2(1.5) + uv_q2;

	// --- Sample noise ---
	float noise_val = texture(PerlinNoise, noise_uv).r;

	// ------------------------------------------------------------
	// 3 种模式的 threshold 计算
	// ------------------------------------------------------------

	float threshold = 0.0;

	float p = clamp(progress, 0.0, 1.0);   // 这里锁死范围

	if (mode == 0) {
	    // -------------------------
	    // 模式 0：出现（从无到有）
	    // progress 从 0→1，逐渐显现
	    // -------------------------
	    threshold = 1.0 - p;                  // 越大越多像素出现
	}
	else if (mode == 1) {
	    // -------------------------
	    // 模式 1：消散（从有到无）
	    // progress 从 0→1，逐渐消失
	    // -------------------------
	    threshold = p;            // 越小越多像素消失
	}
	else {
	    // -------------------------
	    // 模式 2：周期性变化
	    // 使用 sin(TIME) 作为阈值
	    // -------------------------
	    float t = abs(sin(TIME * 0.5));        // 0~1
	    threshold = t;                         // 来回溶解
	}

	threshold = clamp(threshold, 0.0, 1.0);

	// --- Dissolve logic ---
	float dissolve_raw = 1.0 - (noise_val - threshold);
	float dissolve = pow(dissolve_raw, edge_power);

	vec4 dissolve_color = ColorParameter * dissolve;

	// Alpha step (hard cut)
	float alpha_cut = ceil(noise_val - threshold);

	vec4 final_color = vec4(dissolve_color.rgb + base_color.rgb, alpha_cut);

	COLOR = mix(final_color, base_color, is_transparent);
	}
