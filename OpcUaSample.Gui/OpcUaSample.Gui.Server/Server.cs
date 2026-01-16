using System.Collections.Generic;
using Opc.Ua;
using Opc.Ua.Configuration;
using Opc.Ua.Server;

namespace OpcUaSample.Gui.Server;

public partial class Server : Form
{
    private ApplicationInstance application;
    private MyUaServer server;

    public Server()
    {
        InitializeComponent();
        lblServerStatus.Text = "Stopped";
    }

    private async void BtnStartServer_Click(object sender, EventArgs e)
    {
        try
        {
            // 애플리케이션 인스턴스 설정
            application = new ApplicationInstance
            {
                ApplicationName = "SimpleServer",
                ApplicationType = ApplicationType.Server
            };

            // 수동으로 설정 객체 생성
            Opc.Ua.ApplicationConfiguration config = new()
            {
                ApplicationName = "SimpleServer",
                ApplicationUri = "urn:localhost:SimpleServer",
                ProductUri = "http://test.org/UA/SimpleServer",
                ApplicationType = ApplicationType.Server,

                // 보안 설정 (인증서 경로 지정)
                SecurityConfiguration = new SecurityConfiguration
                {
                    ApplicationCertificate = new CertificateIdentifier
                    {
                        StoreType = @"Directory",
                        StorePath = @"%CommonApplicationData%\OPC Foundation\CertificateStores\MachineDefault",
                        SubjectName = "SimpleServer"
                    },
                    TrustedIssuerCertificates = new CertificateTrustList
                    {
                        StoreType = @"Directory",
                        StorePath = @"%CommonApplicationData%\OPC Foundation\CertificateStores\UA Certificate Authorities"
                    },
                    TrustedPeerCertificates = new CertificateTrustList
                    {
                        StoreType = @"Directory",
                        StorePath = @"%CommonApplicationData%\OPC Foundation\CertificateStores\UA Applications"
                    },
                    RejectedCertificateStore = new CertificateTrustList
                    {
                        StoreType = @"Directory",
                        StorePath = @"%CommonApplicationData%\OPC Foundation\CertificateStores\RejectedCertificates"
                    },
                    AutoAcceptUntrustedCertificates = true, // 인증 자동 수락 (테스트용)
                    AddAppCertToTrustedStore = true
                },

                // 전송 쿼터 설정 (기본값)
                TransportQuotas = new TransportQuotas { OperationTimeout = 15000 },

                // 서버 구성 (엔드포인트 설정)
                ServerConfiguration = new ServerConfiguration
                {
                    BaseAddresses = { "opc.tcp://localhost:62541/SimpleServer" },
                    SecurityPolicies =
                {
                    new ServerSecurityPolicy
                    {
                        SecurityMode = MessageSecurityMode.SignAndEncrypt,
                        SecurityPolicyUri = SecurityPolicies.Basic256Sha256
                    },
                    new ServerSecurityPolicy // 보안 없음 모드 (테스트용)
                    {
                        SecurityMode = MessageSecurityMode.None,
                        SecurityPolicyUri = SecurityPolicies.None
                    }
                }
                }
            };

            // 설정 유효성 검사 (필수)
            await config.ValidateAsync(ApplicationType.Server);

            // 검증된 설정을 애플리케이션에 주입
            application.ApplicationConfiguration = config;

            // 인증서 검증 (Async 필수)
            bool certOk = await application.CheckApplicationInstanceCertificatesAsync(false, 2048);
            if (!certOk)
            {
                MessageBox.Show("인증서 생성/검증 중 문제가 발생했습니다.");
            }

            // 서버 인스턴스 생성 및 노드 매니저 연결
            server = new MyUaServer(this);

            // 서버 시작 (Async 필수)
            await application.StartAsync(server);

            // UI 업데이트 (UI 스레드에서 실행되도록 Invoke 사용 가능하지만, async void에서는 보통 안전)
            lblServerStatus.Text = "서버 실행 중 (Async Mode):\nopc.tcp://localhost:62541/SimpleServer";
            BtnStartServer.Enabled = false;
            BtnStopServer.Enabled = true;
        }
        catch (Exception ex)
        {
            MessageBox.Show("서버 시작 오류: " + ex.Message);
        }
    }

    private void BtnStopServer_Click(object sender, EventArgs e)
    {
        if (server != null)
        {
            server?.Stop();
            lblServerStatus.Text = "Stopped";
            BtnStartServer.Enabled = true;
            BtnStopServer.Enabled = false;
        }
    }

