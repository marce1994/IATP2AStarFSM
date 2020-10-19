using UnityEngine;

public class NoiseGenerator
{
    int width = 25;
    int height = 25;
    
    public float scale = 20f;

    public float offsetX = 100f;
    public float offsetY = 100f;

    public NoiseGenerator(int width, int height)
    {
        this.width = width;
        this.height = height;
    }

    public float[,] GenerateNoise() {
        var result = new float[width, height];
        
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                result[x, y] = Calculate(x, y);
            }
        }

        return result;
    }

    float Calculate(int x, int y) {

        float xCoord = (float)x / width * scale + offsetX;
        float yCoord = (float)y / height * scale + offsetY;
        return Mathf.PerlinNoise(xCoord, yCoord);
    }
}
