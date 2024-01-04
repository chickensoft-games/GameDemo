shader_type canvas_item;

uniform sampler2D color_gradient;
uniform sampler2D noiseTexture;
uniform float speed: hint_range (0.0,5.0);
uniform float variation: hint_range (0.0,3.0);
uniform float width: hint_range (0.0,1.0);
uniform float opacity: hint_range (0.0,1.0);

void fragment() {
	vec2 noise_uv = vec2(UV.x + TIME * speed, UV.y - TIME*speed);
	float noise_sample = texture(noiseTexture, noise_uv).r;
	float x_offset = noise_sample*variation - variation/2.0;
	vec4 col = texture(color_gradient, vec2((UV.x-0.5)/width + 0.5 + x_offset/width, UV.y));
	col.a = opacity;
	COLOR = col;
}