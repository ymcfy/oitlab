using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Timers;
using UnityEngine;
public class ParticleTest : MonoBehaviour
{
    public ParticleSystem system;
    public ParticleSystem system1;
    public GameObject go;
    public List<Vector3> list = new List<Vector3>();
    public List<float> listX = new List<float>();
    public List<float> listY = new List<float>();
    public List<float> listZ = new List<float>();
    public List<double> listMole6 = new List<double>();
    public List<double> listMole6Copy = new List<double>();
    //声明一个变量，储存当前加载到第几个时间段
    public int times = 0;
    private int a = 0;
    private int b = 1;
    private bool paticalSwitch = true;
    private System.Timers.Timer timer;
    //private int releaseSourcePoint = 0;
    private int onePoint = 0;
    private int twoPoint = 0;
    private int zeroPoint = 0;
    private int threePoint = 0;
    private int fourPoint = 0;
    private int fivePoint = 0;
    private int sixPoint = 0;
    private int sevenPoint = 0;

    /// <summary>
    /// FLACS扩散的相应UI，例如开始暂停等等
    /// </summary>
    public GameObject FLACSLeackSpread;
    /// <summary>
    /// 扩散训练UI，包含FLACS模拟和数值模拟等UI
    /// </summary>
    public GameObject LeakTraining;
    private void Start()
    {
        StreamReader sr = new StreamReader("D:\\grid.txt", Encoding.Default);
        string line;
        //判断当前是x y z的哪一个方向
        int judgeXyz = 1;
        while ((line = sr.ReadLine()) != null)
        {
            char[] separator = { ' ', ' ', ' ' };
            //line = sr.ReadLine();
            if (line.Contains(":") && line.Contains("-"))
            {
                string[] data = line.Split(separator);
                for (int i = 0; i < data.Length; i++)
                {
                    if (data[i].Trim().Contains("x-direction"))
                    {
                        //接下来的数据都是x方向的
                        judgeXyz = 1;
                    }
                    if (data[i].Trim().Contains("y-direction"))
                    {
                        //接下来的数据都是y方向的
                        judgeXyz = 2;
                    }
                    if (data[i].Trim().Contains("z-direction"))
                    {
                        //接下来的数据都是z方向的
                        judgeXyz = 3;
                    }
                    switch (judgeXyz)
                    {
                        case 1:
                            if (data[i].Trim() != string.Empty && data[i].Trim().Contains("."))
                            {
                                // 保存当前行的每一列数据
                                listX.Add(float.Parse(data[i].Trim()));
                            }
                            break;
                        case 2:
                            if (data[i].Trim() != string.Empty && data[i].Trim().Contains("."))
                            {
                                // 保存当前行的每一列数据
                                listY.Add(float.Parse(data[i].Trim()));
                            }
                            break;
                        case 3:
                            if (data[i].Trim() != string.Empty && data[i].Trim().Contains("."))
                            {
                                // 保存当前行的每一列数据
                                listZ.Add(float.Parse(data[i].Trim()));
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
        }
        //float[] fx = new float[] { 220 ,   222 ,   224 ,   226 ,   228 ,
        //230 ,   232 ,   234 ,   236 ,   238 ,
        //240 ,   242 ,   244 ,   246 ,   248 ,
        //250 ,   252 ,   254 ,   256 ,   258 ,
        //260 ,   262 ,   264 ,   266 ,   268 ,
        //270 ,   272 ,   274 ,   276 ,   278 ,
        //280 ,   282 ,   284 ,   286 ,   288 ,
        //290 ,   292 ,   294 ,   296 ,   298 ,
        //300 ,   302 ,   304 ,   306 ,   308 ,
        //310 ,   312 ,   314 ,   316 ,   318 ,
        //320 ,   322 ,   324 ,   326 ,   328 ,
        //330 ,   332 ,   334 ,   336 ,   338 ,
        //340 ,   342 ,   344 ,   346 ,   348 ,
        //350 ,   352 ,   354 ,   356 ,   358 ,
        //360 ,   362 ,   364 ,   366 ,   368 ,
        //370 ,   372 ,   374 ,   376 ,   378 ,
        //380 ,   382 ,   384 ,   386 ,   388 ,
        //390 ,   392 ,   394 ,   396 ,   398 ,
        //400 ,   402 ,   404 ,   406 ,   408 ,
        //410 ,   412 ,   414 ,   416 ,   418
        // };
        //listX.AddRange(fx);
        //float[] fy = new float[] { 80  ,   82  ,   84  ,   86  ,   88  ,
        //90  ,   92  ,   94  ,   96  ,   98  ,
        //100 ,   102 ,   104 ,   106 ,   108 ,
        //110 ,   112 ,   114 ,   116 ,   118 ,
        //120 ,   122 ,   124 ,   126 ,   128 ,
        //130 ,   132 ,   134 ,   136 ,   138 ,
        //140 ,   142 ,   144 ,   146 ,   148 ,
        //150 ,   152 ,   154 ,   156 ,   158 ,
        //160 ,   162 ,   164 ,   166 ,   168 ,
        //170 ,   172 ,   174 ,   176 ,   178 ,
        //180 ,   182 ,   184 ,   186 ,   188 ,
        //190 ,   192 ,   194 ,   196 ,   198 ,
        //200 ,   202 ,   204 ,   206 ,   208 ,
        //210 ,   212 ,   214 ,   216 ,   218 ,
        //220 ,   222 ,   224 ,   226 ,   228 ,
        //230 ,   232 ,   234 ,   236 ,   238 };
        //listY.AddRange(fy);
        //float[] fz = new float[] { 0 ,  1 ,  2 ,  3 ,  4,
        //5 ,  6  , 7 ,  8  , 9,
        //10,  11,  12  ,13, 14
        // };
        //listZ.AddRange(fz);

        //第一种   XZY有待商榷
        //for (int i = 0; i < listX.Count; i++)
        //{
        //    for (int j = 0; j < listZ.Count; j++)
        //    {
        //        for (int k = 0; k < listY.Count; k++)
        //        {
        //            Vector3 vector3 = new Vector3(listX[i], listZ[j], listY[k]);
        //            list.Add(vector3);
        //        }
        //    }
        //}
        //第二种  ZXY  不合适
        //for (int i = 0; i < listZ.Count; i++)
        //{
        //    for (int j = 0; j < listX.Count; j++)
        //    {
        //        for (int k = 0; k < listY.Count; k++)
        //        {
        //            Vector3 vector3 = new Vector3(listZ[i], listX[j], listY[k]);
        //            list.Add(vector3);
        //        }
        //    }
        //}
        //第三种  XYZ  不合适
        //for (int i = 0; i < listX.Count; i++)
        //{
        //    for (int j = 0; j < listY.Count; j++)
        //    {
        //        for (int k = 0; k < listZ.Count; k++)
        //        {
        //            Vector3 vector3 = new Vector3(listX[i], listY[j], listZ[k]);
        //            list.Add(vector3);
        //        }
        //    }
        //}
        //第四种  YXZ 不合适
        //for (int i = 0; i < listX.Count; i++)
        //{
        //    for (int j = 0; j < listY.Count; j++)
        //    {
        //        for (int k = 0; k < listZ.Count; k++)
        //        {
        //            //Vector3 vector3 = new Vector3(listY[j]-88, listX[i]-218, listZ[k]-78f);
        //            //Vector3 vector3 = new Vector3(listY[j] - 180, listX[i] - 258, listZ[k] - 20f);
        //            Vector3 vector3 = new Vector3(listY[j], listX[i], listZ[k]);
        //            list.Add(vector3);
        //        }
        //    }
        //}
        //第五种  YZX   有待商榷
        for (int i = 0; i < listY.Count; i++)
        {
            for (int j = 0; j < listZ.Count; j++)
            {
                for (int k = 0; k < listX.Count; k++)
                {
                    Vector3 vector3 = new Vector3(listY[i], listZ[j], listX[k]);
                    list.Add(vector3);
                }
            }
        }
        //第六种 ZYX  不合适
        //for (int i = 0; i < listZ.Count; i++)
        //{
        //    for (int j = 0; j < listY.Count; j++)
        //    {
        //        for (int k = 0; k < listX.Count; k++)
        //        {
        //            Vector3 vector3 = new Vector3(listZ[i], listY[j], listX[k]);
        //            list.Add(vector3);
        //        }
        //    }
        //}
    }

    private DateTime startTime;
    private bool isRender = false;
    private void Update()
    {
        if (a == b)
        {
            b++;
            loadData();
        }
    }
    /// <summary>
    /// 跳转到flacsUI
    /// </summary>
    public void FlacsUi()
    {
        //关闭所有二级、三级菜单
        //ButtonHoverControl.closeAllTwoMenu();
        //ButtonTwoHoverControl.closeAllThreeMenu();
        FLACSLeackSpread.SetActive(true);
        //退出扩散训练UI
        LeakTraining.SetActive(false);
    }
    /// <summary>
    /// 停止模拟
    /// </summary>
    public void stopSimulation()
    {
        if (timer != null)
        {
            timer.Close();
        }
        a = 0;
        b = 1;
    }
    /// <summary>
    /// 暂停模拟
    /// 未想好如何实现
    /// </summary>
    public void pauseSimulation()
    {

    }
    /// <summary>
    /// 退出模拟
    /// </summary>
    public void exitSimulation()
    {
        stopSimulation();
        closeUI();
        //LeakTraining.SetActive(true);
    }
    /// <summary>
    /// 退出模拟时关闭UI
    /// </summary>
    public void closeUI()
    {
        FLACSLeackSpread.SetActive(false);
    }
    private void ServerStart(object sender, ElapsedEventArgs e)
    {
        Debug.Log(DateTime.Now + "============定时器数据接收服务开启============");
    }

    /// <summary>
    /// 目的是把0-1的值转化为0-255
    /// 使用公式：gnew=255*((g-Gmin)/(Gmax-Gmin))
    /// </summary>
    private byte normalize(double g)
    {
        //if (g > 0.1)
        //{
        //    g = 0.1;
        //}
        //if (g < 0.01)
        //{
        //    g = 0.01;
        //}
        return (byte)(255 * g);
        //return (byte)(255 * ((g - 0.01) / (0.1 - 0.01)));
    }

    /// <summary>
    /// 颜色判断
    /// </summary>
    //private 

    public void loadData()
    {
        //system.Stop(true);
        //system.Clear(true);

        listMole6Copy.Clear();
        listMole6Copy.AddRange(listMole6);
        Debug.Log("开始渲染" + listMole6Copy.Count);
        Debug.Log("坐标数量"+list.Count);
        listMole6Copy.Sort();
        Debug.Log("最小值是：" + listMole6Copy[0]);
        Debug.Log("第二个值是：" + listMole6Copy[1]);
        Debug.Log("第三个值是：" + listMole6Copy[2]);
        Debug.Log("最大值是：" + listMole6Copy[listMole6Copy.Count - 1]);
        Debug.Log(listMole6Copy.Contains(9.54032020481622E-41) + "----" + normalize(2.2588931E-42) + "-----" + normalize(0.004));

        //打印所有坐标数据，判断是不是保存成功
        GameObject go = GameObject.Instantiate(Resources.Load("cube")) as GameObject;
        go.transform.position = list[1];

        //当前颜色
        Color32 color ;
        for (int i = 0; i < list.Count; i++)
        //for (int i = 0; i < 15; i++)
        {
            //根据浓度计算透明度
            //if (listMole6[i] < 1e)
            //{

            //}
            //记录当前浓度值，后续根据不同浓度修改
            byte concentration = 0;
            if (listMole6[i] == 0)
            {
                zeroPoint++;
                color = new Color32(0,0,0, 0);
                concentration = 0;
            }
            if (0 < listMole6[i] && listMole6[i] < 0.01)
            {
                twoPoint++;
                color = new Color32(255, 0, 0, 100);
                concentration = 3;
            }
            else if (0.01 <= listMole6[i] && listMole6[i] < 0.03)
            {
                threePoint++;
                color = new Color32(255, 125, 0, 100);
                concentration = 7;
            }
            else if (0.03 <= listMole6[i] && listMole6[i] < 0.06)
            {
                fourPoint++;
                color = new Color32(255, 255, 0, 100);
                concentration = 15;
            }
            else if (0.06 <= listMole6[i] && listMole6[i] < 0.1)
            {
                fivePoint++;
                color = new Color32(255, 0, 255, 100);
                concentration = 25;
            }
            else if (0.1 <= listMole6[i] && listMole6[i] <= 0.5)
            {
                sixPoint++;
                color = new Color32(255, 0, 125, 100);
                concentration = 128;
            }
            else if (listMole6[i] > 0.5)
            {
                sevenPoint++;
                color = new Color32(255, 125, 255, 100);
                concentration = 255;
            }


            //Debug.Log(list[i]);
            ParticleSystem.EmitParams emitParams = new ParticleSystem.EmitParams
            {
                position = list[i],
                //startSize = (float)(listMole6[i] * Math.Pow(10, 2) * 3),
                //startSize = (float)listMole6[i],
                //startColor = new Color32(255, 255, 255, 255),
                //startColor = color,
                startColor = new Color32(255, 255, 255, normalize(listMole6[i])),
                //startColor = new Color32(255, 255, 255, concentration),
                startSize = 2f,
                startLifetime = 525f,
                angularVelocity = 0f
            };
            emitParams.ResetVelocity();
            if (paticalSwitch == true)
            {
                //paticalSwitch = false;
                system.Emit(emitParams, 1);
                system.Play(); // Continue normal emissions
            }
            else
            {
                //paticalSwitch = true;
                system1.Emit(emitParams, 1);
                system1.Play(); // Continue normal emissions
            }

        }
        Debug.Log("0---" + zeroPoint+"比例："+ Math.Round(((decimal)zeroPoint / listMole6.Count), 4));
        Debug.Log("0-0.01---" + twoPoint + "比例：" + Math.Round(((decimal)twoPoint / listMole6.Count), 4));
        Debug.Log("0.01-0.03---" + threePoint + "比例：" + Math.Round(((decimal)threePoint / listMole6.Count), 4));
        Debug.Log("0.03-0.06---" + fourPoint + "比例：" + Math.Round(((decimal)fourPoint / listMole6.Count), 4));
        Debug.Log("0.06-0.1---" + fivePoint + "比例：" + Math.Round(((decimal)fivePoint / listMole6.Count), 4));
        Debug.Log("0.1-0.5---" + sixPoint + "比例：" + Math.Round(((decimal)sixPoint / listMole6.Count), 4));
        Debug.Log("0.5-1---" + sevenPoint + "比例：" + Math.Round(((decimal)sevenPoint / listMole6.Count), 4));
        if (paticalSwitch == true)
        {
            paticalSwitch = false;
        }
        else
        {
            paticalSwitch = true;
        }
        //Debug.Log(DateTime.Now.TimeOfDay.ToString());
    }
    public void getCenter() { }
    /// <summary>
    /// 删除系统中所有的粒子系统
    /// </summary>
    public void clearPartical()
    {
        system.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }
    /// <summary>
    /// 每读取一次五秒钟数据，声明一个变量a，默认为0,读取一次，a++
    /// 然后在update里有一个变量b默认为1，如果a==b，那么调用一次渲染，同时b++
    /// </summary>
    public void loadMoleData6()
    {
        startTime = DateTime.Now;
        isRender = true;

        timer = new System.Timers.Timer();  //五秒访问

        timer.Elapsed += new ElapsedEventHandler(loadMoleData);
        //timer.Elapsed += new ElapsedEventHandler(test);
        timer.Start();
        timer.Interval = 5000;

        timer.AutoReset = true;
        timer.Enabled = true;
    }
    public void test(object source, System.Timers.ElapsedEventArgs e)
    {
        Debug.Log("测试");
    }

    private void OnDestroy()
    {
        if (timer != null)
        {
            timer.Close();
        }

    }

    /// <summary>
    /// 静态加载数据
    /// </summary>
    public void loadMoleDataStatic()
    {
        //a3730100  - 副本200秒.NFMOLE  data_39.txt  data39.txt
        StreamReader sr = new StreamReader("D:\\a3730100  - 副本200秒.NFMOLE", Encoding.Default);
        string line;
        Debug.Log("开始读取");
        //清空浓度list
        listMole6.Clear();
        listMole6Copy.Clear();
        //时间段递增
        //times++;
        //声明一个变量，用来达到当前变量
        //int curTimes = 0;
        while ((line = sr.ReadLine()) != null)
        {
            //if (curTimes < times)
            //{
            //    if (line.Contains("N"))
            //    {
            //        curTimes++;
            //    }
            //    else
            //    {
            //    }
            //}
            //else
            //{
            //if (line.Contains("N"))
            //{
            //    curTimes++;
            //}
            //else
            //{
            //需要限定只能在数字里进行
            //line.Length<=15是为了避开空行和时间行
            if (line.Length <= 15
                || line.Contains("G") || line.Contains("d") || line.Contains("r"))
            {
                if (line.Contains("TIME"))
                {
                    Debug.Log("当前时间" + line);
                }
            }
            else
            {
                string[] data = line.Split(' ');
                //如果遇到NFMOLE那么curTimes++,如果curTimes>times,那么就结束当前方法
                //if (line.Contains("N"))
                //{
                //    curTimes++;
                //}
                //else
                //{
                //代码到此处，说明已经超过所需5秒数据到了下一次5秒数据
                //if (curTimes > times)
                //{
                //    Debug.Log(list.Count);
                //    Debug.Log(listMole6.Count + "listMole6.Count");
                //    Debug.Log("a===" + a);
                //    Debug.Log("b===" + b);
                //    a++;
                //    return;
                //}
                //如果还在当前5秒数据，则加入listMole6
                for (int i = 0; i < data.Length; i++)
                {
                    if (data[i].Trim() != string.Empty && data[i].Trim().Contains("."))
                        //if (data[i].Trim() != string.Empty)
                    {
                        //Debug.Log(data[i].Trim());
                        listMole6.Add(float.Parse(data[i].Trim()));
                    }
                }
                //}
            }
            //}
            //}
        }
        sr.Close();
        loadData();
    }

    /// <summary>
    /// 动态加载数据
    /// </summary>
    public void loadMoleData(object source, System.Timers.ElapsedEventArgs e)
    {
        StreamReader sr = new StreamReader("D:\\data_39.txt", Encoding.Default);
        string line;
        Debug.Log("开始读取");
        //清空浓度list
        listMole6.Clear();
        //时间段递增
        times++;
        //声明一个变量，用来达到当前变量
        int curTimes = 0;
        while ((line = sr.ReadLine()) != null)
        {
            if (curTimes < times)
            {
                if (line.Contains("N"))
                {
                    curTimes++;
                }
                else
                {
                }
            }
            else
            {
                if (line.Contains("N"))
                {
                    curTimes++;
                }
                else
                {
                    //需要限定只能在数字里进行
                    //line.Length<=15是为了避开空行和时间行
                    if (line.Length <= 15
                        || line.Contains("G") || line.Contains("d") || line.Contains("r"))
                    {
                        if (line.Contains("TIME"))
                        {
                            Debug.Log("当前时间" + line);
                        }
                    }
                    else
                    {
                        string[] data = line.Split(' ');
                        //如果遇到NFMOLE那么curTimes++,如果curTimes>times,那么就结束当前方法
                        if (line.Contains("N"))
                        {
                            curTimes++;
                        }
                        else
                        {
                            //代码到此处，说明已经超过所需5秒数据到了下一次5秒数据
                            if (curTimes > times)
                            {
                                Debug.Log(list.Count);
                                Debug.Log(listMole6.Count + "listMole6.Count");
                                Debug.Log("a===" + a);
                                Debug.Log("b===" + b);
                                a++;
                                return;
                            }
                            //如果还在当前5秒数据，则加入listMole6
                            for (int i = 0; i < data.Length; i++)
                            {
                                if (data[i].Trim() != string.Empty && data[i].Trim().Contains("."))
                                {
                                    //Debug.Log(data[i].Trim());
                                    listMole6.Add(float.Parse(data[i].Trim()));
                                }
                            }
                        }
                    }
                }
            }
        }
        sr.Close();

    }
}
