using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AtomBuilder
{
    private GameObject Core;
    private GameObject Branch;
    private GameObject AtomBorder;
    public List<double> Branches = new List<double>();
    public string Id;
    public string name;

    public GameObject TextCanvas;

    public AtomBuilder(string id, List<double> branches, GameObject core, GameObject branch, GameObject textCanvas, GameObject atomBorder, string name)
    {
        Core = core;
        Branches = branches;
        Id = id;
        Branch = branch;
        TextCanvas = textCanvas;
        AtomBorder = atomBorder;
        this.name = name;
    }
    // Start is called before the first frame update
    public GameObject Build()
    {
        //AtomsColors.MAP.TryGetValue(name, out var color);
        //var atomRenderer = Core.GetComponent<SpriteRenderer>();
        //atomRenderer.color = color;

        GameObject core = GameObject.Instantiate(Core);
        core.name = Id;

        GameObject atomBorder = GameObject.Instantiate(AtomBorder, core.transform);
        atomBorder.name = "atomBorder";
        atomBorder.transform.GetComponent<SpriteRenderer>().enabled = false;
        foreach (var branch in Branches)
        {
            GameObject br = GameObject.Instantiate(Branch, core.transform);
            br.transform.localPosition = calculateLocalPosition(branch);
            br.transform.eulerAngles = calculateEulerAngle(branch);
        }



        core.GetComponentInChildren<TextMesh>().text = Id;

        AtomsColors.MAP.TryGetValue(name, out var color);
        core.GetComponent<SpriteRenderer>().color = new Color(color.r/255,color.g/255, color.b/255, 1);

        return core;
    }

    private Vector3 calculateEulerAngle(double angle)
    {
        int temp = System.Convert.ToInt32(angle);
        if(angle % 90 == 0)
        {
            return new Vector3(0, 0, System.Convert.ToInt32(angle));
        }
        if (225 % angle == 0) {
            return new Vector3(0, 0, -45);
        }
        else return new Vector3(0,0,45);
    }

    public Vector2 calculateLocalPosition(double angle)
    {
        switch (angle)
        {
            case 0:
                return new Vector2(0, 0.6f);
            case 90:
                return new Vector2(0.6f, 0);
            case 180:
                return new Vector2(0, -0.6f);
            case 270:
                return new Vector2(-0.6f, 0);
            case 45:
                return new Vector2(0.4f, 0.4f);
            case 135:
                return new Vector2(0.4f, -0.4f);
            case 225:
                return new Vector2(-0.4f, -0.4f);
            case 315:
                return new Vector2(-0.4f, 0.4f);
            default:
                throw new System.Exception("Can not calculate local position");
        }
    }

    
}
