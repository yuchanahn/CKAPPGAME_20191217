#include <iostream>

extern "C" {

	__declspec(dllexport) int __stdcall TEST() { return 0; }
}