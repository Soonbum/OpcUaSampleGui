using Opc.Ua;
using Opc.Ua.Client;
using Opc.Ua.Configuration;
using System;
using System.Security.Policy;
using System.Threading;

namespace OpcUaSample.Gui.Client;

public partial class Client : Form
{
    private Session _session;
    private Subscription _subscription;
    private const string ServerUrl = "opc.tcp://localhost:62541/SimpleServer";

    public Client()
    {
        InitializeComponent();

        // 초기 버튼 상태 설정
        BtnRead.Enabled = false;
        BtnSubscribe.Enabled = false;
        BtnWrite.Enabled = false;
    }

    private async void BtnConnect_Click(object sender, EventArgs e)
    {
        try
        {
            Log("연결 시도 중...");

            // 설정 생성 (수동 설정)
            Opc.Ua.ApplicationConfiguration config = new()
            {
                ApplicationName = "SimpleClient",
                ApplicationUri = "urn:localhost:SimpleClient",
                ApplicationType = ApplicationType.Client,
                SecurityConfiguration = new SecurityConfiguration
                {
                    ApplicationCertificate = new CertificateIdentifier { StoreType = @"Directory", StorePath = @"%CommonApplicationData%\OPC Foundation\CertificateStores\MachineDefault", SubjectName = "SimpleClient" },
                    AutoAcceptUntrustedCertificates = true,
                    TrustedIssuerCertificates = new CertificateTrustList { StoreType = @"Directory", StorePath = @"%CommonApplicationData%\OPC Foundation\CertificateStores\UA Certificate Authorities" },
                    TrustedPeerCertificates = new CertificateTrustList { StoreType = @"Directory", StorePath = @"%CommonApplicationData%\OPC Foundation\CertificateStores\UA Applications" },
                    RejectedCertificateStore = new CertificateTrustList { StoreType = @"Directory", StorePath = @"%CommonApplicationData%\OPC Foundation\CertificateStores\RejectedCertificates" }
                },
                TransportQuotas = new TransportQuotas { OperationTimeout = 15000 },
                ClientConfiguration = new ClientConfiguration { DefaultSessionTimeout = 60000 }
            };

            await config.ValidateAsync(ApplicationType.Client);

            // 엔드포인트 찾기
            var uri = new Uri(ServerUrl);
            using var discoveryClient = DiscoveryClient.Create(uri, EndpointConfiguration.Create(config));
            var endpoints = await discoveryClient.GetEndpointsAsync(null);

            // '보안 없음' 우선 선택 (테스트용) --> 인증을 강화하려면 아래 코멘트 처리된 코드를 대신 사용할 것
            var selectedEndpoint = endpoints.FirstOrDefault(e => e.SecurityMode == MessageSecurityMode.None);
            if (selectedEndpoint == null) selectedEndpoint = endpoints.First();

            var endpoint = new ConfiguredEndpoint(null, selectedEndpoint, EndpointConfiguration.Create(config));

            //// 보안 모드가 SignAndEncrypt인 엔드포인트 탐색
            //var selectedEndpoint = endpoints.FirstOrDefault(e => e.SecurityMode == MessageSecurityMode.SignAndEncrypt);
            //if (selectedEndpoint == null) throw new Exception("서버에서 보안 연결(SignAndEncrypt)을 지원하지 않습니다.");

            //var endpoint = new ConfiguredEndpoint(null, selectedEndpoint, EndpointConfiguration.Create(config));

            // 세션 생성 및 연결
            _session = await Session.Create(
                config,
                endpoint,
                false, // updateBeforeConnect
                false, // checkDomain
                "SimpleSession",
                60000,
                new UserIdentity(new AnonymousIdentityToken()),
                []
            );

            Log($"서버 연결 성공! (SessionId: {_session.SessionId})");

            // 버튼 활성화
            BtnConnect.Enabled = false;
            BtnDisconnect.Enabled = true;
            BtnRead.Enabled = true;
            BtnSubscribe.Enabled = true;
            BtnWrite.Enabled = true;
        }
        catch (Exception ex)
        {
            Log($"[Error] 연결 실패: {ex.Message}");
        }
    }

    private void BtnDisconnect_Click(object sender, EventArgs e)
    {
        try
        {
            // 구독이 있다면 먼저 정리
            if (_subscription != null)
            {
                _subscription.Dispose();
                _subscription = null;
            }

            if (_session != null)
            {
                _session.Close();
                _session.Dispose();
                _session = null;
            }

            Log("서버와의 연결이 끊어졌습니다.");

            // 버튼 상태 복구
            BtnConnect.Enabled = true;
            BtnDisconnect.Enabled = false;
            BtnRead.Enabled = false;
            BtnSubscribe.Enabled = false;
            BtnUnsubscribe.Enabled = false;
            BtnWrite.Enabled = false;
        }
        catch (Exception ex)
        {
            Log($"[Error] 연결 해제 실패: {ex.Message}");
        }
    }

