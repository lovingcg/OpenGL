// #version 330 core
// out vec4 FragColor;

// uniform Mat{
//     vec4 aAmbient;
//     vec4 Diffuse;
//     vec4 aSpecular;
// };
// struct DirLight {
//     vec3 direction;

//     vec3 ambient;
//     vec3 diffuse;
//     vec3 specular;
// };

// struct PointLight {
//     vec3 position;

//     float constant;
//     float linear;
//     float quadratic;

//     vec3 ambient;
//     vec3 diffuse;
//     vec3 specular;
// };

// struct SpotLight {
//     vec3 position;
//     vec3 direction;
//     float cutOff;
//     float outerCutOff;

//     float constant;
//     float linear;
//     float quadratic;

//     vec3 ambient;
//     vec3 diffuse;
//     vec3 specular;
// };

// #define NR_POINT_LIGHTS 4

// in vec3 FragPos;
// in vec3 Normal;
// in vec2 TexCoords;
// in mat3 TBN;

// uniform vec3 viewPos;
// uniform DirLight dirLight;
// uniform PointLight pointLights[NR_POINT_LIGHTS];
// uniform SpotLight spotLight;
// uniform sampler2D texture_diffuse1;
// uniform sampler2D texture_normal1;
// uniform sampler2D texture_specular1;
// uniform bool hasTexture;

// vec3 CalcDirLight(DirLight light, vec3 normal, vec3 viewDir);
// vec3 CalcPointLight(PointLight light, vec3 normal, vec3 fragPos, vec3 viewDir);
// vec3 CalcSpotLight(SpotLight light, vec3 normal, vec3 fragPos, vec3 viewDir);

// void main()
// {
//     //vec3 norm = normalize(Normal);
//     vec3 norm = texture(texture_normal1, TexCoords).rgb;
//     norm = normalize(norm * 2.0 - 1.0);  // this normal is in tangent space
//     norm = normalize(TBN * norm);
//     vec3 viewDir = normalize(viewPos - FragPos);
//     vec3 result = CalcDirLight(dirLight, norm, viewDir);
//     // phase 2: point lights
//     for (int i = 0; i < NR_POINT_LIGHTS; i++)
//         result += CalcPointLight(pointLights[i], norm, FragPos, viewDir);
//     // phase 3: spot light
//     result += CalcSpotLight(spotLight, norm, FragPos, viewDir);
//     FragColor = vec4(result, 1.0);
// }

// vec3 CalcDirLight(DirLight light, vec3 normal, vec3 viewDir)
// {
//     vec3 lightDir = normalize(-light.direction);
//     // diffuse shading
//     float diff = max(dot(normal, lightDir), 0.0);
//     // specular shading
//     vec3 reflectDir = reflect(-lightDir, normal);
//     float spec = pow(max(dot(viewDir, reflectDir), 0.0), 32.0f);
//     vec3 diffuse, specular;
//     // combine results
//     if (hasTexture)
//     {
//         diffuse = light.diffuse * diff * vec3(texture(texture_diffuse1, TexCoords));
//         specular = light.specular * spec * vec3(texture(texture_specular1, TexCoords));
//         return (diffuse + specular);
//     }
//     else
//     {
//         diffuse = light.diffuse * diff * Diffuse.rgb;
//         specular = light.specular * spec * aSpecular.rgb;
//         vec3 ambient = light.ambient * aAmbient.rgb;
//         return (ambient + diffuse + specular);
//     }
// }

// // calculates the color when using a point light.
// vec3 CalcPointLight(PointLight light, vec3 normal, vec3 fragPos, vec3 viewDir)
// {
//     vec3 lightDir = normalize(light.position - fragPos);
//     // diffuse shading
//     float diff = max(dot(normal, lightDir), 0.0);
//     // specular shading
//     vec3 reflectDir = reflect(-lightDir, normal);
//     float spec = pow(max(dot(viewDir, reflectDir), 0.0), 32.0f);
//     // attenuation
//     float distance = length(light.position - fragPos);
//     float attenuation = 1.0 / (light.constant + light.linear * distance + light.quadratic * (distance * distance));
//     // combine results
//     if (hasTexture)
//     {
//         vec3 diffuse = light.diffuse * diff * vec3(texture(texture_diffuse1, TexCoords));
//         vec3 specular = light.specular * spec * vec3(texture(texture_specular1, TexCoords));
//         diffuse *= attenuation;
//         specular *= attenuation;
//         return (diffuse + specular);
//     }
//     else
//     {
//         vec3 diffuse = light.diffuse * diff * Diffuse.rgb;
//         vec3 specular = light.specular * spec * aSpecular.rgb;
//         vec3 ambient = light.ambient * aAmbient.rgb;
//         ambient *= attenuation;
//         diffuse *= attenuation;
//         specular *= attenuation;
//         return (ambient + diffuse + specular);
//     }
// }

