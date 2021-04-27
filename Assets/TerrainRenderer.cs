/**
    Michał Wójcik 2021
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainRenderer : MonoBehaviour
{
    //Obiekt służący za szablon dla obiektów renderujących linie
    [SerializeField]
    private LineRenderer lineRenderer;
    //Wczytana tekstura mapy wysokości
    [SerializeField]
    private Texture2D heightMap;
    //Wartość mnożnika wysokości terenu
    [SerializeField]
    [Range(1, 10f)]
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
            SetVertices();
        }
    }
    //Bufory na obiekty renderujące linie
    private LineRenderer[] vertLineRenderers;
    private LineRenderer[] horLineRenderers;

    void Start()
    {
        if(heightMap == null)
        {
            return;
        }

        // Pionowych linii jest tyle ile poziomo pikseli, a poziomych linii tyle ile pionowo pikseli.
        vertLineRenderers = new LineRenderer[heightMap.width];
        horLineRenderers = new LineRenderer[heightMap.height];
        
        // Tworzenie obiektów renderujących linie pionowe
        for (int i = 0; i < heightMap.width; i++)
        {
            // Z szablonu tworzony jest nowy LineRenderer. Każdy ma tyle punktów, ile pionowo jest pikseli.
            LineRenderer lr = Instantiate<LineRenderer>(lineRenderer, transform);
            lr.positionCount = heightMap.height;
            // W pętli ustawiana jest pozycja każdego wierzchołka linii - szerokość i długość to współrzędne pikseli,
            // a wysokość to wartość koloru w skali szarości pomnożona przez mnożnik wysokości terenu.
            for (int j = 0; j < heightMap.height; j++)
            {
                lr.SetPosition(j, new Vector3(i, heightMap.GetPixel(i, j).grayscale*terrainHeight, j));   
            }
            // Obiekt renderujący zachowywany jest w buforze dla ułatwienia dostępu do późniejszej modyfikacji wierzchołków
            vertLineRenderers[i] = lr;
        }

        //Tworzenie obiektów renderujących linie poziome - analogicznie do linii pionowych, z zamianą osi X oraz Z.
        for (int i = 0; i < heightMap.height; i++)
        {
            LineRenderer lr = Instantiate<LineRenderer>(lineRenderer, transform);
            lr.positionCount = heightMap.width;
            for (int j = 0; j < heightMap.width; j++)
            {
                lr.SetPosition(j, new Vector3(j, heightMap.GetPixel(j, i).grayscale*terrainHeight, i));
            }
            horLineRenderers[i] = lr;
        }

        // Wyłączenie rysowania na scenie obiektu będącego szablonem.
        lineRenderer.gameObject.SetActive(false);
    }

    // Metoda służąca do aktualizacji wysokości wierzchołków po pierwszym renderze
    private void SetVertices()
    {
        // W pętli ustawiana jest pozycja każdego wierzchołka poszczególnych linii - szerokość i długość to współrzędne pikseli,
        // a wysokość to wartość koloru w skali szarości pomnożona przez mnożnik wysokości terenu.
        
        // Dla linii pionowych
        for (int i = 0; i < heightMap.width; i++)
        {
            for (int j = 0; j < heightMap.height; j++)
            {
                vertLineRenderers[i].SetPosition(j, new Vector3(i, heightMap.GetPixel(i, j).grayscale * terrainHeight, j));
            }
        }

        // Dla linii dla linii poziomych
        for (int i = 0; i < heightMap.height; i++)
        {
            for (int j = 0; j < heightMap.width; j++)
            {
                horLineRenderers[i].SetPosition(j, new Vector3(j, heightMap.GetPixel(j, i).grayscale * terrainHeight, i));
            }
        }
    }

    private void Update()
    {
        // Klawisz  [  zmniejsza mnożnik wysokości terenu, a klawisz  ]  zwiększa go.
        // Użycie property TerrainHeight aktualizuje wszystkie wierzchołki po zmianie wartości.
        if (Input.GetKey(KeyCode.LeftBracket))
        {
            TerrainHeight -= 1f * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.RightBracket))
        {
            TerrainHeight += 1f * Time.deltaTime;
        }
    }
}
