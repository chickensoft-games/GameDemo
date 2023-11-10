shader_type spatial;
render_mode depth_draw_opaque;

uniform float height_scale = 0.5;
uniform float wave_speed = 0.1;
uniform float upper_transparency = 0.89;
uniform float global_transparency = 0.9;

uniform vec2 time_factor = vec2(2.0, 3.0);
uniform sampler2D texture_albedo : source_color;
uniform vec3 uv1_scale;
uniform vec3 uv1_offset;

uniform vec4 color1 : source_color = vec4(0.78, 0.91, 0.93, 1.0);
uniform vec4 color2 : source_color = vec4(0.71, 0.89, 0.95, 1.0);

varying flat vec3 out_color;
varying flat vec3 soft_color;

vec3 lerpColor(vec4 a, vec4 b, float t){
	float rr = a.r + (b.r - a.r) * t;
	float gg = a.g + (b.g - a.g) * t;
	float bb = a.b + (b.b - a.b) * t;
	return vec3(rr, gg, bb);
	}

float hash(vec2 p) {
  return fract(sin(dot(p * 17.17, vec2(14.91, 67.31))) * 4791.9511);
}



float noise(vec2 x) {
  vec2 p = floor(x);
  vec2 f = fract(x);
  f = f * f * (3.0 - 2.0 * f);
  vec2 a = vec2(1.0, 0.0);
  return mix(mix(hash(p + a.yy), hash(p + a.xy), f.x),
         mix(hash(p + a.yx), hash(p + a.xx), f.x), f.y);
}

float fbm(vec2 x, float time) {
  float height = 0.0;
  float amplitude = 0.5;
  float frequency = 0.5;
  for (int i = 0; i < 6; i++){
    height += noise(x * frequency + time * time_factor * wave_speed) * amplitude;
    amplitude *= 0.5;
    frequency *= 2.0;
  }
  return height;
}

void vertex() {
	
	UV=UV*uv1_scale.xy+uv1_offset.xy;
	
	out_color = vec3(color1.r, color1.g, color1.b);
	soft_color = vec3(color1.r, color1.g, color1.b);
	
	float height = fbm(VERTEX.xz * 4.0, TIME);
	VERTEX.y += height * height_scale;
	COLOR.xyz = vec3(height);
	
	if (VERTEX.y > 0.3){
		out_color = lerpColor(color1, color2, clamp((VERTEX.y) / 3.0, 0.5, 1.0));
		soft_color = vec3(color2.r, color2.g, color2.b);
		
		
		
	}
}



void fragment(){
	
	ALBEDO = COLOR.xyz;
	vec2 base_uv = UV;
	vec4 albedo_tex = texture(texture_albedo,base_uv);
	ALPHA = 1.0; // global_transparency;
	ALBEDO = out_color * albedo_tex.rgb * COLOR.xyz;
	 if (soft_color.r==color2.r && soft_color.g==color2.g && color2.b==color2.b) {   
       ALPHA = upper_transparency;
    }
	EMISSION = vec3(0.4);
}