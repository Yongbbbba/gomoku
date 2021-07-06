#ifndef GOMOKU_UTIL_H
#define GOMOKU_UTIL_H
using namespace std;
#include <vector>
#include <sstream>
class Util {
public:
	// [Put]123 -> 이런 형식의 패킷을 주고받을 때 방 이름을 쉽게 파싱하기 위하여 getTokens사용
	vector<string> getTokens(string input, char delimiter);
};
#endif