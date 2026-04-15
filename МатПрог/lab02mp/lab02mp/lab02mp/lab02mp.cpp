#include <iostream>
#include <iomanip>
#include <time.h>
#include "Combi.h"
#include "Salesman.h"
#include "Auxil.h"

#define SPACE(n) std::setw(n) << " "
#define MAX_CITIES 12

using namespace std;

// Демонстрация генератора подмножеств (Задание 1)
void demonstrateSubset() {
    cout << "\n================================================================\n";
    cout << "ЗАДАНИЕ 1: Генератор подмножеств\n";
    cout << "================================================================\n";

    char AA[][2] = { "A", "B", "C", "D" };
    int n = sizeof(AA) / 2;

    cout << "Исходное множество: { ";
    for (int i = 0; i < n; i++)
        cout << AA[i] << (i < n - 1 ? ", " : " ");
    cout << "}\n";

    cout << "Генерация всех подмножеств:\n";
    combi::subset s1(n);
    int ns = s1.getfirst();

    while (ns >= 0) {
        cout << "{ ";
        for (int i = 0; i < ns; i++)
            cout << AA[s1.ntx(i)] << (i < ns - 1 ? ", " : " ");
        cout << "}\n";
        ns = s1.getnext();
    }

    cout << "Всего подмножеств: " << s1.count() << " (2^" << n << " = " << (1 << n) << ")\n";
}

// Демонстрация генератора сочетаний (Задание 2)
void demonstrateCombination() {
    cout << "\n================================================================\n";
    cout << "ЗАДАНИЕ 2: Генератор сочетаний\n";
    cout << "================================================================\n";

    char AA[][2] = { "A", "B", "C", "D" };
    int n = sizeof(AA) / 2;
    int m = 3;

    cout << "Исходное множество: { ";
    for (int i = 0; i < n; i++)
        cout << AA[i] << (i < n - 1 ? ", " : " ");
    cout << "}\n";

    cout << "Генерация сочетаний из " << n << " по " << m << ":\n";
    combi::xcombination xc(n, m);
    int nc = xc.getfirst();

    while (nc >= 0) {
        cout << "#" << setw(2) << xc.nc << ": { ";
        for (int i = 0; i < nc; i++)
            cout << AA[xc.ntx(i)] << (i < nc - 1 ? ", " : " ");
        cout << "}\n";
        nc = xc.getnext();
    }

    cout << "Всего сочетаний: " << xc.count() << " (C(" << n << "," << m << ") = "
        << xc.count() << ")\n";
}

// Демонстрация генератора перестановок (Задание 3)
void demonstratePermutation() {
    cout << "\n================================================================\n";
    cout << "ЗАДАНИЕ 3: Генератор перестановок (алгоритм Джонсона-Троттера)\n";
    cout << "================================================================\n";

    char AA[][2] = { "A", "B", "C", "D" };
    int n = sizeof(AA) / 2;

    cout << "Исходное множество: { ";
    for (int i = 0; i < n; i++)
        cout << AA[i] << (i < n - 1 ? ", " : " ");
    cout << "}\n";

    cout << "Генерация всех перестановок:\n";
    combi::permutation p(n);
    __int64 np = p.getfirst();

    while (np >= 0) {
        cout << "#" << setw(2) << p.np << ": [ ";
        for (int i = 0; i < n; i++)
            cout << AA[p.ntx(i)] << (i < n - 1 ? ", " : " ");
        cout << "]\n";
        np = p.getnext();
    }

    cout << "Всего перестановок: " << p.count() << " (" << n << "! = " << p.count() << ")\n";
}

// Демонстрация генератора размещений (Задание 4)
void demonstrateAccomodation() {
    cout << "\n================================================================\n";
    cout << "ЗАДАНИЕ 4: Генератор размещений\n";
    cout << "================================================================\n";

    char AA[][2] = { "A", "B", "C", "D" };
    int n = sizeof(AA) / 2;
    int m = 3;

    cout << "Исходное множество: { ";
    for (int i = 0; i < n; i++)
        cout << AA[i] << (i < n - 1 ? ", " : " ");
    cout << "}\n";

    cout << "Генерация размещений из " << n << " по " << m << ":\n";
    combi::accomodation acc(n, m);
    int na = acc.getfirst();

    while (na >= 0) {
        cout << "#" << setw(2) << acc.na << ": [ ";
        for (int i = 0; i < na; i++)
            cout << AA[acc.ntx(i)] << (i < na - 1 ? ", " : " ");
        cout << "]\n";
        na = acc.getnext();
    }

    cout << "Всего размещений: " << acc.count() << " (A(" << n << "," << m << ") = "
        << acc.count() << ")\n";
}

