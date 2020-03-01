Shader "ITE/Curve" 
{
    Properties
    {
        [PerRendererData] _Color("Color", Color) = (1,1,1,1)
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        LOD 100
        Pass 
        {
            Lighting Off
            ZWrite On
            Cull Back
            SetTexture[_] 
            {
                constantColor[_Color]
                Combine constant
            }
        }
    }
}