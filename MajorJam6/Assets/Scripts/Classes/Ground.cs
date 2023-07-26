using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SerializeField]
public class GroundProperty
{
    public Vector3Int position;
    public float humidity;
    public float K, N, P;
}

public class Ground : MonoBehaviour
{
    public GameObject groundBlockPrefab;
    public int size = 10;
    public List<GroundProperty> groundBlocksProp = new List<GroundProperty>();
    public MeshFilter combinedGround;
    MeshRenderer meshRenderer;
    public bool editMode = true;
    public void Generate(){
        for(int i = 0; i < size; i += 1){
            for(int ii = 0; ii < size; ii += 1){
                GameObject newBlock = Instantiate(groundBlockPrefab, new Vector3(i, 0, ii), Quaternion.identity);
                newBlock.transform.parent = transform;
            }
        }
    }
    public void SaveGround(){
        // thanks to Bunzaga on the forums for the help with this holy fuck

        List<Material> materials = new List<Material>();
        ArrayList combineInstanceArrays = new ArrayList();
        MeshFilter[] meshFilters = gameObject.GetComponentsInChildren<MeshFilter>();

        foreach (MeshFilter meshFilter in meshFilters)
        {
            MeshRenderer meshRenderer = meshFilter.GetComponent<MeshRenderer>();
            if (!meshRenderer || !meshFilter.sharedMesh || meshRenderer.sharedMaterials.Length != meshFilter.sharedMesh.subMeshCount)
            {
                continue;
            }

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
        foreach(GroundProperty property in groundBlocksProp){
            GameObject newBlock = Instantiate(groundBlockPrefab, new Vector3(property.position.x, property.position.y, property.position.z), Quaternion.identity);
            newBlock.transform.parent = transform;
            newBlock.GetComponent<EditableBlock>().properties = property;
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
