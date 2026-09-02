#pragma once
#define INF 0x7fffffff  // бесконечность

// Решение задачи коммивояжера полным перебором
int salesman(
    int n,              // [in] количество городов
    const int* d,       // [in] массив [n*n] расстояний
    int* r              // [out] массив [n] оптимальный маршрут
);