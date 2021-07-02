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



## 동기적 TCP 통신 예제

서버와 연결하면 서버가 현재 시간을 클라이언트에 보내주고 연결이 끊기는 소켓 예제

### client

```c++
#include <iostream>
#include <boost/array.hpp>
#include <boost/asio.hpp>

using boost::asio::ip::tcp;
using namespace std;

int main()
{
	try
	{

		// 기본적으로 Boost Asio 프로그램은 하나의 IO Service 객체를 가집니다.
		boost::asio::io_service io_service;
		// 도메인 이름을 TCP 종단점으로 바뀌기 위해 Resolver를 사용
		tcp::resolver resolver(io_service);
		// 서버로는 로컬 서버, 서비스는 Daytime 프로토콜을 적어주기
		tcp::resolver::query query("localhost", "daytime");
		// DNS를 거쳐 IP주소 및 포트 번호를 얻어오기
		tcp::resolver::iterator endpoint_iterator = resolver.resolve(query);
		// 소켓 객체를 초기화하여 서버에 연결
		tcp::socket socket(io_service);
		boost::asio::connect(socket, endpoint_iterator);
		while (true) 
		{
			// 버퍼 및 오류 처리 변수를 선언
			boost::array<char, 128> buf;
			boost::system::error_code error;
			//버퍼를 이용해 서버로부터 데이터를 받아오기
			size_t len = socket.read_some(boost::asio::buffer(buf), error);
			if (error == boost::asio::error::eof) break;
			else if (error)
				throw boost::system::system_error(error);
			//버퍼에 담긴 데이터를 화면에 출력
			cout.write(buf.data(), len);
		}

	}
	catch (exception& e)
	{
		cerr << e.what() << endl;
	}

}

```



### server

```c++
#define _CRT_SECURE_NO_WARNINGS
#include <ctime>
#include <iostream>
#include <string>
#include <boost/asio.hpp>

using boost::asio::ip::tcp;
using namespace std;


// 서버 컴퓨터의 날짜 및 시간 정보를 반환 
string make_daytime_string()
{
	time_t now = time(0);
	return ctime(&now);
}

int main()
{
	try {
		// 기본적으로 Boost Asio 프로그램은 하나의 IO Service 객체를 가진다.
		boost::asio::io_service io_service;
		// 80번: HTTP 프로토콜 13번: DAYTIME 프로토콜
		// TCP 프로토콜의 13번 포트로 연결을 받는 수동 소켓을 생성
		// winsock2 보다 간결. bind와 listen을 한 번에
		tcp::acceptor acceptor(io_service, tcp::endpoint(tcp::v4(), 13));
		// 모든 클라이언트에 대해 무한정 반복 수행
		while (true)
		{
			// 클라이언트 소켓 객체를 생성해 연결을 기다립니다.
			tcp::socket socket(io_service);
			acceptor.accept(socket);
			// 연결이 완료되면 해당 클라이언트에게 보낼 메시지를 생성
			string message = make_daytime_string();
			// 해당 클라이언트에게 메시지를 담아 전송
			boost::system::error_code ignored_error;
			boost::asio::write(socket, boost::asio::buffer(message), ignored_error);
		}
	}
	catch (exception& e) {
		cerr << e.what() << endl;
	}
	return 0;
}

```

