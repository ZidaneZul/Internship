            #pragma vertex vert
            #pragma fragment frag

            //compiles all variants needed by ForwardBase (forward rendering base) pass type. The variants deal with different lightmap types and main directional light having shadows on or off.
            #pragma multi_compile_fwdbase

            //expands to several variants to handle different fog types
            #pragma multi_compile_fog

            //We only target the HoloLens (and the Unity editor), so take advantage of shader model 5.
            #pragma target 5.0
            #pragma only_renderers d3d11

            //shader features are only compiled if a material uses them
            #pragma shader_feature _USEMAINCOLOR_ON
            #pragma shader_feature _USEMAINTEX_ON
            #pragma shader_feature _USESOCCLUSIONMAP_ON
            #pragma shader_feature _USEBUMPMAP_ON
            #pragma shader_feature _USEAMBIENT_ON
            #pragma shader_feature _USEDIFFUSE_ON
            #pragma shader_feature _USESPECULAR_ON
            #pragma shader_feature _USEGLOSSMAP_ON
            #pragma shader_feature _SHADE4_ON
            #pragma shader_feature _USEEMISSIONCOLOR_ON
            #pragma shader_feature _USEEMISSIONTEX_ON

            //scale and offset will apply to all
            #pragma shader_feature _MainTex_SCALE_ON
            #pragma shader_feature _MainTex_OFFSET_ON

            //may be set from script so generate both paths
            #pragma multi_compile __ _NEAR_PLANE_FADE_ON
            
            #include "Lighting.cginc"
            #include "AutoLight.cginc"

            #include "HoloToolkitCommon.cginc"
            #include "macro.cginc"

            #define USE_PER_PIXEL (_USEBUMPMAP_ON || _USEGLOSSMAP_ON)
            #define PIXEL_SHADER_USES_WORLDPOS  (USE_PER_PIXEL && (_USESPECULAR_ON || _SHADE4_ON))
            #define USES_TEX_XY (_USEMAINTEX_ON || _USEOCCLUSIONMAP_ON || _USEEMISSIONTEX_ON || _USEBUMPMAP_ON || _USEGLOSSMAP_ON)

            #if _USEMAINCOLOR_ON
                float4 _Color;
            #endif	

            #if _USEMAINTEX_ON
                UNITY_DECLARE_TEX2D(_MainTex);
            #endif

            #if _USEMAINTEX_ON
                UNITY_DECLARE_TEX2D(_SecondaryTex);
            #endif
        
            #if _USEBUMPMAP_ON
                UNITY_DECLARE_TEX2D(_BumpMap);
            #endif
           
            #if _USESPECULAR_ON
                float3 _SpecularColor;
                float _Specular;
                float _Gloss;
            #endif

            #if _USEGLOSSMAP_ON
                UNITY_DECLARE_TEX2D(_MetallicGlossMap);
            #endif

            #if _USEEMISSIONCOLOR_ON
                float4 _EmissionColor;
            #endif

            #if _USEEMISSIONTEX_ON
                UNITY_DECLARE_TEX2D(_EmissionTex);
            #endif

            float4 _TextureScaleOffset;

            struct a2v
            {
                float4 vertex : POSITION;

                #if _USEBUMPMAP_ON
                #else
                    float3 normal : NORMAL;
                #endif

                #if _USEVERTEXCOLOR_ON
                    float4 color : COLOR;
                #endif
                #if USES_TEX_XY
                    float2 texcoord : TEXCOORD0;
                #endif

                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                
                #if _USEVERTEXCOLOR_ON
                    float4 color : COLOR;
                #endif

                #if _USEBUMPMAP_ON
                #else
                    #if _USEGLOSSMAP_ON
                        float3 normal : NORMAL;
                    #endif
                #endif

                #if USES_TEX_XY || _NEAR_PLANE_FADE_ON
                    float3 texXYFadeZ : TEXCOORD0;
                #endif

                #if LIGHTMAP_ON
                    float2 lmap : TEXCOORD1;
                #else
                    float3 vertexLighting : TEXCOORD1;
                #endif

                #if PIXEL_SHADER_USES_WORLDPOS
                    float3 worldPos: TEXCOORD2;
                #endif

                LIGHTING_COORDS(3, 4)                
                UNITY_FOG_COORDS(5)
                UNITY_VERTEX_OUTPUT_STEREO
            };

            v2f vert(a2v v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_OUTPUT(v2f, o);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

                o.pos = UnityObjectToClipPos(v.vertex);

                #if _USEVERTEXCOLOR_ON
                    #if _USEMAINCOLOR_ON
                        o.color = v.color * _Color;
                    #else
                        o.color = v.color;
                    #endif
                #endif

                #if (_USESPECULAR_ON && USE_PER_PIXEL) || _SHADE4_ON
                    float4 worldPos = mul(unity_ObjectToWorld, v.vertex);
                #endif

                #if PIXEL_SHADER_USES_WORLDPOS
                    o.worldPos = worldPos;
                #endif

                #if USES_TEX_XY
                    o.texXYFadeZ.xy = TRANSFORM_TEX_MAINTEX(v.texcoord.xy, _TextureScaleOffset);
                #endif

                #if LIGHTMAP_ON
                    o.lmap.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
                #else
                    #if USE_PER_PIXEL
                    //no bump maps, do per-vertex lighting
                    #else
                        #if FLIP_NORMALS
                            float3 normalWorld = UnityObjectToWorldNormal(-v.normal);
                        #else
                            float3 normalWorld = UnityObjectToWorldNormal(v.normal);
                        #endif

                        #if _USEAMBIENT_ON
                            //grab ambient color from Unity's spherical harmonics					
                            o.vertexLighting += ShadeSH9(float4(normalWorld, 1.0));
                        #endif
                        #if _USEDIFFUSE_ON
                            o.vertexLighting += HoloTKLightingLambertian(normalWorld, _WorldSpaceLightPos0.xyz, _LightColor0.rgb);
                        #endif
                        #if _USESPECULAR_ON
                            o.vertexLighting += HoloTKLightingBlinnPhong(normalWorld, _WorldSpaceLightPos0.xyz, _LightColor0.rgb, UnityWorldSpaceViewDir(mul(unity_ObjectToWorld, v.vertex)), _Specular, _Gloss, _SpecularColor);
                        #endif
                        #if _SHADE4_ON
                            //handles point and spot lights
                            o.vertexLighting += Shade4PointLights(unity_4LightPosX0, unity_4LightPosY0, unity_4LightPosZ0,
                                                                  unity_LightColor[0].rgb, unity_LightColor[1].rgb, unity_LightColor[2].rgb, unity_LightColor[3].rgb,
                                                                  unity_4LightAtten0, worldPos, normalWorld);
                        #endif
                    #endif
                #endif


                #if _USEBUMPMAP_ON
                #else
                    #if _USEGLOSSMAP_ON
                        #if FLIP_NORMALS
                            o.normal = -v.normal;
                        #else
                            o.normal = v.normal;
                        #endif
                    #endif
                #endif
                
                //fade away objects closer to the camera
                #if _NEAR_PLANE_FADE_ON
                    o.texXYFadeZ.z = ComputeNearPlaneFadeLinear(v.vertex);
                #endif

                TRANSFER_VERTEX_TO_FRAGMENT(o);
                UNITY_TRANSFER_FOG(o, o.pos);
                return o;
            }

            float4 frag(v2f IN) : SV_Target
            {
                #if _USEMAINTEX_ON
                    float4 color = UNITY_SAMPLE_TEX2D(_MainTex, IN.texXYFadeZ.xy);
                #else
                    float4 color = 1;
                #endif

                #if _USEOCCLUSIONMAP_ON
                    color *= UNITY_SAMPLE_TEX2D(_OcclusionMap, IN.texXYFadeZ.xy);
                #endif

                #if _USEVERTEXCOLOR_ON
                    color *= IN.color;
                //if vertex color is on, we've already scaled it by the main color if needed in the vertex shader
                #elif _USEMAINCOLOR_ON 
                    color *= _Color;
                #endif

                //light attenuation from shadows cast onto the object
                //TODO: get shadow attenuation working with 2 sides
                float lightAttenuation = 1;// SHADOW_ATTENUATION(IN);
                float3 lightColorShadowAttenuated = 0;

                #if LIGHTMAP_ON
                    float3 lightmapResult = DecodeLightmap(UNITY_SAMPLE_TEX2D(unity_Lightmap, IN.lmap.xy));
                    #ifdef SHADOWS_SCREEN
                        lightColorShadowAttenuated = min(lightmapResult, lightAttenuation * 2);
                    #else
                        lightColorShadowAttenuated = lightmapResult;
                    #endif	
                #else //not using lightmapping
                    #if USE_PER_PIXEL
                        //if a normal map is on, it makes sense to do most calculations per-pixel
                        //unpack can be expensive if normal map is dxt5
                        float3 normalObject;
                        #if (_USEBUMPMAP_ON)
                            normalObject = UnpackNormal(UNITY_SAMPLE_TEX2D(_BumpMap, IN.texXYFadeZ.xy));
                        #else
                            normalObject = IN.normal;
                        #endif

                        #if FLIP_NORMALS
                            float3 normalWorld = UnityObjectToWorldNormal(-normalObject);
                        #else
                            float3 normalWorld = UnityObjectToWorldNormal(normalObject);
                        #endif

                        #if _USEAMBIENT_ON
                            //grab ambient color from Unity's spherical harmonics					
                            lightColorShadowAttenuated += ShadeSH9(float4(normalWorld, 1.0));
                        #endif
                        #if _USEDIFFUSE_ON
                            lightColorShadowAttenuated += HoloTKLightingLambertian(normalWorld, _WorldSpaceLightPos0.xyz, _LightColor0.rgb);
                        #endif
                        #if _USESPECULAR_ON
                            float gloss = _Gloss;
                            #if _USEGLOSSMAP_ON
                                gloss *= UNITY_SAMPLE_TEX2D(_MetallicGlossMap, IN.texXYFadeZ.xy).y;
                            #endif
                            lightColorShadowAttenuated += HoloTKLightingBlinnPhong(normalWorld, _WorldSpaceLightPos0.xyz, _LightColor0.rgb, UnityWorldSpaceViewDir(IN.worldPos), _Specular, gloss, _SpecularColor);
                        #endif
                        #if _SHADE4_ON
                            //This handles point and directional lights
                            lightColorShadowAttenuated += Shade4PointLights(unity_4LightPosX0, unity_4LightPosY0, unity_4LightPosZ0,
                                                                            unity_LightColor[0].rgb, unity_LightColor[1].rgb, unity_LightColor[2].rgb, unity_LightColor[3].rgb,
                                                                            unity_4LightAtten0, IN.worldPos, normalWorld);
                        #endif
                    #else
                        //no normal map, so vertex lighting is sufficient
                        lightColorShadowAttenuated = IN.vertexLighting;
                    #endif
                    //could save some work here in the 0 case
                    lightColorShadowAttenuated *= lightAttenuation;
                #endif
                
                color.rgb *= lightColorShadowAttenuated;

                #if _USEEMISSIONTEX_ON
                    color.rgb += UNITY_SAMPLE_TEX2D(_EmissionTex, IN.texXYFadeZ.xy);
                #endif

                #if _USEEMISSIONCOLOR_ON
                    color.rgb += _EmissionColor;
                #endif

                #if _NEAR_PLANE_FADE_ON
                    color.rgb *= IN.texXYFadeZ.z;
                #endif

                UNITY_APPLY_FOG(IN.fogCoord, color);
                
                return color;
            }
