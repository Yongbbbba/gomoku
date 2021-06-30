#define _WINSOCK_DEPRECATED_NO_WARNINGS
#include <iostream>
#include <string>
#include <WinSock2.h>


using namespace std;

void ShowErrorMessage(string message)
{
	cout << "[오류 발생]: " << message << endl;
	system("pause");
	exit(1);
}

int main()
{
	WSADATA wsaData;
	SOCKET clientSocket;
	SOCKADDR_IN serverAddress;

	int serverPort = 9876;
	char received[1024];
	string sent;

	if (WSAStartup(MAKEWORD(2, 2), &wsaData) != 0) // winsock 초기화
		ShowErrorMessage("WSAStartup()");

	// 소켓 생성 - TCP
	clientSocket = socket(PF_INET, SOCK_STREAM, 0); 

	if (clientSocket == INVALID_SOCKET)
		ShowErrorMessage("socket()");

	memset(&serverAddress, 0, sizeof(serverAddress));
	serverAddress.sin_family = AF_INET;
	serverAddress.sin_addr.s_addr = inet_addr("127.0.0.1");  // 문자열 ip를 네트워크 바이트 형식으로
	serverAddress.sin_port = htons(serverPort);  // 2바이트 정수를 네트워크 바이트 형식으로

	// 서버에 연결 시도
	if (connect(clientSocket, (SOCKADDR*)&serverAddress, sizeof(serverAddress)) == SOCKET_ERROR)
		ShowErrorMessage("connect()");
	cout << "[현재 상태] connect()" << endl;
	
	// 반복적으로 서버로 메시지를 전송하고 에코 메시지를 전달 받기
	while (true)
	{
		cout << "[메시지 전송]: ";
		getline(cin, sent);
		if (sent == "") continue;

		send(clientSocket, sent.c_str(), sent.length(), 0);  // c_str -> string을 c스타일의 문자열로 바꾸기 위하여
		int length = recv(clientSocket, received, sizeof(received), 0);
		received[length] = '\n';
		if (strcmp(received, "[exit]") == 0)
		{
			cout << "[서버 종료]" << endl;
			break;
		}
		cout << "[서버 메시지]: " << received << endl;
	}

	closesocket(clientSocket);
	WSACleanup();
	system("pause");

	return 0;
}