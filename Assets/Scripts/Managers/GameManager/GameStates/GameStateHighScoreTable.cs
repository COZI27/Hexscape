using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateHighScoreTable : GameStateBase
{

    //GameObject scoreBoardCanvas;
    //GameObject titleParticle;

    //private string pathScoreBoard = "Prefabs/GUI/ScoreboardCanvas";

    //private string pathTitleParticle = "Prefabs/Particles/TextParticle";

    //private string pathHiScoreTableLevel = "Assets/Resources/Levels/Menus/HiScoreTable.json";
    private string pathHiScoreTableLevel = "Levels/Menus/HiScoreTable";
    //private string pathScoreboardMat = "Prefabs/Particles/TextParticle/TextMat";
    private string pathScoreboardMat = "Prefabs/Particles/TextParticle/LeaderboardMat";

    //Leaderboard.LeaderboardEntry[] scoresToDisplay;
    //int playerScoreEntryIndex; // the current player's score in the array


    public string pathBackgroundMaskPrefab = "Prefabs/BackgroundMask";
    private GameObject backgroundMaskObj;
    private Renderer backgroundMaskRenderer;
    private Color backgroundCol = new Color(.2f,.2f,.2f,100);

    GameObject boardObject;
    Leaderboard.LeaderboardDisplay leaderboardComp;

    public GameStateHighScoreTable()
    {
        InitialiseStateTransitions();
    }

    public override void StartGameState()
    {


        GenerateBoard();

        Level loadedLevel = LevelLoader.Instance.LoadLevelFile(pathHiScoreTableLevel);
        if (loadedLevel != null)
        {
            CreateLevel(
                loadedLevel,
                -30,
                false,
                false
            );
        }

        if (!GameManager.instance.GetIsOffline())
        {
            GameObject lb = CameraCanvas.instance.transform.Find("Leaderboard").gameObject;
            leaderboardComp = lb.GetComponent<Leaderboard.LeaderboardDisplay>();
            if (leaderboardComp != null) leaderboardComp.DownloadScoreData();
        }

        backgroundMaskObj = GameObject.Instantiate(Resources.Load(pathBackgroundMaskPrefab) as GameObject, MapSpawner.Instance.GetCurrentMapHolder().transform);
        backgroundMaskRenderer = backgroundMaskObj.GetComponent<MeshRenderer>();

        GameManager.instance.StartCoroutine(FadeMask(true, 1.5f, backgroundMaskRenderer.material));


        Camera.main.GetComponent<CameraFollow>().TiltCamera(80, 1.5f);
    }

    public override void StateUpdate()
    {
        //Handle scroll input...

        var d = Input.GetAxis("Mouse ScrollWheel");
        if (d > 0f)
        {
            // scroll up
            leaderboardComp.ScrollBoard(1);
        }
        else if (d < 0f)
        {
            // scroll down
            leaderboardComp.ScrollBoard(-1);
        }
    }

    protected override void InitialiseStateTransitions()
    {
        stateTransitions = new Dictionary<Command, TransitionData<GameStateBase>>
        {
            { Command.Begin, new TransitionData<GameStateBase>(typeof(GameStateMenuMain))  },
            { Command.End, new TransitionData<GameStateBase>(typeof(GameStateMenuMain))  },
        };
    }

    public override bool CleanupGameState()
    {
        GameManager.instance.StartCoroutine(FadeMask(false, 1, backgroundMaskRenderer.material));
        GameManager.instance.StartCoroutine(GameManager.instance.DestroyObjectAfterInterval(backgroundMaskObj, 1));
        Camera.main.GetComponent<CameraFollow>().TiltCamera(90, 1);
        GameObject.Destroy(boardObject);

        return true;
    }

    Mesh CreateMesh()
    {
        //https://catlikecoding.com/unity/tutorials/procedural-grid/

        Mesh m = new Mesh();
        m.name = "ScriptedMesh";

        int xSize = 1;
        int ySize = 3;

        Vector3[] vertices = new Vector3[(xSize + 1) * (ySize + 1)];
        Vector2[] uv = new Vector2[vertices.Length];



        //populate curve
        float[] arrayToCurve = new float[ySize +1];     
        for (int y = 0; y < arrayToCurve.Length; y++)
        {
            Debug.Log("arrayToCurve.length = " + arrayToCurve.Length + ". y = " + y);
            arrayToCurve[y] = Mathf.Sin(Mathf.PI / 2 * Mathf.Sin(y * 0.4f)) * 30;
            Debug.Log("arrayToCurve " + arrayToCurve[y]);
        }


        foreach (float point in arrayToCurve)
        {
            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.transform.position = new Vector3(0, point, 0);
            sphere.name = "arrayToCurve: " + point;
        }


        //float[] spacedPoints =  CalculateEvenlySpacedPoints(arrayToCurve, 1);

        //float[] yPositions = new float[ySize];
        //int curvedLength = (ySize * Mathf.RoundToInt(1.0f)) - 1;
        //List<float> curvedPositions = new List<float>(curvedLength);


            //  for (int i = 0, y = 0; y <= ySize; y++)
        for (int i = 0, y = 0; y <= ySize; y++)
        {
            for (int x = 0; x <= xSize; x++, i++)
            {
                Debug.Log("arrayToCurve.length = " + arrayToCurve.Length + ". y = " + y);
                Debug.Log("arrayToCurve val = " + arrayToCurve[y]);
                vertices[i] = new Vector3(x, arrayToCurve[y], y); // < Works (kind of)! 


                //vertices[i] = new Vector3(x, Mathf.Sin(Mathf.PI / 2 * Mathf.Sin(y * 0.4f)) * 30, y);
                //vertices[i] = new Vector3(x, Mathf.Sin(Mathf.PI / 2 * Mathf.Sin(y)), y);
                //vertices[i] = new Vector3(x, Mathf.Sin(Mathf.Pow(y, 0.5f))  * 20 , y);
                uv[i] = new Vector2((float)x / xSize, (float)y / ySize);

                
            }
        }
        m.vertices = vertices;
        m.uv = uv;

        int[] triangles = new int[xSize * ySize * 6];
        for (int ti = 0, vi = 0, y = 0; y < ySize; y++, vi++)
        {
            for (int x = 0; x < xSize; x++, ti += 6, vi++)
            {
                triangles[ti] = vi;
                triangles[ti + 3] = triangles[ti + 2] = vi + 1;
                triangles[ti + 4] = triangles[ti + 1] = vi + xSize + 1;
                triangles[ti + 5] = vi + xSize + 2;
            }
        }

        m.triangles = triangles;

        m.RecalculateNormals();

        return m;
    }


    private void GenerateBoard()
    {

        CameraCanvas.instance.ChangeDisplayType(DisplayObjectMap.EDisplayType.Leaderboard);

        boardObject = new GameObject("HighScoreBoard");
        boardObject.AddComponent<MeshFilter>().mesh = Resources.Load<Mesh>("Board") as Mesh; 
        //boardObject.AddComponent<MeshFilter>().mesh = CreateMesh();
        boardObject.transform.position = MapSpawner.Instance.GetCurrentMapHolder().transform.position - new Vector3(0, 16.3f, -2.2f); // -20.6, 2.7
        boardObject.transform.Rotate(-2.5f, 180, 0);
        boardObject.transform.localScale = new Vector3(1, 3, 1);




        //GameObject boardObj =  Resources.Load(Board)

        Material boardMat = Resources.Load(pathScoreboardMat, typeof(Material)) as Material;
        boardObject.AddComponent<MeshRenderer>().material = Resources.Load(pathScoreboardMat, typeof(Material)) as Material; //.material = energyFillMat;
    }



    IEnumerator FadeMask(bool isFadeIn, float cycleTime, Material mat)
    {
        Color32 startColour;
        Color32 endColour;
        if (isFadeIn)
        {
            startColour = Color.white;
            endColour = backgroundCol;
        }
        else
        {
            startColour = backgroundCol;
            endColour = Color.white;
        }


        float currentTime = 0;
        while (currentTime < cycleTime)
        {
            currentTime += Time.deltaTime;
            float t = currentTime / cycleTime;
            Color32 currentColor = Color32.Lerp(startColour, endColour, t);
            //mat.color = currentColor;

            mat.SetColor("_BaseColor", currentColor);
            yield return null;
        }
    }


    //public float[] CalculateEvenlySpacedPoints(float[] points, float spacing, float resolution = 1)
    //{
    //    List<float> evenlySpacedPoints = new List<float>();



    //    evenlySpacedPoints.Add(points[0]);
    //    float previousPoint = points[0];
    //    float distanceSinceLastEvenPoint = 0;

    //    //https://www.youtube.com/watch?v=d9k97JemYbM

    //    for (int segIndex = 0; segIndex < points.Length; segIndex++ )
    //    {
    //        float t = 0; 
    //        while (t <= 0) {
    //            t += 0.1f;
    //            float pointOnCurve = EvaluateCubic(points[0], points[1], points[2], points[3], t);
    //        Debug.Log("pointOnCurve = " + pointOnCurve);
    //            distanceSinceLastEvenPoint += Mathf.Abs(previousPoint - pointOnCurve);

    //            if (distanceSinceLastEvenPoint >= spacing)
    //            {
    //                float overshootDist = distanceSinceLastEvenPoint - spacing;
    //                float newEvenSpacedPoint = pointOnCurve + Mathf.Abs(previousPoint - pointOnCurve);
    //                evenlySpacedPoints.Add(newEvenSpacedPoint);
    //                distanceSinceLastEvenPoint = overshootDist;
    //                previousPoint = newEvenSpacedPoint;
    //            }

    //            previousPoint = pointOnCurve;
    //        }
    //    }
    //    return evenlySpacedPoints.ToArray();
    //}

    //public float EvaluateQuadratic(float a, float b, float c, float t)
    //{
    //    float p0 = Mathf.Lerp(a, b, t);
    //    float p1 = Mathf.Lerp(b, c, t);
    //    return Mathf.Lerp(p0, p1, t);
    //}

    //public float EvaluateCubic(float a, float b, float c, float d, float t)
    //{
    //    float p0 = EvaluateQuadratic(a, b, c, t);
    //    float p1 = EvaluateQuadratic(b, c, d, t);
    //    return Mathf.Lerp(p0, p1, t);
    //}
}
