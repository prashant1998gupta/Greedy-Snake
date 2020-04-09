using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


namespace SA
{
    public class GameManager : MonoBehaviour
    {

        public int maxHeight = 15;
        public int maxwidht = 17;

        public Color color1;
        public Color color2;
        public Color appColor = Color.red;
        //public Color appColor1 = Color.blue;
        public Color playerColor = Color.black;

        public Transform cameraHolder;
        GameObject playerObject;
        GameObject appleObject;
        //GameObject appleObject1;
        GameObject tailParent;
        Node playerNode;
        Node appNode;
        //Node appNode1;

        Sprite playerSprite;


        GameObject mapObject;
        SpriteRenderer mapRenderer;

        Node[,] grid;
        List<Node> avaiableNode = new List<Node>();
        List<SpacialNode> tail = new List<SpacialNode>();

        int currentScore;


        public TMPro.TextMeshProUGUI currentScoreText;
        public TMPro.TextMeshProUGUI highScoreText;


        bool up, down, left, right;
        bool swipeUp, swipeDown, swippeLeft, swipeRight;

        public bool isGameOver;



        public float moveRate = 10f;
        float timer;

        Direction targetDirection;
        Direction curDirection;
        public enum Direction
        {
            up, down, left, right
        }




        public UnityEvent game_Over;
        public UnityEvent onScore;


        public float maxTime;
        public float minSwipeDis;

        float startTime;
        float endTime;

        Vector3 startPos;
        Vector3 endPos;

        float swipeTime;
        float swipeDistance;




        #region init
        public void Start()
        {
            ClearReferances();
            CreatMap();
            PlacePlayer();
            PlaceCamera();
            CreatApple();
            // CreatApple1();
            targetDirection = Direction.right;
            game_Over.AddListener(GameOver);
            currentScore = 0;
            updateScore();
        }

        IEnumerator GameOverProcess()
        {
            yield return new WaitForSeconds(3);
            Debug.Log("Wait for 3 secand");
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);

        }


        void GameOver()
        {
            FindObjectOfType<AudioManager>().Play(SoundType.GAMEOVER);
            StartCoroutine("GameOverProcess");
            PlayerPrefs.SetInt("CurrentScore", currentScore);
        }

        public void ClearReferances()
        {
            if (mapObject != null)
                Destroy(mapObject);

            if (playerObject != null)
                Destroy(playerObject);

            if (appleObject != null)
                Destroy(appleObject);

            foreach (var t in tail)
            {
                if (t.obj != null)
                    Destroy(t.obj);

            }
            tail.Clear();
            avaiableNode.Clear();

            grid = null;

        }



        private void CreatMap()
        {

            mapObject = new GameObject("map");
            mapRenderer = mapObject.AddComponent<SpriteRenderer>();

            grid = new Node[maxwidht, maxHeight];

            Texture2D txt = new Texture2D(maxwidht, maxHeight);
            for (int x = 0; x < maxwidht; x++)
            {

                for (int y = 0; y < maxHeight; y++)
                {

                    Vector3 tp = Vector3.zero;
                    tp.x = x;
                    tp.y = y;

                    Node n = new Node()
                    {

                        x = x,
                        y = y,
                        worldPosition = tp

                    };

                    grid[x, y] = n;
                    avaiableNode.Add(n);


                    #region visual
                    if (x % 2 != 0)
                    {
                        if (y % 2 != 0)
                        {
                            txt.SetPixel(x, y, color2);
                        }
                        else
                        {
                            txt.SetPixel(x, y, color1);
                        }
                    }
                    else
                    {
                        if (y % 2 != 0)
                        {
                            txt.SetPixel(x, y, color1);
                        }
                        else
                        {
                            txt.SetPixel(x, y, color2);
                        }
                    }
                    #endregion

                }

            }
            // bydefault filter
            txt.filterMode = FilterMode.Point;

            txt.Apply();
            Rect rect = new Rect(0, 0, maxwidht, maxHeight);
            Sprite sprite = Sprite.Create(txt, rect, Vector2.zero, 1, 0, SpriteMeshType.FullRect);
            mapRenderer.sprite = sprite;
        }

        void PlacePlayer()
        {
            playerObject = new GameObject("Player");
            SpriteRenderer playerRenderer = playerObject.AddComponent<SpriteRenderer>();
            playerSprite = CreateSprite(playerColor);
            playerRenderer.sprite = playerSprite;
            // playerRenderer.sprite = CreateSprite(playerColor);
            playerRenderer.sortingOrder = 1;
            playerNode = GetNode(3, 3);
            // playerNode = GetNode(2, 3);
            placePlayerObject(playerObject, playerNode.worldPosition);


            playerObject.transform.localScale = Vector3.one * 1f;

            tailParent = new GameObject("tailParent");

        }

