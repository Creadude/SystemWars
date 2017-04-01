
cbuffer cbPerObject 
{
	float4x4 World;
	float4x4 View;
	float4x4 Projection;
	float4x4 WorldViewProjection;
};
cbuffer cbAmbient
{
	float ColorSaturation;
	float4 AmbientLightColor;
	float AmbientLightIntensity;
}
cbuffer cbDiffuse
{
	
	float4 DiffuseLightColor;
	float4 DiffuseLightDirection;
	float DiffuseLightIntensity;
}
float4x4 LightViewProj;
Texture2D ShadowMap;
const float DepthBias = 0.02;
float2 ShadowMapSize = (2048, 2048);
SamplerState ShadowMapSampler
{
	Texture = (ShadowMap);
	MinFilter = point;
	MagFilter = point;
	MipFilter = point;
	AddressU = Wrap;
	AddressV = Wrap;
};

struct VertexShaderInput
{
    float4 Position : SV_POSITION;
	float4 Color : COLOR0;
};

struct VertexShaderOutput
{
    float4 Position : SV_POSITION;
	float4 Color : COLOR0;
	float4 PositionWorld : NORMAL0;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;
    output.Position = mul(input.Position, WorldViewProjection);
	output.PositionWorld = mul(input.Position, World);
	output.Color = input.Color;
    return output;
};

float CalcShadowTermPCF(float light_space_depth, float ndotl, float2 shadow_coord)
{
	float shadow_term = 0;

	//float2 v_lerps = frac(ShadowMapSize * shadow_coord);

	float variableBias = clamp(0.001 * tan(acos(ndotl)), 0, DepthBias);

	//safe to assume it's a square
	float size = 1 / ShadowMapSize.x;

	float samples[4];
	samples[0] = (light_space_depth - variableBias < ShadowMap.Sample(ShadowMapSampler, shadow_coord).r);
	samples[1] = (light_space_depth - variableBias < ShadowMap.Sample(ShadowMapSampler, shadow_coord + float2(size, 0)).r);
	samples[2] = (light_space_depth - variableBias < ShadowMap.Sample(ShadowMapSampler, shadow_coord + float2(0, size)).r);
	samples[3] = (light_space_depth - variableBias < ShadowMap.Sample(ShadowMapSampler, shadow_coord + float2(size, size)).r);

	shadow_term = (samples[0] + samples[1] + samples[2] + samples[3]) / 4.0;
	//shadow_term = lerp(lerp(samples[0],samples[1],v_lerps.x),lerp(samples[2],samples[3],v_lerps.x),v_lerps.y);

	return shadow_term;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{

	float3 normal = cross(ddy(input.PositionWorld.xyz), ddx(input.PositionWorld.xyz));
	normal = normalize(normal);
	float lightIntensity = dot(normal, DiffuseLightDirection);

	///SHADOW
	float NdotL = lightIntensity;
	float4 lightingPosition = mul(input.PositionWorld, LightViewProj);
		//Find the position in the shadow map for this pixel
		float2 ShadowTexCoord = mad(0.5f, lightingPosition.xy / lightingPosition.w, float2(0.5f, 0.5f));
		ShadowTexCoord.y = 1.0f - ShadowTexCoord.y;
	// Get the current depth stored in the shadow map
	float ourdepth = (lightingPosition.z / lightingPosition.w);
	float shadowContribution = CalcShadowTermPCF(ourdepth, NdotL, ShadowTexCoord);
	///SHADOW



	float4 diffuse = lightIntensity * DiffuseLightColor * DiffuseLightIntensity * (input.Color * ColorSaturation);
	float4 ambient = AmbientLightColor * AmbientLightIntensity;
	return saturate(diffuse + ambient) * shadowContribution;
};

technique Technique1
{
    pass Pass1
    {
		ZEnable = true;
		ZWriteEnable = true;
		AlphaBlendEnable = false;
        VertexShader = compile vs_5_0 VertexShaderFunction();
        PixelShader = compile ps_5_0 PixelShaderFunction();
    }
	
}
