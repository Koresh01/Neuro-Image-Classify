Shader "Custom/GLColorShader"
{
    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        Pass
        {
            ZWrite On
            Cull Off
            Lighting Off
            Fog { Mode Off }
            BindChannels {
                Bind "Color", color
                Bind "Vertex", vertex
            }
        }
    }
}
