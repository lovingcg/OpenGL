#version 330 core
out vec4 FragColor;

in VS_OUT {
    vec3 FragPos;
    vec3 Normal;
    vec2 TexCoords;
    vec4 FragPosLightSpace;
} fs_in;

uniform sampler2D diffuseTexture;
uniform sampler2D shadowMap;

uniform vec3 lightPos;
uniform vec3 viewPos;

// Shadow map related variables
#define NUM_SAMPLES 100
#define BLOCKER_SEARCH_NUM_SAMPLES NUM_SAMPLES
#define PCF_NUM_SAMPLES NUM_SAMPLES
#define NUM_RINGS 10

#define PI 3.141592653589793
#define PI2 6.283185307179586
float Stride=4;

vec2 poissonDisk[NUM_SAMPLES];

highp float rand_2to1(vec2 uv ) { 
    // 0 - 1
	const highp float a = 12.9898, b = 78.233, c = 43758.5453;
	highp float dt = dot( uv.xy, vec2( a,b ) ), sn = mod( dt, PI );
	return fract(sin(sn) * c);
}
void poissonDiskSample( const in vec2 randomSeed ) {

  float ANGLE_STEP = PI2 * float( NUM_RINGS ) / float( NUM_SAMPLES );
  float INV_NUM_SAMPLES = 1.0 / float( NUM_SAMPLES );

  float angle = rand_2to1( randomSeed ) * PI2;
  float radius = INV_NUM_SAMPLES;
  float radiusStep = radius;

  for( int i = 0; i < NUM_SAMPLES; i ++ ) {
    poissonDisk[i] = vec2( cos( angle ), sin( angle ) ) * pow( radius, 0.75 );
    radius += radiusStep;
    angle += ANGLE_STEP;
  }
}

float poissonDiskSamples(vec4 fragPosLightSpace)
{
    vec3 projCoords = fragPosLightSpace.xyz / fragPosLightSpace.w;
    // transform to [0,1] range
    projCoords = projCoords * 0.5 + 0.5;
    // get closest depth value from light's perspective (using [0,1] range fragPosLight as coords)
    // float closestDepth = texture(shadowMap, projCoords.xy).r; 
    // get depth of current fragment from light's perspective
    float currentDepth = projCoords.z;
    vec3 normal = normalize(fs_in.Normal);
    vec3 lightDir = normalize(lightPos - fs_in.FragPos);
    float bias = max(0.05 * (1.0 - dot(normal, lightDir)), 0.005);
    float shadow = 0.0;
    vec2 texelSize = Stride / textureSize(shadowMap, 0);
    poissonDiskSample(projCoords.xy);
    for(int i=0;i<NUM_SAMPLES;i++)
    {
        float pcfDepth = texture(shadowMap, projCoords.xy + poissonDisk[i] * texelSize).r; 
        shadow += currentDepth - bias > pcfDepth  ? 1.0 : 0.0;       
    }
    shadow /= NUM_SAMPLES;
    
    // keep the shadow at 0.0 when outside the far_plane region of the light's frustum.
    if(projCoords.z > 1.0)
        shadow = 0.0;
        
    return shadow;
}

