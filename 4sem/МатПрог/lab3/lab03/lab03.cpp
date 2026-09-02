#include <iostream>
#include <vector>
#include <numeric>
#include <algorithm>
#include <iomanip>

using namespace std;

const int INF = 1e9; // Бесконечность

int main() {
    setlocale(LC_ALL, "rus");

    int n = 9; // Параметр из условия лабораторной
    const int N = 5; // Количество городов

    // Матрица расстояний по условию:
    // Город 1: INF, n, 19+n, 3+n, 22-n
    // Город 2: 1+n, INF, 12+n, 40-n, 68-n
    // Город 3: 7+n, 2*n, INF, 72, 37+n
    // Город 4: 18+n, 55-n, 3*n, INF, 2*n
    // Город 5: 85-n, 32+n, 45, 12+n, INF

    int dist[N][N] = {
        {INF, n, 19 + n, 3 + n, 22 - n},
        {1 + n, INF, 12 + n, 40 - n, 68 - n},
        {7 + n, 2 * n, INF, 72, 37 + n},
        {18 + n, 55 - n, 3 * n, INF, 2 * n},
        {85 - n, 32 + n, 45, 12 + n, INF}
    };

    cout << "--- Матрица расстояний (n = " << n << ") ---" << endl;
    for (int i = 0; i < N; i++) {
        for (int j = 0; j < N; j++) {
            if (dist[i][j] >= INF / 2) cout << setw(4) << "INF";
            else cout << setw(4) << dist[i][j];
        }
        cout << endl;
    }
    cout << "-----------------------------------" << endl << endl;

    // Города нумеруются 0, 1, 2, 3, 4 (что соответствует 1, 2, 3, 4, 5)
    // Стартуем всегда из Города 1 (индекс 0), перебираем остальные
    vector<int> cities = { 1, 2, 3, 4 };

    int min_cost = INF;
    vector<int> best_path;
    int count = 1;

    cout << "--- Генерация маршрутов ---" << endl;
    do {
        int current_cost = 0;
        int current_city = 0; // Начинаем с Города 1 (индекс 0)
        bool possible = true;

        // Считаем стоимость пути через перестановку
        for (int i = 0; i < cities.size(); i++) {
            int next_city = cities[i];
            if (dist[current_city][next_city] >= INF / 2) {
                possible = false;
                break;
            }
            current_cost += dist[current_city][next_city];
            current_city = next_city;
        }

        // Замыкаем маршрут (возврат в Город 1)
        if (possible) {
            if (dist[current_city][0] >= INF / 2) {
                possible = false;
            }
            else {
                current_cost += dist[current_city][0];
            }
        }

        // Вывод маршрута
        if (possible) {
            cout << count++ << ": 1 -> ";
            for (int c : cities) cout << (c + 1) << " -> ";
            cout << "1 (Длина: " << current_cost << ")" << endl;

            if (current_cost < min_cost) {
                min_cost = current_cost;
                best_path = cities;
            }
        }
    } while (next_permutation(cities.begin(), cities.end()));

    cout << "\n===================================================" << endl;
    cout << "ОПТИМАЛЬНЫЙ МАРШРУТ: 1 -> ";
    for (int c : best_path) cout << (c + 1) << " -> ";
    cout << "1" << endl;
    cout << "МИНИМАЛЬНАЯ ДЛИНА: " << min_cost << endl;
    cout << "===================================================" << endl;

    return 0;
}