    private async void BtnRead_Click(object sender, EventArgs e)
    {
        if (_session == null || !_session.Connected) return;

        try
        {
            // HelloWorld 노드 읽기
            ReadValueId nodeToRead = new ReadValueId
            {
                NodeId = new NodeId("HelloWorld", 2), // MyNodeManager에서 정의한 ID
                AttributeId = Attributes.Value
            };

            DataValue result = await _session.ReadValueAsync(nodeToRead.NodeId);
            Log($"[Read] HelloWorld: {result.Value}");
        }
        catch (Exception ex)
        {
            Log($"[Error] 읽기 실패: {ex.Message}");
        }
    }

    private async void BtnSubscribe_Click(object sender, EventArgs e)
    {
        if (_session == null || !_session.Connected) return;

        try
        {
            if (_subscription != null)
            {
                Log("이미 구독 중입니다.");
                return;
            }

            // 구독 컨테이너 생성 (1초 간격)
            _subscription = new Subscription(_session.DefaultSubscription);
            _subscription.PublishingEnabled = true;
            _subscription.PublishingInterval = 1000;

            // 감시할 아이템(Temperature) 생성
            MonitoredItem item = new MonitoredItem(_subscription.DefaultItem);
            item.StartNodeId = new NodeId("3DPrinter/Temperature", 2); // 서버의 온도 노드 ID
            item.AttributeId = Attributes.Value;
            item.DisplayName = "Temperature";
            item.SamplingInterval = 1000;

            // 알림 이벤트 연결
            item.Notification += OnNotification;

            // 세션에 추가
            _subscription.AddItem(item);
            _session.AddSubscription(_subscription);

            // 서버에 구독 생성 요청
            await _subscription.CreateAsync();

            Log("[Subscribe] 온도 데이터 실시간 감시 시작...");
            BtnSubscribe.Enabled = false;
            BtnUnsubscribe.Enabled = true;
        }
        catch (Exception ex)
        {
            Log($"[Error] 구독 실패: {ex.Message}");
            _subscription = null;
        }
    }

    private async void BtnUnsubscribe_Click(object sender, EventArgs e)
    {
        if (_subscription == null) return;

        try
        {
            // 서버에서 구독 삭제
            await _subscription.DeleteAsync(true);
            _subscription.Dispose();
            _subscription = null;

            Log("[Unsubscribe] 구독이 해제되었습니다.");

            // 버튼 상태 전환
            BtnSubscribe.Enabled = true;
            BtnUnsubscribe.Enabled = false;
        }
        catch (Exception ex)
        {
            Log($"[Error] 구독 해제 실패: {ex.Message}");
        }
    }

    private async void BtnWrite_Click(object sender, EventArgs e)
    {

        if (_session == null || !_session.Connected) return;

        try
        {
            // IsActive 노드에 'true' 값 쓰기
            WriteValue valueToWrite = new WriteValue
            {
                NodeId = new NodeId("3DPrinter/IsActive", 2), // 경로 주의: 폴더/변수명
                AttributeId = Attributes.Value
            };

            valueToWrite.Value.Value = true;
            valueToWrite.Value.StatusCode = StatusCodes.Good;
            valueToWrite.Value.ServerTimestamp = DateTime.MinValue;
            valueToWrite.Value.SourceTimestamp = DateTime.MinValue;

            WriteValueCollection valuesToWrite = new WriteValueCollection { valueToWrite };

            // [수정된 부분] WriteAsync 호출 방식 변경
            // out 파라미터 제거 -> WriteResponse 객체로 반환받음
            WriteResponse response = await _session.WriteAsync(
                null,           // RequestHeader
                valuesToWrite,  // nodesToWrite
                System.Threading.CancellationToken.None // CancellationToken
            );

            // 결과 확인 (response 객체 안에 Results가 들어있음)
            StatusCodeCollection results = response.Results;
            DiagnosticInfoCollection diagnostics = response.DiagnosticInfos;

            if (StatusCode.IsGood(results[0]))
            {
                Log("[Write] 장비 가동 명령(IsActive=true) 전송 성공!");
            }
            else
            {
                Log($"[Error] 쓰기 실패: {results[0]}");
            }
        }
        catch (Exception ex)
        {
            Log($"[Error] 쓰기 예외: {ex.Message}");
        }
    }

    // 데이터 변경 시 호출되는 콜백 함수
    private void OnNotification(MonitoredItem item, MonitoredItemNotificationEventArgs e)
    {
        foreach (var value in item.DequeueValues())
        {
            // UI 스레드 안전하게 접근
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => UpdateLogWithTemp(item.DisplayName, value.Value)));
            }
            else
            {
                UpdateLogWithTemp(item.DisplayName, value.Value);
            }
        }
    }

    private void UpdateLogWithTemp(string name, object value)
    {
        Log($"[Event] {name} 변경됨: {value}");
    }

    // =============================================================
    // 유틸리티 및 종료 처리
    // =============================================================
    private void Log(string message)
    {
        if (TxtLog.InvokeRequired)
        {
            TxtLog.Invoke(new Action(() => Log(message)));
            return;
        }
        TxtLog.AppendText($"[{DateTime.Now:HH:mm:ss}] {message}\r\n");
    }

    private void OnFormClosing(object sender, FormClosingEventArgs e)
    {
        try
        {
            _session?.Close();
            _session?.Dispose();
        }
        catch { }
        base.OnFormClosing(e);
    }
}
