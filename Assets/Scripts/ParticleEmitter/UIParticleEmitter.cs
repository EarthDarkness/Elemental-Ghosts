using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR 
using UnityEditor;
#endif

[RequireComponent(typeof(CanvasRenderer))]
[ExecuteInEditMode]
public class UIParticleEmitter : Image
{


    public enum Direction { Radius, Cone };
    public enum ColorType { Single, Double, Gradient, Double_Gradient };
    public enum SizeType { Single, Double };

    public bool PreWarm = false;

    public AnimationCurve SizeCurveX = AnimationCurve.Linear(1, 0, 1, 1);
    public AnimationCurve SizeCurveY = AnimationCurve.Linear(1, 0, 1, 1);

    public AnimationCurve SizeCurveX_2 = AnimationCurve.Linear(1, 0, 1, 1);
    public AnimationCurve SizeCurveY_2 = AnimationCurve.Linear(1, 0, 1, 1);

    public List<UIParticle> particlePool = new List<UIParticle>();
    public List<UIParticle> activeParticles = new List<UIParticle>();
    public List<UIParticle> inactiveParticles = new List<UIParticle>();

    public float EmitterDuration = 1f;
    public bool Loop = true;



    public Color[] colors;
    public Gradient gradient;
    public Gradient gradient2;
    public int EmissionRate = 100;
    public int PoolSize = 500;
    public float[] Size = new float[2];

    CanvasRenderer render;
    float Timer = 0f;

    public float LifeSpan = 1f;
    public Direction direction;
    public ColorType colorType;
    public SizeType sizeType;
    //RADIUS
    public Vector2 Radius;
    public float innerRadius = 0.25f;
    public float Angle = 90f;
    public float Threshold = 0f;


    List<UIVertex> vecs;
    List<int> tri;

    /*List<UIVertex> l_vecs = new List<UIVertex>();
	List<int> l_tri = new List<int> ();
*/

    // Use this for initialization

    VertexHelper vh;



    void Reset()
    {
        Size = new float[2];
        Size[0] = 1f;
        Size[1] = 1f;
        loopTimer = 0f;
        colors = new Color[2];
        colors[0] = Color.white;
        colors[1] = Color.white;
        gradient = new Gradient();
        gradient2 = new Gradient();

        PoolSize = 100;

        base.SetVerticesDirty();
        vh = new VertexHelper();
        particlePool.Clear();
        vecs = new List<UIVertex>(new UIVertex[PoolSize * 4]);
        tri = new List<int>(new int[PoolSize * 6]);
        particlePool.Clear();
        for (int i = 0; i < PoolSize; i++)
        {
            particlePool.Add(new UIParticle());
            UIVertex[] vertices = new UIVertex[4];
        }
        vh.AddUIVertexStream(new List<UIVertex>(vecs),
            new List<int>(tri));
    }
    void Start()
    {
        loopTimer = 0f;


        base.SetVerticesDirty();
        vh = new VertexHelper();
        particlePool.Clear();
        vecs = new List<UIVertex>(new UIVertex[PoolSize * 4]);
        tri = new List<int>(new int[PoolSize * 6]);
        particlePool.Clear();
        for (int i = 0; i < PoolSize; i++)
        {
            particlePool.Add(new UIParticle());
            UIVertex[] vertices = new UIVertex[4];
        }
        vh.AddUIVertexStream(new List<UIVertex>(vecs),
            new List<int>(tri));

        loopTimer = 0f;



        if (PreWarm)
        {


            float nEmissions = (LifeSpan) / 0.1f;

            for (int j = 0; j < nEmissions; j++)
            {

                float cRate = EmissionRate / 10f;
                for (int i = 0; i < cRate; i++)
                {
                    var particle = Emit();
                    if (particle == null)
                        break;
                    particle.life = j * 0.1f * LifeSpan;

                    SetDestination(particle);
                    particle.colorType = Random.Range(0, 2);
                    if (colorType == ColorType.Single)
                    {
                        particle.myGetColor += GetColorSingle;
                    }
                    else if (colorType == ColorType.Double)
                    {
                        particle.myGetColor += GetColorDouble;
                    }
                    else if (colorType == ColorType.Gradient)
                    {
                        particle.myGetColor += GetColorGradient;
                    }
                    else if (colorType == ColorType.Double_Gradient)
                    {
                        particle.myGetColor += GetColorDoubleGradient;
                    }

                    if (sizeType == SizeType.Single)
                    {

                        particle.SizeType = 0;
                    }
                    else if (sizeType == SizeType.Double)
                    {

                        particle.SizeType = Random.Range(0, 2);
                    }
                    particle.LifeSpan = LifeSpan;

                }

            }
        }

    }







