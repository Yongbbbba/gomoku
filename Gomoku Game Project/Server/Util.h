#ifndef GOMOKU_UTIL_H
#define GOMOKU_UTIL_H
using namespace std;
#include <vector>
#include <sstream>
class Util {
public:
	// [Put]123 -> �̷� ������ ��Ŷ�� �ְ���� �� �� �̸��� ���� �Ľ��ϱ� ���Ͽ� getTokens���
	vector<string> getTokens(string input, char delimiter);
};
#endif