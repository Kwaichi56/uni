#include "pch.h"
#include "Auxil.h"
#include <iostream>
#include <ctime>
#include <locale>

using namespace std;

#define CYCLE 1000000 

int main() {
    setlocale(LC_ALL, "rus");
    double av1 = 0, av2 = 0;
    clock_t t1, t2;

    auxil::start();
    t1 = clock(); 

    for (int i = 0; i < CYCLE; i++) {
        av1 += (double)auxil::iget(-100, 100); // Сумма для int 
        av2 += auxil::dget(-100, 100);         // Сумма для double 
    }

    t2 = clock(); 

    cout << "Количество циклов:         " << CYCLE << endl;
    cout << "Среднее значение (int):    " << av1 / CYCLE << endl;
    cout << "Среднее значение (double): " << av2 / CYCLE << endl;
    cout << "Продолжительность (у.е):   " << (t2 - t1) << endl;
    cout << "Продолжительность (сек):   " << ((double)(t2 - t1)) / CLOCKS_PER_SEC << endl;

    system("pause");
    return 0;
}