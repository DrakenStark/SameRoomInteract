using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace DrakenStark
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.NoVariableSync)]
    public class SameRoomPickup : UdonSharpBehaviour
    {
        [SerializeField] private VRC.SDK3.Components.VRCPickup _vRCPickup = null;
        [SerializeField] private LayerMask _wallLayers = 0;
        private Vector3 _offset = new Vector3(0f, 0.1f, 0f);
        private int _loopCount = 0;
        private Vector3 _playerPos = Vector3.zero;
        private float _distance = 0f;

        public override void OnAvatarEyeHeightChanged(VRCPlayerApi player, float prevEyeHeightAsMeters)
        {
            if (player.isLocal) {
                _offset = new Vector3(0f, player.GetAvatarEyeHeightAsMeters()/2, 0f);
            }
        }

        private void OnEnable()
        {
            //Debug.Log(name + ": Wall detection started.");
            _loopCount++;
            _rayCheck();
        }

        public void _rayCheck()
        {
            if (gameObject.activeSelf && _loopCount < 2)
            {
                _playerPos = Networking.LocalPlayer.GetPosition() + _offset;
                _distance = Vector3.Distance(_playerPos, transform.position);
                _vRCPickup.pickupable = !Physics.Raycast(transform.position, (_playerPos - transform.position).normalized, out RaycastHit hit, _distance, _wallLayers);
                //Debug.Log(name + ": " + (Physics.Raycast(transform.position, (playerPos - transform.position).normalized, out RaycastHit hit2, distance, _wallLayers) ? "Wall detected" : "No wall detected") + ".");
                SendCustomEventDelayedSeconds(nameof(_rayCheck), .5f);
            }
            else if (_loopCount > 0)
            {
                _loopCount--;
            }
        }

        //RayDraws to visually see the rays are correctly calculated.
        /*
        public void Update()
        {
            Debug.DrawRay(transform.position, (Networking.LocalPlayer.GetPosition() + _offset - transform.position).normalized, Color.white, Time.deltaTime);
            Debug.DrawRay(Networking.LocalPlayer.GetPosition() + _offset, (transform.position - (Networking.LocalPlayer.GetPosition() + _offset)).normalized, Color.cyan, Time.deltaTime);
        }
        */
    }
}