//#include "Combi.h"
//#include <iostream>
//#include <iomanip>
//#include <vector>
//#include <limits>
//
//using namespace std;
//
//const int INF = numeric_limits<int>::max() / 2; // "бесконечность"
//
//// Исходная матрица расстояний для n=5
//const int N = 5;
//int D[N][N] = {
//    {INF, 5,   24,  8,   17},
//    {6,   INF, 17,  35,  63},
//    {12,  10,  INF, 72,  42},
//    {23,  50,  15,  INF, 10},
//    {80,  37,  45,  17,  INF}
//};
//
//int main() {
//    setlocale(LC_ALL, "Russian");
//    cout << "=== Задача коммивояжёра (полный перебор) ===\n";
//    cout << "Матрица расстояний (n = " << N << "):\n";
//    for (int i = 0; i < N; i++) {
//        for (int j = 0; j < N; j++) {
//            if (i == j) cout << setw(4) << "?";
//            else cout << setw(4) << D[i][j];
//        }
//        cout << endl;
//    }
//
//    // Используем генератор перестановок для городов 1..N-1 (0-индексация)
//    combi::permutation perm(N - 1); // перестановка городов 1,2,3,4
//    int min_cost = INF;
//    vector<int> best_route;
//
//    cout << "\nПеребор всех маршрутов:\n";
//    int count = 0;
//    do {
//        // Формируем маршрут: 0 -> p[0] -> p[1] -> ... -> p[N-2] -> 0
//        int cost = D[0][perm.ntx(0) + 1]; // 1-й переход (город 0 -> первый в перестановке)
//        for (int i = 0; i < N - 2; i++) {
//            cost += D[perm.ntx(i) + 1][perm.ntx(i + 1) + 1];
//        }
//        cost += D[perm.ntx(N - 2) + 1][0]; // возврат в начало
//
//        // Вывод маршрута
//        cout << "Маршрут: 1";
//        for (int i = 0; i < N - 1; i++) {
//            cout << " -> " << (perm.ntx(i) + 1);
//        }
//        cout << " -> 1, длина = " << cost << endl;
//
//        if (cost < min_cost) {
//            min_cost = cost;
//            best_route.clear();
//            best_route.push_back(0);
//            for (int i = 0; i < N - 1; i++) best_route.push_back(perm.ntx(i) + 1);
//            best_route.push_back(0);
//        }
//        count++;
//    } while (perm.getnext() > 0);
//
//    cout << "\n========================================\n";
//    cout << "Всего рассмотрено маршрутов: " << count << endl;
//    cout << "Оптимальный маршрут: ";
//    for (size_t i = 0; i < best_route.size(); i++) {
//        if (i > 0) cout << " -> ";
//        cout << (best_route[i] + 1);
//    }
//    cout << "\nМинимальная длина: " << min_cost << endl;
//
//    return 0;
//}