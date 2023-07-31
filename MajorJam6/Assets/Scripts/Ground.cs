using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GroundProperty
{
    public float humidity;
    public float K, N, P;
    public bool markedForUpdate = false;
    public Vector3Int pos;
    public int type;
    public Quaternion specialRotation;
}

public class Ground : MonoBehaviour
{
    public NutrientMap nutrientMapper;
    public GameObject groundBlockPrefab;
    public GameObject groundWedgePrefab;
    public GameObject groundCornerPrefab;
    public GameObject groundInvCornerPrefab;
    public int size = 10;
    public int height = 4;
    public float seed = 0;
    public Dictionary<Vector2Int, StaticEntity> plants = new Dictionary<Vector2Int, StaticEntity>();
    public Dictionary<Vector3Int, GroundProperty> groundBlocksProp = new Dictionary<Vector3Int, GroundProperty>();

    public Dictionary<Vector3Int, EditableBlock> groundEditableBlocks = new Dictionary<Vector3Int, EditableBlock>();
    public EditableBlock[] editableBlocksList;

    // !!! ONLY ACCESSIBLE AT PLANTING MODE !!!
    public GroundProperty[] groundPropertyArray;

    public MeshFilter combinedGround;
    MeshRenderer meshRenderer;
    public bool editMode = true;
    public void Generate(){
        for(int i = 0; i < size; i += 1){
            for(int ii = 0; ii < size; ii += 1){
                float heightHere =  Mathf.PerlinNoise(((float)i/(float)size)+seed, ((float)ii/(float)size)+seed) * (float)height;
                for(int h = 0;h <= (int)(heightHere); h+=1){
                    PlaceBlock(new Vector3Int(i, h, ii));
                }
            }
        }
        editableBlocksList = new EditableBlock[groundEditableBlocks.Values.Count];
        groundEditableBlocks.Values.CopyTo(editableBlocksList,0);
        nutrientMapper.UpdateMaps();
        
    }
    void AssignNutrients(Vector3Int pos, EditableBlock editable){
        editable.properties.N = Mathf.PerlinNoise(((float)pos.x/(float)size)+seed+552435, ((float)pos.z/(float)size)+seed+552435);
        if(editable.properties.N < 0.5f){
            editable.properties.N = 0;
        }
        editable.properties.N *= 10;
        editable.properties.P = Mathf.PerlinNoise(((float)pos.x/(float)size)+seed+73467, ((float)pos.z/(float)size)+seed+73467);
        if(editable.properties.P < 0.5f){
            editable.properties.P = 0;
        }
        editable.properties.P *= 10;
        editable.properties.K = Mathf.PerlinNoise(((float)pos.x/(float)size)+seed+34123, ((float)pos.z/(float)size)+seed+34123);
        if(editable.properties.K < 0.5f){
            editable.properties.K = 0;
        }
        editable.properties.K *= 10;
        editable.properties.type = editable.type;
        if(editMode){
            if(groundBlocksProp.ContainsKey(pos)){
                editable.properties.pos = pos;
                editable.properties.specialRotation = editable.transform.rotation;
                groundBlocksProp[pos] = editable.properties;
            } else {
                editable.properties.pos = pos;
                editable.properties.specialRotation = editable.transform.rotation;
                groundBlocksProp.Add(pos, editable.properties);
            }
        }
        
    }
    public bool PlaceBlock(Vector3Int where){
        // Place a block on the map while in edit mode
        // update all blocks around it to become wedges/corners/inverted corners to make it look like a mound of dirt
        // returns true on success
        if(groundEditableBlocks.ContainsKey(where)){
            if(groundEditableBlocks[where].type == 0){
                // if the current position is blocked, dont try anything more
                return false;
            } else {
                GameObject newBlock = Instantiate(groundBlockPrefab, (Vector3)where, Quaternion.identity);
                newBlock.transform.parent = transform;
                DestroyImmediate(groundEditableBlocks[where].gameObject);
                groundEditableBlocks[where] = newBlock.GetComponent<EditableBlock>();
                AssignNutrients(where,groundEditableBlocks[where]);
            }
        } else {
            GameObject newBlock = Instantiate(groundBlockPrefab, (Vector3)where, Quaternion.identity);
            newBlock.transform.parent = transform;
            groundEditableBlocks.Add(where, newBlock.GetComponent<EditableBlock>());
            AssignNutrients(where,groundEditableBlocks[where]);
        }
        Vector3Int[] positions = {Vector3Int.right, Vector3Int.forward, Vector3Int.back, Vector3Int.left};
        foreach(Vector3Int offset in positions){
            if((where+offset).x >= size-1 || (where+offset).y >= size-1 || (where+offset).z >= size-1 || 
                (where+offset).x < 0 || (where+offset).y < 0 || (where+offset).z < 0){
                continue;
            }
            if(!groundEditableBlocks.ContainsKey(where + offset)){
                GameObject newWedge = Instantiate(groundWedgePrefab, (Vector3)(where + offset), Quaternion.LookRotation(offset, Vector3.up));
                newWedge.transform.parent = transform;
                groundEditableBlocks.Add(where + offset, newWedge.GetComponent<EditableBlock>());
                AssignNutrients(where + offset,groundEditableBlocks[where + offset]);
            } else {
                if(groundEditableBlocks[where + offset].type == 1 ){
                    if(groundEditableBlocks[where+offset].transform.forward != -offset){
                        Quaternion rotation;
                        rotation = Quaternion.LookRotation(groundEditableBlocks[where+offset].transform.forward + offset, Vector3.up);
                        GameObject newInvCorner = Instantiate(groundInvCornerPrefab, (Vector3)(where + offset), rotation);
                        newInvCorner.transform.parent = transform;
                        DestroyImmediate(groundEditableBlocks[where+offset].gameObject);
                        groundEditableBlocks[where+offset] = newInvCorner.GetComponent<EditableBlock>();
                        AssignNutrients(where + offset,groundEditableBlocks[where + offset]);
                    } else{
                        // two opposing wedges make a full block?
                        GameObject newBlock = Instantiate(groundBlockPrefab, (Vector3)(where + offset), Quaternion.identity);
                        newBlock.transform.parent = transform;
                        DestroyImmediate(groundEditableBlocks[where+offset].gameObject);
                        groundEditableBlocks[where+offset] = newBlock.GetComponent<EditableBlock>();
                        AssignNutrients(where + offset,groundEditableBlocks[where + offset]);
                    }
                    
                } else if (groundEditableBlocks[where + offset].type == 2 ){
                    GameObject newWedge = Instantiate(groundWedgePrefab, (Vector3)(where + offset), Quaternion.LookRotation(offset, Vector3.up));
                    newWedge.transform.parent = transform;
                    DestroyImmediate(groundEditableBlocks[where+offset].gameObject);
                    groundEditableBlocks[where+offset] = newWedge.GetComponent<EditableBlock>();
                    AssignNutrients(where + offset,groundEditableBlocks[where + offset]);
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
            if(!groundEditableBlocks.ContainsKey(where + offset)){
                GameObject newCorner = Instantiate(groundCornerPrefab, (Vector3)(where + offset), Quaternion.LookRotation(positions[i], Vector3.up));
                newCorner.transform.parent = transform;
                groundEditableBlocks.Add(where + offset, newCorner.GetComponent<EditableBlock>());
                AssignNutrients(where + offset,groundEditableBlocks[where + offset]);
            } else {
                if(groundEditableBlocks[where + offset].type != 0 ){
                    // do something to combine wedges together? or just replace them? idk
                }
            }
            i += 1;
        }
        Vector3Int[] cornersAndSides = {Vector3Int.forward+Vector3Int.right, Vector3Int.forward+Vector3Int.left, Vector3Int.back+ Vector3Int.right, Vector3Int.back+Vector3Int.left,Vector3Int.right, Vector3Int.forward, Vector3Int.back, Vector3Int.left};
        foreach(Vector3Int pos in cornersAndSides){
            Vector3Int offset = pos + Vector3Int.down;
            if((where+offset).x >= size-1 || (where+offset).y >= size-1 || (where+offset).z >= size-1 || 
                (where+offset).x < 0 || (where+offset).y < 0 || (where+offset).z < 0){
                continue;
            }
            if(!groundEditableBlocks.ContainsKey(where + offset)){
                GameObject newBlock = Instantiate(groundWedgePrefab, (Vector3)(where + offset), Quaternion.LookRotation(offset, Vector3.up));
                newBlock.transform.parent = transform;
                groundEditableBlocks.Add(where + offset, newBlock.GetComponent<EditableBlock>());
                AssignNutrients(where + offset,groundEditableBlocks[where + offset]);
            } else {
                if(groundEditableBlocks[where + offset].type != 0 ){
                    GameObject newBlock = Instantiate(groundBlockPrefab, (Vector3)(where + offset), Quaternion.identity);
                    newBlock.transform.parent = transform;
                    DestroyImmediate(groundEditableBlocks[where+offset].gameObject);
                    groundEditableBlocks[where+offset] = newBlock.GetComponent<EditableBlock>();
                    AssignNutrients(where + offset,groundEditableBlocks[where + offset]);
                }
            }
        }
        return true;
    }
    public void SaveGround(){
        // thanks to Bunzaga on the forums for the help with this holy fuck

        List<Material> materials = new List<Material>();
        MeshFilter meshFilterCombine = gameObject.GetComponent<MeshFilter>();
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
            if(meshFilter == meshFilterCombine){
                continue;
            }
            meshFilter.GetComponent<EditableBlock>().properties.pos = Vector3Int.FloorToInt(meshFilter.transform.position);
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
        groundPropertyArray = new GroundProperty[groundBlocksProp.Values.Count];
        groundBlocksProp.Values.CopyTo(groundPropertyArray, 0);
        // attached to this gameobject
        
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
        GetComponent<MeshCollider>().sharedMesh = meshFilterCombine.sharedMesh;
        groundEditableBlocks.Clear();
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
        meshRenderer.enabled = true;
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
        groundEditableBlocks.Clear();
        foreach(Vector3Int position in groundBlocksProp.Keys){
            GroundProperty property = groundBlocksProp[position];
            if(property.type == 0){
                GameObject newBlock = Instantiate(groundBlockPrefab, (Vector3)position, property.specialRotation);
                newBlock.transform.parent = transform;
                groundEditableBlocks.Add(position, newBlock.GetComponent<EditableBlock>());
                
            }
            if(property.type == 1){
                GameObject newBlock = Instantiate(groundWedgePrefab, (Vector3)position, property.specialRotation);
                newBlock.transform.parent = transform;
                groundEditableBlocks.Add(position, newBlock.GetComponent<EditableBlock>());
                
            }
            if(property.type == 2){
                GameObject newBlock = Instantiate(groundCornerPrefab, (Vector3)position, property.specialRotation);
                newBlock.transform.parent = transform;
                groundEditableBlocks.Add(position, newBlock.GetComponent<EditableBlock>());
                
            }
            if(property.type == 3){
                GameObject newBlock = Instantiate(groundInvCornerPrefab, (Vector3)position, property.specialRotation);
                newBlock.transform.parent = transform;
                groundEditableBlocks.Add(position, newBlock.GetComponent<EditableBlock>());
                
            }
            groundEditableBlocks[position].properties = property;
            groundEditableBlocks[position].test_thing = true;
        }
        editableBlocksList = new EditableBlock[groundEditableBlocks.Values.Count];
        groundEditableBlocks.Values.CopyTo(editableBlocksList,0);
    }
    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }
    public void SwitchMode(){
        if(editMode){
            SaveGround();
            editMode = false;
        } else {
            LoadGround();
            editMode = true;
        }
    }
    float delay = 0;
    void Update()
    {
        if(editMode){
            delay =0;
            foreach(EditableBlock editableBlock in editableBlocksList){
                if(editableBlock.test_thing){
                    editableBlock.UpdateHumidity();
                    if(editableBlock.properties.humidity <= 100){
                        editableBlock.meshRenderer.material = editableBlock.types[0]; 
                    }
                    if(editableBlock.properties.humidity < 51){
                        editableBlock.meshRenderer.material = editableBlock.types[1]; 
                    }
                    if(editableBlock.properties.humidity < 26){
                        editableBlock.meshRenderer.material = editableBlock.types[2]; 
                    }
                    if(editableBlock.properties.humidity < 5){
                        editableBlock.meshRenderer.material = editableBlock.types[3]; 
                    }
                    editableBlock.test_thing = false;
                }
            }
        } else {
            delay += Time.deltaTime;
            if(delay > 300){
                delay = 0;
                nutrientMapper.UpdateMaps();
            }
        }
    }
}
