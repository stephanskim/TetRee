using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class Game : MonoBehaviour {
    //Grid Variables
    public static int gridHeight = 20;
    public static int gridLength = 10;
    public static Transform[,] grid = new Transform[gridLength, gridHeight];

    //Base Scores
    public int score1 = 40;
    public int score2 = 100;
    public int score3 = 300;
    public int score4 = 1200;
    private int rowsThisTurn = 0;
    public Text hudScore;
    public static int currentScore = 0;
    private int scoreMod = 1;

    public Text lvlCount;
    public static int currentLevel = 0;

    public Text linesCount;
    public int linesCleared = 0;
    private bool firstLvl = false;
    private int lineBuffer = 0;

    public static int songChoice = 1;
    public AudioSource song;
    public AudioClip song1;
    public AudioClip song2;


    //Piece Counters
    public int iPiece = 0;
    public int lPiece = 0;
    public int jPiece = 0;
    public int zPiece = 0;
    public int sPiece = 0;
    public int tPiece = 0;
    public int oPiece = 0;
    public Text hudIpiece;
    public Text hudlpiece;
    public Text hudjpiece;
    public Text hudzpiece;
    public Text hudspiece;
    public Text hudtpiece;
    public Text hudopiece;

    public float fallSpeed = 1.0f;

    private GameObject previewPiece;
    private GameObject nextPiece;
    private bool gameStart = false;
    private bool firstPiece = false;
    private Vector2 previewPos = new Vector2(15.0f, 13.0f);
    private Vector2 nextPos = new Vector2(5.0f, 19.0f);

    public static int CurrentLevel {
        get {
            return currentLevel;
        }

        set {
            currentLevel = value;
        }
    }


    // Use this for initialization
    void Start () {
        spawnPiece();
	}

    void Awake() {
        song = GetComponent<AudioSource>();
        updateSong();
    }

    void Update() {
        updateScore();
        updateUI();
        updateLevel();
        updateSpeed();
        
    }

    void updateSong() {
        if(songChoice == 2) {
            song.clip = song2;
        }
        else {
            song.clip = song1;
        }
        song.Play();
    }
    void updateLevel() {
        if (linesCleared == (currentLevel * 10 + 10) || linesCleared == Mathf.Max(100, (currentLevel * 10 - 50)) && !firstLvl) {
            currentLevel++;
            firstLvl = true;
            lineBuffer = lineBuffer % 10;
        }
        else if (lineBuffer >= 10 && firstLvl) {
            currentLevel++;
            lineBuffer = lineBuffer % 10;
        }
        scoreMod = currentLevel + 1;
    }

    void updateSpeed() {
        if (currentLevel < 10) {
            fallSpeed = .8f - (currentLevel * .067f);
        }
        if (currentLevel <= 12 && currentLevel >=10) {
            fallSpeed = .063f;
        }
        if (currentLevel > 12 && currentLevel < 16) {
            fallSpeed = .0315f;
        }
        if (currentLevel > 15 && currentLevel < 19) {
            fallSpeed = .0039375f;
        }
        if (currentLevel > 18 && currentLevel < 29) {
            fallSpeed = .001f;
        }
        if (currentLevel > 28) {
            fallSpeed = .0001f;
        }
        

    }

    //Updates HUD
    public void updateUI() {
        hudScore.text = currentScore.ToString();
        hudIpiece.text = iPiece.ToString();
        hudlpiece.text = lPiece.ToString();
        hudjpiece.text = jPiece.ToString();
        hudzpiece.text = zPiece.ToString();
        hudspiece.text = sPiece.ToString();
        hudtpiece.text = tPiece.ToString();
        hudopiece.text = oPiece.ToString();
        lvlCount.text = currentLevel.ToString();
        linesCount.text = linesCleared.ToString();
    }

    //Checks if player has topped out
    public bool topOut(Tetrimino tet) {
        for (int x = 0; x < gridLength; ++x) {
            foreach(Transform mino in tet.transform) {
                Vector2 pos = Round(mino.position);

                if (pos.y > gridHeight-1) {
                    return true;
                }
            }
        }

        return false;
    }

    //Loads game over screen
    public void gameOver () {
        currentScore = 0;
        SceneManager.LoadScene("gameover");
    }

    //Checks for full rows
    public bool isRowFull(int row) {

        for (int x = 0; x < gridLength; ++x) {
            if(grid[x, row] == null) {
                return false;
            }
        }

        rowsThisTurn++;
        return true;
    }

    //Deletes mino
    public void deleteMino(int y) {
        for (int x = 0; x < gridLength; ++x) {
            Destroy(grid[x, y].gameObject);
            grid[x, y] = null;
        }
    }

    //Moves a row down
    public void rowDown(int y) {
        for (int x = 0; x < gridLength; ++x) {
            if (grid[x , y] != null) {
                grid[x, y - 1] = grid[x, y];
                grid[x, y] = null;
                grid[x, y - 1].position += new Vector3(0, -1, 0);
            }
        }
    }

    //Moves all rows down
    public void moveAllDown(int y) {
        for (int i = y; i < gridHeight; ++i) {
            rowDown(i);
        }
    }

    //Deletes a row
    public void deleteRow() {
        for (int y = 0; y < gridHeight; ++y) {
            if(isRowFull(y)) {
                deleteMino(y);
                moveAllDown(y + 1);
                --y;
            }
        }
    }

    //Updates the score variable
    public void updateScore() {
        if (rowsThisTurn > 0) {
            switch(rowsThisTurn) {
                case 1:
                    currentScore += score1*scoreMod;
                    linesCleared++;
                    lineBuffer++;
                    break;
                case 2:
                    currentScore += score2*scoreMod;
                    linesCleared += 2;
                    lineBuffer += 2;
                    break;
                case 3:
                    currentScore += score3*scoreMod;
                    linesCleared += 3;
                    lineBuffer += 3;
                    break;
                case 4:
                    currentScore += score4*scoreMod;
                    linesCleared += 4;
                    lineBuffer += 4;
                    break;
            }
            rowsThisTurn = 0;
        }
    }

    //Updates the grid
    public void updateGrid(Tetrimino tet) {

        for (int y = 0; y < gridHeight; ++y) {
            for (int x = 0; x < gridLength; ++x) {
                if (grid[x, y] != null) {
                    if (grid[x, y].parent == tet.transform) {
                        grid[x, y] = null;
                    }
                }
            }
        }

        foreach(Transform mino in tet.transform) {
            Vector2 pos = Round(mino.position);

            if (pos.y < gridHeight) {
                grid[(int)pos.x, (int)pos.y] = mino;
            }
        }
    }

    //Spawns a new piece
    public void spawnPiece() {
        if (!gameStart) {
            gameStart = true;
            nextPiece = (GameObject)Instantiate(Resources.Load(getRandPiece(), typeof(GameObject)), nextPos, Quaternion.identity);
            previewPiece = (GameObject)Instantiate(Resources.Load(getRandPiece(), typeof(GameObject)), previewPos, Quaternion.identity);
            previewPiece.GetComponent<Tetrimino>().enabled = false;
        }
        else {
            previewPiece.transform.localPosition = nextPos;
            nextPiece = previewPiece;
            nextPiece.GetComponent<Tetrimino>().enabled = true;
            previewPiece = (GameObject)Instantiate(Resources.Load(getRandPiece(), typeof(GameObject)), previewPos, Quaternion.identity);
            previewPiece.GetComponent<Tetrimino>().enabled = false;
        }
        
    }

    //Generates random piece
    string getRandPiece() {
        int randTet = Random.Range(1, 8);

        string randPiece = "";

        switch(randTet) {
            case 1:
                randPiece = "Prefabs/piece_l";
                lPiece++;
                break;
            case 2:
                randPiece = "Prefabs/piece_long";
                iPiece++;
                break;
            case 3:
                randPiece = "Prefabs/piece_s";
                sPiece++;
                break;
            case 4:
                randPiece = "Prefabs/piece_z";
                zPiece++;
                break;
            case 5:
                randPiece = "Prefabs/piece_t";
                tPiece++;
                break;
            case 6:
                randPiece = "Prefabs/piece_o";
                oPiece++;
                break;
            case 7:
                randPiece = "Prefabs/piece_j";
                jPiece++;
                break;
        }

        return randPiece;
    }

    //Checks to see if the piece is in the grid
    public bool ifInGrid(Vector2 pos) {
        return ((int)pos.x >= 0 && (int)pos.x < gridLength && (int)pos.y >= 0);
    }

    //Gets the position in the grid
    public Transform getTransformAtGridPos(Vector2 pos) {
        if(pos.y > gridHeight) {
            return null;
        }
        else {
            return grid[(int)pos.x, (int)pos.y];
        }
    }

    //Rounds the pipece's position
    public Vector2 Round (Vector2 pos) {
        return new Vector2(Mathf.Round(pos.x), Mathf.Round(pos.y));
    }
}
