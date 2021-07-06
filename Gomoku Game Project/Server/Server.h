#ifndef GOMOKU_SERVER_H
#define GOMOKU_SERVER_H
#define _CRT_SECURE_NO_WARNINGS
#pragma comment (lib, "ws2_32.lib")
#include <iostream>
using namespace std;
#include <Winsock.h>
#include <vector>
#include "Util.h"
#include "Client.h"

static class Server {
private:
	static SOCKET serverSocket;
	static WSAData wsaData;
	static SOCKADDR_IN serverAddress;
	static int nextID;  // 다음에 접속할 클라이언트의 ID, 하나씩 증가시킴
	static vector<Client*> connections;  // 접속한 클라이언트 관리 -> map으로 바꾸는 것이 좋을 듯. find, delete 등을 위하여
	static Util util;
public:
	static void start();
	static int clientCountInRoom(int roomID);
	static void playClient(int roomID);
	static void exitClient(int roomID);
	static void putClient(int roomID, int x, int y);
	static void fullClient(Client* client);
	static void enterClient(Client* client);
	static void ServerThread(Client* client);
};
#endif