Shader "Custom/3TexturesBack"
{
    Properties
    {
        _Color("Main Color", COLOR) = (1, 1, 1, 1)
        _MainTex("Texture", 2D) = "white" {}
        _SubTex("Texture", 2D) = "white" {}
        _SubTex1("Texture", 2D) = "white" {}
    }

    SubShader
    {
        Tags { "Queue" = "Transparent"}
        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha

             SetTexture[_MainTex] {
                Combine texture //DOUBLE
             }

             SetTexture[_SubTex] {
                ConstantColor[_Color]
                Combine texture lerp(texture) previous, constant
             }

             SetTexture[_SubTex1] {
                ConstantColor[_Color]
                Combine texture lerp(texture) previous, constant
             }
        }//Pass

    }//SubShader

}
