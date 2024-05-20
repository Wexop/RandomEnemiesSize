using StaticNetcodeLib;
using Unity.Netcode;
using UnityEngine;

namespace RandomEnemiesSize
{
    [StaticNetcode]
    public class NetworkSize
    {
        [ClientRpc]
        public static void ExampleClientRpc(string exampleString)
        {
            Debug.Log(exampleString);
        }
    }
}