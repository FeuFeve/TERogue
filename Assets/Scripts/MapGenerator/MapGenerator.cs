using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour {
    public List<Texture2D> presetList;
    public List<Texture2D> pathList;
    public int seed;

    public static string[,] nextMap (List<Texture2D> presetList, List<Texture2D> pathList, int layeredPresets, int layeredPaths, int seed, int col, int row) {
        if (presetList.Count == 0 || pathList.Count == 0) return null;

        // Random.InitState(seed);
        string[,] ans = new string[col, row];

        //Initialisation
        for (int x = 0; x < col; x++) {
            for (int y = 0; y < row; y++) {
                ans[x, y] = "floor";
            }
        }

        //Adding n layers of details
        for (int i = 0; i < layeredPresets; i++) {
            Texture2D preset = presetList[Random.Range(0, presetList.Count)];
            Color[] colors = preset.GetPixels(0, 0, col, row);
            for (int x = 0; x < col; x++) {
                for (int y = 0; y < row; y++) {
                    Color c = colors[y * col + x];
                    if (c.a > 0.5f) { // if Transparent, we ignore
                        int colorFlags = 0b000;
                        colorFlags += (int)c.r * 0b001; //R
                        colorFlags += (int)c.g * 0b010; //G
                        colorFlags += (int)c.b * 0b100; //B

                        if (colorFlags == 0b000) { //black, a pit
                            ans[x, y] = "void";
                        } else if (colorFlags == 0b111) { //white, a wall
                            ans[x, y] = "obstacle";
                        }
                    }
                }
            }
        }

        //Adding n layers of paths
        for (int i = 0; i < layeredPaths; i++) {
            Texture2D path = pathList[Random.Range(0, pathList.Count)];
            Color[] colors = path.GetPixels(0, 0, col, row);
            for (int x = 0; x < col; x++) {
                for (int y = 0; y < row; y++) {
                    Color c = colors[y * col + x];
                    if (c.a > 0.5f) { // if Transparent, we ignore
                        ans[x, y] = "floor";
                    }
                }
            }
        }

        return ans;
    }

}
