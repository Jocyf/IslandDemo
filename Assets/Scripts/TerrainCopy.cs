using UnityEngine;

public class TerrainCopy : MonoBehaviour
{
    public GameObject FromGameObject;
    public GameObject ToGameObject;

    private void Start()
    {
        var sourceTerrain = FromGameObject.GetComponent<Terrain>();
        var destTerrain = ToGameObject.GetComponent<Terrain>();
        var destData = destTerrain.terrainData;
        string desired = "ClonedTerrain";
        Debug.Log("Complete", CloneTerrain(sourceTerrain, destData, desired));
    }

    public GameObject CloneTerrain(Terrain sourceTerrain, TerrainData workData, string desiredObjectName)
    {
        // READONLY: TODO: Heightmap Width
        // READONLY: TODO: Heightmap Height
        // Heightmap Resolution
        workData.heightmapResolution = sourceTerrain.terrainData.heightmapResolution;
        // Heightmap Scale TODO: READ ONLY :(
        // workData.heightmapScale = sourceTerrain.terrainData.heightmapScale;
        // Size
        workData.size = sourceTerrain.terrainData.size;
        // Waving Grass Strength
        workData.wavingGrassStrength = sourceTerrain.terrainData.wavingGrassStrength;
        // Waving Grass Amount
        workData.wavingGrassAmount = sourceTerrain.terrainData.wavingGrassAmount;
        // Waving Grass Speed
        workData.wavingGrassSpeed = sourceTerrain.terrainData.wavingGrassSpeed;
        // Waving Grass Tint
        workData.wavingGrassTint = sourceTerrain.terrainData.wavingGrassTint;
        // Detail Width TODO: READ ONLY :(
        // workData.detailWidth = sourceTerrain.terrainData.detailWidth;
        // Detail Height TODO: READ ONLY :(
        // workData.detailHeight = sourceTerrain.terrainData.detailHeight;
        // Detail Prototypes = DetailPrototype[]
        DetailPrototype[] workPrototypes = new DetailPrototype[sourceTerrain.terrainData.detailPrototypes.Length];

        for (int dp = 0; dp < workPrototypes.Length; dp++)
        {
            DetailPrototype clonedPrototype = new DetailPrototype();

            // prototype
            clonedPrototype.prototype = sourceTerrain.terrainData.detailPrototypes[dp].prototype;
            // prototypeTexture
            clonedPrototype.prototypeTexture = sourceTerrain.terrainData.detailPrototypes[dp].prototypeTexture;
            // minWidth
            clonedPrototype.minWidth = sourceTerrain.terrainData.detailPrototypes[dp].minWidth;
            // maxWidth
            clonedPrototype.maxWidth = sourceTerrain.terrainData.detailPrototypes[dp].maxWidth;
            // minHeight
            clonedPrototype.minHeight = sourceTerrain.terrainData.detailPrototypes[dp].minHeight;
            // maxHeight
            clonedPrototype.maxHeight = sourceTerrain.terrainData.detailPrototypes[dp].maxHeight;
            // noiseSpread
            clonedPrototype.noiseSpread = sourceTerrain.terrainData.detailPrototypes[dp].noiseSpread;
            // bendFactor
            clonedPrototype.bendFactor = sourceTerrain.terrainData.detailPrototypes[dp].bendFactor;
            // healthyColor
            clonedPrototype.healthyColor = sourceTerrain.terrainData.detailPrototypes[dp].healthyColor;
            // dryColor
            clonedPrototype.dryColor = sourceTerrain.terrainData.detailPrototypes[dp].dryColor;
            // renderMode
            clonedPrototype.renderMode = sourceTerrain.terrainData.detailPrototypes[dp].renderMode;

            workPrototypes[dp] = clonedPrototype;
        }

        workData.detailPrototypes = workPrototypes;

        // Tree Instances = TreeInstance[]
        TreeInstance[] workTrees = new TreeInstance[sourceTerrain.terrainData.treeInstances.Length];

        for (int ti = 0; ti < workTrees.Length; ti++)
        {
            TreeInstance clonedTree = new TreeInstance();

            // position
            clonedTree.position = sourceTerrain.terrainData.treeInstances[ti].position;
            // widthScale
            clonedTree.widthScale = sourceTerrain.terrainData.treeInstances[ti].widthScale;
            // heightScale
            clonedTree.heightScale = sourceTerrain.terrainData.treeInstances[ti].heightScale;
            // color
            clonedTree.color = sourceTerrain.terrainData.treeInstances[ti].color;
            // lightmapColor
            clonedTree.lightmapColor = sourceTerrain.terrainData.treeInstances[ti].lightmapColor;
            // prototypeIndex
            clonedTree.prototypeIndex = sourceTerrain.terrainData.treeInstances[ti].prototypeIndex;

            workTrees[ti] = clonedTree;
        }

        workData.treeInstances = workTrees;

        // Tree Prototypes = TreePrototype[]
        TreePrototype[] workTreePrototypes = new TreePrototype[sourceTerrain.terrainData.treePrototypes.Length];

        for (int tp = 0; tp < workTreePrototypes.Length; tp++)
        {
            TreePrototype clonedTreePrototype = new TreePrototype();

            // prefab
            clonedTreePrototype.prefab = sourceTerrain.terrainData.treePrototypes[tp].prefab;
            // bendFactor
            clonedTreePrototype.bendFactor = sourceTerrain.terrainData.treePrototypes[tp].bendFactor;

            workTreePrototypes[tp] = clonedTreePrototype;
        }

        workData.treePrototypes = workTreePrototypes;

        // Alphamap Layers TODO: READ ONLY :(
        // workData.alphamapLayers = sourceTerrain.terrainData.alphamapLayers;
        // Alphamap Resolution
        workData.alphamapResolution = sourceTerrain.terrainData.alphamapResolution;
        // Alphamap Width TODO: READ ONLY :(
        // workData.alphamapWidth = sourceTerrain.terrainData.alphamapWidth;
        // Alphamap Height TODO: READ ONLY :(
        // workData.alphamapHeight = sourceTerrain.terrainData.alphamapHeight;
        // Base Map Resolution
        workData.baseMapResolution = sourceTerrain.terrainData.baseMapResolution;
        // Splat Prototypes = SplatPrototype[]
        SplatPrototype[] workSplatPrototypes = new SplatPrototype[sourceTerrain.terrainData.splatPrototypes.Length];

        for (int sp = 0; sp < workSplatPrototypes.Length; sp++)
        {
            SplatPrototype clonedSplatPrototype = new SplatPrototype();
            // texture
            clonedSplatPrototype.texture = sourceTerrain.terrainData.splatPrototypes[sp].texture;
            // tileSize
            clonedSplatPrototype.tileSize = sourceTerrain.terrainData.splatPrototypes[sp].tileSize;
            // tileOffset
            clonedSplatPrototype.tileOffset = sourceTerrain.terrainData.splatPrototypes[sp].tileOffset;

            workSplatPrototypes[sp] = clonedSplatPrototype;
        }

        workData.splatPrototypes = workSplatPrototypes;

        // TODO: Figure out how to copy the resolutionPerPatch - currently hard coded to 16
        workData.SetDetailResolution(sourceTerrain.terrainData.detailResolution, 16);

        float[,] sourceHeights = sourceTerrain.terrainData.GetHeights(0, 0, sourceTerrain.terrainData.heightmapResolution, sourceTerrain.terrainData.heightmapResolution);
        workData.SetHeights(0, 0, sourceHeights);

        float[,,] sourceAlphamaps = sourceTerrain.terrainData.GetAlphamaps(0, 0, sourceTerrain.terrainData.alphamapWidth, sourceTerrain.terrainData.alphamapHeight);
        workData.SetAlphamaps(0, 0, sourceAlphamaps);

        float[,,] newAlphamaps = workData.GetAlphamaps(0, 0, workData.alphamapWidth, workData.alphamapHeight);
        // Detail Layers

        int numDetailLayers = sourceTerrain.terrainData.detailPrototypes.Length;
        for (int layNum = 0; layNum < numDetailLayers; layNum++)
        {
            int[,] thisDetailLayer = sourceTerrain.terrainData.GetDetailLayer(0, 0, sourceTerrain.terrainData.detailWidth, sourceTerrain.terrainData.detailHeight, layNum);
            workData.SetDetailLayer(0, 0, layNum, thisDetailLayer);
        }

        // FUNCTIONS:::
        for (int dli = 0; dli < sourceTerrain.terrainData.detailPrototypes.Length; dli++)
        {
            int[,] curDetailLayer = sourceTerrain.terrainData.GetDetailLayer(0, 0, sourceTerrain.terrainData.detailResolution, sourceTerrain.terrainData.detailResolution, dli);

            workData.SetDetailLayer(0, 0, dli, curDetailLayer);
        }
        // Get Detail Layer
        // Set Detail Layer
        // Get Alphamaps
        // Set Alphamaps
        // Refresh Prototypes
        // Set Detail Resolution
        // Set Heights


        // Step 2: Terrain.CreateTerrainGameObject()
        // TODO: See if I really need this...
        Terrain workTerrain = Terrain.CreateTerrainGameObject(workData).GetComponent<Terrain>();
        workTerrain.gameObject.name = desiredObjectName;
        // Step 3: Copy remaining settings

        // Terrain Data
        // DONE ABOVE
        // Tree Distance
        workTerrain.treeDistance = sourceTerrain.treeDistance;
        // Tree Billboard Distance
        workTerrain.treeBillboardDistance = sourceTerrain.treeBillboardDistance;
        // Tree Cros Fade Length
        workTerrain.treeCrossFadeLength = sourceTerrain.treeCrossFadeLength;
        // Tree Maximum Full LOD Count
        workTerrain.treeMaximumFullLODCount = sourceTerrain.treeMaximumFullLODCount;
        // Detail Object Distance
        workTerrain.detailObjectDistance = sourceTerrain.detailObjectDistance;
        // Detail Object Density
        workTerrain.detailObjectDensity = sourceTerrain.detailObjectDensity;
        // Heightmap Pixel Error
        workTerrain.heightmapPixelError = sourceTerrain.heightmapPixelError;
        // Heightmap Maximum LOD
        workTerrain.heightmapMaximumLOD = sourceTerrain.heightmapMaximumLOD;
        // Baseman Distance
        workTerrain.basemapDistance = sourceTerrain.basemapDistance;
        // Lightmap Index
        workTerrain.lightmapIndex = sourceTerrain.lightmapIndex;
        // Cast Shadows
        workTerrain.shadowCastingMode = sourceTerrain.shadowCastingMode;
        // Editor Render Flags

        // Step 4: Flush
        workData.RefreshPrototypes();
        workTerrain.Flush();

        // Step 5: Duplicate children
        for (int i = 0; i < sourceTerrain.transform.childCount; i++)
        {
            Transform child = sourceTerrain.transform.GetChild(i);

            GameObject newChild = Instantiate(child.gameObject) as GameObject;
            newChild.gameObject.name = child.gameObject.name;
            newChild.transform.parent = workTerrain.transform;
        }

        return workTerrain.gameObject;
    }

}