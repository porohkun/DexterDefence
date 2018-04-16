Shader "Sprites/Two Textures"
{
        Properties
        {
                _MainTex ("Sprite Texture", 2D) = "white" {}
                _Color ("Tint", Color) = (1,1,1,1)
                [MaterialToggle]
				PixelSnap ("Pixel snap", Float) = 0
				[PerRendererData]
				_OverlayAlpha("Overlay Alpha", Float) = 0
        }

        SubShader
        {
                Tags
                {
                        "Queue"="Transparent"
                        "IgnoreProjector"="True"
                        "RenderType"="TransparentCutout"
                        "PreviewType"="Plane"
                        "CanUseSpriteAtlas"="True"
                }

                Cull Off
                Lighting Off
                ZWrite On
                Blend One OneMinusSrcAlpha

                Pass
                {
                CGPROGRAM
                        #pragma vertex vert
                        #pragma fragment frag
                        #pragma multi_compile _ PIXELSNAP_ON
                        #include "UnityCG.cginc"
                       
                        struct appdata_t
                        {
                                float4 vertex    : POSITION;
                                float4 color     : COLOR;
                                float2 texcoord0 : TEXCOORD0;
                                float2 texcoord1 : TEXCOORD1;
								float2 texcoord2 : TEXCOORD2;
                        };

                        struct v2f
                        {
                                float4 vertex    : SV_POSITION;
                                fixed4 color     : COLOR;
                                half2 texcoord0  : TEXCOORD0;
                                half2 texcoord1  : TEXCOORD1;
								half2 texcoord2  : TEXCOORD2;
                        };
                       
                        fixed4 _Color;

                        v2f vert(appdata_t IN)
                        {
                                v2f OUT;
                                OUT.vertex = UnityObjectToClipPos(IN.vertex);
                                OUT.texcoord0 = IN.texcoord0;
                                OUT.texcoord1 = IN.texcoord1;
								OUT.texcoord2 = IN.texcoord2;
                                OUT.color = IN.color * _Color;
                                #ifdef PIXELSNAP_ON
                                OUT.vertex = UnityPixelSnap (OUT.vertex);
                                #endif

                                return OUT;
                        }

                        sampler2D _MainTex;
                        float _AlphaSplitEnabled;
						half _OverlayAlpha;

                        fixed4 SampleSpriteTexture (float2 uv)
                        {
                                fixed4 color = tex2D (_MainTex, uv);
                                if (_AlphaSplitEnabled)
                                        color.a = tex2D (_MainTex, uv).r;
                                return color;
                        }

                        fixed4 frag(v2f IN) : SV_Target
                        {
                                fixed4 c = SampleSpriteTexture (IN.texcoord0);

                                half4 decal1 = tex2D(_MainTex, IN.texcoord1);
                                c.rgb = lerp (c.rgb, decal1.rgb, decal1.a);
                                clip(c.a - 0.5);

								half4 decal2 = tex2D(_MainTex, IN.texcoord2);
								c.rgb = lerp(c.rgb, decal2.rgb, decal2.a * _OverlayAlpha);
								clip(c.a - 0.5);

                                return c * IN.color;
                        }
                ENDCG
                }
        }
}