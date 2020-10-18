using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public LevelBuilder LevelBuilder;
    public GameObject NextLevelButton;
    public Atom Atom;

    //var atom = FindObjectsOfType<Atom>().Where(a => a.tag == "Atom").First();
    //setMainAtom(atom);

    public void setMainAtom(Atom clickedOn)
    {
        if(Atom != null)
        {
            var currentAtomBorder = Atom.transform.Find("atomBorder");
            currentAtomBorder.transform.GetComponent<SpriteRenderer>().enabled = false;
        }

        Atom = clickedOn;
        var atomBorder = Atom.transform.Find("atomBorder");
        //atomBorder.gameObject.SetActive(true);
        atomBorder.transform.GetComponent<SpriteRenderer>().enabled = true;

        // Do things with clickedGameObject;
    }//

    private bool readyForInput;
    // Start is called before the first frame update
    void Start()
    {   
        NextLevelButton.SetActive(false);
        ResetLevel();
    }

    void Update()
    {
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        input.Normalize();

        if (input.sqrMagnitude > 0.5) // Mygtukas paspaustas arba laikomas
        {
            if (readyForInput)
            {
                readyForInput = false;
                Atom.Move(input);
                NextLevelButton.SetActive(CheckIfLevelCompleted());
            }
        }
        else
        {
            readyForInput = true;
        }
    }

    public void LoadNewLevel()
    {
        NextLevelButton.SetActive(false);
        LevelBuilder.setNextLevel();
        StartCoroutine(ResetSceneAsync());
    }

    public void ForceNewLevel()
    {
        LevelBuilder.setNextLevel();
        StartCoroutine(ResetSceneAsync());
    }

    public void ForceBackLevel()
    {
        LevelBuilder.setBackLevel();
        StartCoroutine(ResetSceneAsync());
    }

    public async void CalculateAStar()
    {
        Action calculate = new Action(() =>
        {
            Debug.Log("Starting A* calculation");
            var atomList = FindObjectsOfType<Atom>().Where(a => a.tag == "Atom");

            foreach (var atom in atomList)
            {
                foreach (var item in "UDLR")
                {
                    setMainAtom(atom);
                    atom.Move(item.getMovementCordinates());
                }
            }
        });

        await Task.Run(calculate);

    }

    private bool CheckIfLevelCompleted()
    {
        var atomList = FindObjectsOfType<Atom>().Where(a => a.tag == "Atom");
        List<string> answer = LevelBuilder.level.Answer;
        int y = 0, x = 0, rememberedXPosition=0, error = 0;
        bool first = true;


         foreach (var row in answer)
         {
            foreach (var character in row)
            {
                if (character != '.')
                {
                    if (first)
                    {
                        var atoms = atomList.Where(a => a.name == character.ToString());
                        var firstAtom = findFirstAnswerAtom(atoms);
                        x = Convert.ToInt32(firstAtom.x);
                        y = Convert.ToInt32(firstAtom.y);
                        first = false;
                        rememberedXPosition = x - error;
                    } else
                    {
                        var atoms = atomList.Where(a => a.name == character.ToString());
                        if (anyOfItemsIsInCorrectPossition(atoms, x, y)) {
                            Debug.Log($"Nice {atoms.First().name}");
                        }
                        else return false;
                        

                    }
                }
                error++;
                x++;
            }
            x = rememberedXPosition;
            y--;
        }
        return true;

    }

    private bool anyOfItemsIsInCorrectPossition(IEnumerable<Atom> atoms, int x, int y)
    {
        foreach (var atom in atoms)
        {
            var position = atom.transform.position;
            if (position.x == x && position.y == y)
            {
                return true;
            }
        }
        return false;
    }
    private Vector2 findFirstAnswerAtom(IEnumerable<Atom> atoms)
    {
        int x=200, y=-200;
        foreach (var atom in atoms)
        {
            var position = atom.transform.position;
            if(y <= position.y && x >= position.x)
            {
                x = Convert.ToInt32(position.x);
                y = Convert.ToInt32(position.y);
            }
        }

        return new Vector2(x, y);

    }

    public void ResetLevel()
    {
        StartCoroutine(ResetSceneAsync());
    }

    IEnumerator ResetSceneAsync()
    {
        if (SceneManager.sceneCount > 1)
        {
            AsyncOperation asyncUnload = SceneManager.UnloadSceneAsync("LevelScene");
            while (!asyncUnload.isDone)
            {
                yield return null;
                Debug.Log("Removing Scene......");
            }

            Debug.Log("Successfully Removed scene");
            Resources.UnloadUnusedAssets();
        }

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("LevelScene", LoadSceneMode.Additive);

        while (!asyncLoad.isDone)
        {
            yield return null;
            Debug.Log("Loading Scene.....");
        }

        SceneManager.SetActiveScene(SceneManager.GetSceneByName("LevelScene"));
        LevelBuilder.Build();
        var atom = FindObjectsOfType<Atom>().Where(a => a.tag == "Atom").First();
        setMainAtom(atom);
        Debug.Log("New Level Loaded");
    }

    // Update is called once per frame
 
}
