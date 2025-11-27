# OpcUaSampleGui

OPC UA 샘플 GUI 프로그램입니다.

# 다음은 OPC UA에 대한 설명입니다.

* OPC UA (Open Platform Communication Unified Architecture)란 무엇인가?

  - OPC UA는 OPC의 후속 버전 통신 프로토콜

  - OPC Classic, 즉 기존의 OPC-DA는 마이크로소프트 윈도우에서 제공하는 프로토콜로 OLE, DCOM 기술을 사용하기 때문에 윈도우 플랫폼에서만 사용할 수 있다는 단점이 있음

  - OPC 서버가 다양한 벤더의 PLC 통신 정보를 통합하고 OPC 클라이언트(관제용 PC)가 일관된 인터페이스로 통신할 수 있게 함

  - OPC Classic에서 제공하는 서비스 (서버 기능)

    * DA (Data Access): 현재의 PLC 값을 가져올 수 있음

    * AE (Alarms & Events): 알람 발생시 이벤트로 클라이언트에게 알려줌

    * HDA (Historical Data Access): 현재의 PLC 값들을 OPC 서버에 취합하여 히스토리 데이터에 접근할 수 있게 해줌

  - OPC UA의 차이점

    * 기존 서버들의 통합 (DA, AE, HDA가 통합됨)

    * 보안 강화

    * 크로스 플랫폼 지원 (윈도우 비-종속)

* OPC 통신의 주체 및 구축 순서 (4가지)

  - OPC UA 서버 구축 순서
  
    * 애플리케이션 이름, config 파일 이름 지정

    * Config 파일 로드 및 인증서 확인/생성

    * 노드 관리자 생성 및 주소 공간 정의

    * UAServer 객체 생성 및 노드 관리자 연결

    * 서버 시작 (엔드포인트 활성화)

    * 종료 전까지 클라이언트 요청 처리 및 데이터 업데이트 대기

  - OPC UA 클라이언트 구축 순서

    * Config 파일 로드 및 인증서 확인

    * 서버 엔드포인트 URL 지정

    * UA 클라이언트 객체 생성 및 세션 연결 시도

    * 사용자 인증 시도

    * Subscription 생성 및 MonitoredItems 등록

    * DataChange 알림 대기 및 요청

  - Publisher 구축 순서 (서버에 종속되어 있으며 서버의 주소 공간의 데이터를 Publish함)
  
    * DataSet 생성 및 필드 정의

    * Transport 프로토콜 지정 및 URL 정의 (예: UDP, MQTT) - URL 예시: opc.udp://239.0.0.1:4840 또는 mqtt://localhost:1883

    * Config 파일 생성 (UADP 또는 JSON 인코딩 선택)

    * Publisher 애플리케이션 시작 (이후부터 강제 종료 전까지 계속 대기)

  - Subscriber 구축 순서 (정보를 취득하는 노드)
  
    * DataSetReader 생성 및 DataSet 매핑

    * 수신 이벤트 핸들러 등록

    * Subscriber 애플리케이션 시작

    * 프로그램 종료할 때까지 메시지 수신 및 이벤트 처리

* OPC 통신의 주체별 역할

  - 서버(필수): 각각의 장비들의 정보를 취합하거나 명령할 수 있도록 통합 관리하며 클라이언트와의 양방향 통신을 통해 보안 연결, 원격 명령, 구독 신청 등을 처리함

  - 클라이언트(옵션): 서버로부터 실시간 성이 높은 데이터를 읽거나 원격 명령을 내리거나 구독 서비스를 신청하는 역할을 함 (원격 명령이 없으면 없어도 됨)

  - Publisher(옵션): 데이터를 생성(발행)하는 역할만 함 (서버의 확장 모듈 역할)

  - Subscriber(옵션): 데이터를 수신(구독)하는 역할만 함 (경량 클라이언트 역할)

* 참조

  - https://red-nose-cousin.tistory.com/2

  - https://github.com/OPCFoundation/UA-.NETStandard/

  - https://opcfoundation.github.io/UA-.NETStandard/
