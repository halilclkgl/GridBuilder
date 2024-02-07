using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ObjectPlacement : MonoBehaviour
{
    public GameObject[] objectPrefab; // Eklenmek istenen objenin prefab'�
    [SerializeField] int currentObj = 0;
    public GridBuildingSystem gridBuildingSystem;
    GridXZ grid;
    public bool x;
    private GameObject previewObject; // �ng�sterim nesnesi
    private bool canPlaceObject = false; // Objenin yerle�tirilebilece�i durum

    int previewObjectLength;
    int previewObjectWidth;
    public int length;
    public int width;

    private void Start()
    {
        grid = gridBuildingSystem.ReturnGrid();
        length = objectPrefab[currentObj].GetComponent<Obj>().length;
        width = objectPrefab[currentObj].GetComponent<Obj>().width;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (currentObj < objectPrefab.Length - 1)
            {
                currentObj++;
                length = objectPrefab[currentObj].GetComponent<Obj>().length;
                width = objectPrefab[currentObj].GetComponent<Obj>().width;
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (currentObj > 0)
            {
                currentObj--;
                length = objectPrefab[currentObj].GetComponent<Obj>().length;
                width = objectPrefab[currentObj].GetComponent<Obj>().width;
            }
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            x = !x;
        }

        if (!x)
            return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            Vector3 worldPosition = hit.point;
            GridPos gridPos = WorldToGridPosition(worldPosition);

            // tam say� yap
            Vector3 roundedPosition = RoundPositionToNearestTen(worldPosition);

            if (Input.GetMouseButtonDown(0))
            {
                if (canPlaceObject && previewObject != null)
                {
                    Destroy(previewObject);

                    List<GridPos> occupiedCells = PlaceObject(roundedPosition, length, width);

                    foreach (GridPos pos in occupiedCells)
                    {
                        Debug.Log("��gal Edilen H�cre Koordinat�: " + pos.x + ", " + pos.z);
                    }
                }
            }
            else
            {
                UpdatePreviewObject(roundedPosition);
            }
        }
    }

    private void UpdatePreviewObject(Vector3 position)
    {
        GridPos gridPos = WorldToGridPosition(position);
        if (length % 2 == 0)
        {
            position.x += 5;
        }
        if (width % 2 == 0)
        {
            position.z += 5;
        }

        if (previewObject == null || previewObjectLength != length || previewObjectWidth != width)
        {
            CreateOrUpdatePreviewObject(position);
        }
        else
        {
            // �ng�sterim nesnesini yeni konuma ta��
            previewObject.transform.position = position;
        }

        // Obje yerle�tirebilece�imizi kontrol et
        canPlaceObject = !IsCellsOccupied(gridPos.x, gridPos.z, length, width);

        // �ng�sterim nesnesinin rengini g�ncelle
        Renderer previewRenderer = previewObject.GetComponent<Renderer>();
        if (previewRenderer != null)
        {
            previewRenderer.material.color = canPlaceObject ? Color.green : Color.red;
        }
    }

    private List<GridPos> PlaceObject(Vector3 position, int objWidth, int objHeight)
    {
        List<GridPos> occupiedCells = new List<GridPos>();

        // Pozisyonu grid h�cresine d�n��t�r
        GridPos gridPos = WorldToGridPosition(position);

        // Objeyi yerle�tirece�imiz ba�lang�� noktas�n� hesapla
        int startX = gridPos.x;
        int startZ = gridPos.z;

        // Ba�lang�� noktas�n� uygun �ekilde ayarla
        startX -= objWidth % 2 == 0 ? (objWidth / 2) - 1 : objWidth / 2;
        startZ -= objHeight % 2 == 0 ? (objHeight / 2) - 1 : objHeight / 2;

        if (objWidth % 2 == 0 && objHeight % 2 == 0)
        {
            position.z += 5;
            position.x += 5;
        }

        // Ger�ek nesneyi yerle�tir
        GameObject newObject = Instantiate(objectPrefab[currentObj], position, Quaternion.identity);

        // H�creleri i�gal et ve i�gal edilen h�creleri listeye ekle
        for (int xOffset = 0; xOffset < objWidth; xOffset++)
        {
            for (int zOffset = 0; zOffset < objHeight; zOffset++)
            {
                int newX = startX + xOffset;
                int newZ = startZ + zOffset;

                // H�creyi i�gal edildi olarak i�aretle
                grid.GetGridObject(newX, newZ).SetOccupied(true);

                // ��gal edilen h�crenin koordinat�n� listeye ekle
                occupiedCells.Add(new GridPos(newX, newZ));
            }
        }

        return occupiedCells;
    }

    private GridPos WorldToGridPosition(Vector3 worldPosition)
    {
        int x = Mathf.RoundToInt((worldPosition.x - grid.GetOriginPosition().x) / grid.GetCellSize());
        int z = Mathf.RoundToInt((worldPosition.z - grid.GetOriginPosition().z) / grid.GetCellSize());
        x = Mathf.Max(x, 0);
        z = Mathf.Max(z, 0);

        return new GridPos(x, z);
    }

    private Vector3 RoundPositionToNearestTen(Vector3 position)
    {
        float roundedX = Mathf.Round(position.x / 10) * 10;
        float roundedY = Mathf.Round(position.y / 10) * 10;
        float roundedZ = Mathf.Round(position.z / 10) * 10;

        return new Vector3(roundedX, roundedY, roundedZ);
    }

    private void CreateOrUpdatePreviewObject(Vector3 position)
    {
        if (previewObject == null || previewObjectLength != length || previewObjectWidth != width)
        {
            if (previewObject != null)
            {
                Destroy(previewObject);
            }

            // Transparan bir materyal kullanarak �nizleme nesnesini olu�tur
            previewObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Destroy(previewObject.GetComponent<BoxCollider>()); // K�p�n �arp��ma bile�enini kald�r
            previewObject.transform.position = position; // Pozisyonunu ayarla
            previewObject.transform.localScale = new Vector3(length * 10, 10, width * 10);
            Renderer renderer = previewObject.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = new Color(1f, 1f, 1f, 0.5f); // Transparan beyaz renk
            }

            // �ng�sterim nesnesinin boyutunu g�ncelle
            previewObjectLength = length;
            previewObjectWidth = width;
        }
        else
        {
            previewObject.transform.localScale = new Vector3(length * 10, 10, width * 10);
            // �ng�sterim nesnesini yeni konuma ta��
            previewObject.transform.position = position;
        }
    }

    private bool IsCellsOccupied(int startX, int startZ, int sizeX, int sizeZ)
    {
        //  Debug.Log("StartX: " + startX + ", StartZ: " + startZ + ", SizeX: " + sizeX + ", SizeZ: " + sizeZ);

        int gridWidth = grid.GetWidth();
        int gridHeight = grid.GetHeight();

        if (startX < 0 || startX >= gridWidth || startZ < 0 || startZ >= gridHeight)
        {
            //   Debug.LogError("Ba�lang�� koordinatlar� grid s�n�rlar�n�n d���nda");
            return true; // H�cre dolu kabul edilir (s�n�rlar d���nda)
        }
        if (sizeX <= 0 || sizeZ <= 0 || startX + sizeX > gridWidth || startZ + sizeZ > gridHeight)
        {
            //   Debug.LogError("Boyutlar grid s�n�rlar�n�n d���nda veya ge�ersiz");
            return true; // H�cre dolu kabul edilir 
        }

        startX -= sizeX % 2 == 0 ? sizeX / 2 - 1 : sizeX / 2;
        startZ -= sizeZ % 2 == 0 ? sizeZ / 2 - 1 : sizeZ / 2;

        for (int x = startX; x < startX + sizeX; x++)
        {
            for (int z = startZ; z < startZ + sizeZ; z++)
            {
                if (x >= 0 && x < gridWidth && z >= 0 && z < gridHeight && grid.IsCellOccupied(x, z))
                {
                    //  Debug.Log("H�cre dolu X: " + x + ", Z: " + z);
                    return true;
                }
            }
        }

        //   Debug.Log("H�creler bo�.");
        return false; // H�creler bo� ise false d�nd�r
    }

}

