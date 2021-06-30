# boostAsio

## boost.asio의 기본적인 사용법

### IO Service

 Boost.Asio의 IO Service는 통신의 핵심적인 역할을 수행합니다. IO Service는 커널에서 발생한 입출력 이벤트를 디스패치 해주는 기능을 합니다. 결과적으로 커널에서 발생한 네트워크상의 접속 및 데이터 처리 이벤트를 담당합니다.

![image-20210701014832914](C:\Users\multicampus\AppData\Roaming\Typora\typora-user-images\image-20210701014832914.png)

### 종단점(EndPoint)

종단점이란 네트워크에 존재하는 특정한 컴퓨터에서 실행되고 있는 하나의 프로그램을 의미.

보통 종단점은 네트워크 통신 기능을 가지고 있는 모듈을 의미하며, IP 주소 및 포트 번호의 한 쌍 자체를 종단점이라고 말하기도 합니다.



### DNS (Domain Name Server)

도메인 주소를 IP 주소로 변환해주는 서버. boost asio에서는 도메인 주소를 통해 IP 주소를 알아내고, 종단점을 구하기 위한 목적으로 DNS 기능을 지원



### Query

Boost Asio의 Query 클래스는 도메인 주소와 프로토콜을 이용해 DNS 질의문을 생성



### Acceptor

서버 프로그램이 IO Service와 종단점을 이용해 클라이언트의 접속을 기다리도록 해줍니다.



### 수동 소켓(Passive Socket)

연결 수립 요청을 기다리는 소켓을 의미. 서버 프로그램의 Acceptor를 통해서 만들어질 수 있습니다.



### 능동 소켓(Active Socket)

능동 소켓(Active Socket)은 다른 컴퓨터로 데이터를 보내거나 받기 위한 연결 수립 과정에서 사용되는 능동적인 소켓을 의미



### 반복자(Iterator)

여러 개의 IP 주소가 존재할 때 종단점이 여러 개 존재할 수 있으므로, 개별적인 종단점에 접근하고자 할 때 사용할 수 있음



