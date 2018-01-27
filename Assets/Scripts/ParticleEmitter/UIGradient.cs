using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

 


[RequireComponent(typeof(CanvasRenderer))]
[ExecuteInEditMode]
public class UIGradient : Image {
	
	public  Texture mainTexture{get{ return mainTexture;}}

	public Sprite sprite{
		get{ return sprite;}
		set{ 
			sprite = value;
			SetSprite ();
		}
	}

	CanvasRenderer render;
	Mesh mesh ;
	RectTransform rect;
	//public Material material;
	List<Vector3> vertices;
	Vector2[] uv;
	int[] triangles;

	public List<Color> colors;


	[ContextMenu("Reset Colors")]
	public void Reset () {


		colors = new List<Color>();
		colors.Add( Color.white);
		colors.Add(Color.white);
		colors.Add(Color.white);
		colors.Add(Color.white);


	}
	void SetSprite(){
		if (sprite != null) {
			Texture2D text = new Texture2D ((int)sprite.rect.width, (int)sprite.rect.height);
			Color[] pixels = sprite.texture.GetPixels ((int)sprite.rect.x, (int)sprite.rect.y, (int)sprite.rect.width, (int)sprite.rect.height);
			text.SetPixels (pixels);
			text.Apply ();
			material.mainTexture = text;
		}

	}

	protected override void OnPopulateMesh(VertexHelper vh)
    {
		
        Vector2 corner1 = Vector2.zero;
        Vector2 corner2 = Vector2.zero;

        corner1.x = 0f;
        corner1.y = 0f;
        corner2.x = 1f;
        corner2.y = 1f;

        corner1.x -= rectTransform.pivot.x;
        corner1.y -= rectTransform.pivot.y;
        corner2.x -= rectTransform.pivot.x;
        corner2.y -= rectTransform.pivot.y;

        corner1.x *= rectTransform.rect.width;
        corner1.y *= rectTransform.rect.height;
        corner2.x *= rectTransform.rect.width;
        corner2.y *= rectTransform.rect.height;

        vh.Clear();

        UIVertex vert = UIVertex.simpleVert;

        vert.position = new Vector2(corner1.x, corner1.y);
		Color col = color;
		if(colors.Count>0)
			col=colors[0];
		vert.color = col*color;
        vh.AddVert(vert);

        vert.position = new Vector2(corner1.x, corner2.y);
		col = color;
		if (colors.Count > 1)
			col = colors [1];
		vert.color =col*color;
        vh.AddVert(vert);

        vert.position = new Vector2(corner2.x, corner2.y);
		col = color;
		if (colors.Count > 2)
			col = colors [2];
		vert.color = col*color;
        vh.AddVert(vert);

        vert.position = new Vector2(corner2.x, corner1.y);
		col = color;
		if (colors.Count > 3)
			col = colors [3];
		vert.color = col*color;
        vh.AddVert(vert);

        vh.AddTriangle(0, 1, 2);
        vh.AddTriangle(2, 3, 0);



		while (colors.Count > 4) {
			colors.RemoveAt (colors.Count - 1);
		}
    }
	// Update is called once per frame
	void Update () {
		
		 UpdateGeometry();
		/*
		mesh.Clear ();

		if (vertices.Count==0) {
		}

		vertices[0] = new Vector3(-rect.rect.width/2f,-rect.rect.height/2f);
		vertices[1] = new Vector3(-rect.rect.width/2f,rect.rect.height/2f);
		vertices[2] = new Vector3(rect.rect.width/2f,rect.rect.height/2f);
		vertices[3] = new Vector3(rect.rect.width/2f,-rect.rect.height/2f);


		mesh.vertices = vertices.ToArray();
		mesh.uv = uv;
		mesh.triangles = triangles;

		mesh.colors =colors;

		render.SetMesh (mesh);*/



	}
}
