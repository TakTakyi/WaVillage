using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Back : MonoBehaviour
{
    private MeshRenderer Render;
    private float offset;
    private float suboffset;
    private float sub1offset;
    public float speed;
    public float subspeed;
    public float sub1speed;

    // Start is called before the first frame update
    void Start()
    {
        Render = GetComponent<MeshRenderer>();
        speed = 0.1f;
        subspeed = 0.3f;
        sub1speed = 0.5f;
    }

    // Update is called once per frame
    void Update()
    {
        offset += (Time.deltaTime * speed);
        suboffset += (Time.deltaTime * subspeed);
        sub1offset += (Time.deltaTime * sub1speed);
        Render.material.mainTextureOffset = new Vector2(offset, 0);
        Render.material.SetTextureOffset("_SubTex", new Vector2(suboffset, 0));
        Render.material.SetTextureOffset("_SubTex1", new Vector2(sub1offset, 0));
        //Render.materials[1].mainTextureOffset = new Vector2(offset, 0);
        //Render.materials[2].mainTextureOffset = new Vector2(offset, 0);
    }
}
