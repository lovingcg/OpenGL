/*
#version 330 core
layout (location = 0) in vec3 aPos;
layout (location = 1) in vec3 aNormal;
layout (location = 2) in vec2 aTexCoords;

out vec3 FragPos;
out vec3 Normal;
out vec2 TexCoords;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

void main()
{
    FragPos = vec3(model * vec4(aPos, 1.0));
    Normal = mat3(transpose(inverse(model))) * aNormal;
    TexCoords = aTexCoords;
    gl_Position = projection * view * model * vec4(aPos, 1.0);
}
*/

#version 330 core
layout (location = 0) in vec3 aPos;
layout (location = 1) in vec3 aNormal;
layout (location = 2) in vec2 aTexCoords;
layout (location = 3) in vec3 aTangent;
//layout (location = 4) in vec3 aBitTangent;
 
out vec3 tFragPos;
out vec2 texCoords;
out vec3 tViewPos;
out vec3 tLightPos;
 
uniform vec3 viewPos;
uniform vec3 lightPos;
 
uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;
 
void main()
{
    gl_Position = projection * view * model * vec4(aPos,1.0f);
    texCoords = aTexCoords;
    // calculate TBN
    mat3 normalMatrix = transpose(inverse(mat3(model)));
    vec3 T = normalize(normalMatrix * aTangent);
    vec3 N = normalize(normalMatrix * aNormal);
    T = normalize(T - dot(T,N) * N);        //Gram-Schmidt process
    vec3 B = normalize(cross(T,N));
 
    mat3 TBN = transpose(mat3(T,B,N));            //the transpose and inverse is the same
 
    tFragPos = TBN * vec3(model * vec4(aPos,1.0f)); // this should be in tangent space
    tViewPos = TBN * viewPos;
    tLightPos = TBN * lightPos;
}
