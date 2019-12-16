using System;
using System.Runtime.InteropServices;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using System.Collections.Concurrent;
using UnityEngine.UI;
using FlatBuffers;

public class Server__TCP : MonoBehaviour
{
    public bool DEBUG_MODE = true;

    public static Server__TCP Instance;
    [SerializeField] InputField ipf;
    public string Url;
    public int port;

    public int MY_ID;

    public static int __ping = 0;

    ConcurrentQueue<Action> actions = new ConcurrentQueue<Action>();

    internal static void TestPing()
    {
        var fbb = new FlatBufferBuilder(1);
        fbb.Finish(fping.Createfping(fbb, eFB_Type.fping, DateTime.Now.ToBinary()).Value);
        Instance.Send(fbb.SizedByteArray());
    }




    #region private members 	
    private TcpClient socketConnection;
    private Thread clientReceiveThread;
    #endregion
    // Use this for initialization 	
    void Start()
    {
        //ConnectToTcpServer();
        Time.timeScale = 0;

    }


    public void OnStartServer()
    {
        Url = ipf.text;
        ConnectToTcpServer();
        Time.timeScale = 1;
    }

    private void Awake()
    {
        MY_ID = -1;
        Instance = this;
    }


    public void Send(byte[] str)
    {
        ClientSend(str);
    }
    public void Send(string str)
    {
        SendChat(str);
    }
    private void OnDestroy()
    {
        socketConnection.Close();
        clientReceiveThread.Abort();
    }
    private void ConnectToTcpServer()
    {
        try
        {
            clientReceiveThread = new Thread(new ThreadStart(ListenForData));
            clientReceiveThread.IsBackground = true;
            clientReceiveThread.Start();
        }
        catch (Exception e)
        {
            Debug.Log("On client connect exception " + e);
        }
    }

    public void SendTest()
    {
        var fbb = new FlatBufferBuilder(1);
        fbb.Finish(fping.Createfping(fbb, eFB_Type.fping, DateTime.Now.ToBinary()).Value);
        Send(fbb.SizedByteArray());
    }

    struct RawData
    {
        public bool success;

        public byte[] data;
        public int dataLen;

        public byte[] lastData;
        public int lastDataLen;
    };

    RawData get_header_tail_to_data(byte[] data, int len)
    {
        if (len < sizeof(int)) return new RawData();

        RawData d = new RawData();
        int ReadDataLen;
        ReadDataLen = BitConverter.ToInt32(data, 0);

        if (len < ReadDataLen) return new RawData();

        if (ReadDataLen > 1024 || ReadDataLen <= 0)
        {
            print("[Error] 받을 수 없는 패킷을 받음.");
        }
        else
        {
            print($"{ReadDataLen}만큼 읽어 드림.");
        }

        d.dataLen = ReadDataLen - sizeof(int);                                  // 헤더의 길이만큼 빼주기.
        d.data = new byte[d.dataLen];
        Array.Copy(data, 0, d.data, 0, d.dataLen);

        if ((len - ReadDataLen) > 0)
        {
            d.lastDataLen = len - ReadDataLen;
            d.lastData = new byte[d.lastDataLen];
            Array.Copy(data, ReadDataLen, d.lastData, 0, d.lastDataLen);

            print("뭉쳐오는 패킷 확인");
        }
        else
        {
            d.lastDataLen = 0;
        }
        d.success = true;

        return d;
    }



