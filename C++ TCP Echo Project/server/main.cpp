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
	SOCKET serverSocket, clientSocket;
	SOCKADDR_IN serverAddress, clientAddress;

	int serverPort = 9876;
	char received[1024];  // recvBuff

	// winsock �ʱ�ȭ. 2.2 ���� ���
	if (WSAStartup(MAKEWORD(2, 2), &wsaData) != 0)  // �����ϸ� 0�� ��ȯ
	{
		ShowErrorMessage("WSAStartup()");
	}

	serverSocket = socket(PF_INET, SOCK_STREAM, 0);  // TCP ���� ����, IPv4

	if (serverSocket == INVALID_SOCKET)
		ShowErrorMessage("socket()");

	memset(&serverAddress, 0, sizeof(serverAddress));
	serverAddress.sin_family = AF_INET;
	serverAddress.sin_addr.s_addr = htonl(INADDR_ANY);  // 4����Ʈ ������ ��Ʈ��ũ ����Ʈ �������� ��ȯ
	serverAddress.sin_port = htons(serverPort);  // 2����Ʈ ������ ��Ʈ��ũ ����Ʈ ��������

	// ���ε�
	if (bind(serverSocket, (SOCKADDR*)&serverAddress, sizeof(serverAddress)) == SOCKET_ERROR)
		ShowErrorMessage("bind()");
	cout << "[���� ����] bind()" << endl;

	// listen 
	if (listen(serverSocket, 5) == SOCKET_ERROR)
		ShowErrorMessage("listen()");
	cout << "[���� ����] listen()" << endl;

	// accept
	int sizeClientAddress = sizeof(clientAddress);
	clientSocket = accept(serverSocket, (SOCKADDR*)&clientAddress, &sizeClientAddress);
	cout << "[���� ����] accept()" << endl;

	if (clientSocket == INVALID_SOCKET)
		ShowErrorMessage("accept()");
	
	while (true)
	{
		// Ŭ���̾�Ʈ�� �޽����� ���۷� �޾Ƽ� �״�� �ٽ� ����
		int length = recv(clientSocket, received, sizeof(received), 0);
		received[length] = NULL;  // ������ ���ڸ� NULL�� �ٲ㼭 �߶󳻱�
		cout << "[Ŭ���̾�Ʈ �޽���]: " << received << endl;
		cout << "[�޽��� ����]: " << received << endl;  // ����~~~
		if (strcmp(received, "[exit]") == 0)  // strcmp -> ���ٸ� 0�� ��ȯ��
		{
			send(clientSocket, received, sizeof(received) - 1, 0);  // ���͸� �ļ� �����ϱ� ������ ���� ���๮�ڴ� �����ϱ� ���� -1 
			cout << "[���� ����]" << endl;
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