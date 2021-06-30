#define _WINSOCK_DEPRECATED_NO_WARNINGS
#include <iostream>
#include <string>
#include <WinSock2.h>


using namespace std;

void ShowErrorMessage(string message)
{
	cout << "[���� �߻�]: " << message << endl;
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

	if (WSAStartup(MAKEWORD(2, 2), &wsaData) != 0) // winsock �ʱ�ȭ
		ShowErrorMessage("WSAStartup()");

	// ���� ���� - TCP
	clientSocket = socket(PF_INET, SOCK_STREAM, 0); 

	if (clientSocket == INVALID_SOCKET)
		ShowErrorMessage("socket()");

	memset(&serverAddress, 0, sizeof(serverAddress));
	serverAddress.sin_family = AF_INET;
	serverAddress.sin_addr.s_addr = inet_addr("127.0.0.1");  // ���ڿ� ip�� ��Ʈ��ũ ����Ʈ ��������
	serverAddress.sin_port = htons(serverPort);  // 2����Ʈ ������ ��Ʈ��ũ ����Ʈ ��������

	// ������ ���� �õ�
	if (connect(clientSocket, (SOCKADDR*)&serverAddress, sizeof(serverAddress)) == SOCKET_ERROR)
		ShowErrorMessage("connect()");
	cout << "[���� ����] connect()" << endl;
	
	// �ݺ������� ������ �޽����� �����ϰ� ���� �޽����� ���� �ޱ�
	while (true)
	{
		cout << "[�޽��� ����]: ";
		getline(cin, sent);
		if (sent == "") continue;

		send(clientSocket, sent.c_str(), sent.length(), 0);  // c_str -> string�� c��Ÿ���� ���ڿ��� �ٲٱ� ���Ͽ�
		int length = recv(clientSocket, received, sizeof(received), 0);
		received[length] = '\n';
		if (strcmp(received, "[exit]") == 0)
		{
			cout << "[���� ����]" << endl;
			break;
		}
		cout << "[���� �޽���]: " << received << endl;
	}

	closesocket(clientSocket);
	WSACleanup();
	system("pause");

	return 0;
}