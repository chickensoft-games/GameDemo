// Fireworks shader
// https://godotshaders.com/shader/fireworks/
// Based on shadertoy shader by Martijn Steinrucken aka BigWings - 2015 
// (https://www.shadertoy.com/view/lscGRl)

shader_type canvas_item;

uniform float TWOPI = 6.283185307179586 ;
uniform float NUM_EXPLOSIONS = 8.0;
uniform float NUM_PARTICLES = 70.0;

// Noise functions by Dave Hoskins

uniform vec3 MOD3 = vec3(0.1031,0.11369,0.13787);

vec3 hash31(float p) {
   vec3 p3 = fract(vec3(p) * MOD3);
   p3 += dot(p3, p3.yzx + 19.19);
   return fract(vec3((p3.x + p3.y)*p3.z, (p3.x+p3.z)*p3.y, (p3.y+p3.z)*p3.x));
}

float hash12(vec2 p) // attention changement vec2 p en vec3
{
                vec3 p3  = fract(vec3(p.xy, 0.0) * MOD3);
    p3 += dot(p3, p3.yzx + 19.19);
    return fract((p3.x + p3.y) * p3.z);
}

float circ(vec2 uv, vec2 pos, float size) {
                uv -= pos;
    
    size *= size;
    return smoothstep(size*1.1, size, dot(uv, uv));
}

float lighter(vec2 uv, vec2 pos, float size) {
    uv -= pos;
    size *= size;
    return size/dot(uv, uv);
}

vec3 explosion(vec2 uv, vec2 p, float seed, float t) {
                
    vec3 col = vec3(0.);
    
    vec3 en = hash31(seed);
    vec3 baseCol = en;
    for(float i=0.; i<NUM_PARTICLES; i++) {
               vec3 n = hash31(i)-.5;
       
                               vec2 startP = p-vec2(0., t*t*.1);        
        vec2 endP = startP+normalize(n.xy)*n.z;
        
        
        float pt = 1.-pow(t-1., 2.);
        vec2 pos = mix(p, endP, pt);    
        float size = mix(.01, .005, smoothstep(0., .1, pt));
        size *= smoothstep(1., .1, pt);
        
        float sparkle = (sin((pt+n.z)*100.)*.5+.5);
        sparkle = pow(sparkle, pow(en.x, 3.)*50.)*mix(0.01, .01, en.y*n.y);
		float B = smoothstep(en.x-en.z, en.x+en.z, t)*smoothstep(en.y+en.z, en.y-en.z, t);
        size += sparkle*B;
        
        col += baseCol*lighter(uv, pos, size);
    }
    
    return col;
}


void fragment() {
    vec2 iResolution = vec2(1920,1080);
	vec2 uv = FRAGCOORD.xy / iResolution.xy;
                uv.x -= .5;
    uv.x *= iResolution.x/iResolution.y;
    
    float n = hash12(uv+10.0);
    float t = TIME*.5;
    
    vec3 c = vec3(0.);
    
    for(float i=0.; i<NUM_EXPLOSIONS; i++) {
               float et = t+i*1234.45235;
        float id = floor(et);
        et -= id;
        
        vec2 p = hash31(id).xy;
        p.x -= .5;
        p.x *= 1.6;
        c += explosion(uv, p, id, et);
		}
    
	float alpha = c.r + c.g + c.b / 3.0;
    COLOR = vec4(c, alpha);
}