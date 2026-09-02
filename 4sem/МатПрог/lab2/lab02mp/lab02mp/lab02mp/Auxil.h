#pragma once
#include <time.h>

namespace auxil
{
    void start();                // инициализация генератора случайных чисел
    int iget(int a, int b);      // случайное целое в интервале [a, b]
    double dget(double a, double b); // случайное вещественное в интервале [a, b]
}