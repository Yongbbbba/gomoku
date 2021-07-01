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
