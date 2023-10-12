// Very fast shader that uses the Unity light system.
// Compiles down to only performing the operations you're actually using.
// Uses material property drawers rather than a custom editor for ease of maintenance.

// Textured ambient+diffuse:
// Stats for Vertex shader:
//        d3d11: 24 avg math (9..44)
// Stats for Fragment shader:
//        d3d11: 2 avg math (1..5), 0 avg texture (0..2)

Shader "HoloToolkit/Fast Configurable 2 Side"
{
    Properties
    {
        [Header(Base Texture and Color)]
        [Indent]
            [Toggle] _UseVertexColor("Vertex Color Enabled?", Float) = 0
            [Toggle] _UseMainColor("Main Color Enabled?", Float) = 0
            _Color("Main Color", Color) = (1,1,1,1)		
            [Toggle] _UseMainTex("Main Texture Enabled?", Float) = 0
            [NoScaleOffset]_MainTex("Main Texture", 2D) = "red" {}
            [Toggle] _UseOcclusionMap("Occlusion/Detail Texture Enabled?", Float) = 0
            [NoScaleOffset]_OcclusionMap("Occlusion/Detail Texture", 2D) = "blue" {}
        [Dedent]
       
        [Space(12)]
        [Header(Lighting)]
        [Indent]
            [Toggle] _UseAmbient("Ambient Lighting Enabled?", Float) = 1
            [Toggle] _UseDiffuse("Diffuse Lighting Enabled?", Float) = 1
            [Toggle] _UseSpecular("Specular Lighting Enabled?", Float) = 0
            [Indent]
                _SpecularColor("Specular Color", Color) = (0.5, 0.5, 0.5, 1)
                [PowerSlider(5.0)]_Specular("Specular (Specular Power)", Range(1.0, 100.0)) = 10.0
                _Gloss("Gloss (Specular Scale)", Range(0.1, 10.0)) = 1.0
                [Toggle] _UseGlossMap("Use Gloss Map? (per-pixel)", Float) = 0
                [NoScaleOffset]_MetallicGlossMap("Gloss Map", 2D) = "white" {}
            [Dedent]
            [Toggle] _Shade4("Use additional lighting data? (Expensive!)", Float) = 0
            [Toggle] _UseBumpMap("Normal Map Enabled? (per-pixel)", Float) = 0
            [NoScaleOffset][Normal] _BumpMap("Normal Map", 2D) = "bump" {}
        [Dedent]

        [Space(12)]
        [Header(Emission)]
        [Indent]
            [Toggle] _UseEmissionColor("Emission Color Enabled?", Float) = 0
            _EmissionColor("Emission Color", Color) = (1,1,1,1)
            [Toggle] _UseEmissionTex("Emission Texture Enabled?", Float) = 0
            [NoScaleOffset] _EmissionTex("Emission Texture", 2D) = "blue" {}
        [Dedent]

        [Space(12]
        [Header(Texture Scale and Offset)]
        [Indent]
            [Toggle(_MainTex_SCALE_ON)] _MainTex_SCALE("Use Texture Scale? (Applies to all textures)", Float) = 0
            [Toggle(_MainTex_OFFSET_ON)] _MainTex_OFFSET("Use Texture Offset? (Applies to all textures)", Float) = 0
            _TextureScaleOffset("Texture Scale (XY) and Offset (ZW)", Vector) = (1, 1, 0, 0)
        [Dedent]

        [Space(12)]
        [Header(Alpha Blending)]
        [Indent]
            [Enum(UnityEngine.Rendering.BlendMode)] _SrcBlend("SrcBlend", Float) = 1 //"One"
            [Enum(UnityEngine.Rendering.BlendMode)] _DstBlend("DestBlend", Float) = 0 //"Zero"
            [Enum(UnityEngine.Rendering.BlendOp)] _BlendOp("BlendOp", Float) = 0 //"Add"
        [Dedent]

        [Space(12)]
        [Header(Misc.)]
        [Indent]
            [Enum(UnityEngine.Rendering.CompareFunction)] _ZTest("ZTest", Float) = 4 //"LessEqual"
            [Enum(Off,0,On,1)] _ZWrite("ZWrite", Float) = 1 //"On"

            [Enum(UnityEngine.Rendering.ColorWriteMask)] _ColorWriteMaskFront("ColorWriteMaskFront", Float) = 15 //"All"
            [Enum(UnityEngine.Rendering.ColorWriteMask)] _ColorWriteMaskBack("ColorWriteMaskBack", Float) = 15 //"All"
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "PerformanceChecks"="False" }
        LOD 100
        Blend[_SrcBlend][_DstBlend]
        BlendOp[_BlendOp]
        ZTest[_ZTest]
        ZWrite[_ZWrite]

        Pass
        {
            Name "FORWARD"
            Tags { "LightMode" = "ForwardBase" }
            Cull Back
            ColorMask[_ColorWriteMaskFront]

            CGPROGRAM
            //compiles all variants needed by ForwardBase (forward rendering base) pass type. The variants deal with different lightmap types and main directional light having shadows on or off.
            #pragma multi_compile_fwdbase

            //expands to several variants to handle different fog types
            #pragma multi_compile_fog

            //We only target the HoloLens (and the Unity editor), so take advantage of shader model 5.
            #pragma target 5.0
            #pragma only_renderers d3d11

            //shader features are only compiled if a material uses them
			#pragma shader_feature _USEVERTEXCOLOR_ON
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

            #include "FastConfigurable2Side.cginc"			
            #pragma vertex vert
            #pragma fragment frag
            ENDCG
        }

        Pass
        {
            Name "BACK"
            Tags{ "LightMode" = "ForwardBase" }
            Cull Front
            ColorMask[_ColorWriteMaskBack]

            CGPROGRAM
            //compiles all variants needed by ForwardBase (forward rendering base) pass type. The variants deal with different lightmap types and main directional light having shadows on or off.
            #pragma multi_compile_fwdbase

            //expands to several variants to handle different fog types
            #pragma multi_compile_fog

            //We only target the HoloLens (and the Unity editor), so take advantage of shader model 5.
            #pragma target 5.0
            #pragma only_renderers d3d11

            //shader features are only compiled if a material uses them
			#pragma shader_feature _USEVERTEXCOLOR_ON
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

            #define FLIP_NORMALS 1
            #include "FastConfigurable2Side.cginc"
            #pragma vertex vert
            #pragma fragment frag
            ENDCG
        }
    } 
    Fallback "VertexLit" //for shadows
}