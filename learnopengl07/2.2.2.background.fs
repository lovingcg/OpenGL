#version 330 core
out vec4 FragColor;
in vec3 WorldPos;

uniform samplerCube environmentMap;
uniform int flag;

void main()
{
    vec3 envColor;
    if(flag==0 || flag==1 )
        envColor = textureLod(environmentMap, WorldPos, 0.0).rgb;
    if(flag==2 || flag==3|| flag==4)
    {
        vec3 worldPos;
        if(WorldPos.y>0 || WorldPos.y<0)
            worldPos=vec3(WorldPos.x,-WorldPos.y,WorldPos.z);
        else if(WorldPos.x>0 || WorldPos.x<0)
            worldPos=vec3(-WorldPos.x,WorldPos.y,WorldPos.z);
        else if(WorldPos.z>0 || WorldPos.z<0)
            worldPos=vec3(WorldPos.x,WorldPos.y,-WorldPos.z);
        envColor = textureLod(environmentMap, worldPos, 0.0).rgb;
    }
    // HDR tonemap and gamma correct
    envColor = envColor / (envColor + vec3(1.0));
    envColor = pow(envColor, vec3(1.0/2.2)); 
    
    FragColor = vec4(envColor, 1.0);
}
