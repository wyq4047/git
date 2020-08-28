#version 330 core
out vec4 FragColor; 
in vec2 TexCoords;
in vec3 WorldPos;
in vec3 Normal;

uniform vec3 camPos;

uniform vec3  albedo; //基础反射率
uniform float metallic;
uniform float roughness;
uniform float ao; 


// lights
uniform vec3 lightPositions[4];
uniform vec3 lightColors[4];
const int max_lights = 4;


const float PI = 3.14159265359;

float DistributionGGX(vec3 N, vec3 H, float roughness);
float GeometrySchlickGGX(float NdotV, float roughness);
float GeometrySmith(vec3 N, vec3 V, vec3 L, float roughness);
vec3 fresnelSchlickRoughness(float cosTheta, vec3 F0 ); 

void main(){
    vec3 N = normalize(Normal);
    vec3 V = normalize(camPos - WorldPos);

    vec3 F0 = vec3(0.04); 
    F0 = mix(F0, albedo, metallic);

    // reflectance equation
    vec3 Lo = vec3(0.0);
	for(int i = 0 ; i < max_lights; i ++){
		
		vec3 L = normalize(lightPositions[i] - WorldPos);
		vec3 H = (L + V) / length(L+V);
		//light L
		float distance = length(lightPositions[i] - WorldPos);
		float attenuation = 1.0 / (distance*distance);
		vec3 radiance = lightColors[i] / attenuation;
		
		//cook-torrance
		float NDF = DistributionGGX(N, H, roughness);        
        float G   = GeometrySmith(N, V, L, roughness);      
        vec3 F    = fresnelSchlick(max(dot(H, V), 0.0), F0);  
		vec3 kS = F;
		vec3 kD = vec3(1.0) - F;
		kD *= 1.0 - metallic;
		
		vec3 nominator    = NDF * G * F;
        float denominator = 4.0 * max(dot(N, V), 0.0) * max(dot(N, L), 0.0) + 0.001; 
        vec3 specular     = nominator / denominator;
		
		float NdotL = max(dot(N, L), 0.0);                
        Lo += (kD * albedo / PI + specular) * radiance * NdotL; 
		
	}
	
	vec3 ambient = vec3(0.03) * albedo * ao;
    vec3 color = ambient + Lo;

    color = color / (color + vec3(1.0));
    color = pow(color, vec3(1.0/2.2));  

    FragColor = vec4(color, 1.0);

}

vec3 fresnelSchlickRoughness(float cosTheta, vec3 F0 ){
	return F0 + (1-F0)pow((1-costheta),5);
}


float DistributionGGX(vec3 N, vec3 H, float roughness){
	float a2 = roughness* roughness;
	float nh = max(dot(N,H),0.0);
	float nh2 = nh* nh;
	float down = nh2*(a2-1) + 1;
	down = PI * down * down;
	return a2 / down;
}


float GeometrySmith(vec3 N, vec3 V, vec3 L, float roughness){
	float ndotv = max(dot(N,V),0.0);
	float ndotl = max(dot(N,L),0,0);
	float g1 = GeometrySmith(ndotv,roughness);
	float g2 = GeometrySmith(ndotl,roughness);
	
	return g1*g2;
}


float GeometrySchlickGGX(float NdotV, float roughness){
{
    float r = (roughness + 1.0);
    float k = (r*r) / 8.0;

    float nom   = NdotV;
    float denom = NdotV * (1.0 - k) + k;

    return nom / denom;

}
