        void PlaceCamera()
        {
            Node n = GetNode(maxwidht / 2, maxHeight / 2);

            Vector3 p = n.worldPosition;
            p += Vector3.one * .5f;
            cameraHolder.position = p;
        }


        void CreatApple()
        {
            appleObject = new GameObject("Apple1");
            SpriteRenderer appRenderer = appleObject.AddComponent<SpriteRenderer>();
            appRenderer.sprite = CreateSprite(appColor);
            appRenderer.sortingOrder = 1;
            RendomlyPlaceApple();
        }

        /*void CreatApple1()
        {
            appleObject1 = new GameObject("Apple2");
            SpriteRenderer appRenderer1 = appleObject1.AddComponent<SpriteRenderer>();
            appRenderer1.sprite = CreateSprite(appColor1);
            appRenderer1.sortingOrder = 1;
            RendomlyPlaceApple1();
        }*/

        #endregion

        #region update


        private void Update()
        {

            if (isGameOver == true)
                return;

            GetInput();
            SwipeInput();

            SetPlayeDirection();

            timer += Time.deltaTime;

            if (timer > moveRate)
            {
                timer = 0;
                curDirection = targetDirection;
                MovePlayer();
            }

        }

        void SwipeInput()
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);

                if (touch.phase == TouchPhase.Began)
                {
                    startTime = Time.time;
                    startPos = touch.position;
                }
                else if (touch.phase == TouchPhase.Ended)
                {
                    endTime = Time.time;
                    endPos = touch.position;

                    swipeTime = endTime - startTime;
                    swipeDistance = (endPos - startPos).magnitude;

                    if (swipeTime < maxTime && swipeDistance > minSwipeDis)
                    {
                        Swipe();
                    }
                }
            }
        }

        void Swipe()
        {
            Vector2 distance = endPos - startPos;
            if (Mathf.Abs(distance.x) > Mathf.Abs(distance.y))
            {
                //Debug.Log("Horizantal Swipe");

                if (distance.x > 0)
                {
                    Debug.Log("Right Swipe");
                    swipeRight = true;
                }



                if (distance.x < 0)
                {
                    Debug.Log("Left Swipe");
                    swippeLeft = true;
                }


            }
            else if (Mathf.Abs(distance.x) < Mathf.Abs(distance.y))
            {
                // Debug.Log("Vertical Swipe");

                if (distance.y > 0)
                {
                    Debug.Log("Up Swipe");
                    swipeUp = true;
                }

                if (distance.y < 0)
                {
                    Debug.Log("Down Swipe");
                    swipeDown = true;
                }


            }
        }

        void GetInput()
        {
            up = Input.GetButtonDown("Up") || swipeUp;
            down = Input.GetButtonDown("Down") || swipeDown;
            left = Input.GetButtonDown("Left") || swippeLeft;
            right = Input.GetButtonDown("Right") || swipeRight;
            swipeUp = false; swipeDown = false; swippeLeft = false; swipeRight = false;
        }
        void SetPlayeDirection()
        {
            if (up)
            {
                SetDirection(Direction.up);


            }
            else if (down)
            {
                SetDirection(Direction.down);

            }
            else if (left)
            {
                SetDirection(Direction.left);

            }

            else if (right)
            {
                SetDirection(Direction.right);

            }
        }

        void SetDirection(Direction d)
        {
            if (!isOpposit(d))
            {
                targetDirection = d;
                FindObjectOfType<AudioManager>().Play(SoundType.SWIPESOUND);

            }
        }
        void MovePlayer()
        {


            int x = 0;
            int y = 0;

            switch (curDirection)
            {
                case Direction.up:
                    y = 1;
                    break;

                case Direction.down:
                    y = -1;
                    break;

                case Direction.left:
                    x = -1;
                    break;

                case Direction.right:
                    x = 1;
                    break;
            }

            Node targetNode = GetNode(playerNode.x + x, playerNode.y + y);
            if (targetNode == null)
            {
                game_Over.Invoke();
                isGameOver = true;

                // game over 

            }
            else
            {
                if (isTailNode(targetNode))
                {
                    game_Over.Invoke();
                    isGameOver = true;
                    // Game over

                }

                else
                {
                    bool isScore = false;

                    if (targetNode == appNode)
                    {
                        Debug.Log(FindObjectOfType<AudioManager>());
                        FindObjectOfType<AudioManager>().Play(SoundType.PLAYEREAT);
                        isScore = true;
                    }

                    Node privousNode = playerNode;
                    avaiableNode.Add(privousNode);

                    if (isScore)
                    {
                        tail.Add(CreatTailNode(privousNode.x, privousNode.y));
                        avaiableNode.Remove(privousNode);

                    }

                    // move tail
                    MoveTail();

                    placePlayerObject(playerObject, targetNode.worldPosition);
                    playerNode = targetNode;
                    avaiableNode.Remove(playerNode);


                    if (isScore)
                    {
                        currentScore = currentScore + 10;
                        if (currentScore > PlayerPrefs.GetInt("highScoreText"))
                        {
                            PlayerPrefs.SetInt("highScoreText", currentScore);
                        }

                        onScore.Invoke();

                        if (avaiableNode.Count > 0)
                        {
                            RendomlyPlaceApple();
                            // RendomlyPlaceApple1();

                        }

                        else
                        {
                            // you win
                        }
                    }
                }
            }
        }


        void MoveTail()
        {
            Node prveNode = null;

            for (int i = 0; i < tail.Count; i++)
            {
                SpacialNode p = tail[i];
                avaiableNode.Add(p.node);

                if (i == 0)
                {
                    prveNode = p.node;
                    p.node = playerNode;
                }
                else
                {
                    Node prev = p.node;
                    p.node = prveNode;
                    prveNode = prev;
                }

                avaiableNode.Remove(p.node);
                placePlayerObject(p.obj, p.node.worldPosition);

            }
        }

        #endregion



        #region uitilits



        public void updateScore()
        {
            currentScoreText.text = currentScore.ToString();
            highScoreText.text = PlayerPrefs.GetInt("highScoreText", 0).ToString();
        }


        bool isOpposit(Direction d)
        {
            switch (d)
            {
                default:
                case Direction.up:
                    if (curDirection == Direction.down || curDirection == Direction.up)
                        return true;
                    else
                        return false;

                case Direction.down:
                    if (curDirection == Direction.up || curDirection == Direction.down)
                        return true;
                    else
                        return false;

                case Direction.right:
                    if (curDirection == Direction.left || curDirection == Direction.right)
                        return true;
                    else
                        return false;

                case Direction.left:
                    if (curDirection == Direction.right || curDirection == Direction.left)
                        return true;
                    else
                        return false;


            }
        }

        bool isTailNode(Node n)
        {
            for (int i = 0; i < tail.Count; i++)
            {
                if (tail[i].node == n)
                    return true;

            }
            return false;
        }

        void placePlayerObject(GameObject obj, Vector3 pos)
        {
            pos += Vector3.one * .5f;
            obj.transform.position = pos;
        }



        void RendomlyPlaceApple()
        {
            int ran = Random.Range(0, avaiableNode.Count);
            Node n = avaiableNode[ran];
            placePlayerObject(appleObject, n.worldPosition);

            appNode = n;
        }
        /* void RendomlyPlaceApple1()
         {
             int ran1 = Random.Range(0, avaiableNode.Count);
             Node n1 = avaiableNode[ran1];
             placePlayerObject(appleObject1, n1.worldPosition);

             appNode1 = n1;
         }*/

        Node GetNode(int x, int y)
        {
            if (x < 0 || x > maxwidht - 1 || y < 0 || y > maxHeight - 1)
                return null;
            return grid[x, y];
        }

        SpacialNode CreatTailNode(int x, int y)
        {
            SpacialNode s = new SpacialNode();
            s.node = GetNode(x, y);
            s.obj = new GameObject();
            s.obj.transform.parent = tailParent.transform;
            s.obj.transform.position = s.node.worldPosition;
            s.obj.transform.localScale = Vector3.one * .9f;
            SpriteRenderer r = s.obj.AddComponent<SpriteRenderer>();
            r.sprite = playerSprite;
            r.sortingOrder = 1;

            return s;
        }
        Sprite CreateSprite(Color targetColor)
        {
            Texture2D txt = new Texture2D(1, 1);
            txt.SetPixel(0, 0, targetColor);
            txt.Apply();
            txt.filterMode = FilterMode.Point;
            Rect rect = new Rect(0, 0, 1, 1);
            return Sprite.Create(txt, rect, Vector2.one * .5f, 1f, 0, SpriteMeshType.FullRect);
        }


        #endregion
    }


}
