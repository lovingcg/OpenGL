#version 330 core
out vec4 FragColor;

in vec3 TexCoords;

uniform samplerCube skybox;
// uniform float dx;
// uniform float dy;

void main()
{    
    //vec3 TexCoordsdx=vec3(TexCoords.x+dx,TexCoords.y+dy,TexCoords.z);
    FragColor = texture(skybox, TexCoords);
    //FragColor = texture(skybox, TexCoordsdx);
}