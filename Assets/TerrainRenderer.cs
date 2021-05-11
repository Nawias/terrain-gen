/**
    Michał Wójcik 2021
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainRenderer : MonoBehaviour
{
    //Wczytana tekstura mapy wysokości
    [SerializeField]
    private Texture2D heightMap;
    [SerializeField]
    private Shader shader;
    //Wartość mnożnika wysokości terenu
    [SerializeField]
    private float terrainHeight;
    //Property definiujące dostęp do zmiennej terrainHeight
    public float TerrainHeight
    {
        get
        {
            return terrainHeight;
        }
        set
        {
            terrainHeight = value;
            updateMeshData();
        }
    }

    private MeshRenderer meshRenderer;
    private MeshFilter meshFilter;
    private Mesh mesh;

    void Start()
    {
        
        if (heightMap == null)
        {
            return;
        }

        meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = new Material(shader);
        meshFilter = gameObject.AddComponent<MeshFilter>();
        mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        getMeshData(out Vector3[] vertices, out Color[] colors,out int[] triangles);
        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangles,0);
        mesh.RecalculateNormals();
        mesh.SetColors(colors);
        meshFilter.mesh = mesh;
    }
    //Metoda konwertuje mapę wysokości na wierzchołki wraz z kolorami i trójkąty do wyrenderowania
    void getMeshData(out Vector3[] vertices,out Color[] colors, out int[] triangles)
    {
        vertices = new Vector3[heightMap.height * heightMap.width];
        colors = new Color[heightMap.height * heightMap.width];
        List<int> trianglesList = new List<int>();

        for(int y = 0; y < heightMap.height; y++)
        {
            for(int x = 0; x < heightMap.width; x++)
            {
                //Wysokość wierzchołka na współrzędnych x,y odpowiada wartości na współrzędnych x,y piksela w skali szarości 
                //pomnożonego przez zmienną terrainHeight
                float vertexHeight = heightMap.GetPixel(x, y).grayscale;
                vertices[x + y * heightMap.width] = new Vector3(x-heightMap.width/2, vertexHeight * terrainHeight, y-heightMap.height/2);
                
                //Kolor wierzchołka przechodzi od cyjanu przez zieleń i żółć do czerwieni
                colors[x + y * heightMap.width] = new Color(Mathf.Clamp(2f* vertexHeight, 0f,1f), Mathf.Clamp(2f *(1f - vertexHeight),0f,1f), Mathf.Pow(1f- vertexHeight,3f));
                
                //Dla każdego wierzchołka x>0 y>0 generowane są 2 ściany z sąsiadami z zakresu [x-1,x],[y-1,y]
                if (x > 0 && y > 0)
                {
                    trianglesList.Add(x + (y - 1) * heightMap.width);
                    trianglesList.Add(x - 1 + (y - 1) * heightMap.width);
                    trianglesList.Add(x - 1 + y * heightMap.width);

                    trianglesList.Add(x - 1 + y * heightMap.width);
                    trianglesList.Add(x + y * heightMap.width);
                    trianglesList.Add(x + (y - 1) * heightMap.width);
                }
            }
        }
        triangles = trianglesList.ToArray();

    }
    //Metoda aktualizuje wysokości wierzchołków w Meshu
    void updateMeshData()
    {
        Vector3[] vertices = mesh.vertices;
        for (int y = 0; y < heightMap.height; y++)
        {
            for (int x = 0; x < heightMap.width; x++)
            {
                vertices[x + y * heightMap.width] = new Vector3(x - heightMap.width / 2, heightMap.GetPixel(x, y).grayscale * terrainHeight, y - heightMap.height / 2);
            }
        }
        meshFilter.mesh.SetVertices(vertices);
        meshFilter.mesh.RecalculateNormals();

    }

    private void Update()
    {
        // Klawisz  [  zmniejsza mnożnik wysokości terenu, a klawisz  ]  zwiększa go.
        // Klawisze strzałek obracają i skalują scenę
        // Użycie property TerrainHeight aktualizuje wszystkie wierzchołki po zmianie wartości.
        if (Input.GetKey(KeyCode.LeftBracket))
        {
            TerrainHeight -= 1f * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.RightBracket))
        {
            TerrainHeight += 1f * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            gameObject.transform.localScale = new Vector3(gameObject.transform.localScale.x + Time.deltaTime, gameObject.transform.localScale.y + Time.deltaTime, gameObject.transform.localScale.z + Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            gameObject.transform.localScale = new Vector3(gameObject.transform.localScale.x - Time.deltaTime, gameObject.transform.localScale.y - Time.deltaTime, gameObject.transform.localScale.z - Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            gameObject.transform.Rotate(Vector3.up, Time.deltaTime * 10);
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            gameObject.transform.Rotate(Vector3.up, -Time.deltaTime * 10);
        }
    }
}
