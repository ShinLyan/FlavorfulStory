//Add Tile logic
using System;
using UnityEngine;
public static class TileGridFactory
{
    public static Tile CreateTile(Type tileType)
    {
        Tile newTile = Activator.CreateInstance(tileType) as Tile; // Заменить на более производительный вариант

        //ToDo: Call base constructor and initialize

        return newTile;
    }
    public static Tile[,] CreateTileArray(Type tileType, int Width, int Length)
    {
        Tile[,] newTileArray = new Tile[Width, Length];
        for (int i = 0; i < Width; i++)
        {
            for (int j = 0; j < Length; j++)
            {
                newTileArray[i, j] = Activator.CreateInstance(tileType) as Tile; // Заменить на более производительный вариант
                //ToDo: Call base constructor and initialize
            }
        }
        return newTileArray;
    }
}

/// <summary> Базовый класс сетки.</summary>
public class Grid
{
    //Tiles collection
    private Tile[,] _tiles;
    //Grid type enum
    public enum GridType
    {
        Placeable,
        Farm,
        Empty
    }
    //Constructor
    public Grid(int width, int length, int tileSize, Type type)
    {
        _tiles = TileGridFactory.CreateTileArray(type, width / tileSize, length / tileSize);
    }
    //Getter
    public Tile[,] GetTiles()
    {
        return _tiles;
    }
}
