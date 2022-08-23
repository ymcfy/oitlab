using UnityEngine;
using System.Collections;
//using DG.Tweening;
using UnityEngine.EventSystems;
using UnityEngine.Events;

/// <summary>
/// 视角控制，包括右键改变视角等，既挂载在主摄像机上也挂载在其他摄像机上，如巡游摄像机
/// </summary>
public class CameraControl : MonoBehaviour
{
    public GameObject tongxun;
    float 位置x, 位置y, 位置z;
    float 角度x, 角度y, 角度z;
    float 移动速度, 视角改变速度;
    public Vector3 A;

    //默认双击时间间隔
    public float doubleClickInterval = 1.5f;
    //是否有一次单击
    private bool hasOneClick = false;
    //计时器
    private float timer = 0;

    [SerializeField]
    UnityEvent doubleClick = new UnityEvent(); public float Interval = 0.5f;

    private float firstClicked = 0;
    private float secondClicked = 0;

    private float Scale = 0.2f;     //鼠标前后点击的间隔

    private double lastKickTime; // 上一次鼠标抬起的时间（用来处理双击）


    // Use this for initialization
    void Start()
    {
        移动速度 = 0.3f;
        视角改变速度 = 45;
        A = this.GetComponent<Transform>().position;
        lastKickTime = Time.realtimeSinceStartup;
    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetMouseButtonDown(0))
        //{
        //    //检测上次点击的时间和当前时间差 在一定范围内认为是双击
        //    if (Time.realtimeSinceStartup - lastKickTime < Scale)
        //    {
        //        print("双击");
        //        //在这里写入双击所要做的事情
        //        //双击关闭所有的二级三级菜单
        //        ButtonHoverControl.closeAllTwoMenu();
        //        ButtonTwoHoverControl.closeAllThreeMenu();
        //    }
        //    lastKickTime = Time.realtimeSinceStartup;//重新设置上次点击的时间
        //}
        //if (hasOneClick == true)
        //{
        //    timer += Time.deltaTime;
        //    if (timer > doubleClickInterval)
        //    {
        //        hasOneClick = false;
        //        timer = 0;
        //    }
        //}
        //调用滑轮缩放的镜头移动方法
        cameraMovesWithZoom();
    }

    //public void OnClick()
    //{
    //    if (hasOneClick == false)
    //    {
    //        hasOneClick = true;
    //        //OneClick();
    //    }
    //    else
    //    {
    //        if (timer < doubleClickInterval)
    //        {
    //            hasOneClick = false;
    //            //DoubleClick();
    //            Debug.Log("双击了");
    //            timer = 0;
    //        }
    //        else
    //        {
    //            timer = 0;
    //            //OneClick();
    //        }
    //    }
    //}

    /// <summary>
    /// 实现滑轮缩放的镜头移动
    /// </summary>
    public void cameraMovesWithZoom()
    {
        位置x = this.GetComponent<Camera>().transform.position.x;
        //位置x = this.GetComponent<Camera>().main.transform.position.x;
        位置y = this.GetComponent<Camera>().transform.position.y;
        位置z = this.GetComponent<Camera>().transform.position.z;
        角度x = this.GetComponent<Camera>().transform.eulerAngles.x;
        角度y = this.GetComponent<Camera>().transform.eulerAngles.y;
        角度z = this.GetComponent<Camera>().transform.eulerAngles.z;
        /* if (Input.GetKey(KeyCode.Escape))
         {

             tongxun.GetComponent<NetController>().SubcribeRealtimeData("RealTimeCarLoading");
             tongxun.GetComponent<NetController>().UnsubcribeRealtimeData("RealTimeOilCan");
             tongxun.GetComponent<NetController>().UnsubcribeRealtimeData("RealTimeValve");
             tongxun.GetComponent<NetController>().UnsubcribeRealtimeData("RealTimeSecurity");
             tongxun.GetComponent<NetController>().UnsubcribeRealtimeData("RealTimeFireControlValve");
             Application.Quit();
         }*/
        //左键双击
        //if (Input.GetMouseButton(0))
        //{
        //    if (hasOneClick == false)
        //    {
        //        hasOneClick = true;
        //        //OneClick();
        //    }
        //    else
        //    {
        //        if (timer < doubleClickInterval)
        //        {
        //            hasOneClick = false;
        //            //DoubleClick();
        //            Debug.Log("双击了");
        //            timer = 0;
        //        }
        //        else
        //        {
        //            timer = 0;
        //            //OneClick();
        //        }
        //    }
        //}
        //右键按下，视角旋转
        if (Input.GetMouseButton(1))
        {
            角度y += Input.GetAxis("Mouse X");
            角度x -= Input.GetAxis("Mouse Y");
            transform.rotation = Quaternion.Euler(角度x, 角度y, 0);
        }
        //视角缩放
        if (位置y < 1 && Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            this.GetComponent<Camera>().transform.Translate(0, 0, 0);
        }
        else
        {
            this.GetComponent<Camera>().transform.Translate(视角改变速度 * Input.GetAxis("Mouse ScrollWheel") * Vector3.forward);
        }

        //为了不干扰模型查看时输入名称，暂时注释  杨民  20220414
        ////视角移动WASD
        //if (Input.GetKey(KeyCode.W))
        //{
        //    this.GetComponent<Camera>().transform.position = new Vector3(移动速度 * Mathf.Sin(角度y * Mathf.PI / 180) + 位置x, 位置y, 移动速度 * Mathf.Cos(角度y * Mathf.PI / 180) + 位置z);
        //}
        //if (Input.GetKey(KeyCode.A))
        //{
        //    this.GetComponent<Camera>().transform.Translate(Vector3.left * 移动速度);
        //}
        //if (Input.GetKey(KeyCode.D))
        //{
        //    this.GetComponent<Camera>().transform.Translate(Vector3.right * 移动速度);
        //}
        //if (Input.GetKey(KeyCode.S))
        //{
        //    this.GetComponent<Camera>().transform.position = new Vector3(-移动速度 * Mathf.Sin(角度y * Mathf.PI / 180) + 位置x, 位置y, -移动速度 * Mathf.Cos(角度y * Mathf.PI / 180) + 位置z);
        //}
        ////快速移动
        //if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.LeftShift))
        //{
        //    this.GetComponent<Camera>().transform.position = new Vector3(移动速度 * 10 * Mathf.Sin(角度y * Mathf.PI / 180) + 位置x, 位置y, 移动速度 * 10 * Mathf.Cos(角度y * Mathf.PI / 180) + 位置z);
        //}
        //if (Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.LeftShift))
        //{
        //    this.GetComponent<Camera>().transform.Translate(Vector3.left * 移动速度 * 10);
        //}
        //if (Input.GetKey(KeyCode.D) && Input.GetKey(KeyCode.LeftShift))
        //{
        //    this.GetComponent<Camera>().transform.Translate(Vector3.right * 移动速度 * 10);
        //}
        //if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.LeftShift))
        //{
        //    this.GetComponent<Camera>().transform.position = new Vector3(-移动速度 * 10 * Mathf.Sin(角度y * Mathf.PI / 180) + 位置x, 位置y, -移动速度 * 10 * Mathf.Cos(角度y * Mathf.PI / 180) + 位置z);
        //}

        //杨民注释
        //上下左右
        //if (Input.GetKey(KeyCode.UpArrow))
        //{
        //    this.GetComponent<Camera>().transform.position = new Vector3(移动速度 * Mathf.Sin(角度y * Mathf.PI / 180) + 位置x, 位置y, 移动速度 * Mathf.Cos(角度y * Mathf.PI / 180) + 位置z);
        //}
        //if (Input.GetKey(KeyCode.LeftArrow))
        //{
        //    this.GetComponent<Camera>().transform.Translate(Vector3.left * 移动速度);
        //}
        //if (Input.GetKey(KeyCode.RightArrow))
        //{
        //    this.GetComponent<Camera>().transform.Translate(Vector3.right * 移动速度);
        //}
        //if (Input.GetKey(KeyCode.DownArrow))
        //{
        //    this.GetComponent<Camera>().transform.position = new Vector3(-移动速度 * Mathf.Sin(角度y * Mathf.PI / 180) + 位置x, 位置y, -移动速度 * Mathf.Cos(角度y * Mathf.PI / 180) + 位置z);
        //}
        //快速移动
        if (Input.GetKey(KeyCode.UpArrow) && Input.GetKey(KeyCode.RightShift))
        {
            this.GetComponent<Camera>().transform.position = new Vector3(移动速度 * 10 * Mathf.Sin(角度y * Mathf.PI / 180) + 位置x, 位置y, 移动速度 * 10 * Mathf.Cos(角度y * Mathf.PI / 180) + 位置z);
        }
        if (Input.GetKey(KeyCode.LeftArrow) && Input.GetKey(KeyCode.RightShift))
        {
            this.GetComponent<Camera>().transform.Translate(Vector3.left * 移动速度 * 10);
        }
        if (Input.GetKey(KeyCode.RightArrow) && Input.GetKey(KeyCode.RightShift))
        {
            this.GetComponent<Camera>().transform.Translate(Vector3.right * 移动速度 * 10);
        }
        if (Input.GetKey(KeyCode.DownArrow) && Input.GetKey(KeyCode.RightShift))
        {
            this.GetComponent<Camera>().transform.position = new Vector3(-移动速度 * 10 * Mathf.Sin(角度y * Mathf.PI / 180) + 位置x, 位置y, -移动速度 * 10 * Mathf.Cos(角度y * Mathf.PI / 180) + 位置z);
        }
        //if (GameObject.Find("Canvas_button/RawImage_clcaozuo").transform.localScale.x == 0)
        //{

        //    if (Input.GetAxis("Mouse ScrollWheel") < 0)
        //    {
        //        if (this.GetComponent<Camera>().fieldOfView <= 100)
        //            this.GetComponent<Camera>().fieldOfView += 1;
        //    }
        //    //Zoom in
        //    if (Input.GetAxis("Mouse ScrollWheel") > 0)
        //    {
        //        if (this.GetComponent<Camera>().fieldOfView > 20)
        //            this.GetComponent<Camera>().fieldOfView -= 1;
        //    }
        //}
        //按下中建，视角平移
        if (Input.GetMouseButton(2))
        {
            transform.Translate(Vector3.left * Input.GetAxis("Mouse X"));
            transform.Translate(Vector3.down * Input.GetAxis("Mouse Y"));
        }
        Vector3 point_shirushi = new Vector3(this.GetComponent<Camera>().transform.position.x, 330, this.GetComponent<Camera>().transform.position.z);
        //GameObject.Find("shirushi").transform.DOMove(point_shirushi, 3f);
    }


    public void zhongzhishijiao()
    {
        //关闭所有二级、三级菜单
        //ButtonHoverControl.closeAllTwoMenu();
        //ButtonTwoHoverControl.closeAllThreeMenu();
        this.GetComponent<Transform>().position = A;
        this.transform.rotation = Quaternion.Euler(10, 0, 0);
    }

    //public void OnPointerDown(PointerEventData eventData)
    //{
    //    //throw new System.NotImplementedException();
    //    secondClicked = Time.realtimeSinceStartup;

    //    if (secondClicked - firstClicked < Interval)
    //    {
    //        Debug.Log("双击");
    //        doubleClick.Invoke();

    //    }
    //    else
    //    {
    //        firstClicked = secondClicked;
    //    }  
    //}

    //public void OnPointerClick(PointerEventData eventData)
    //{
    //    //throw new System.NotImplementedException();

    //    //if (eventData.clickCount == 2)
    //    //{
    //    //    Debug.Log("双击");
    //    //}
    //    Debug.Log("双击");
    //}
    }
