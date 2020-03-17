using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;


namespace SA
{
    public class GameManager : MonoBehaviour
    {

        public int maxHeight = 15;
        public int maxwidht = 17;

        public Color color1;
        public Color color2;
        public Color appColor = Color.red;
        public Color appColor1 = Color.blue;
        public Color playerColor = Color.black;

        public Transform cameraHolder;
        GameObject playerObject;
        GameObject appleObject;
        //GameObject appleObject1;
        GameObject tailParent;
        Node playerNode;
        Node appNode;
        Node appNode1;

        Sprite playerSprite;


        GameObject mapObject;
        SpriteRenderer mapRenderer;

        Node[,] grid;
        List<Node> avaiableNode = new List<Node>();
        List<SpacialNode> tail = new List<SpacialNode>();


        bool up, down, left, right;
        public float moveRate = .5f;
        float timer;

        Direction targetDirection;
        Direction curDirection;
        public enum Direction
        {
            up, down, left, right
        }

        IEnumerator Test()
        {
            yield return new WaitForSeconds(3);
            Debug.Log("Wait for 3 secand");

        }

       
       public UnityEvent game_Over;

        #region init
        public void Start()
        {
            StartCoroutine(Test());
            ClearReferances();
            CreatMap();
            PlacePlayer();
            PlaceCamera();
            CreatApple();
           // CreatApple1();
            targetDirection = Direction.right;
           
        }

        public void ClearReferances()  
        {
            if (mapObject != null)
            Destroy(mapObject);

            if (playerObject != null)
                Destroy(playerObject);
            foreach( var t in tail)
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
            //CreatTailNode(2, 3);
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
            GetInput();
            SetPlayeDirection();

            timer += Time.deltaTime;

            if (timer > moveRate)
            {
                timer = 0;
                curDirection = targetDirection;
                MovePlayer();
            }

        }

        void GetInput()
        {
            up = Input.GetButtonDown("Up");
            down = Input.GetButtonDown("Down");
            left = Input.GetButtonDown("Left");
            right = Input.GetButtonDown("Right");
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
                StartCoroutine(Test());
                
                // game over 
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);

            }
            else
            {
                if (isTailNode(targetNode))
                {
                    game_Over.Invoke();
                    // Game over
                    StartCoroutine(Test());
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
                }

                else
                {
                    bool isScore = false;

                    if ((targetNode == appNode) || (targetNode == appNode1))
                    {
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
                        if (avaiableNode.Count > 0 )
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

        bool isOpposit(Direction d)
        {
            switch (d)
            {
                default:
                case Direction.up:
                    if (curDirection == Direction.down)
                        return true;
                    else
                        return false;

                case Direction.down:
                    if (curDirection == Direction.up)
                        return true;
                    else
                        return false;

                case Direction.right:
                    if (curDirection == Direction.left)
                        return true;
                    else
                        return false;

                case Direction.left:
                    if (curDirection == Direction.right)
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

        Node GetNode(int x , int y)
        {
            if (x < 0 || x > maxwidht - 1 || y < 0 || y > maxHeight - 1)
                return null;
            return grid[x, y];
        }

        SpacialNode CreatTailNode(int x , int y)
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
            Texture2D txt = new Texture2D(1,1);
            txt.SetPixel(0, 0, targetColor);
            txt.Apply();
            txt.filterMode = FilterMode.Point;
            Rect rect = new Rect(0, 0, 1, 1);
            return Sprite.Create(txt, rect, Vector2.one * .5f, 1f, 0, SpriteMeshType.FullRect);
        }
       

        #endregion
    }


}
