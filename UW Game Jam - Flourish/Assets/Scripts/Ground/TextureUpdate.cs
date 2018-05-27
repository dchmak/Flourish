/*
 * Created by Brandon Brisbane 
 */

using UnityEngine;

public class TextureUpdate : MonoBehaviour {

    [Header("Objects")]
    public GameObject player;

    [Header("Colors")]
    public Color dirtColor;
    public Color grassColor;
    [Range(0f, 1f)] public float grassThreshold = 0.5f;

    [Header("Resolution")]
    [Range(50, 2000)] public int textureSize;

    [Header("Draw settings")]
    [Range(0f,10f)] public float size;
    [Range(1f,20f)]public float phase;
    [Range(0.1f,0.9f)] public float wavemag;

    private Water water;
    private Texture2D ground;
    private Renderer rend;
    
    // Use this for initialization
    void Start () {
        /*
        Color test = Color.Lerp(dirtColor, grassColor, 0.7f);
        print(test);
        print(Mathf.InverseLerp(dirtColor.b, grassColor.b, test.b));
        */

        water = Water.instance;

        rend = GetComponent<Renderer>();

        //grass = (Texture2D)rend.material.mainTexture;

        ground = new Texture2D(textureSize, textureSize);

        Color[] colors = ground.GetPixels();

        //print(colors.Length);

        for (int i = 0; i < colors.Length; i++) {
            colors[i] = dirtColor;
        }
        ground.SetPixels(colors);
        ground.Apply();

        rend.material.mainTexture = ground;
    }

    /*
    public void green() {
        grass = (Texture2D)rend.material.mainTexture;

        Vector2 pp = new Vector2(player.transform.position.x, player.transform.position.z);
        Vector2 ep = new Vector2(Water.instance.WaterHitPos.x, Water.instance.WaterHitPos.z);

        float dx = Math.Abs(pp.x - ep.x);
        float dy = Math.Abs(pp.y - ep.y);

        double gam = -Math.Atan2(dy, dx);
        gam = gam * (gam >= 0 ? 1 : 0) + (gam + 2 * Math.PI) * (gam > 0 ? 1 : 0);

        for (int i = Math.Min((int)pp.x, (int)ep.x); i < Math.Max((int)pp.x, (int)ep.x); i++) {

            for (int j = Math.Min((int)pp.y, (int)ep.y) - (int)((2.0 / 3) * dx); j < Math.Max((int)pp.x, (int)ep.x) + (int)((2.0 / 3) * dx); j++) {

                double th = -Math.Atan2(j, i);
                th = th * (th >= 0 ? 1 : 0) + (th + 2 * Math.PI) * (th > 0 ? 1 : 0);
                if (Length(i, j) <= Length(R(th - gam) * Math.Cos(th), 0.5 * R(th - gam) * Math.Sin(th)) || Length(i, j) <= Length(R(th - gam + Math.PI) * Math.Cos(th), 0.5 * R(th - gam + Math.PI) * Math.Sin(th))) {
                    if (!grass.GetPixel(i, j).Equals(grassColor)) {
                        GameManager.instance.greenPixel += 1;
                        //print(i.ToString() + " | " + j.ToString());
                        //grass.SetPixel(i, j, grassColor);
                        Paint(i, j);
                    }
                }
            }
        }

        //grass.Apply();
    }
    */

    public void Paint(float x, float y) {
        int pixelX = Mathf.RoundToInt(x * textureSize / transform.localScale.x);
        int pixelY = Mathf.RoundToInt(y * textureSize / transform.localScale.z);
        //print(x.ToString() + " | " + y.ToString() + " || " + pixelX.ToString() + " | " + pixelY.ToString());
        //print(brushSize);
        int pixelRadius = Mathf.RoundToInt(water.waterSize * textureSize / transform.localScale.z);

        for (int j = pixelY - pixelRadius; j < pixelY + pixelRadius; j++) {
            for (int i = pixelX - pixelRadius; i < pixelX + pixelRadius; i++) {
                float distance = Mathf.Sqrt(Mathf.Pow(i - pixelX, 2) + Mathf.Pow(j - pixelY, 2));
                if (distance < pixelRadius) {
                    Color color = Color.Lerp(ground.GetPixel(i, j), grassColor, (float)(pixelRadius - distance) / (float)pixelRadius * Time.deltaTime * water.waterStregth);
                    ground.SetPixel(i, j, color);
                }
            }
        }

        ground.Apply();
    }

    /*
    private double R(double t){
        return (-size*(1-Math.Cos(t))+wavemag*Math.Sin(phase*t));
    }

    private double Length(Double x, Double y)
    {
        return Math.Sqrt(Math.Pow(x,2) + Math.Pow(y,2));
    }
	
	// Update is called once per frame
	void Update () {
        //rend.material.mainTexture = grass;
    }
    */

    public float GetGreenPixelRatio() {
        float count = 0;
        Color[] colors = ground.GetPixels();
        foreach (Color color in colors) {
            /*
            print(
                Mathf.InverseLerp(dirtColor.r, grassColor.r, color.r) + " | " +
                Mathf.InverseLerp(dirtColor.g, grassColor.g, color.g) 
                );
                */
            if (
                Mathf.InverseLerp(dirtColor.r, grassColor.r, color.r) > grassThreshold &&
                Mathf.InverseLerp(dirtColor.g, grassColor.g, color.g) > grassThreshold
                ) {
                //print("Found!");
                count++;
            }
        }

        //print(count);
        return count / (textureSize * textureSize);
    }
}