    private void ListenForData()
    {
        //var t = DateTime.Now.ToBinary() - m_ping.Num;
        //System.DateTime.Now.ToBinary();
        try
        {
            if (DEBUG_MODE)
                socketConnection = new TcpClient("Localhost", port);
            else
                socketConnection = new TcpClient(Url, port);

            socketConnection.NoDelay = true;
            Byte[] bytes = new Byte[1024];

            byte[] mBuffer = null;
            int mBuf_len = 0;

            var fbb = new FlatBufferBuilder(1);
            fbb.Finish(fid.Createfid(fbb, eFB_Type.fid).Value);
            Instance.Send(fbb.SizedByteArray());

            while (true)
            {
                using (NetworkStream stream = socketConnection.GetStream())
                {
                    int length;
                    
                    while ((length = stream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        byte[] PrevBuf = mBuffer;
                        mBuffer = new byte[mBuf_len + bytes.Length];

                        if (PrevBuf != null)
                        {
                            Array.Copy(PrevBuf, 0, mBuffer, 0, PrevBuf.Length);
                        }
                        Array.Copy(bytes, 0, mBuffer, PrevBuf == null ? 0 : PrevBuf.Length, length);
                        mBuf_len += length;

                        while (true)
                        {
                            var obj = get_header_tail_to_data(mBuffer, mBuf_len);
                            if (obj.success)
                            {
                                var incommingData = new byte[obj.dataLen];
                                Array.Copy(mBuffer, 4, incommingData, 0, obj.dataLen);
                                ByteBuffer bb = new ByteBuffer(incommingData);

                                Base baseData = Base.GetRootAsBase(bb);

                                var Data = baseData;

                                Debug.Log(Data.FType.ToString());
                                if (Data.FType == eFB_Type.fping)
                                {
                                    __ping = (int)((DateTime.Now.ToBinary() - fping.GetRootAsfping(Data.ByteBuffer).Time) * 0.0001f);
                                }
                                else
                                {
                                    var Data2 = baseData;
                                    actions.Enqueue(() =>
                                    {
                                        YCRead.REACT[Data2.FType](Data2);
                                    });
                                }

                                if (obj.lastDataLen > 0)
                                {
                                    mBuffer = obj.lastData;
                                    mBuf_len = obj.lastDataLen;
                                }
                                else
                                {
                                    mBuffer = null;
                                    mBuf_len = 0;
                                    break;
                                }
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                }
            }
        }
        catch (SocketException socketException)
        {
            Debug.Log("Socket exception: " + socketException);
        }
    }
    private void Update()
    {
        if (!actions.IsEmpty)
        {
            foreach (var i in actions)
            {
                Action act;
                actions.TryDequeue(out act);
                act();
            }
        }
    }
    /*
    class PingData
    {
        long mTime;
        char[] _data;
        public long Time
        {
            get { return mTime; }
            set { mTime = value; }
        }
        public int x
        {
            get { return _data[0]; }
            set { _data[0] = value; }
        }
        public int y
        {
            get { return _data[1]; }
            set { _data[1] = value; }
        }

        public int[] data
        {
            get { return _data; }
            set
            {
                _data[0] = value[0];
                _data[1] = value[1];
            }
        }


        public PingData()
        {
            Time
            _data = new int[2];
        }

    }
    */
    
    void ClientSend(byte[] str)
    {
        if (socketConnection == null)
        {
            Debug.Log("서버가 닫혀있음.");
            return;
        }
        try
        {
            NetworkStream stream = socketConnection.GetStream();
            if (stream.CanWrite)
            {
                int len = str.Length + 4;
                byte[] size = new byte[4];
                size[3] = (byte)(len >> 24);
                size[2] = (byte)(len >> 16);
                size[1] = (byte)(len >> 8);
                size[0] = (byte)len;

                byte[] clientMessageAsByteArray = new byte[str.Length + size.Length];


                Array.Copy(size, 0, clientMessageAsByteArray, 0, size.Length);
                Array.Copy(str, 0, clientMessageAsByteArray, size.Length, str.Length);

                stream.Write(clientMessageAsByteArray, 0, clientMessageAsByteArray.Length);
            }
            else
            {
                Debug.Log("Notsend!");
            }
        }
        catch (SocketException socketException)
        {
            Debug.Log("Socket exception: " + socketException);
        }
    }
    




    private void SendChat(string str)
    {
        if (socketConnection == null)
        {
            Debug.Log("null");
            return;
        }
        try
        {
            NetworkStream stream = socketConnection.GetStream();
            Debug.Log("채팅보낼 준비.");
            if (stream.CanWrite)
            {
                Debug.Log("채팅 보냄.");
                byte[] clientMessageAsByteArray = Encoding.ASCII.GetBytes(str);
                stream.Write(clientMessageAsByteArray, 0, clientMessageAsByteArray.Length);
            }
        }
        catch (SocketException socketException)
        {
            Debug.Log("Socket exception: " + socketException);
        }
    }

    private void SendToServer(byte[] buf, int len)
    {
        if (socketConnection == null)
        {
            Debug.Log("null");
            return;
        }
        try
        {
            NetworkStream stream = socketConnection.GetStream();
            Debug.Log("채팅보낼 준비.");
            if (stream.CanWrite)
            {
                int llen = len + 4;
                Debug.Log(llen.ToString());

                byte[] size = new byte[4];
                size[3] = (byte)(llen >> 24);
                size[2] = (byte)(llen >> 16);
                size[1] = (byte)(llen >> 8);
                size[0] = (byte)llen;

                Debug.Log($"0:{size[0]}1:{size[1]}2:{size[2]}3:{size[3]}");

                byte[] clientMessageAsByteArray = new byte[len + size.Length];


                Array.Copy(size, 0, clientMessageAsByteArray, 0, size.Length);
                Array.Copy(buf, 0, clientMessageAsByteArray, size.Length, len);
                
                stream.Write(clientMessageAsByteArray, 0, clientMessageAsByteArray.Length);
            }
        }
        catch (SocketException socketException)
        {
            Debug.Log("Socket exception: " + socketException);
        }
    }
}