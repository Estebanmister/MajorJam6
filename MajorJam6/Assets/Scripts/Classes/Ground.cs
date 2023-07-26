using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SerializeField]
public class GroundProperty
{
    public float humidity;
    public float K, N, P;
}

public class Ground : MonoBehaviour
{
    public GameObject groundBlockPrefab;
    public GameObject groundWedgePrefab;
    public GameObject groundCornerPrefab;
    public int size = 10;
    public Dictionary<Vector3Int, GroundProperty> groundBlocksProp = new Dictionary<Vector3Int, GroundProperty>();

    Dictionary<Vector3Int, EditableBlock> groundBlocksTemp = new Dictionary<Vector3Int, EditableBlock>();
    public MeshFilter combinedGround;
    MeshRenderer meshRenderer;
    public bool editMode = true;
    public void Generate(){
        for(int i = 0; i < size; i += 1){
            for(int ii = 0; ii < size; ii += 1){
                GameObject newBlock = Instantiate(groundBlockPrefab, new Vector3(i, 0, ii), Quaternion.identity);
                newBlock.transform.parent = transform;
                groundBlocksTemp.Add(new Vector3Int(i, 0, ii), newBlock.GetComponent<EditableBlock>());
                if(Mathf.PerlinNoise((float)i/(float)size, (float)ii/(float)size) > 0.5f){
                    PlaceBlock(new Vector3Int(i, 1, ii));
                }
            }
        }
    }
    public void PlaceBlock(Vector3Int where){
        if(groundBlocksTemp.ContainsKey(where)){
            if(groundBlocksTemp[where].type == 0){
                return;
            } else {
                GameObject newBlock = Instantiate(groundBlockPrefab, (Vector3)where, Quaternion.identity);
                newBlock.transform.parent = transform;
                DestroyImmediate(groundBlocksTemp[where].gameObject);
                groundBlocksTemp[where] = newBlock.GetComponent<EditableBlock>();
            }
        } else {
            GameObject newBlock = Instantiate(groundBlockPrefab, (Vector3)where, Quaternion.identity);
            newBlock.transform.parent = transform;
            groundBlocksTemp.Add(where, newBlock.GetComponent<EditableBlock>());
        }

        // todo: move all this to EditableBlock, make the blocks turn themselves into wedges/corners automatically

        Vector3Int[] positions = {Vector3Int.right, Vector3Int.forward, Vector3Int.back, Vector3Int.left};
        foreach(Vector3Int offset in positions){
            if((where+offset).x >= size-1 || (where+offset).y >= size-1 || (where+offset).z >= size-1 || 
                (where+offset).x < 0 || (where+offset).y < 0 || (where+offset).z < 0){
                continue;
            }
            if(!groundBlocksTemp.ContainsKey(where + offset)){
                GameObject newWedge = Instantiate(groundWedgePrefab, (Vector3)(where + offset), Quaternion.LookRotation(offset, Vector3.up));
                newWedge.transform.parent = transform;
                groundBlocksTemp.Add(where + offset, newWedge.GetComponent<EditableBlock>());
            } else {
                if(groundBlocksTemp[where + offset].type != 0 ){
                    GameObject newWedge = Instantiate(groundWedgePrefab, (Vector3)(where + offset), Quaternion.LookRotation(offset, Vector3.up));
                    newWedge.transform.parent = transform;
                    DestroyImmediate(groundBlocksTemp[where+offset].gameObject);
                    groundBlocksTemp[where+offset] = newWedge.GetComponent<EditableBlock>();
                } else {

                }
            }
        }
        Vector3Int[] corners = {Vector3Int.forward+Vector3Int.right, Vector3Int.forward+Vector3Int.left, Vector3Int.back+ Vector3Int.right, Vector3Int.back+Vector3Int.left};
        int i = 0;
        foreach(Vector3Int offset in corners){
            if((where+offset).x >= size-1 || (where+offset).y >= size-1 || (where+offset).z >= size-1 || 
                (where+offset).x < 0 || (where+offset).y < 0 || (where+offset).z < 0){
                i += 1;
                continue;
            }
            if(!groundBlocksTemp.ContainsKey(where + offset)){
                GameObject newCorner = Instantiate(groundCornerPrefab, (Vector3)(where + offset), Quaternion.LookRotation(positions[i], Vector3.up));
                newCorner.transform.parent = transform;
                groundBlocksTemp.Add(where + offset, newCorner.GetComponent<EditableBlock>());
            } else {
                if(groundBlocksTemp[where + offset].type != 0 ){
                    // do something to combine wedges together? or just replace them? idk
                }
            }
            i += 1;
        }

    }
    public void SaveGround(){
        // thanks to Bunzaga on the forums for the help with this holy fuck

        List<Material> materials = new List<Material>();
        ArrayList combineInstanceArrays = new ArrayList();
        MeshFilter[] meshFilters = gameObject.GetComponentsInChildren<MeshFilter>();
        groundBlocksProp.Clear();
        foreach (MeshFilter meshFilter in meshFilters)
        {
            MeshRenderer meshRenderer = meshFilter.GetComponent<MeshRenderer>();
            if (!meshRenderer || !meshFilter.sharedMesh || meshRenderer.sharedMaterials.Length != meshFilter.sharedMesh.subMeshCount)
            {
                continue;
            }
            groundBlocksProp.Add(Vector3Int.FloorToInt(meshFilter.transform.position),meshFilter.GetComponent<EditableBlock>().properties);
            for (int s = 0; s < meshFilter.sharedMesh.subMeshCount; s++)
            {
                // this loop wont be needed probably
                // remove at the end of development if it was not used

                // check by name of material
                int materialArrayIndex = Contains(materials, meshRenderer.sharedMaterial.name);

                if (materialArrayIndex == -1)
                {
                materials.Add(meshRenderer.sharedMaterial);
                materialArrayIndex = materials.Count - 1;
                }
                combineInstanceArrays.Add(new ArrayList());

                CombineInstance combineInstance = new CombineInstance();
                combineInstance.transform = meshRenderer.transform.localToWorldMatrix;
                combineInstance.subMeshIndex = s;
                combineInstance.mesh = meshFilter.sharedMesh;
                (combineInstanceArrays[materialArrayIndex] as ArrayList).Add(combineInstance);
            }
        }

        // attached to this gameobject
        MeshFilter meshFilterCombine = gameObject.GetComponent<MeshFilter>();
        MeshRenderer meshRendererCombine = gameObject.GetComponent<MeshRenderer>();

        // create lists of meshes according to their material
        Mesh[] meshes = new Mesh[materials.Count];
        // each group of meshes will be combined together
        CombineInstance[] combineInstances = new CombineInstance[materials.Count];

        // for each material, make their combine instance
        for (int m = 0; m < materials.Count; m++)
        {
            CombineInstance[] combineInstanceArray = (combineInstanceArrays[m] as ArrayList).ToArray(typeof(CombineInstance)) as CombineInstance[];
            meshes[m] = new Mesh();
            // most worlds wont be anywhere near 200x200 blocks, but just in case
            meshes[m].indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
            meshes[m].CombineMeshes(combineInstanceArray, true, true);

            combineInstances[m] = new CombineInstance();
            combineInstances[m].mesh = meshes[m];
            combineInstances[m].subMeshIndex = 0;
        }

        // combine into one
        meshFilterCombine.sharedMesh = new Mesh();
        meshFilterCombine.sharedMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        meshFilterCombine.sharedMesh.CombineMeshes(combineInstances, false, false);
        groundBlocksTemp.Clear();
        // destroy other meshes
        foreach (Mesh oldMesh in meshes)
        {
            oldMesh.Clear();
            DestroyImmediate(oldMesh);
        }

        // Assign materials
        Material[] materialsArray = materials.ToArray();
        meshRendererCombine.materials = materialsArray;

        foreach (MeshFilter meshFilter in meshFilters)
        {
            if(meshFilter == meshFilterCombine){
                continue;
            }
            DestroyImmediate(meshFilter.gameObject);
        }
        editMode = false;
        }

    int Contains(List<Material> searchList, string searchName) {
        for (int i = 0; i < searchList.Count; i++)
        {
            if (searchList[i].name == searchName)
            {
                return i;
            }
        }
        return -1;
    }
    public void LoadGround(){
        // load the ground for editing
        meshRenderer.enabled = false;
        foreach(Vector3Int position in groundBlocksProp.Keys){
            GroundProperty property = groundBlocksProp[position];
            GameObject newBlock = Instantiate(groundBlockPrefab, new Vector3(position.x, position.y, position.z), Quaternion.identity);
            newBlock.transform.parent = transform;
            newBlock.GetComponent<EditableBlock>().properties = property;
            groundBlocksTemp.Add(position, newBlock.GetComponent<EditableBlock>());
        }
        editMode = true;
    }
    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        Generate();
    }

    // Update is called once per frame
    void Update()
    {
        if(editMode){
            SaveGround();
        }
    }
}