// Функция для генерации матрицы расстояний (Задание 5)
void generateDistanceMatrix(int* d, int n, int infCount) {
    for (int i = 0; i < n; i++) {
        for (int j = 0; j < n; j++) {
            if (i == j)
                d[i * n + j] = 0;
            else
                d[i * n + j] = auxil::iget(10, 300);
        }
    }

    for (int k = 0; k < infCount; k++) {
        int i, j;
        do {
            i = auxil::iget(0, n - 1);
            j = auxil::iget(0, n - 1);
        } while (i == j);

        d[i * n + j] = INF;
        d[j * n + i] = INF;
    }
}

// Функция для вывода матрицы расстояний
void printMatrix(const int* d, int n) {
    cout << "Матрица расстояний:\n";
    for (int i = 0; i < n; i++) {
        for (int j = 0; j < n; j++) {
            if (d[i * n + j] == INF)
                cout << setw(6) << "INF";
            else
                cout << setw(6) << d[i * n + j];
        }
        cout << endl;
    }
}

// Решение задачи коммивояжера (Задание 5)
void solveTravelingSalesman() {
    cout << "\n================================================================\n";
    cout << "ЗАДАНИЕ 5: Задача коммивояжера (10 городов)\n";
    cout << "================================================================\n";

    const int N5 = 10;
    int* d5 = new int[N5 * N5];
    int* r5 = new int[N5];

    generateDistanceMatrix(d5, N5, 3);
    printMatrix(d5, N5);

    clock_t t_start = clock();
    int minDistance = salesman(N5, d5, r5);
    clock_t t_end = clock();

    cout << "\nОПТИМАЛЬНЫЙ МАРШРУТ:\n";
    cout << "0";
    for (int i = 1; i < N5; i++)
        cout << " -> " << r5[i];
    cout << " -> 0\n";

    if (minDistance == INF)
        cout << "Маршрут не найден (все пути бесконечны)\n";
    else
        cout << "Длина маршрута: " << minDistance << " км\n";

    cout << "Время вычисления: " << (t_end - t_start) << " мс\n";

    delete[] d5;
    delete[] r5;
}

// Исследование временной зависимости (Задание 6)
void investigateTimeDependency() {
    cout << "\n================================================================\n";
    cout << "ЗАДАНИЕ 6: Исследование зависимости времени от количества городов\n";
    cout << "================================================================\n";
    cout << "Количество городов | Время (мс)\n";
    cout << "-------------------|-----------\n";

    int* d6 = new int[MAX_CITIES * MAX_CITIES];
    int* r6 = new int[MAX_CITIES];

    for (int cities = 6; cities <= MAX_CITIES; cities++) {
        for (int i = 0; i < cities; i++) {
            for (int j = 0; j < cities; j++) {
                if (i == j)
                    d6[i * cities + j] = 0;
                else
                    d6[i * cities + j] = auxil::iget(10, 300);
            }
        }

        for (int k = 0; k < 2; k++) {
            int i, j;
            do {
                i = auxil::iget(0, cities - 1);
                j = auxil::iget(0, cities - 1);
            } while (i == j);
            d6[i * cities + j] = INF;
            d6[j * cities + i] = INF;
        }

        clock_t t_start = clock();
        salesman(cities, d6, r6);
        clock_t t_end = clock();

        cout << setw(17) << cities << " | " << setw(9) << (t_end - t_start) << "\n";
    }

    delete[] d6;
    delete[] r6;
}

int main() {
    setlocale(LC_ALL, "rus");
    auxil::start();
    demonstrateSubset();
    demonstrateCombination();
    demonstratePermutation();
    demonstrateAccomodation();
    // Решение задачи коммивояжера (Задание 5)
    solveTravelingSalesman();

    // Исследование временной зависимости (Задание 6)
    investigateTimeDependency();

    cout << "\n\n";
    system("pause");
    return 0;
}