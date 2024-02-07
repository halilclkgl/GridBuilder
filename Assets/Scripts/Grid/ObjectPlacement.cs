using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ObjectPlacement : MonoBehaviour
{
    public GameObject[] objectPrefab; // Eklenmek istenen objenin prefab'ý
    [SerializeField] int currentObj = 0;
    public GridBuildingSystem gridBuildingSystem;
    GridXZ grid;
    public bool x;
    private GameObject previewObject; // Öngösterim nesnesi
    private bool canPlaceObject = false; // Objenin yerleþtirilebileceði durum

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

            // tam sayý yap
            Vector3 roundedPosition = RoundPositionToNearestTen(worldPosition);

            if (Input.GetMouseButtonDown(0))
            {
                if (canPlaceObject && previewObject != null)
                {
                    Destroy(previewObject);

                    List<GridPos> occupiedCells = PlaceObject(roundedPosition, length, width);

                    foreach (GridPos pos in occupiedCells)
                    {
                        Debug.Log("Ýþgal Edilen Hücre Koordinatý: " + pos.x + ", " + pos.z);
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
            // Öngösterim nesnesini yeni konuma taþý
            previewObject.transform.position = position;
        }

        // Obje yerleþtirebileceðimizi kontrol et
        canPlaceObject = !IsCellsOccupied(gridPos.x, gridPos.z, length, width);

        // Öngösterim nesnesinin rengini güncelle
        Renderer previewRenderer = previewObject.GetComponent<Renderer>();
        if (previewRenderer != null)
        {
            previewRenderer.material.color = canPlaceObject ? Color.green : Color.red;
        }
    }

    private List<GridPos> PlaceObject(Vector3 position, int objWidth, int objHeight)
    {
        List<GridPos> occupiedCells = new List<GridPos>();

        // Pozisyonu grid hücresine dönüþtür
        GridPos gridPos = WorldToGridPosition(position);

        // Objeyi yerleþtireceðimiz baþlangýç noktasýný hesapla
        int startX = gridPos.x;
        int startZ = gridPos.z;

        // Baþlangýç noktasýný uygun þekilde ayarla
        startX -= objWidth % 2 == 0 ? (objWidth / 2) - 1 : objWidth / 2;
        startZ -= objHeight % 2 == 0 ? (objHeight / 2) - 1 : objHeight / 2;

        if (objWidth % 2 == 0 && objHeight % 2 == 0)
        {
            position.z += 5;
            position.x += 5;
        }

        // Gerçek nesneyi yerleþtir
        GameObject newObject = Instantiate(objectPrefab[currentObj], position, Quaternion.identity);

        // Hücreleri iþgal et ve iþgal edilen hücreleri listeye ekle
        for (int xOffset = 0; xOffset < objWidth; xOffset++)
        {
            for (int zOffset = 0; zOffset < objHeight; zOffset++)
            {
                int newX = startX + xOffset;
                int newZ = startZ + zOffset;

                // Hücreyi iþgal edildi olarak iþaretle
                grid.GetGridObject(newX, newZ).SetOccupied(true);

                // Ýþgal edilen hücrenin koordinatýný listeye ekle
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

            // Transparan bir materyal kullanarak önizleme nesnesini oluþtur
            previewObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Destroy(previewObject.GetComponent<BoxCollider>()); // Küpün çarpýþma bileþenini kaldýr
            previewObject.transform.position = position; // Pozisyonunu ayarla
            previewObject.transform.localScale = new Vector3(length * 10, 10, width * 10);
            Renderer renderer = previewObject.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = new Color(1f, 1f, 1f, 0.5f); // Transparan beyaz renk
            }

            // Öngösterim nesnesinin boyutunu güncelle
            previewObjectLength = length;
            previewObjectWidth = width;
        }
        else
        {
            previewObject.transform.localScale = new Vector3(length * 10, 10, width * 10);
            // Öngösterim nesnesini yeni konuma taþý
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
            //   Debug.LogError("Baþlangýç koordinatlarý grid sýnýrlarýnýn dýþýnda");
            return true; // Hücre dolu kabul edilir (sýnýrlar dýþýnda)
        }
        if (sizeX <= 0 || sizeZ <= 0 || startX + sizeX > gridWidth || startZ + sizeZ > gridHeight)
        {
            //   Debug.LogError("Boyutlar grid sýnýrlarýnýn dýþýnda veya geçersiz");
            return true; // Hücre dolu kabul edilir 
        }

        startX -= sizeX % 2 == 0 ? sizeX / 2 - 1 : sizeX / 2;
        startZ -= sizeZ % 2 == 0 ? sizeZ / 2 - 1 : sizeZ / 2;

        for (int x = startX; x < startX + sizeX; x++)
        {
            for (int z = startZ; z < startZ + sizeZ; z++)
            {
                if (x >= 0 && x < gridWidth && z >= 0 && z < gridHeight && grid.IsCellOccupied(x, z))
                {
                    //  Debug.Log("Hücre dolu X: " + x + ", Z: " + z);
                    return true;
                }
            }
        }

        //   Debug.Log("Hücreler boþ.");
        return false; // Hücreler boþ ise false döndür
    }

}

