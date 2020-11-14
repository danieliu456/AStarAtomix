using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.InteropServices.ComTypes;

public class LevelBuilder : MonoBehaviour
{
    public GameObject Core;
    public GameObject Branch;
    public GameObject TextCanvas;
    public GameObject AtomBorder;
    public List<ElementMap> ElementsMap;
    public List<AtomBuilder> answerAtoms;
    public Level level;
    public int CurrentLevel;

    GameObject GetElementFromMap(char character)
    {
        ElementMap _element = ElementsMap.Find(element => element.symbol == character);
        return _element?.gameObject;
    }

    public void setNextLevel()
    {
        CurrentLevel += 1;
        Debug.Log($"Setting next level {CurrentLevel} :)");
        if (CurrentLevel >= GetComponent<Levels>().gameLevels.Count) CurrentLevel = 0;
    }

    public void setBackLevel()
    {
        CurrentLevel -= 1;
        Debug.Log($"Setting back level {CurrentLevel} :)");
        if (CurrentLevel <= 0) CurrentLevel = GetComponent<Levels>().gameLevels.Count - 1;
    }

    public void Build()
    {
        level = GetComponent<Levels>().gameLevels[CurrentLevel];
        answerAtoms = new List<AtomBuilder>();

        // Ieskom vidurio pozicijos, kad butu patogu generuoti zemelapi
        int RememberedXPosition = -level.Width / 2;
        int XPosition = -level.Width / 2;
        int YPosition = level.Height / 2;

        foreach (var row in level.Rows)
        {
            foreach (var character in row)
            {
                GameObject gameObject = GetElementFromMap(character);

                if (gameObject != null)
                {
                    Instantiate(gameObject, new Vector3(XPosition, YPosition, 0), Quaternion.identity);
                }
                else
                {
                    Element element = level.Elements.Find(elem => elem.Name == character.ToString());
                    if(element != null)
                    {
                        var atomBuilder = new AtomBuilder(element.Name, element.Degrees, Core, Branch, TextCanvas, AtomBorder, element.Char);
                        answerAtoms.Add(atomBuilder);
                        GameObject atom = atomBuilder.Build();
                        atom.tag = "Atom";
                        
                        atom.transform.position = new Vector3(XPosition, YPosition, 0);

                    }
                }

                XPosition += 1;
            }
            YPosition -= 1;
            XPosition = RememberedXPosition;
        }

        int RememberedAnswerXPosition = level.Width/2 + 1;
        int AnswerYPosition = level.Height/2 + 1;
        int tempX = RememberedAnswerXPosition;
        foreach (var row in level.Answer)
        {
            foreach (var character in row )
            {
                if(character != '.')
                {
                    var atomBuilder = answerAtoms.Find(answer => answer.Id == character.ToString());
                    GameObject answ = atomBuilder.Build();
                    answ.tag = "answer";
                    answ.transform.position = new Vector3(tempX, AnswerYPosition, 0);
                }
                tempX++;
            }
            AnswerYPosition--;
            tempX = RememberedAnswerXPosition;
        }
    }
}

[Serializable]
public class ElementMap
{
    public char symbol;
    public GameObject gameObject;
}