    public void LogMessage(string message)
    {
        if (txtServerLog.InvokeRequired)
        {
            txtServerLog.Invoke(new Action(() => LogMessage(message)));
            return;
        }
        txtServerLog.AppendText($"[{DateTime.Now:HH:mm:ss}] {message}\r\n");
        txtServerLog.ScrollToCaret();
    }
}

internal class MyNodeManager_3DPrinter : CustomNodeManager2
{
    private Server _mainForm; // UI 접근을 위한 참조

    // 노드를 나중에 업데이트하기 위해 멤버 변수로 보관
    private BaseDataVariableState _temperatureNode;
    private BaseDataVariableState _statusNode;
    private System.Threading.Timer _simulationTimer; // 값 변경 시뮬레이션용

    public MyNodeManager_3DPrinter(IServerInternal server, Opc.Ua.ApplicationConfiguration configuration, Server mainForm) : base(server, configuration, "http://test.org/UA/SimpleServer")
    {
        _mainForm = mainForm;
        // 1초마다 센서 값 변경 시뮬레이션 시작
        _simulationTimer = new System.Threading.Timer(DoSimulation, null, 1000, 1000);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _simulationTimer?.Dispose();
        }
        base.Dispose(disposing);
    }

    // 데이터 읽기 요청 감지
    public override void Read(
        OperationContext context,
        double maxAge,
        IList<ReadValueId> nodesToRead,
        IList<DataValue> values,
        IList<ServiceResult> errors)
    {
        _mainForm.LogMessage($"[Read Request] Items: {nodesToRead.Count}");
        base.Read(context, maxAge, nodesToRead, values, errors);
    }

    // 데이터 쓰기 요청 감지
    public override void Write(
        OperationContext context,
        IList<WriteValue> nodesToWrite,
        IList<ServiceResult> results)
    {
        foreach (var node in nodesToWrite)
        {
            _mainForm.LogMessage($"[Write Request] NodeId: {node.NodeId}, Value: {node.Value.Value}");
        }
        base.Write(context, nodesToWrite, results);
    }

    // 구독(MonitoredItem) 생성 감지 --> 수정 필요
    public override void CreateMonitoredItems(
    OperationContext context,
    uint subscriptionId,
    double publishingInterval,
    TimestampsToReturn timestampsToReturn,
    IList<MonitoredItemCreateRequest> itemsToCreate,
    IList<ServiceResult> errors,
    IList<MonitoringFilterResult> filterResults,
    IList<IMonitoredItem> monitoredItems,
    bool globalIdIdUsed,
    ref long lastMonitoredItemId)
    {
        _mainForm.LogMessage($"[Subscription Start] Count: {itemsToCreate.Count}");

        //return base.CreateMonitoredItems(
        //    context, subscriptionId, publishingInterval, timestampsToReturn,
        //    itemsToCreate, errors, filterResults, monitoredItems,
        //    globalIdIdUsed, ref lastMonitoredItemId);
    }

    // 구독(MonitoredItem) 해제 감지 --> 수정 필요
    public override void DeleteMonitoredItems(
    OperationContext context,
    IList<IMonitoredItem> monitoredItems,
    IList<bool> processedStatus,
    IList<ServiceResult> errors)
    {
        // 서버 UI에 로그 출력
        _mainForm.LogMessage($"[Subscription End] Items Unsubscribed. Count: {monitoredItems.Count}");

        //return base.DeleteMonitoredItems(context, monitoredItems, processedStatus, errors);
    }

    public override void CreateAddressSpace(IDictionary<NodeId, IList<IReference>> externalReferences)
    {
        lock (Lock)
        {
            // Objects 폴더 참조 가져오기 (여기에 자식들을 추가할 것입니다)
            if (!externalReferences.TryGetValue(ObjectIds.ObjectsFolder, out var references))
            {
                externalReferences[ObjectIds.ObjectsFolder] = references = [];
            }

            // =================================================================
            // 1. Hello World 노드 생성
            // =================================================================
            BaseDataVariableState helloNode = CreateVariable(null, "HelloWorld", "HelloWorld", BuiltInType.String, ValueRanks.Scalar);
            helloNode.Value = "Hello World is Back!";
            helloNode.AccessLevel = AccessLevels.CurrentRead;

            // 노드 매니저에 등록
            AddPredefinedNode(SystemContext, helloNode);

            // Objects 폴더 목록에 추가 (그래야 클라이언트가 Browse 할 때 보임)
            references.Add(new NodeStateReference(ReferenceTypeIds.Organizes, false, helloNode.NodeId));

            // =================================================================
            // 2. 3D Printer 폴더 및 노드 생성
            // =================================================================
            // "3DPrinter" 폴더 생성
            FolderState printerFolder = CreateFolder(null, "3DPrinter", "3D Printer");
            // [수정 1] 폴더를 노드 매니저에 등록 (필수)
            AddPredefinedNode(SystemContext, printerFolder);

            // Objects 폴더 목록에 3DPrinter 폴더 추가
            references.Add(new NodeStateReference(ReferenceTypeIds.Organizes, false, printerFolder.NodeId));

            // 온도 센서 노드 (Double, 읽기 전용)
            // NodeId를 클라이언트와 맞추기 위해 "3DPrinter/Temperature"로 지정
            _temperatureNode = CreateVariable(printerFolder, "3DPrinter/Temperature", "Temperature", BuiltInType.Double, ValueRanks.Scalar);
            _temperatureNode.Value = 20.5;
            _temperatureNode.AccessLevel = AccessLevels.CurrentRead;
            // 변수를 노드 매니저에 등록 (필수)
            AddPredefinedNode(SystemContext, _temperatureNode);

            // 상태 제어 노드 (Boolean, 읽기/쓰기 가능 -> 제어용)
            // NodeId를 "3DPrinter/IsActive"로 지정
            _statusNode = CreateVariable(printerFolder, "3DPrinter/IsActive", "IsActive", BuiltInType.Boolean, ValueRanks.Scalar);
            _statusNode.Value = false;
            _statusNode.AccessLevel = AccessLevels.CurrentReadOrWrite;
            _statusNode.UserAccessLevel = AccessLevels.CurrentReadOrWrite;
            // 변수를 노드 매니저에 등록 (필수)
            AddPredefinedNode(SystemContext, _statusNode);
        }
    }

    // 헬퍼 함수: 폴더 생성
    private FolderState CreateFolder(NodeState parent, string path, string name)
    {
        FolderState folder = new(parent)
        {
            SymbolicName = name,
            ReferenceTypeId = ReferenceTypeIds.Organizes,
            TypeDefinitionId = ObjectTypeIds.FolderType,
            NodeId = new NodeId(path, NamespaceIndex),
            BrowseName = new QualifiedName(path, NamespaceIndex),
            DisplayName = new LocalizedText(name),
            WriteMask = AttributeWriteMask.None,
            UserWriteMask = AttributeWriteMask.None,
            EventNotifier = EventNotifiers.None
        };
        parent?.AddChild(folder);
        return folder;
    }

    // 헬퍼 함수: 변수 생성
    private BaseDataVariableState CreateVariable(NodeState parent, string path, string name, BuiltInType type, int valueRank)
    {
        BaseDataVariableState variable = new(parent)
        {
            SymbolicName = name,
            ReferenceTypeId = ReferenceTypeIds.Organizes,
            TypeDefinitionId = VariableTypeIds.BaseDataVariableType,
            NodeId = new NodeId(path, NamespaceIndex),
            BrowseName = new QualifiedName(path, NamespaceIndex),
            DisplayName = new LocalizedText(name),
            WriteMask = AttributeWriteMask.None,
            UserWriteMask = AttributeWriteMask.None,
            DataType = (uint)type,
            ValueRank = valueRank
        };
        parent?.AddChild(variable);
        return variable;
    }

    private void DoSimulation(object state)
    {
        // 서버가 실행 중일 때만 동작
        if (_temperatureNode == null) return;

        lock (Lock)
        {
            // 현재 온도 가져오기
            double currentTemp = (double)_temperatureNode.Value;

            // 랜덤하게 온도 변화 (+- 0.5도)
            Random rnd = new();
            currentTemp += (rnd.NextDouble() - 0.5);

            // 값 업데이트
            _temperatureNode.Value = currentTemp;
            _temperatureNode.Timestamp = DateTime.UtcNow; // 시간 갱신 중요

            // [중요] 변경 사항을 구독자(Client)에게 알림
            _temperatureNode.ClearChangeMasks(SystemContext, true);
        }
    }
}

internal class MyUaServer : StandardServer
{
    private Server _mainForm;

    public MyUaServer(Server mainForm)
    {
        _mainForm = mainForm;
    }

    // 이 메서드는 서버가 초기화될 때 딱 한 번 호출됩니다.
    protected override MasterNodeManager CreateMasterNodeManager(IServerInternal server, Opc.Ua.ApplicationConfiguration configuration)
    {
        var nodeManagers = new List<INodeManager>
        {
            // 여기서 내 NodeManager를 리스트에 "직접" 담습니다.
            // (Factory 오류 없이 인스턴스를 바로 넣을 수 있는 유일한 곳입니다.)
            new MyNodeManager_3DPrinter(server, configuration, _mainForm)
        };

        // MasterNodeManager에게 내 리스트를 전달하며 생성합니다.
        return new MasterNodeManager(server, configuration, null, [.. nodeManagers]);
    }
}