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

            // [핵심] Builder 대신 수동으로 설정 객체 생성
            // 빌더 내부의 불확실한 로직을 피하기 위해 직접 값을 할당합니다.
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
                    AutoAcceptUntrustedCertificates = true, // 개발용 자동 수락
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

            // 3. 설정 유효성 검사 (필수)
            await config.ValidateAsync(ApplicationType.Server);

            // 4. 검증된 설정을 애플리케이션에 주입
            application.ApplicationConfiguration = config;

            // 인증서 검증 (Async 필수)
            bool certOk = await application.CheckApplicationInstanceCertificatesAsync(false, 2048);
            if (!certOk)
            {
                MessageBox.Show("인증서 생성/검증 중 문제가 발생했습니다.");
            }

            // 서버 인스턴스 생성 및 노드 매니저 연결
            server = new MyUaServer();

            // 서버 시작 (Async 필수)
            await application.StartAsync(server);

            // UI 업데이트 (UI 스레드에서 실행되도록 Invoke 사용 가능하지만, async void에서는 보통 안전)
            lblServerStatus.Text = "서버 실행 중 (Async Mode):\nopc.tcp://localhost:62541/SimpleServer";
            BtnStartServer.Enabled = false;
        }
        catch (Exception ex)
        {
            MessageBox.Show("서버 시작 오류: " + ex.Message);
            // NullReferenceException -- 서버 시작 오류: Object reference not set to an instance of an object.
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
}

internal class MyNodeManager : CustomNodeManager2
{
    public MyNodeManager(IServerInternal server, Opc.Ua.ApplicationConfiguration configuration)
        : base(server, configuration, "http://test.org/UA/SimpleServer") { }

    public override void CreateAddressSpace(IDictionary<NodeId, IList<IReference>> externalReferences)
    {
        lock (Lock)
        {
            BaseDataVariableState variable = new(null)
            {
                NodeId = new NodeId("HelloWorld", NamespaceIndex),
                BrowseName = new QualifiedName("HelloWorld", NamespaceIndex),
                DisplayName = new LocalizedText("HelloWorld"),
                WriteMask = AttributeWriteMask.None,
                UserWriteMask = AttributeWriteMask.None,
                DataType = DataTypeIds.String,
                ValueRank = ValueRanks.Scalar,
                AccessLevel = AccessLevels.CurrentRead,
                UserAccessLevel = AccessLevels.CurrentRead,
                Value = "Hello World (Async Server!)"
            };

            if (!externalReferences.TryGetValue(ObjectIds.ObjectsFolder, out var references))
            {
                externalReferences[ObjectIds.ObjectsFolder] = references = [];
            }
            references.Add(new NodeStateReference(ReferenceTypeIds.Organizes, false, variable.NodeId));
            AddPredefinedNode(SystemContext, variable);
        }
    }
}

internal class MyUaServer : StandardServer
{
    // 이 메서드는 서버가 초기화될 때 딱 한 번 호출됩니다.
    protected override MasterNodeManager CreateMasterNodeManager(IServerInternal server, Opc.Ua.ApplicationConfiguration configuration)
    {
        var nodeManagers = new List<INodeManager>
        {
            // 여기서 내 NodeManager를 리스트에 "직접" 담습니다.
            // (Factory 오류 없이 인스턴스를 바로 넣을 수 있는 유일한 곳입니다.)
            new MyNodeManager(server, configuration)
        };

        // MasterNodeManager에게 내 리스트를 전달하며 생성합니다.
        return new MasterNodeManager(server, configuration, null, [.. nodeManagers]);
    }
}