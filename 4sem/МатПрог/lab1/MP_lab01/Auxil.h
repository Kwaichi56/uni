#pragma once
#pragma once
#include <cstdlib>

namespace auxil {
    void start();                          // Старт генератора случайных чисел
    double dget(double rmin, double rmax); // Получить случайное double 
    int iget(int rmin, int rmax);          // Получить случайное int 
}