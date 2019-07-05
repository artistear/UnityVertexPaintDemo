Shader "Custom/Drawable Shader" {
    Properties {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _SubTex ("Second (RGB)", 2D) = "white" {}
        _JustVertexColor ("Just Vertex Color", Float) = 0
    }
    SubShader {
        Tags { "IgnoreProjector"="True" "RenderType"="Opaque" }
        LOD 200
       
        CGPROGRAM
        #pragma surface surf Lambert
        #pragma shader_feature JUST_VERTEX_COLOR
       
        sampler2D _MainTex;
        sampler2D _SubTex;
        fixed _JustVertexColor;
 
        struct Input {
            float2 uv_MainTex;
            float4 color: Color;
        };
 
        void surf (Input IN, inout SurfaceOutput o) {
            half4 c = tex2D (_MainTex, IN.uv_MainTex);
            half4 c2 = tex2D (_SubTex, IN.uv_MainTex);
            fixed3 black = fixed3(0.0,0.0,0.0);

            if (_JustVertexColor == 1.0) {
                o.Albedo = IN.color.rgb;
            }else {
                if (IN.color.r == 0 && IN.color.g == 0 && IN.color.b == 0)
                    o.Albedo = c2.rgb;
                else if (any(IN.color.rgb != fixed3(1, 1, 1)))
                    o.Albedo = c.rgb;
                else
                    o.Albedo = c.rgb;
            }
        }
        ENDCG
    }
    FallBack "Diffuse"
}