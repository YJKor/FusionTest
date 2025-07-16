using UnityEngine;
using Fusion;

public class NetworkRig : NetworkBehaviour
{
    // �ν����Ϳ��� �ƹ�Ÿ�� �� ������ �������ݴϴ�.
    [SerializeField] private Transform head;
    [SerializeField] private Transform leftHand;
    [SerializeField] private Transform rightHand;

    private HardwareRig hardwareRig; // ���� �ִ� XR Origin�� ������ ����
    private NetworkCharacterController _ncc; // NetworkCharacterController ����

    void Awake()
    {
        // ������Ʈ ������ �̸� �����ɴϴ�.
        _ncc = GetComponent<NetworkCharacterController>();
    }

    public override void Spawned()
    {
        // �� �ƹ�Ÿ�� '��' �ƹ�Ÿ�� ��쿡�� ����˴ϴ�.
        if (Object.HasInputAuthority)
        {
            // ���� �ִ� �� �ϳ��� HardwareRig�� ã���ϴ�.
            hardwareRig = FindObjectOfType<HardwareRig>();
            if (hardwareRig == null) Debug.LogError("������ HardwareRig�� ã�� �� �����ϴ�!");
        }
    }


    public override void FixedUpdateNetwork()
    {
        // �Է� ������ �ִ� �÷��̾�(���� �÷��̾�)�� �Է��� �޾� �����Դϴ�.
        if (GetInput(out PlayerNetworkInput input))
        {
            // �Է� ���� ���� �̵� ���͸� ����մϴ�.
            Vector3 moveDirection = transform.forward * input.Movement.y + transform.right * input.Movement.x;

            // Move �Լ��� ȣ���Ͽ� ĳ���͸� �����Դϴ�. (���� �浹 �����)
            _ncc.Move(moveDirection * Runner.DeltaTime * 5f); // 5f�� �̵� �ӵ�
        }
    }




    public override void Render()
    {
        // Render������ �� �̻� ������ ��ġ(transform.position)�� �������� �ʽ��ϴ�.
        // ���� �Ӹ��� ���� ������ ��ġ�� ����ϴ�.
        if (Object.HasInputAuthority && hardwareRig != null)
        {
            // ������ ȸ���� ������ �Ӹ��� ���󰩴ϴ�.
            transform.rotation = Quaternion.Euler(0, hardwareRig.head.rotation.eulerAngles.y, 0);

            // �Ӹ��� ���� ��ġ/ȸ�� ���߱�
            head.position = hardwareRig.head.position;
            //head.rotation = hardwareRig.head.rotation;

            leftHand.position = hardwareRig.leftHand.position;
            //leftHand.rotation = hardwareRig.leftHand.rotation;

            rightHand.position = hardwareRig.rightHand.position;
            //rightHand.rotation = hardwareRig.rightHand.rotation;
        }
    }
}