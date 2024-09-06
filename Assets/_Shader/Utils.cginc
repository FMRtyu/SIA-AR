#ifndef UTILS_INCLUDED
#define UTILS_INCLUDED

sampler2D _MainTex;
half _Metallic, _Glossiness;
struct Input
{
    float2 uv_MainTex;
};
void surf(Input IN, inout SurfaceOutputStandard o)
{
    half4 c = tex2D(_MainTex, IN.uv_MainTex);
    o.Albedo = c.rgb;
    o.Alpha = c.a;
    o.Metallic = _Metallic;
    o.Smoothness = _Glossiness;
}

#endif