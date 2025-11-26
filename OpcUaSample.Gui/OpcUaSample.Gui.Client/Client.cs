using Opc.Ua;
using Opc.Ua.Client;
using Opc.Ua.Configuration;
using System;
using System.Security.Policy;

namespace OpcUaSample.Gui.Client;

public partial class Client : Form
{
    public Client()
    {
        InitializeComponent();
    }

    private async void btnRead_Click(object sender, EventArgs e)
    {
        string serverUrl = "opc.tcp://localhost:62541/SimpleServer";

        try
        {
            // 클라이언트 설정 생성 (수동 설정 방식)
            Opc.Ua.ApplicationConfiguration config = new()
            {
                ApplicationName = "SimpleClient",
                ApplicationUri = "urn:localhost:SimpleClient",
                ApplicationType = ApplicationType.Client,
                SecurityConfiguration = new SecurityConfiguration
                {
                    ApplicationCertificate = new CertificateIdentifier { StoreType = @"Directory", StorePath = @"%CommonApplicationData%\OPC Foundation\CertificateStores\MachineDefault", SubjectName = "SimpleClient" },
                    AutoAcceptUntrustedCertificates = true, // 서버 인증서 신뢰
                    TrustedIssuerCertificates = new CertificateTrustList { StoreType = @"Directory", StorePath = @"%CommonApplicationData%\OPC Foundation\CertificateStores\UA Certificate Authorities" },
                    TrustedPeerCertificates = new CertificateTrustList { StoreType = @"Directory", StorePath = @"%CommonApplicationData%\OPC Foundation\CertificateStores\UA Applications" },
                    RejectedCertificateStore = new CertificateTrustList { StoreType = @"Directory", StorePath = @"%CommonApplicationData%\OPC Foundation\CertificateStores\RejectedCertificates" }
                },
                TransportQuotas = new TransportQuotas { OperationTimeout = 15000 },
                ClientConfiguration = new ClientConfiguration { DefaultSessionTimeout = 60000 }
            };

            // 설정 유효성 검사
            await config.ValidateAsync(ApplicationType.Client);

            // 문자열을 Uri 객체로 변환
            var uri = new Uri(serverUrl);
            using var client = DiscoveryClient.Create(uri, EndpointConfiguration.Create(config));
            var endpoints = await client.GetEndpointsAsync(null);

            // 테스트를 위해 '보안 없음(None)' 엔드포인트 선택
            var selectedEndpoint = endpoints.FirstOrDefault(e => e.SecurityMode == MessageSecurityMode.None);

            if (selectedEndpoint == null)
            {
                // None이 없으면 첫 번째 것 선택 (보통 SignAndEncrypt)
                selectedEndpoint = endpoints.First();
            }

            var endpoint = new ConfiguredEndpoint(null, selectedEndpoint, EndpointConfiguration.Create(config));

            // 세션 생성 및 연결
            using var session = await Session.Create(
                config,
                endpoint,
                false, // updateBeforeConnect
                false, // checkDomain
                "SimpleSession",
                60000,
                new UserIdentity(new AnonymousIdentityToken()),
                []
            );
            // 데이터 읽기 (서버에서 정의한 ns=2;s=HelloWorld)
            // 네임스페이스 인덱스는 서버 로직에 따라 다를 수 있으므로 NodeId 문자열("ns=2;s=HelloWorld")을 직접 쓸 수도 있습니다.
            ReadValueId nodeToRead = new()
            {
                NodeId = new NodeId("HelloWorld", 2),
                AttributeId = Attributes.Value
            };

            DataValue result = await session.ReadValueAsync(nodeToRead.NodeId);

            // 결과 출력
            txtResult.Text = $"서버 응답: {result.Value}";
        }
        catch (Exception ex)
        {
            MessageBox.Show($"클라이언트 오류: {ex.Message}");
        }
    }
}
