using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using static UIBoardManager;
using static SharedDelegates;

public class BoardManager : MonoBehaviour
{
    public delegate void StartGame(bool readyToStart);
    public static event StartGame OnStartGame;
    public delegate void BoardPiecePlaced(int shipID);
    public static event BoardPiecePlaced OnBoardPiecePlaced;
	public static event AttackEventHandler OnAttack;
    

    public int shipSize = 0;
    public bool horizontal = true;

    public PlayerBoard playerBoard;
    public EnemyBoard enemyBoard;

    public GameObject playerBoardUnitPrefab;
    public GameObject enemyBoardUnitPrefab;
    public GameObject blockVisualizerPrefab;

    [Header("Player Piece Prefab Reference")]
    public List<GameObject> boardPiecesPrefab;

    [Header("_____")]
    bool PLACE_BLOCK = true;

    [SerializeField]
    private int currentShipID;
    GameObject tmpHighlight = null;
    RaycastHit tmpHitHighlight;
    GameObject tempBlockHolder = null;

    private bool OK_TO_PLACE = true;

    [SerializeField]
    private int count = 0;
    private const int TOTAL_SHIP_COUNT = 5;
    bool placeEnemyShips = true;

    //TODO: Stuff for actual game play
    GameObject AttackingBoardUnit = null;
    RaycastHit AttackHighlightHit;

    public bool playerTurn = false;
    bool gameStart = false;

    private void OnEnable()
    {
        OnChangeShip += UIBoardManager_OnChangeShip;
        OnChangeShipOrientation += UIBoardManager_OnChangeOrientation;
        OnFirstPlayerSelected += UIBoardManager_OnPlayerChanged;
        Battleship.OnSwitchPlayer += SwitchPlayer;

    }
	

    private void OnDisable()
    {
        OnChangeShip -= UIBoardManager_OnChangeShip;
        OnChangeShipOrientation -= UIBoardManager_OnChangeOrientation;
		OnFirstPlayerSelected -= UIBoardManager_OnPlayerChanged;
				
    }
	public void SwitchPlayer(PlayerType currentPlayer){
		if (currentPlayer == PlayerType.HUMAN){
            this.playerTurn = true;
            setPlayerTurn();
            Debug.Log($"Switched() to Player Turn: {this.playerTurn}");
            
        } else{
            playerTurn = false;
			cleanVisuals();
		}
    }
	void setPlayerTurn(){
        playerTurn = true;
    }

    private void UIBoardManager_OnChangeOrientation()
    {
        horizontal = !horizontal;
    }

    private void UIBoardManager_OnPlayerChanged(PlayerType player){
        Debug.Log($"First Player: {player}");
		
		switch(player){
			case PlayerType.HUMAN:
                playerTurn = true;
                gameStart = true;
                break;
			case PlayerType.AI:
                Debug.Log("Player is now AI");
                gameStart = true;
                playerTurn = false;
                break;
			default:
                Debug.Log("Error in BoardManager OnSelectedFirstPlayer Switch");
                break;
        }
    }

    private void UIBoardManager_OnChangeShip(int id, int size)
    {
        currentShipID = id;
        shipSize = size;
        Debug.Log("ONCHANGESHIP ID " + currentShipID + " Ship Size" + shipSize);
    }

    private void Start()
    {
        playerBoard = new PlayerBoard(playerBoardUnitPrefab);
        playerBoard.CreatePlayerBoard();
        enemyBoard = new EnemyBoard(enemyBoardUnitPrefab, blockVisualizerPrefab);
        enemyBoard.CreateAIBoard();
		Battleship.playerBoard = playerBoard;
		Battleship.enemyBoard = enemyBoard;
        AILogic.playerboard = playerBoard;
        SetDefaultShip();
    }

    /// <summary>
    /// Sets S1 (aircraft carrier) as default ship.
    /// </summary>
    private void SetDefaultShip()
    {
        currentShipID = 0;
        shipSize = 5;
    }

