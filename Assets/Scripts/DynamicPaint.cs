using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class DynamicPaint : MonoBehaviour
{
    enum PainterTypes {Vertex, UVVertexTexture};
    PainterTypes painterType;
    public Button painterTypeButtonVertexPainter;
    public Button painterTypeButtonUVVertexTexture;

    // UV texture painter
    public GameObject uvVertexTextureTools;
    public Button uvVertexTextureButtonPattern;
    public Button uvVertexTextureButtonWhite;

    // Vertex painter    
    public GameObject vertexPainterTools;
    public Button vertexPainterButtonRed;
    public Button vertexPainterButtonBlue;
    public Button vertexPainterButtonWhite;
    Color vertexPainterColor;
    enum VertexPainterColorTypes {Red, Blue, White, Black};
    VertexPainterColorTypes vertexPainterColorType;

    public GameObject target;
    Vector3[] vertices;
    List<Color> vertexColors;
    Mesh mesh;
    
    void Start()
    {
        // Get the vertices of the target mesh
        mesh = target.GetComponent<MeshFilter>().mesh;
        vertices = mesh.vertices;

        // Store and set the colors of the vertices to white
        vertexColors = new List<Color>();
        for (int i = 0; i < vertices.Length; i++) {
            vertexColors.Add(Color.white);
        }
        mesh.SetColors(vertexColors);

        // Set the default vertex painter color to red
        this.switchVertexPainterColor(VertexPainterColorTypes.Red);

        // Set the default painter type to vertex painter
        this.switchPainterType(PainterTypes.Vertex);
    }

    // Update is called once per frame
    void Update()
    {
        if ((Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) || Input.GetMouseButton(0))
        {
            Vector3 surfacePosition = Vector3.zero;
            int triangleIndex = -1;
            if(HitTestOnSurface(ref surfacePosition, ref triangleIndex)) {

                switch (painterType)
                {
                    case PainterTypes.Vertex:
                        doVertexPaint(surfacePosition, triangleIndex);
                        break;
                    case PainterTypes.UVVertexTexture:
                        doVertexPaint(surfacePosition, triangleIndex);
                        break;
                }
            }
        }
    }

    bool HitTestOnSurface(ref Vector3 surfacePosition, ref int triangleIndex){
		RaycastHit hit;
        if (!Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100f))
            return false;

        MeshCollider meshCollider = hit.collider as MeshCollider;
        if (hit.transform.tag != "TargetObject" || meshCollider == null || meshCollider.sharedMesh == null)
            return false;

        surfacePosition = hit.point;
        triangleIndex = hit.triangleIndex;
		
        return true;
	}

    public Vector3 FindNearestVertex(Vector3 point)
    {
        point = target.transform.InverseTransformPoint(point);

        float minDistance = Mathf.Infinity;
        Vector3 nearestVertex = Vector3.zero;
        foreach (Vector3 vertex in mesh.vertices)
        {
            Vector3 currentVertex = vertex;
            Vector3 diff = point - currentVertex;
            float dist = diff.sqrMagnitude;
            if (dist < minDistance)
            {
                minDistance = dist;
                nearestVertex = currentVertex;
            }
        }

        return nearestVertex;
    }

    // Painter Types
    public void OnPainterTypeButtonVertexPainter() {
        this.switchPainterType(PainterTypes.Vertex);
        this.switchVertexPainterColor(VertexPainterColorTypes.Red);
    }
    public void OnPainterTypeButtonUVVertexTexture() {
        this.switchPainterType(PainterTypes.UVVertexTexture);
        this.switchUVVertexTextureColor(VertexPainterColorTypes.Black);
    }

    private void disableAllPainterTypeButtons() {
        painterTypeButtonVertexPainter.image.color = new Color(1f, 1f, 1f, .5f);
        painterTypeButtonUVVertexTexture.image.color = new Color(1f, 1f, 1f, .5f);
    }
    private void switchPainterType(PainterTypes type) {
        disableAllPainterTypeButtons();

        painterType = type;

        this.setVertexColorsToWhite();

        switch (painterType)
        {
            case PainterTypes.Vertex:
                this.setVertexColorsToWhite();

                target.GetComponent<Renderer>().material.SetFloat("_JustVertexColor", 1);

                painterTypeButtonVertexPainter.image.color = Color.white;

                // Show the vertex painter tools
                vertexPainterTools.GetComponent<CanvasGroup>().alpha = 1;
                vertexPainterTools.GetComponent<CanvasGroup>().interactable = true;
                vertexPainterTools.GetComponent<CanvasGroup>().blocksRaycasts = true;

                // Hide the uv texture painter tools
                uvVertexTextureTools.GetComponent<CanvasGroup>().alpha = 0;
                uvVertexTextureTools.GetComponent<CanvasGroup>().interactable = false;
                uvVertexTextureTools.GetComponent<CanvasGroup>().blocksRaycasts = false;
                break;

            case PainterTypes.UVVertexTexture:
                target.GetComponent<Renderer>().material.SetFloat("_JustVertexColor", 0);

                painterTypeButtonUVVertexTexture.image.color = Color.white;

                vertexPainterColor = Color.black;

                uvVertexTextureTools.GetComponent<CanvasGroup>().alpha = 1;
                uvVertexTextureTools.GetComponent<CanvasGroup>().interactable = true;
                uvVertexTextureTools.GetComponent<CanvasGroup>().blocksRaycasts = true;

                vertexPainterTools.GetComponent<CanvasGroup>().alpha = 0;
                vertexPainterTools.GetComponent<CanvasGroup>().interactable = false;
                vertexPainterTools.GetComponent<CanvasGroup>().blocksRaycasts = false;
                break;
        }
    }


    // Vertex painter
    private void doVertexPaint(Vector3 surfacePosition, int triangleIndex) {
        Vector3 nearestVertex = this.FindNearestVertex(surfacePosition);

        // Select all vertices based on position
        // int[] indexes = vertices.Select((item, i) => {return item == nearestVertex ? i : -1;}).Where(i => i != -1).ToArray();

        // Set vertices based on triangle index
        int[] indexes = new int[3]{mesh.triangles[triangleIndex * 3], mesh.triangles[triangleIndex * 3 + 1], mesh.triangles[triangleIndex * 3 + 2]};

        foreach (int item in indexes)
        {
            // Set the color of the selected vertices
            vertexColors[item] = vertexPainterColor;
        }

        // Apply the colors on the mesh
        mesh.SetColors(vertexColors);
    }
    public void OnVertexPainterButtonRed() {
        this.switchVertexPainterColor(VertexPainterColorTypes.Red);
    }

    public void OnVertexPainterButtonBlue() {
        this.switchVertexPainterColor(VertexPainterColorTypes.Blue);
    }

    public void OnVertexPainterButtonWhite() {
        this.switchVertexPainterColor(VertexPainterColorTypes.White);
    }

    private void disableAllVertexPainterColorButtons() {
        vertexPainterButtonRed.image.color = new Color(1f, 1f, 1f, .5f);
        vertexPainterButtonBlue.image.color = new Color(1f, 1f, 1f, .5f);
        vertexPainterButtonWhite.image.color = new Color(1f, 1f, 1f, .5f);
    }

    private void switchVertexPainterColor(VertexPainterColorTypes color) {
        this.disableAllVertexPainterColorButtons();

        vertexPainterColorType = color;

        switch (vertexPainterColorType)
        {
            case VertexPainterColorTypes.Red:
                vertexPainterColor = Color.red;
                vertexPainterButtonRed.image.color = Color.white;
                break;
            case VertexPainterColorTypes.Blue:
                vertexPainterColor = new Color(0f, .5f, 1f);
                vertexPainterButtonBlue.image.color = Color.white;
                break;
            case VertexPainterColorTypes.White:
                vertexPainterColor = Color.white;
                vertexPainterButtonWhite.image.color = Color.white;
                break;
        }
    }

    // UV Texture Painter
    public void OnUVVertexTexturePattern() {
        this.switchUVVertexTextureColor(VertexPainterColorTypes.Black);
    }
    public void OnUVVertexTextureWhite() {
        this.switchUVVertexTextureColor(VertexPainterColorTypes.White);
    }

    private void disableAllUVVertexTextureColorButtons() {
        uvVertexTextureButtonPattern.image.color = new Color(1f, 1f, 1f, .5f);
        uvVertexTextureButtonWhite.image.color = new Color(1f, 1f, 1f, .5f);
    }

    private void switchUVVertexTextureColor(VertexPainterColorTypes color) {
        this.disableAllUVVertexTextureColorButtons();

        vertexPainterColorType = color;

        switch (vertexPainterColorType)
        {
            case VertexPainterColorTypes.White:
                vertexPainterColor = Color.white;
                uvVertexTextureButtonWhite.image.color = Color.white;
                break;
            case VertexPainterColorTypes.Black:
                vertexPainterColor = Color.black;
                uvVertexTextureButtonPattern.image.color = Color.white;
                break;
        }
    }


    private void setVertexColorsToWhite() {
        vertexColors = new List<Color>();
        for (int i = 0; i < vertices.Length; i++) {
            vertexColors.Add(Color.white);
        }
        mesh.SetColors(vertexColors);
    }
}
