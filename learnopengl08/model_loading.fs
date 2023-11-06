#version 330 core
out vec4 FragColor;

uniform sampler2D texture_diffuse1;
uniform sampler2D texture_specular1;

uniform vec3 lightColor;
uniform float lightLinear;
uniform float lightQuadratic;

in vec3 FragPos;  
in vec3 Normal;  
in vec2 texCoords;
  
uniform vec3 viewPos;
uniform vec3 lightPos;

void main()
{
    vec3 Diffuse = texture(texture_diffuse1, texCoords).rgb;
    // ambient
    vec3 ambient = vec3(0.3 * Diffuse);
    vec3 lighting  = ambient; 
    //vec3 ambient = light.ambient * texture(material.diffuse, TexCoords).rgb;
  	
    // diffuse 
    vec3 norm = normalize(Normal);
    vec3 lightDir = normalize(lightPos - FragPos);
    float diff = max(dot(norm, lightDir), 0.0);
    vec3 diffuse = max(dot(norm, lightDir), 0.0) * Diffuse * lightColor;
    //vec3 diffuse = light.diffuse * diff * texture(material.diffuse, TexCoords).rgb;  

    vec3 viewDir = normalize(viewPos - FragPos);
    vec3 halfwayDir = normalize(lightDir + viewDir);  
    float spec = pow(max(dot(norm, halfwayDir), 0.0), 32.0);
    vec3 specular = lightColor * spec * 0.1f;
    
    // attenuation
    float distance = length(lightPos - FragPos);
    float attenuation = 1.0 / (1.0 + lightLinear * distance + lightQuadratic * distance * distance);
    diffuse *= attenuation;
    specular *= attenuation;
    lighting += diffuse + specular;

    // specular
    // vec3 viewDir = normalize(viewPos - FragPos);
    // vec3 reflectDir = reflect(-lightDir, norm);  
    // float spec = pow(max(dot(viewDir, reflectDir), 0.0), material.shininess);
    // vec3 specular = light.specular * spec * texture(material.specular, TexCoords).rgb;  

    FragColor = vec4(lighting , 1.0f);
    // vec3 result = ambient + diffuse + specular;
    // FragColor = vec4(result, 1.0);
} 