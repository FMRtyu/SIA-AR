Shader "Occlusion FX/Occlusion Mask"
{
    Properties {
        _MainTex    ("Main", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0, 1)) = 0
        _Metallic   ("Metallic", Range(0, 1)) = 0
    }
    SubShader {
        Tags { "RenderType"="Opaque" "Queue"="Geometry-1" }
        Stencil {
            Ref 128
            Comp Always
            Pass Replace
        }
        CGPROGRAM
        #pragma surface surf Standard
        #include "Utils.cginc"
        ENDCG
    }
    FallBack Off
}
