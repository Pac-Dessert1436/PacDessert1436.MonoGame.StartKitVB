Texture2D Texture : register(t0);
SamplerState TextureSampler : register(s0);

cbuffer Parameters : register(b0)
{
    float Time;
}

struct VertexShaderInput
{
    float4 Position : POSITION0;
    float4 Color : COLOR0;
    float2 TextureCoordinate : TEXCOORD0;
};

struct VertexShaderOutput
{
    float4 Position : SV_POSITION;
    float4 Color : COLOR0;
    float2 TextureCoordinate : TEXCOORD0;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;
    output.Position = input.Position;
    output.Color = input.Color;
    output.TextureCoordinate = input.TextureCoordinate;
    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : SV_Target
{
    float4 originalColor = Texture.Sample(TextureSampler, input.TextureCoordinate);
    if (originalColor.a <= 0.0)
        discard;
    
    float blinkValue = abs(sin(Time * 8.0));
    float4 blueColor = float4(0.0, 0.3, 1.0, 1.0);
    float4 whiteColor = float4(1.0, 1.0, 1.0, 1.0);
    float4 frightenedColor = lerp(blueColor, whiteColor, blinkValue);
    
    float4 finalColor;
    finalColor.rgb = frightenedColor.rgb;
    finalColor.a = originalColor.a * input.Color.a;
    
    return finalColor;
}

technique FrightenedTechnique
{
    pass BlinkPass
    {
        VertexShader = compile vs_4_0_level_9_1 VertexShaderFunction();
        PixelShader = compile ps_4_0_level_9_1 PixelShaderFunction();
    }
}