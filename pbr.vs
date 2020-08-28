#version 330 core

layout (location =0) position;
layout (location =1) normal;
layout (location = 2) texcoord;

out vec3 WorldPos;
out vec3 Normal;
out vec2 Texcoord;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

void main(){
	gl_Position = projection * view * model * vec4(position,1.0);
	WorldPos = vec3(model * vec4(position,1.0));
	Normal = vec3(transpose( inverse(model)) * vec4(normal));
	Texcoord = texcoord;
}