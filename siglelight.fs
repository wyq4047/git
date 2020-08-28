#version 330 core
in vec4 fragpos;

uniform lightpos;
uniform float far_plane;

void main(){
	float d = distance(lightpos,fragpos.xyz);
	gl_FragDepth = d/far_plane;
}
