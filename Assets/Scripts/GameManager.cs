using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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
        // TEST STUFF
        //var array = convertFromVectorToArrayIndex(Atom.transform.position);
        //Debug.Log($"{array[0]}, {array[1]}");
        //var vector = convertFromArrayToVectorPosition(array[0], array[1]);
        //Debug.Log(vector);

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

    public void CalculateAStar()
    {
        StartCoroutine(AStarCoroutine());
    }

    public void CalculateAStarWithLog()
    {
        StartCoroutine(AStarCoroutine(true));
    }

    IEnumerator AStarCoroutine(bool log = false)
    {

        using (StreamWriter outputFile = new StreamWriter("LevelCompletion.txt"))
        {
            outputFile.WriteLine("Starting A* Process");
        }
        List<Positions> listOfMoves = new List<Positions>();

        try
        {
            Debug.Log("Starting A* calculation");
            var currentAtoms = FindObjectsOfType<Atom>().Where(a => a.tag == "Atom").ToArray();
            var targetAtoms = FindObjectsOfType<Atom>().Where(a => a.tag == "answer").ToArray();

            Node[] currentNodes = new Node[currentAtoms.Length];
            Node[] targetNodes = new Node[currentAtoms.Length];

            for (int i = 0; i < currentAtoms.Length; i++)
            {
                var postion = currentAtoms[i].transform.position;
                var newPosition = convertFromVectorToArrayIndex(postion);
                var vector = new Vector2(newPosition[0], newPosition[1]);
                var name = currentAtoms[i].name[0].ToString();

                var targetPostion = targetAtoms[i].transform.position;
                var newTargetPosition = convertFromVectorToArrayIndex(targetPostion);
                var targetVector = new Vector2(newTargetPosition[0], newTargetPosition[1]);
                var targetName = targetAtoms[i].name[0].ToString();

                var atomsArgs = currentAtoms[i].name.Split(':');

                currentNodes[i] = new Node(vector, name, Int32.Parse(atomsArgs[1]));
                targetNodes[i] = new Node(targetVector, targetName, 99);
            }

 
            var currentPositions = new Positions(currentNodes);
            var goalPositions = new Positions(targetNodes);
            MapHelper mapHelper = new MapHelper(LevelBuilder.level.Rows, LevelBuilder.level.Answer);

            List<string> map = mapHelper.convertToEmptyMap();
         
            var AStar = new AStar(currentPositions, goalPositions, map, log);
            listOfMoves = AStar.calculateAStar();

            Debug.Log("Printing the path");

            if (log)
            {
                using (StreamWriter outputFile = File.AppendText("LevelCompletion.txt"))
                {
                    outputFile.WriteLine("------------FINISH------------");

                    string Header = $"{listOfMoves.Count } Moves Needed: {DateTime.Now}";
                    outputFile.WriteLine(Header);
                    foreach (var position in listOfMoves)
                    {
                        string positionsInMap = position.ToString(map);

                        outputFile.WriteLine(positionsInMap);
                    }
                }
            }




            //var atomList = FindObjectsOfType<Atom>().Where(a => a.tag == "Atom");

            //foreach (var atom in atomList)
            //{
            //    foreach (var item in "UDLR")
            //    {
            //        setMainAtom(atom);
            //        atom.Move(item.getMovementCordinates());
            //        NextLevelButton.SetActive(CheckIfLevelCompleted());

            //        yield return new WaitForSeconds(0.5f);
            //    }
            //}
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }

        if (listOfMoves == null) yield break; ;

        foreach (var position in listOfMoves)
        {
            var atomList = FindObjectsOfType<Atom>().Where(a => a.tag == "Atom");

            var atomToMove = atomList.Where(a => a.name.Contains(":" + position.MovedNodeuniqueId)).First();
            setMainAtom(atomToMove);
            atomToMove.Move(position.RoundMove);
            NextLevelButton.SetActive(CheckIfLevelCompleted());
            yield return new WaitForSeconds(0.5f);
        }

        yield return new WaitForSeconds(0.5f);

    }

    private bool CheckIfLevelCompleted()
    {
        var atomList = FindObjectsOfType<Atom>().Where(a => a.tag == "Atom" );
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
                        var atoms = atomList.Where(a => a.name.Contains(character.ToString()+ ":"));
                        var firstAtom = findFirstAnswerAtom(atoms);
                        x = Convert.ToInt32(firstAtom.x);
                        y = Convert.ToInt32(firstAtom.y);

                        first = false;
                        rememberedXPosition = x - error;
                    } else
                    {
                        var atoms = atomList.Where(a => a.name.Contains(character.ToString() + ":"));
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

    public bool anyOfItemsIsInCorrectPossition(IEnumerable<Atom> atoms, int x, int y)
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
    public Vector2 findFirstAnswerAtom(IEnumerable<Atom> atoms)
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
    public int[] convertFromVectorToArrayIndex(Vector3 vector)
    {
        int x = Convert.ToInt32(vector.x);
        int y = Convert.ToInt32(vector.y);
        int errorX = LevelBuilder.level.Width / 2;
        int errorY = LevelBuilder.level.Height / 2;
        var cords = new int[] {x + errorX, (y - errorY) * -1 };
        return cords;
    }

    public Vector3 convertFromArrayToVectorPosition(int x, int y)
    {
        int errorX = LevelBuilder.level.Width / 2;
        int errorY = LevelBuilder.level.Height / 2;
        return new Vector3(x - errorX,  (y - errorY)*-1, 0);
    }
}
