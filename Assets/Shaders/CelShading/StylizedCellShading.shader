Shader "Stylized/CellShading" {
    Properties{
        [Header(Surface options)] // Creates a text header
        [MainTexture] _MainTex("Color", 2D) = "white" {}
        [MainColor] _BaseColor("Tint", Color) = (1, 1, 1, 1)
        _BumpScale("Normal scale", Float) = 0
        _BumpMap("Normal map", 2D) = "bump" {}

        _Smoothness("Smoothness", Float) = 0
    }
    SubShader {
        Tags {"RenderPipeline" = "UniversalPipeline" "Queue" = "Geometry" "RenderType" = "Opaque"}
        
        Pass {
            Name "ForwardLit"
            Tags{"LightMode" = "UniversalForward"}
            
            HLSLPROGRAM

            #pragma shader_feature_local _NORMALMAP 
            #pragma shader_feature_local_fragment _SPECULAR_SETUP
            #define _SPECULAR_COLOR

#if UNITY_VERSION >= 202120
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile_fragment _ _ADDITIONAL_LIGHT_SHADOWS
#else
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
#endif
            #pragma multi_compile_fragment _ _SHADOWS_SOFT

            #pragma vertex Vertex
            #pragma fragment Fragment

            #include "StylizedShadingLitPass.hlsl"
            ENDHLSL
        }

//        Pass {
//            // The shadow caster pass, which draws to shadow maps
//            Name "ShadowCaster"
//            Tags{"LightMode" = "ShadowCaster"}
//
//            ColorMask 0 // No color output, only depth
//
//            HLSLPROGRAM
//            #pragma vertex Vertex
//            #pragma fragment Fragment
//
//            #include "MyLitShadowCasterPass.hlsl"
//            ENDHLSL
//        }
    }
}

