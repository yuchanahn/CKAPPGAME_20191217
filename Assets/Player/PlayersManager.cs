using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FlatBuffers;

public class PlayersManager : MonoBehaviour
{
    Dictionary<int, GameObject> players = new Dictionary<int, GameObject>();
    Dictionary<int, GameObject> mobs = new Dictionary<int, GameObject>();

    [SerializeField] GameObject PlayerPrefab_;
    [SerializeField] GameObject MobPrefab_;
    void Start()
    {
        YCRead.REACT[eFB_Type.fout] = (data) =>
        {
            var future_player = fout.GetRootAsfout(data.ByteBuffer);
            if (players.ContainsKey(future_player.Id))
            {
                Destroy( players[future_player.Id]);
            }
        };

        YCRead.REACT[eFB_Type.fboom] = (data) =>
        {
            var future_player = fboom.GetRootAsfboom(data.ByteBuffer);
            if (players.ContainsKey(future_player.Id))
            {
                players[future_player.Id].GetComponent<Movement>().Boom.SetActive(true);
            }
        };

        YCRead.REACT[eFB_Type.fmob] = (data) =>
        {
            var f = fmob.GetRootAsfmob(data.ByteBuffer);
            if (!mobs.ContainsKey(f.Id))
            {
                mobs[f.Id] = Instantiate(MobPrefab_, new Vector3(f.X, MobPrefab_.transform.position.y, f.Y), MobPrefab_.transform.rotation);
            }
            mobs[f.Id].GetComponent<MobMove>().ptime = 0;
            mobs[f.Id].GetComponent<MobMove>().TargetPos = new Vector3(f.X, mobs[f.Id].transform.position.y, f.Y);
        };

        YCRead.REACT[eFB_Type.fid] = (data) =>
        {
            var f = fid.GetRootAsfid(data.ByteBuffer);
            Server__TCP.Instance.MY_ID = f.Id;
            players[Server__TCP.Instance.MY_ID] = FindObjectOfType<Movement>().gameObject;
            players[Server__TCP.Instance.MY_ID].GetComponent<Movement>().id = f.Id;
        };

        YCRead.REACT[eFB_Type.fplayer] = (data) =>
        {
            if (Server__TCP.Instance.MY_ID == -1) return;
            var future_player = fplayer.GetRootAsfplayer(data.ByteBuffer);

            if (Server__TCP.Instance.MY_ID != future_player.Id)
            {
                if (!players.ContainsKey(future_player.Id))
                {
                    players[future_player.Id] = Instantiate(PlayerPrefab_, new Vector3(future_player.V, PlayerPrefab_.transform.position.y, future_player.H), Quaternion.identity);
                    players[future_player.Id].GetComponent<Movement>().id = future_player.Id;
                }
            }

            var cur_player = players[future_player.Id].GetComponent<Movement>();
            //cur_player.name = future_player.Name;


            cur_player.ptime = 0;
            cur_player.y__rot = future_player.X;
            cur_player.TargetPos = new Vector3(future_player.V, FindObjectOfType<Movement>().transform.position.y, future_player.H);
        };
    }
}
