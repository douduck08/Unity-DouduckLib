Shader "Hidden/TexturePacker" {
    Properties {
        _MainTex ("Texture", 2D) = "black" {}
        _TexureR ("", 2D) = "black" {}
        _TexureG ("", 2D) = "black" {}
        _TexureB ("", 2D) = "black" {}
        _TexureA ("", 2D) = "black" {}
        _ChannelR ("", int) = 0
        _ChannelG ("", int) = 0
        _ChannelB ("", int) = 0
        _ChannelA ("", int) = 0
    }
    SubShader {
        Cull Off ZWrite Off ZTest Always

        Pass {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _TextureR, _TextureG, _TextureB, _TextureA;
            int _ChannelR, _ChannelG, _ChannelB, _ChannelA;

            float sample_channel (sampler2D tex, float2 uv, int channel) {
                float4 color = tex2D(tex, uv);
                if (channel == 0) return color.r;
                if (channel == 1) return color.g;
                if (channel == 2) return color.b;
                if (channel == 3) return color.a;
                return 0.0;
            }

            fixed4 frag (v2f_img i) : SV_Target {
                fixed4 color = 0;
                color.r = sample_channel (_TextureR, i.uv, _ChannelR);
                color.g = sample_channel (_TextureG, i.uv, _ChannelG);
                color.b = sample_channel (_TextureB, i.uv, _ChannelB);
                color.a = sample_channel (_TextureA, i.uv, _ChannelA);
                return color;
            }
            ENDCG
        }
    }
}
