shader_type spatial;

group_uniforms colors;
uniform vec3 body_color : source_color;
uniform vec3 leg_color : source_color;
uniform vec3 comb_color : source_color;

group_uniforms surface;
uniform float specular : hint_range(0.0, 1.0, 0.1) = 0.6;
uniform float roughness : hint_range(0.0, 1.0, 0.1) = 0.65;

// Fresnel function from Godot Shaders
// https://godotshaders.com/snippet/fresnel/
float fresnel(float amount, vec3 normal, vec3 view)
{
	return pow((1.0 - clamp(dot(normalize(normal), normalize(view)), 0.0, 1.0 )), amount);
}


void fragment() {
	float f = fresnel(1.0, NORMAL, VIEW);
	f = clamp(smoothstep(0.6, 0.8, f) * NORMAL.y, 0.0, 1.0);
	vec3 color = mix(body_color, comb_color, COLOR.r);
	color = mix(color, leg_color, COLOR.g);
	color = mix(color, vec3(0.0), COLOR.b);
	ALBEDO = color;
	EMISSION = color * f * 2.0;
	SPECULAR = specular;
	ROUGHNESS = mix(roughness, 0.4, COLOR.b);
	SSS_STRENGTH = 0.025 * (1.0 - COLOR.b);
}