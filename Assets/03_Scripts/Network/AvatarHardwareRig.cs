using UnityEngine;
using Fusion;

// 로컬 XR 하드웨어의 Transform을 찾아 네트워크 변수에 동기화하는 역할을 합니다.
public class AvatarHardwareRig : NetworkBehaviour
{
    // --- 네트워크 동기화될 변수들 ---
    [Networked] public Vector3 HeadsetPos { get; set; }
    [Networked] public Quaternion HeadsetRot { get; set; }
    [Networked] public Vector3 LeftHandPos { get; set; }
    [Networked] public Quaternion LeftHandRot { get; set; }
    [Networked] public Vector3 RightHandPos { get; set; }
    [Networked] public Quaternion RightHandRot { get; set; }

    // --- 로컬에서만 참조할 하드웨어 Transform ---
    private Transform _headset;
    private Transform _leftHand;
    private Transform _rightHand;

    // --- 입력 데이터 구조체 (이동용) ---
    public struct NetworkInputData : INetworkInput
    {
        public Vector2 joystickInput;
        // 필요 시 버튼 입력 등 추가
    }

    public override void Spawned()
    {
        if (HasStateAuthority)
        {
            // 로컬 플레이어의 XR Origin을 찾습니다.
            // 실제 프로젝트에서는 더 견고한 방식으로 찾아야 합니다 (예: 싱글톤 매니저)
            var xrOrigin = FindObjectOfType<Unity.XR.CoreUtils.XROrigin>();
            if (xrOrigin != null)
            {
                _headset = xrOrigin.Camera.transform;
                // 컨트롤러는 XRI의 Action 기반 입력을 통해 가져오는 것이 더 안정적입니다.
                // 여기서는 간단히 이름으로 찾습니다.
                _leftHand = xrOrigin.transform.Find("Camera Offset/LeftHand Controller");
                _rightHand = xrOrigin.transform.Find("Camera Offset/RightHand Controller");
            }
            else
            {
                Debug.LogError("XR Origin을 찾을 수 없습니다.");
            }
        }
    }

    public override void FixedUpdateNetwork()
    {
        // 상태 권한이 있는 클라이언트만 하드웨어 정보를 네트워크 변수에 기록합니다.
        if (HasStateAuthority)
        {
            if (_headset != null)
            {
                // XR Origin은 고정되어 있고 카메라/컨트롤러만 움직이므로
                // 월드 좌표계가 아닌, 아바타 기준 로컬 좌표로 변환해주는 것이 좋습니다.
                // 여기서는 간단히 월드 좌표를 사용합니다.
                HeadsetPos = _headset.position;
                HeadsetRot = _headset.rotation;
                LeftHandPos = _leftHand.position;
                LeftHandRot = _leftHand.rotation;
                RightHandPos = _rightHand.position;
                RightHandRot = _rightHand.rotation;
            }
        }
    }
}