float ShadowCalculation(vec4 fragPosLightSpace)
{
    // perform perspective divide
    vec3 projCoords = fragPosLightSpace.xyz / fragPosLightSpace.w;
    // transform to [0,1] range
    projCoords = projCoords * 0.5 + 0.5;
    // get closest depth value from light's perspective (using [0,1] range fragPosLight as coords)
    float closestDepth = texture(shadowMap, projCoords.xy).r; 
    // get depth of current fragment from light's perspective
    float currentDepth = projCoords.z;
    // calculate bias (based on depth map resolution and slope)
    vec3 normal = normalize(fs_in.Normal);
    vec3 lightDir = normalize(lightPos - fs_in.FragPos);
    float bias = max(0.05 * (1.0 - dot(normal, lightDir)), 0.005);
    // check whether current frag pos is in shadow
    // float shadow = currentDepth - bias > closestDepth  ? 1.0 : 0.0;
    // PCF
    float shadow = 0.0;
    vec2 texelSize = 1.0 / textureSize(shadowMap, 0);
    for(int x = -1; x <= 1; ++x)
    {
        for(int y = -1; y <= 1; ++y)
        {
            float pcfDepth = texture(shadowMap, projCoords.xy + vec2(x, y) * texelSize).r; 
            shadow += currentDepth - bias > pcfDepth  ? 1.0 : 0.0;        
        }    
    }
    shadow /= 9.0;
    
    // keep the shadow at 0.0 when outside the far_plane region of the light's frustum.
    if(projCoords.z > 1.0)
        shadow = 0.0;
        
    return shadow;
}
float findBlocker(vec2 uv, float zReceiver ) {
    float dBlocker = zReceiver * 0.01;
    const float wLight = 0.006;
    const float c = 100.0;  
    float sampleSize = wLight * zReceiver * c;
    float sum = 0.01; // 取0.01一是为了避免出现0除问题，二是当多重采样没有贡献时的dBlocker/sum将等于zReceiver
    for(int i = 0;i<BLOCKER_SEARCH_NUM_SAMPLES;++i){ 
        float depthInShadowmap = texture2D(shadowMap,uv+poissonDisk[i]*sampleSize).r;
        if(depthInShadowmap < zReceiver){
            dBlocker += depthInShadowmap;
            sum += 1.0;
        }
    }
    return dBlocker/float(sum);
}
float visibility_PCSS(vec4 fragPosLightSpace){
    vec3 coords = fragPosLightSpace.xyz / fragPosLightSpace.w;
    // transform to [0,1] range
    coords = coords * 0.5 + 0.5;
    // get closest depth value from light's perspective (using [0,1] range fragPosLight as coords)
    float closestDepth = texture(shadowMap, coords.xy).r; 
    // get depth of current fragment from light's perspective
    float currentDepth = coords.z;
    // calculate bias (based on depth map resolution and slope)
    vec3 normal = normalize(fs_in.Normal);
    vec3 lightDir = normalize(lightPos - fs_in.FragPos);
    float bias = max(0.05 * (1.0 - dot(normal, lightDir)), 0.05);
    
    poissonDiskSample(coords.xy);
    // STEP 1: avgblocker depth
    float dBlocker = findBlocker(coords.xy,coords.z);
    // STEP 2: penumbra size
    const float wLight = 0.006;
    float penumbra = (coords.z-dBlocker)/dBlocker * wLight;
    // STEP 3: filtering
    float sum = 0.0;
    for(int i = 0;i<PCF_NUM_SAMPLES;++i){
        float depthInShadowmap = texture2D(shadowMap,coords.xy+poissonDisk[i]*penumbra).r;
        //sum += ((depthInShadowmap + bias)< coords.z?0.0:1.0);
        sum += currentDepth - bias > depthInShadowmap  ? 1.0 : 0.0;    
    }
    sum /= float(PCF_NUM_SAMPLES);
    
    // keep the shadow at 0.0 when outside the far_plane region of the light's frustum.
    if(coords.z > 1.0)
        sum = 0.0;
        
    return sum;
    //return sum/float(PCF_NUM_SAMPLES);
}

void main()
{           
    vec3 color = texture(diffuseTexture, fs_in.TexCoords).rgb;
    vec3 normal = normalize(fs_in.Normal);
    vec3 lightColor = vec3(0.7);
    // ambient
    vec3 ambient = 0.3 * lightColor;
    // diffuse
    vec3 lightDir = normalize(lightPos - fs_in.FragPos);
    float diff = max(dot(lightDir, normal), 0.0);
    vec3 diffuse = diff * lightColor;
    // specular
    vec3 viewDir = normalize(viewPos - fs_in.FragPos);
    vec3 reflectDir = reflect(-lightDir, normal);
    float spec = 0.0;
    vec3 halfwayDir = normalize(lightDir + viewDir);  
    spec = pow(max(dot(normal, halfwayDir), 0.0), 64.0);
    vec3 specular = spec * lightColor;    
    // calculate shadow
    //float shadow = ShadowCalculation(fs_in.FragPosLightSpace); 
    float shadow = poissonDiskSamples(fs_in.FragPosLightSpace);     
    //float shadow = visibility_PCSS(fs_in.FragPosLightSpace);                  
    vec3 lighting = (ambient + (1.0 - shadow) * (diffuse + specular)) * color;    
    
    FragColor = vec4(lighting, 1.0);
}