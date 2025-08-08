using UnityEngine;
using UnityEditor;

public class RotateTerrain : MonoBehaviour
{
    [MenuItem("Game Tools/Rotate 90 Deg")]
    static void Rotate()
    {
        int i, j;
        Terrain ter = Terrain.activeTerrain;
        TerrainData td = ter.terrainData;

        // rotate heightmap
        float[,] hgts = td.GetHeights(0, 0, td.heightmapResolution, td.heightmapResolution);
        float[,] newhgts = new float[hgts.GetLength(0), hgts.GetLength(1)];
        for (j = 0; j < td.heightmapResolution; j++)
        {
            for (i = 0; i < td.heightmapResolution; i++)
            {
                newhgts[td.heightmapResolution - 1 - j, i] = hgts[i, j];
            }
        }
        td.SetHeights(0, 0, newhgts);
        ter.Flush();

        // rotate splatmap
        float[,,] alpha = td.GetAlphamaps(0, 0, td.alphamapWidth, td.alphamapHeight);
        float[,,] newalpha = new float[alpha.GetLength(0), alpha.GetLength(1), alpha.GetLength(2)];
        for (j = 0; j < td.alphamapHeight; j++)
        {
            for (i = 0; i < td.alphamapWidth; i++)
            {
                for (int k = 0; k < td.alphamapLayers; k++)
                {
                    newalpha[td.alphamapHeight - 1 - j, i, k] = alpha[i, j, k];
                }
            }
        }
        td.SetAlphamaps(0, 0, newalpha);

        // rotate trees
        Vector3 size = td.size;
        TreeInstance[] trees = td.treeInstances;
        for (i = 0; i < trees.Length; i++)
        {
            Vector3 pos = trees[i].position;
            pos = new Vector3(1 - pos.z, 0, pos.x);
            pos.y = td.GetInterpolatedHeight(pos.x, pos.z) / size.y;
            trees[i].position = pos;
        }
        td.treeInstances = trees;

        // rotate detail layers
        int num = td.detailPrototypes.Length;
        for (int k = 0; k < num; k++)
        {
            int[,] map = td.GetDetailLayer(0, 0, td.detailWidth, td.detailHeight, k);
            int[,] newmap = new int[map.GetLength(0), map.GetLength(1)];
            for (j = 0; j < td.detailHeight; j++)
            {
                for (i = 0; i < td.detailWidth; i++)
                {
                    newmap[td.detailHeight - 1 - j, i] = map[i, j];
                }
            }
            td.SetDetailLayer(0, 0, k, newmap);
        }
    }
}