// // calculates the color when using a spot light.
// vec3 CalcSpotLight(SpotLight light, vec3 normal, vec3 fragPos, vec3 viewDir)
// {
//     vec3 lightDir = normalize(light.position - fragPos);
//     // diffuse shading
//     float diff = max(dot(normal, lightDir), 0.0);
//     // specular shading
//     vec3 reflectDir = reflect(-lightDir, normal);
//     float spec = pow(max(dot(viewDir, reflectDir), 0.0), 32.0f);
//     // attenuation
//     float distance = length(light.position - fragPos);
//     float attenuation = 1.0 / (light.constant + light.linear * distance + light.quadratic * (distance * distance));
//     // spotlight intensity
//     float theta = dot(lightDir, normalize(-light.direction));
//     float epsilon = light.cutOff - light.outerCutOff;
//     float intensity = clamp((theta - light.outerCutOff) / epsilon, 0.0, 1.0);
//     // combine results
//     if (hasTexture)
//     {
//         vec3 diffuse = light.diffuse * diff * vec3(texture(texture_diffuse1, TexCoords));
//         vec3 specular = light.specular * spec * vec3(texture(texture_specular1, TexCoords));
//         diffuse *= attenuation * intensity;
//         specular *= attenuation * intensity;
//         return (diffuse + specular);
//     }
//     else
//     {
//         vec3 diffuse = light.diffuse * diff * Diffuse.rgb;
//         vec3 specular = light.specular * spec * aSpecular.rgb;
//         vec3 ambient = light.ambient * aAmbient.rgb;
//         ambient *= attenuation * intensity;
//         diffuse *= attenuation * intensity;
//         specular *= attenuation * intensity;
//         return (ambient + diffuse + specular);
//     }


// }
//#version 330 core
//out vec4 FragColor;
//
//in VS_OUT{
//    vec3 FragPos;
//    vec2 TexCoords;
//    vec3 TangentLightPos;
//    vec3 TangentViewPos;
//    vec3 TangentFragPos;
//} fs_in;
//
//uniform sampler2D texture_diffuse1;
//uniform sampler2D texture_normal1;
//uniform sampler2D texture_specular1;
//
//uniform vec3 lightPos;
//uniform vec3 viewPos;
//
//void main()
//{
//    // obtain normal from normal map in range [0,1]
//    vec3 normal = texture(texture_normal1, fs_in.TexCoords).rgb;
//    // transform normal vector to range [-1,1]
//    normal = normalize(normal * 2.0 - 1.0);  // this normal is in tangent space
//
//    // get diffuse color
//    vec3 color = texture(texture_diffuse1, fs_in.TexCoords).rgb;
//    // ambient
//    vec3 ambient = 0.1 * color;
//    // diffuse
//    vec3 lightDir = normalize(fs_in.TangentLightPos - fs_in.TangentFragPos);
//    float diff = max(dot(lightDir, normal), 0.0);
//    vec3 diffuse = diff * color;
//    // specular
//    vec3 viewDir = normalize(fs_in.TangentViewPos - fs_in.TangentFragPos);
//    vec3 reflectDir = reflect(-lightDir, normal);
//    vec3 halfwayDir = normalize(lightDir + viewDir);
//    float spec = pow(max(dot(normal, halfwayDir), 0.0), 32.0);
//
//    vec3 specular = vec3(0.2) * spec * vec3(texture(texture_specular1, fs_in.TexCoords));
//    FragColor = vec4(ambient + diffuse + specular, 1.0);
//}
#version 330 core
 
in vec2 texCoords;
in vec3 tFragPos;
in vec3 tViewPos;
in vec3 tLightPos;
 
uniform vec3 lightColor;
uniform float lightLinear;
uniform float lightQuadratic;
 
uniform sampler2D texture_diffuse1;
uniform sampler2D texture_specular1;
uniform sampler2D texture_normal1;
//uniform sampler2D texture_height1;
 
out vec4 fragColor;
void main()
{
    vec3 Normal = texture(texture_normal1, texCoords).rgb;
 
    Normal = normalize(Normal * 2 - 1.0f) * 1.06f;
 
    vec3 Diffuse = texture(texture_diffuse1, texCoords).rgb;
    float specStrength = texture(texture_specular1,texCoords).r;
    
    // then calculate lighting as usual
    vec3 ambient = vec3(0.3 * Diffuse);
    vec3 lighting  = ambient; 
    vec3 viewDir  = normalize(tViewPos - tFragPos); 
    // diffuse
    vec3 lightDir = normalize(tLightPos - tFragPos);
    vec3 diffuse = max(dot(Normal, lightDir), 0.0) * Diffuse * lightColor;
    // specular
    vec3 halfwayDir = normalize(lightDir + viewDir);  
    float spec = pow(max(dot(Normal, halfwayDir), 0.0), 32.0);
    vec3 specular = lightColor * spec * 0.1f;
    
    // attenuation
    float distance = length(tLightPos - tFragPos);
    float attenuation = 1.0 / (1.0 + lightLinear * distance + lightQuadratic * distance * distance);
    diffuse *= attenuation;
    specular *= attenuation;
    lighting += diffuse + specular;
 
    fragColor = vec4(lighting , 1.0f);
}