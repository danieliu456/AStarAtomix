using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Levels : MonoBehaviour
{
    public List<Level> gameLevels { get; set; } = new List<Level>();
    public string Path { get; set; } = "levels";

    private string text;
    void Awake()
    {
        TextAsset textAsset = (TextAsset)Resources.Load(Path);
        
        if (textAsset)
        {
            Debug.LogWarning($" {Path} : loaded successfully");
        }
        else
        {
            Debug.LogError($" {Path} does not exist :/ please check and try agen :)");
            return;
        }

        text = textAsset.text;
        var levels = text.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);


        foreach (var levelText in levels)
        {
            var temp = levelText.Split(new string[] { "\r\n\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            Level level = new Level(temp[2].Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries))
            {
                Rows = temp[0].Split(new string[] { "\r\n" }, StringSplitOptions.None).ToList(),
                Answer = temp[1].Split(new string[] { "\r\n" }, StringSplitOptions.None).ToList(),
            };
            level.LevelNumber = gameLevels.Count;
            Debug.Log($"Added: { gameLevels.Count} level :)");
            gameLevels.Add(level);
            
        }
    }
}

public class Level
{
    public int LevelNumber { get; set; }
    public List<string> Rows { get; set; } = new List<string>();
    public List<string> Answer { get; set; } = new List<string>();
    public List<Element> Elements { get; set; }

    public Level(string[] elements)
    {
        Elements = new List<Element>();

        foreach (var element in elements)
        {
            Element elem = new Element(element);
            Elements.Add(elem);
        }
    }

    public int Height { get => Rows.Count; }
    public int Width { get => getWidth(); }

    private int getWidth()
    {
        int length = 0;
        foreach (var row in Rows)
        {
            if (row.Length > length) length = row.Length;
        }

        return length;
    }
}

public class Element
{
    public string Name { get; set; }
    public string Char { get; set; }

    public List<double> Degrees { get; set; } = new List<double>();
    public GameObject Elem { get; set; }

    public Element(string element)
    {
        var s = element.Split(' ');
        Name = s[0];
        Char = s[1];



        for (int i = 2; i < s.Length; i++)
        {
            string temp = $"{s[i].Substring(1)}";
            Degrees.Add(CalculateDegrees(temp));
        }
    }

    public int CalculateDegrees(string character)
    {
        switch (character)
        {
            case "U":
                return 0;
            case "R":
                return 90;
            case "D":
                return 180;
            case "L":
                return 270;
            case "RU":
                return 45;
            case "RD":
                return 135;
            case "LD":
                return 225;
            case "LU":
                return 315;
            default:
                throw new ArgumentOutOfRangeException("Could not define degrees for level");
        }
    }
}

public static class AtomsColors
{
    public static Dictionary<string, Color> MAP = new Dictionary<string, Color>
    {
        {"s", new Color(204, 118, 95) },
        {"c", new Color(92, 155, 209) },
        {"o", new Color(151, 151, 151) },
        {"p", new Color(92, 155, 209) },
        {"h", new Color(209, 92, 174) },
        {"n", new Color(132, 215, 101) },
        {"cr", new Color(217, 212, 84)},
        {"f", new Color(209, 92, 92) },
        {"br", new Color(129, 95, 205) },
        {"cl", new Color(209, 166, 92) }
    };

}

public static class NumericExtensions
{
    public static double ToRadians(this double val)
    {
        return (Math.PI / 180) * val;
    }
}