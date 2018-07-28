using Controllers;
using Newtonsoft.Json;
using UniRx;
using UnityEngine;
using Utils.Dev;
using Utils.Network;
using Utils.Network.Json;
using Utils.Toolbox;

public class NPCSpawnerController : MonoBehaviour
{
    [SerializeField] [ReadOnlyWhenPlaying] public GameObject Prefab;

    private void Start()
    {
        var zmqIPCManager = Toolbox.Instance.GetComponent<ZMQIPCManager>();

        zmqIPCManager.MessageObservable
            .Select(message => JsonConvert.DeserializeObject<NetAction>(message.First.ConvertToString()))
            .Where(netAction => netAction.Type == ActionType.SPAWN_NPC)
            .Select(netAction =>
            {
                var payloadString = JsonConvert.SerializeObject(netAction.Payload.ExtraStuff);
                return JsonConvert.DeserializeObject<SpawnNPCPayload>(payloadString);
            })
            .Subscribe(payload =>
            {
                var npc = Instantiate(Prefab, transform.position, Quaternion.identity);
                var npcController = npc.GetComponent<NPCController>();
                npcController.Id = payload.Id;
                npcController.Target = GameObject.Find("Cube").transform;
            });
    }
}