    private void Update()
    {
        if (count < 5)
        {
            placePlayerPieces();
        }
        else
        {
            if (placeEnemyShips)
            {
                EnemyTurn();

            }
        }
        if (gameStart)
        {
			        Debug.Log($"UPDATE(): Player update playerTurn:{this.playerTurn} ");

            if (playerTurn)
            {
                Debug.Log("Player's turn. Atacking...");
                Attack();
            }
			// if(!playerTurn && Battleship.currentPlayer == PlayerType.HUMAN){
            //     Debug.Log("Setting player turn");
            //     //setPlayerTurn();
            // }
        }
    }
    /// <summary>
    /// Performs AI ship placement and starts the game.
    /// </summary>
    private void EnemyTurn()
    {
        enemyBoard.PlaceShips();
		foreach( var board in enemyBoard.gameBoard){
            Debug.Log("GameBoard " + board);
        }
		OnStartGame?.Invoke(true);
        placeEnemyShips = false;
    }
    /// <summary>
    /// placePlayerPieces() occurs for as long as the player is in the
    /// ship placement stage. Continues to render green or red blocks
    /// depending on whether the player can place their ship there or not.
    /// Does not render anything when player is too close to the edge depending
    /// on if horizontal or not.
    /// </summary>
    private void placePlayerPieces()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Debug.Log("Placing player pieces");
        if (Input.mousePosition != null)
        {
            Debug.Log("mousePosition != Null");

            if (Physics.Raycast(ray, out tmpHitHighlight, 100))
            {
                //Debug.Log("Hit = true");
                Debug.Log("We hit " + tmpHitHighlight.transform.name);

                BoardUnit tempUI = tmpHitHighlight.transform.GetComponent<BoardUnit>();
                if (tmpHitHighlight.transform.tag.Equals("PlayerBoardUnit") && !tempUI.isOccupied)
                {
                    Debug.Log("In PlayerBoardUnit");
                    BoardUnit boardData = playerBoard.gameBoard[tempUI.row, tempUI.col].transform.GetComponentInChildren<BoardUnit>();
                    Debug.Log("board data: " + boardData.row + " , " + boardData.col);
                    if (tmpHighlight != null)
                    {
                        Debug.Log("tmpHighLight != NULL");
                        if (boardData.isOccupied)
                        {
                            Debug.Log("occupied");
                            tmpHighlight.GetComponent<Renderer>().material.color = Color.red;
                        }
                        else
                        {
                            Debug.Log("Unoccupied");
                            tmpHighlight.GetComponent<Renderer>().material.color = Color.green;
                        }
                    }
                    if (tempBlockHolder != null)
                    {
                        Debug.Log("destroy blockholder");
                        Destroy(tempBlockHolder);
                    }
                    if (PLACE_BLOCK)
                    {
                        Debug.Log("PLACE_BLOCK");
                        tempBlockHolder = new GameObject();
                        OK_TO_PLACE = true;
                        if (horizontal)
                        {
                            if (tempUI.row <= 10 - shipSize)
                            {

                                Debug.Log("Horizontal");
                                for (int i = 0; i < shipSize; i++)
                                {
                                    RenderCursorHighlight(this, tempUI.row + i, tempUI.col);
                                }

                            }
                            else
                            {
                                for (int i = 0; i < shipSize; i++)
                                {
                                    RenderRedCursorHighlight(this, tempUI.row, tempUI.col);
                                }
                            }
                        }
                        if (!horizontal)
                        {
                            if (tempUI.col <= 10 - shipSize)
                            {
                                Debug.Log("VERTICAL SECTION");
                                for (int i = 0; i < shipSize; i++)
                                {
                                    RenderCursorHighlight(this, tempUI.row, tempUI.col + i);
                                }
                            }
                            else
                            {
                                for (int i = 0; i < shipSize; i++)
                                {
                                    RenderRedCursorHighlight(this, tempUI.row, tempUI.col);
                                }
                            }
                        }
                    }
                }
            }
            if (Input.GetMouseButtonUp(0))
            {
                Debug.Log("Mouse clicked");
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 100))
                {
                    if (hit.transform.tag.Equals("PlayerBoardUnit"))
                    {
                        Debug.Log("TAG IS PLAYERBOARD UNIT (MC)");
                        BoardUnit tempUI = hit.transform.GetComponentInChildren<BoardUnit>();
                        if (PLACE_BLOCK && OK_TO_PLACE)
                        {
                            Debug.Log("PLACE BLOCK && OK TO PLACE (MC)");
                            if (horizontal)
                            {
                                Debug.Log("HORIZONTAL (MC)");
                                if (tempUI.row + shipSize <= 10)
                                {
                                    for (int i = 0; i < shipSize; i++)
                                    {
                                        Debug.Log("PLACING SHIP (MC)");
                                        RenderShipHighlight(this, tempUI.row + i, tempUI.col);
                                    }
                                    PlaceShip(tempUI.row, tempUI.col);
                                }
                                else { Debug.Log("Clicking too close to edge!"); }
                            }
                            if (!horizontal)
                            {
                                Debug.Log("VERT (MC)");
                                if (tempUI.col + shipSize <= 10)
                                {
                                    for (int i = 0; i < shipSize; i++)
                                    {
                                        Debug.Log("PLACING SHIP (MCV)");
                                        RenderShipHighlight(this, tempUI.row, tempUI.col + i);
                                    }
                                    PlaceShip(tempUI.row, tempUI.col);
                                }
                                else { Debug.Log("Clicking too close to edge!"); }
                            }
                        }
                        if (count >= 5)
                        {
                            if (tempBlockHolder != null)
                            {
                                Destroy(tempBlockHolder);
                            }
                        }
                    }
                }
            }
        }
    }
    /// <summary>
    /// Renders green surface if the player can place their ship there, red otherwise.
    /// </summary>
    /// <param name="boardManager"></param>
    /// <param name="row"></param>
    /// <param name="col"></param>
    private void RenderCursorHighlight(BoardManager boardManager, int row, int col)
    {
        GameObject visual = GameObject.Instantiate(blockVisualizerPrefab,
    new Vector3(row, blockVisualizerPrefab.transform.position.y, col),
    blockVisualizerPrefab.transform.rotation) as GameObject;
        GameObject playerBoard = this.playerBoard.gameBoard[row, col];
        if (playerBoard == null) Debug.Log("Null playerboard!!!");
        BoardUnit playerBoardUnit = playerBoard.GetComponentInChildren<BoardUnit>();
        if (!playerBoardUnit.isOccupied)
        {
            visual.GetComponent<Renderer>().material.color = Color.green;
        }
        else
        {
            visual.GetComponent<Renderer>().material.color = Color.red;
            OK_TO_PLACE = false;
        }
        visual.transform.parent = tempBlockHolder.transform;
    }
	//Red Highlight forout of bounds ship placement.
    private void RenderRedCursorHighlight(BoardManager boardManager, int row, int col)
    {
        GameObject visual = GameObject.Instantiate(blockVisualizerPrefab,
    new Vector3(row, blockVisualizerPrefab.transform.position.y, col),
    blockVisualizerPrefab.transform.rotation) as GameObject;
        visual.GetComponent<Renderer>().material.color = Color.red;

        visual.transform.parent = tempBlockHolder.transform;
    }
    /// <summary>
    /// Renders a green surface underneath the ship.
    /// </summary>
    /// <param name="boardManager"></param>
    /// <param name="row"></param>
    /// <param name="col"></param>
    private void RenderShipHighlight(BoardManager boardManager, int row, int col)
    {
        GameObject playerBoard = boardManager.playerBoard.gameBoard[row, col];
        playerBoard.transform.GetComponentInChildren<Renderer>().material.color = Color.green;
        BoardUnit boardData = playerBoard.GetComponentInChildren<BoardUnit>();
        boardData.isOccupied = true;
        this.playerBoard.gameBoard[row, col] = playerBoard;
    }
    /// <summary>
    /// Calls CheckWhichShipWasPlaced() in order to 
    /// render the ship model on the board.
    /// </summary>
    /// <param name="row"></param>
    /// <param name="col"></param>
    private void PlaceShip(int row, int col)
    {
        CheckWhichShipWasPlaced(row, col);
        OK_TO_PLACE = true;
        tmpHighlight = null;
    }
    /// <summary>
    /// checks the current ship ID to make an offset that places it more accurately 
    /// relative to the green renderings that will be created underneath the ship.
    /// Then instantiates the ship's prefab in the proper location.
    /// </summary>
    /// <param name="row"></param>
    /// <param name="col"></param>
    private void CheckWhichShipWasPlaced(int row, int col)
    {
        float shipRowOffset = float.NaN;
        float shipHeightOffset = 0f;
        switch (currentShipID)
        {   //Aircraft carrier offset
            case 0:
                shipRowOffset = 1.8f;
                shipHeightOffset = -.10f+.5f;
                break;
				//Battleship offset
            case 1:
                shipRowOffset = 1.5f;
                shipHeightOffset = -.13f+.5f;
                break;
				//Submarine offset
            case 2:
                shipRowOffset = 1f;
                shipHeightOffset = 0.209f+.5f;
                break;
				//Destroyer offset
            case 3:
                shipRowOffset = 1f;
				shipHeightOffset = 0.24f+.5f;

                break;
				//Patrol offset
            case 4:
                shipRowOffset = .4f;
                shipHeightOffset = .08f+.5f;

                break;
        }
        if (horizontal)
        {
            GameObject testingVisual = GameObject.Instantiate(boardPiecesPrefab[currentShipID],
                                                              new Vector3(row + shipRowOffset,
                                                                          boardPiecesPrefab[currentShipID].transform.position.y + shipHeightOffset,
                                                                          col),
                                                              boardPiecesPrefab[currentShipID].transform.rotation) as GameObject;
            testingVisual.transform.RotateAround(testingVisual.transform.position, Vector3.up, 90.0f);
        }
        else
        {
            GameObject testingVisual = GameObject.Instantiate(boardPiecesPrefab[currentShipID],
                                                              new Vector3(row,
                                                                          boardPiecesPrefab[currentShipID].transform.position.y + shipHeightOffset,
                                                                          col + shipRowOffset),
                                                              boardPiecesPrefab[currentShipID].transform.rotation) as GameObject;
        }
        count++;
        OnBoardPiecePlaced?.Invoke(currentShipID);
        Debug.Log("Ship Count is " + count);
    }
	private void Attack(){
		
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Debug.Log("User is attacking");
        if (Input.mousePosition != null)
        {
            //Debug.Log("mousePosition != Null");

            if (Physics.Raycast(ray, out AttackHighlightHit))
            {
                //Debug.Log("Hit = true");
                Debug.Log("Hovering over " + AttackHighlightHit.transform.name);

                var tempUI = AttackHighlightHit.transform.GetComponent<BoardUnit>();
                Debug.Log("tempUI is of type " +  tempUI.GetType());
                if (AttackHighlightHit.transform.tag.Equals("EnemyBoardUnit"))
                {
                    Debug.Log("In Enemy Board Unit");
                    BoardUnit boardData = enemyBoard.gameBoard[tempUI.row, tempUI.col].transform.GetComponentInChildren<BoardUnit>();
                    Debug.Log("Enemy board data: " + boardData.row + " , " + boardData.col);
                    if (AttackingBoardUnit != null)
                    {
                        Debug.Log("tempAttackHighLight != NULL");
                        if (boardData.isAttacked)
                        {
                            Debug.Log("Board Unit has already been attacked");
                            AttackingBoardUnit.GetComponent<Renderer>().material.color = Color.red;
                            RenderAttackHighlightHit(this, tempUI.row, tempUI.col);
                        }
                        else
                        {
                            Debug.Log("Attackable");
                            Debug.Log("Should be rendering at " + tempUI.row + " "+ tempUI.col);
                            AttackingBoardUnit.GetComponent<Renderer>().material.color = Color.green;
                            RenderAttackHighlightHit(this, tempUI.row, tempUI.col);
                        }
                    }
                    if (tempBlockHolder != null)
                    {
                        Debug.Log("destroy blockholder");
                        Destroy(tempBlockHolder);
                    }
					tempBlockHolder = new GameObject();
					RenderAttackHighlightHit(this, tempUI.row, tempUI.col);

                 
                }
            }
            if (Input.GetMouseButtonUp(0))
            {
                Debug.Log("Attacking Location");
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 100))
                {
                    var boardUnit = hit.transform.GetComponent<BoardUnit>();
                    if (hit.transform.tag.Equals("EnemyBoardUnit") && !boardUnit.isAttacked)
                    {
                        Debug.Log("TAG IS Enemy  UNIT (Click)");
                        BoardUnit tempUI = hit.transform.GetComponentInChildren<BoardUnit>();
                        Debug.Log("Invoking attack");
                        ValueTuple<int , int > coordinate = (tempUI.row, tempUI.col);
						playerTurn = false;
						OnAttack?.Invoke(coordinate, PlayerType.HUMAN);
                    }
                }
            }
        }
	}
	
    private void RenderAttackHighlightHit(BoardManager boardManager, int row, int col)
    {
		int rowOffset = 11;
		int offsetRow = row + rowOffset;
        GameObject visual = GameObject.Instantiate(blockVisualizerPrefab,
    new Vector3(offsetRow, blockVisualizerPrefab.transform.position.y, col),
    blockVisualizerPrefab.transform.rotation) as GameObject;
		Debug.Log("Visual Position " + visual.transform.position);
        GameObject enemyBoard = this.enemyBoard.gameBoard[row, col];
        BoardUnit enemyBoardUnit = enemyBoard.GetComponentInChildren<BoardUnit>();
        if (!enemyBoardUnit.isAttacked)
        {
            Debug.Log("Should render green");
            visual.GetComponent<Renderer>().material.color = Color.green;
        }
        else
        {
            visual.GetComponent<Renderer>().material.color = Color.red;
        }
        visual.transform.parent = tempBlockHolder.transform;
    }
	void cleanVisuals(){
		if (tempBlockHolder != null){
            Destroy(tempBlockHolder);
        }
	}
}
