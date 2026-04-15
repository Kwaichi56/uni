#include "pch.h"
#include <iostream>
#include <ctime>
#include <iomanip>
using namespace std;

unsigned long long fibonacci(int n) {
    if (n <= 1) return n;
    return fibonacci(n - 1) + fibonacci(n - 2);
}

int main() {
    setlocale(LC_ALL, "rus");

    int cycles[] = { 1000, 5000, 10000, 50000, 100000, 500000, 1000000 };
    cout << "Öèêëû\tÂðåìÿ (ñåê)" << endl;
    for (int c : cycles) {
        clock_t t1 = clock();
        double dummy = 0;
        for (int i = 0; i < c; i++) dummy += (double)rand();
        clock_t t2 = clock();
        cout << c << "\t" << (double)(t2 - t1) / CLOCKS_PER_SEC << endl;
    }

    cout << "\nÇÀÄÀÍÈÅ 3 (Ðåêóðñèÿ)\n";
    cout << "n\tÂðåìÿ (ñåê)" << endl;

    for (int n = 40; n <= 45; n++) {
        clock_t t1 = clock();
        fibonacci(n);
        clock_t t2 = clock();
        cout << n << "\t" << (double)(t2 - t1) / CLOCKS_PER_SEC << endl;
    }

    system("pause");
    return 0;
}