    void SetDestination(UIParticle particle)
    {
        if (direction == Direction.Radius)
        {
            Rect rec = this.GetComponent<RectTransform>().rect;

            var dir = new Vector2(Random.Range(-1f, 1f),
                Random.Range(-1f, 1f)).normalized;
            dir = new Vector2(dir.x * rec.width * 0.5f, dir.y * rec.height * 0.5f);
            particle.P1 = dir * (innerRadius);
            particle.P4 = dir;

            particle.P2 = particle.P1 + (particle.P4 - particle.P1) * 0.33333f;
            particle.P3 = particle.P1 + (particle.P4 - particle.P1) * 0.66666f;

        }
        else if (direction == Direction.Cone)
        {

            Rect rec = this.GetComponent<RectTransform>().rect;

            Vector3 temp = Quaternion.AngleAxis(Random.Range(-Angle, Angle), Vector3.forward) * Vector3.right;

            var dir = new Vector2(temp.x,
                temp.y);
            dir = new Vector2(dir.x * rec.width * 0.5f, dir.y * rec.height * 0.5f);
            particle.P1 = new Vector2(-rec.width / 2f, 0) * (1F - Threshold) + dir * Threshold;
            particle.P4 = dir;

            particle.P2 = particle.P1 + (particle.P4 - particle.P1) * 0.33333f;
            particle.P3 = particle.P1 + (particle.P4 - particle.P1) * 0.66666f;
        }
    }
    float loopTimer = 0f;
    void Update()
    {
        //material.mainTexture = fuckIt;
        //		canvasRenderer.GetMaterial().mainTexture=null;

#if UNITY_EDITOR
        if (!EditorApplication.isPlaying) return;
#endif
        if (!Loop)
        {

            loopTimer += Time.deltaTime;
        }
        if (loopTimer >= EmitterDuration + LifeSpan && activeParticles.Count == 0)
        {
            this.gameObject.SetActive(false);///Destroy (this.gameObject);
            loopTimer = 0f;
            for (int i = 0; i < activeParticles.Count; i++)
            {
                activeParticles[i].life = 0;
                particlePool.Add(activeParticles[i]);
            }
        }

        Timer += Time.deltaTime;
        if (Timer >= 0.05f && loopTimer < EmitterDuration)
        {



            Timer = 0f;
            float cRate = EmissionRate / 10f;
            for (int i = 0; i < cRate; i++)
            {
                var particle = Emit();
                if (particle == null)
                    break;
                particle.life = 0f;
                SetDestination(particle);
                particle.colorType = Random.Range(0, 2);
                if (colorType == ColorType.Single)
                {
                    particle.myGetColor += GetColorSingle;
                }
                else if (colorType == ColorType.Double)
                {
                    particle.myGetColor += GetColorDouble;
                }
                else if (colorType == ColorType.Gradient)
                {
                    particle.myGetColor += GetColorGradient;
                }
                else if (colorType == ColorType.Double_Gradient)
                {
                    particle.myGetColor += GetColorDoubleGradient;
                }

                if (sizeType == SizeType.Single)
                {

                    particle.SizeType = 0;
                }
                else if (sizeType == SizeType.Double)
                {

                    particle.SizeType = Random.Range(0, 2);
                }
                particle.LifeSpan = LifeSpan;

            }
        }

        UpdateGeometry();



        //OnPopulateMesh (vh);


    }
    void FixedUpdate()
    {


    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();

        if (vecs == null)
        {
            vecs = new List<UIVertex>(new UIVertex[PoolSize * 4]);
            tri = new List<int>(new int[PoolSize * 6]);
            activeParticles.Clear();
            particlePool.Clear();
            for (int i = 0; i < PoolSize; i++)
            {
                particlePool.Add(new UIParticle());
            }
        }
        else if (vecs.Count != PoolSize * 4)
        {




        }
        UIVertex vec = UIVertex.simpleVert;
        UIParticle p;
        bool added;
        float t;
        Vector2 p_pos = Vector2.zero;


        Color col = Color.black;


        int activeCount = activeParticles.Count;
        int t_index = 0;
        int index = 0;
        for (int i = 0; i < activeParticles.Count; i++)
        {
            index = i * 4;
            t_index = i * 6;
            {



                p = activeParticles[i];

                p.life += Time.deltaTime;
                if (p.life >= p.LifeSpan)
                {

                    AddToPool(p);
                    vec.position = new Vector2(0f, 0f);
                    vec.color = new Color(0f, 0f, 0f, 1f);
                    vecs[index] = vec;
                    vecs[index + 1] = vec;
                    vecs[index + 2] = vec;
                    vecs[index + 3] = vec;

                    tri[t_index] = index;
                    tri[t_index + 1] = 0;
                    tri[t_index + 2] = 0;

                    tri[t_index + 3] = 0;
                    tri[t_index + 4] = 0;
                    tri[t_index + 5] = 0;
                    continue;
                }



                //t = p.life;
                p_pos = Bezier(p.P1, p.P2, p.P3, p.P4, p.life);


                col = p.myGetColor(p.colorType, p.life / p.LifeSpan);




                vec.color = col;
                vec.uv0 = new Vector2(0f, 0f);
                vec.uv1 = vec.uv0;
                vec.uv2 = vec.uv0;
                vec.uv3 = vec.uv0;
                vec.position = new Vector2(p_pos.x - Size[p.SizeType], p_pos.y - Size[p.SizeType]);

                vecs[index] = vec;


                vec.uv0 = new Vector2(0f, 1f);
                vec.uv1 = vec.uv0;
                vec.uv2 = vec.uv0;
                vec.uv3 = vec.uv0;
                vec.position = new Vector2(p_pos.x - Size[p.SizeType], p_pos.y + Size[p.SizeType]);
                vecs[index + 1] = vec;



                vec.uv0 = new Vector2(1f, 1f); ;
                vec.uv1 = vec.uv0;
                vec.uv2 = vec.uv0;
                vec.uv3 = vec.uv0;

                vec.position = new Vector2(p_pos.x + Size[p.SizeType], p_pos.y + Size[p.SizeType]);
                vecs[index + 2] = vec;



                vec.uv0 = new Vector2(1f, 0f); ;
                vec.uv1 = vec.uv0;
                vec.uv2 = vec.uv0;
                vec.uv3 = vec.uv0;
                vec.position = new Vector2(p_pos.x + Size[p.SizeType], p_pos.y - Size[p.SizeType]);
                vecs[index + 3] = vec;


                tri[t_index] = index;
                tri[t_index + 1] = index + 1;
                tri[t_index + 2] = index + 2;

                tri[t_index + 3] = index;
                tri[t_index + 4] = index + 2;
                tri[t_index + 5] = index + 3;








            }


        }
        for (int i = activeCount * 4; i < vecs.Count; i += 4)
        {
            vec.position = new Vector2(0f, 0f);
            vec.color = new Color(0f, 0f, 0f, 1f);
            vecs[i] = vec;
            vecs[i + 1] = vec;
            vecs[i + 2] = vec;
            vecs[i + 3] = vec;


        }
        //		Debug.Log ("Vecs size "+ vecs.Count);
        while (inactiveParticles.Count > 0)
        {

            particlePool.Add(inactiveParticles[0]);
            activeParticles.Remove(inactiveParticles[0]);
            inactiveParticles.RemoveAt(0);

        }


        vh.AddUIVertexStream(vecs, tri);

        /*Vector2 corner1 = Vector2.zero;
		Vector2 corner2 = Vector2.zero;

		

		UIVertex vert = UIVertex.simpleVert;

		vert.position = new Vector2(corner1.x, corner1.y);
		Color col = color;

		vert.color = col*color;
		vh.AddVert(vert);

		vert.position = new Vector2(corner1.x, corner2.y);
		col = color;

		vert.color =col*color;
		vh.AddVert(vert);

		vert.position = new Vector2(corner2.x, corner2.y);
		col = color;

			
		vert.color = col*color;
		vh.AddVert(vert);

		vert.position = new Vector2(corner2.x, corner1.y);
		col = color;
	
		vert.color = col*color;
		vh.AddVert(vert);

		vh.AddTriangle(0, 1, 2);
		vh.AddTriangle(2, 3, 0);*/
        //vh.Clear ();
        //vh.AddUIVertexTriangleStream (vecs);

    }
    Color GetColorSingle(int index, float t)
    {
        return colors[0];
    }
    Color GetColorDouble(int index, float t)
    {
        return colors[index];
    }
    Color GetColorGradient(int index, float t)
    {
        return gradient.Evaluate(t);
    }
    Color GetColorDoubleGradient(int index, float t)
    {
        if (index == 0)
            return gradient.Evaluate(t);
        return gradient2.Evaluate(t);
    }
    Vector2 Bezier(Vector2 P1, Vector2 P2, Vector2 P3, Vector2 P4, float t)
    {


        //return (1f - t) * P1 + t * P4;
        float t1 = 1f - t;
        return (t1 * t1 * t1) * (P1)
            + 3f * (t1 * t1) * t * P2
            + 3f * t1 * (t * t) * P3
            + (t * t * t) * P4;

    }
    void AddToPool(UIParticle particle)
    {
        inactiveParticles.Add(particle);
        //activeParticles.Remove (particle);
    }
    UIParticle Emit()
    {
        if (particlePool.Count > 0)
        {

            UIParticle particle = particlePool[0];
            particlePool.RemoveAt(0);
            activeParticles.Add(particle);


            return particle;
        }
        return null;
    }
}

public class UIParticle
{
    public float LifeSpan = 1f;
    public float life = 0f;
    public Vector2 P1, P2, P3, P4;
    public int colorType = 0;
    public int SizeType = 0;

    public delegate Color GetColor(int ind, float t);
    public GetColor myGetColor;

}
