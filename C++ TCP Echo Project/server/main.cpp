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
	SOCKET serverSocket, clientSocket;
	SOCKADDR_IN serverAddress, clientAddress;

	int serverPort = 9876;
	char received[1024];  // recvBuff

	// winsock 초기화. 2.2 버전 사용
	if (WSAStartup(MAKEWORD(2, 2), &wsaData) != 0)  // 성공하면 0을 반환
	{
		ShowErrorMessage("WSAStartup()");
	}

	serverSocket = socket(PF_INET, SOCK_STREAM, 0);  // TCP 소켓 생성, IPv4

	if (serverSocket == INVALID_SOCKET)
		ShowErrorMessage("socket()");

	memset(&serverAddress, 0, sizeof(serverAddress));
	serverAddress.sin_family = AF_INET;
	serverAddress.sin_addr.s_addr = htonl(INADDR_ANY);  // 4바이트 정수를 네트워크 바이트 형식으로 변환
	serverAddress.sin_port = htons(serverPort);  // 2바이트 정수를 네트워크 바이트 형식으로

	// 바인딩
	if (bind(serverSocket, (SOCKADDR*)&serverAddress, sizeof(serverAddress)) == SOCKET_ERROR)
		ShowErrorMessage("bind()");
	cout << "[현재 상태] bind()" << endl;

	// listen 
	if (listen(serverSocket, 5) == SOCKET_ERROR)
		ShowErrorMessage("listen()");
	cout << "[현재 상태] listen()" << endl;

	// accept
	int sizeClientAddress = sizeof(clientAddress);
	clientSocket = accept(serverSocket, (SOCKADDR*)&clientAddress, &sizeClientAddress);
	cout << "[현재 상태] accept()" << endl;

	if (clientSocket == INVALID_SOCKET)
		ShowErrorMessage("accept()");
	
	while (true)
	{
		// 클라이언트의 메시지를 버퍼로 받아서 그대로 다시 전달
		int length = recv(clientSocket, received, sizeof(received), 0);
		received[length] = NULL;  // 마지막 문자를 NULL로 바꿔서 잘라내기
		cout << "[클라이언트 메시지]: " << received << endl;
		cout << "[메시지 전송]: " << received << endl;  // 에코~~~
		if (strcmp(received, "[exit]") == 0)  // strcmp -> 같다면 0을 반환함
		{
			send(clientSocket, received, sizeof(received) - 1, 0);  // 엔터를 쳐서 전송하기 때문에 끝에 개행문자는 제거하기 위해 -1 
			cout << "[서버 종료]" << endl;
			break;
		}
		send(clientSocket, received, sizeof(received) - 1, 0);

	}

	closesocket(clientSocket);
	closesocket(serverSocket);
	WSACleanup();
	system("pause");
	return 0;
	
}