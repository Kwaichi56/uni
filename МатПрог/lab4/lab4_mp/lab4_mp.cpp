#include <iostream>
#include <vector>
#include <string>
#include <algorithm>
#include <random>
#include <chrono>
#include <fstream>
#include <iomanip>

using namespace std;

enum Direction { TOP, LEFT, LEFTTOP, NONE_DIR };

string generateRandomLatinString(int length)
{
    static const string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    random_device rd;
    mt19937 gen(rd());
    uniform_int_distribution<int> dist(0, (int)alphabet.size() - 1);

    string s;
    s.reserve(length);
    for (int i = 0; i < length; i++)
        s += alphabet[dist(gen)];
    return s;
}

int min3(int a, int b, int c)
{
    return min(a, min(b, c));
}

// -------------------- Левенштейн: рекурсия --------------------
int levenshteinRecursive(const string& x, const string& y, int lx, int ly)
{
    if (lx == 0) return ly;
    if (ly == 0) return lx;

    return min3(
        levenshteinRecursive(x, y, lx - 1, ly) + 1,
        levenshteinRecursive(x, y, lx, ly - 1) + 1,
        levenshteinRecursive(x, y, lx - 1, ly - 1) + (x[lx - 1] == y[ly - 1] ? 0 : 1)
    );
}

// -------------------- Левенштейн: ДП --------------------
int levenshteinDP(const string& x, const string& y, vector<vector<int>>& d)
{
    int lx = (int)x.size();
    int ly = (int)y.size();

    d.assign(lx + 1, vector<int>(ly + 1, 0));

    for (int i = 0; i <= lx; i++) d[i][0] = i;
    for (int j = 0; j <= ly; j++) d[0][j] = j;

    for (int i = 1; i <= lx; i++)
    {
        for (int j = 1; j <= ly; j++)
        {
            d[i][j] = min3(
                d[i - 1][j] + 1,
                d[i][j - 1] + 1,
                d[i - 1][j - 1] + (x[i - 1] == y[j - 1] ? 0 : 1)
            );
        }
    }

    return d[lx][ly];
}

void printLevenshteinTable(const string& x, const string& y, const vector<vector<int>>& d)
{
    cout << "\nТаблица Левенштейна:\n\n";
    cout << setw(4) << " ";
    cout << setw(4) << "#";
    for (char c : y) cout << setw(4) << c;
    cout << "\n";

    for (int i = 0; i <= (int)x.size(); i++)
    {
        if (i == 0) cout << setw(4) << "#";
        else cout << setw(4) << x[i - 1];

        for (int j = 0; j <= (int)y.size(); j++)
            cout << setw(4) << d[i][j];
        cout << "\n";
    }
}

// -------------------- LCS: рекурсия --------------------
int lcsRecursive(const string& x, const string& y, int lenx, int leny)
{
    if (lenx == 0 || leny == 0) return 0;

    if (x[lenx - 1] == y[leny - 1])
        return 1 + lcsRecursive(x, y, lenx - 1, leny - 1);

    return max(
        lcsRecursive(x, y, lenx - 1, leny),
        lcsRecursive(x, y, lenx, leny - 1)
    );
}

// -------------------- LCS: ДП --------------------
int lcsDP(const string& x, const string& y,
    vector<vector<int>>& C,
    vector<vector<Direction>>& B)
{
    int lenx = (int)x.size();
    int leny = (int)y.size();

    C.assign(lenx + 1, vector<int>(leny + 1, 0));
    B.assign(lenx + 1, vector<Direction>(leny + 1, NONE_DIR));

    for (int i = 1; i <= lenx; i++)
    {
        for (int j = 1; j <= leny; j++)
        {
            if (x[i - 1] == y[j - 1])
            {
                C[i][j] = C[i - 1][j - 1] + 1;
                B[i][j] = LEFTTOP;
            }
            else if (C[i - 1][j] >= C[i][j - 1])
            {
                C[i][j] = C[i - 1][j];
                B[i][j] = TOP;
            }
            else
            {
                C[i][j] = C[i][j - 1];
                B[i][j] = LEFT;
            }
        }
    }

    return C[lenx][leny];
}

void buildLCS(const string& x,
    const vector<vector<Direction>>& B,
    int i, int j,
    string& result)
{
    if (i == 0 || j == 0) return;

    if (B[i][j] == LEFTTOP)
    {
        buildLCS(x, B, i - 1, j - 1, result);
        result += x[i - 1];
    }
    else if (B[i][j] == TOP)
    {
        buildLCS(x, B, i - 1, j, result);
    }
    else
    {
        buildLCS(x, B, i, j - 1, result);
    }
}

void printLCSTables(const string& x, const string& y,
    const vector<vector<int>>& C,
    const vector<vector<Direction>>& B)
{
    cout << "\nМатрица C (длины LCS):\n\n";
    cout << setw(6) << " ";
    cout << setw(6) << "#";
    for (char c : y) cout << setw(6) << c;
    cout << "\n";

    for (int i = 0; i <= (int)x.size(); i++)
    {
        if (i == 0) cout << setw(6) << "#";
        else cout << setw(6) << x[i - 1];

        for (int j = 0; j <= (int)y.size(); j++)
            cout << setw(6) << C[i][j];
        cout << "\n";
    }

    cout << "\nМатрица B (направления):\n\n";
    cout << setw(10) << " ";
    cout << setw(10) << "#";
    for (char c : y) cout << setw(10) << c;
    cout << "\n";

    for (int i = 0; i <= (int)x.size(); i++)
    {
        if (i == 0) cout << setw(10) << "#";
        else cout << setw(10) << x[i - 1];

        for (int j = 0; j <= (int)y.size(); j++)
        {
            string cell = "-";
            if (i > 0 && j > 0)
            {
                if (B[i][j] == TOP) cell = "TOP";
                else if (B[i][j] == LEFT) cell = "LEFT";
                else if (B[i][j] == LEFTTOP) cell = "LEFTTOP";
            }
            cout << setw(10) << cell;
        }
        cout << "\n";
    }
}

template<typename Func>
double measureMs(Func f)
{
    auto t1 = chrono::high_resolution_clock::now();
    f();
    auto t2 = chrono::high_resolution_clock::now();
    chrono::duration<double, milli> diff = t2 - t1;
    return diff.count();
}

int main()
{
    setlocale(LC_ALL, "rus");

    // -------------------- Задание 1 --------------------
    string S1 = generateRandomLatinString(300);
    string S2 = generateRandomLatinString(200);

    cout << "ЗАДАНИЕ 1\n";
    cout << "Сгенерирована строка S1 длиной 300\n";
    cout << "Сгенерирована строка S2 длиной 200\n";
    cout << "Первые 50 символов S1: " << S1.substr(0, 50) << "\n";
    cout << "Первые 50 символов S2: " << S2.substr(0, 50) << "\n\n";

    // -------------------- Задание 2 --------------------
    cout << "ЗАДАНИЕ 2\n";
    string s1small = S1.substr(0, 10);
    string s2small = S2.substr(0, 10);

    vector<vector<int>> levTable;
    int levRec = levenshteinRecursive(s1small, s2small, (int)s1small.size(), (int)s2small.size());
    int levDp = levenshteinDP(s1small, s2small, levTable);

    cout << "Для первых 10 символов:\n";
    cout << "Рекурсивно: " << levRec << "\n";
    cout << "Динамическое программирование: " << levDp << "\n";
    printLevenshteinTable(s1small, s2small, levTable);

    // -------------------- Задание 3 --------------------
    cout << "\nЗАДАНИЕ 3\n";
    cout << "Сравнение времени Левенштейна:\n";
    cout << left << setw(8) << "n"
        << setw(20) << "Рекурсия, мс"
        << setw(20) << "ДП, мс" << "\n";

    ofstream foutLev("levenshtein_times.csv");
    foutLev << "n,recursion_ms,dp_ms\n";

    for (int n = 1; n <= 10; n++)
    {
        string a = S1.substr(0, n);
        string b = S2.substr(0, n);

        double tRec = measureMs([&]() {
            volatile int ans = levenshteinRecursive(a, b, (int)a.size(), (int)b.size());
            (void)ans;
            });

        double tDp = measureMs([&]() {
            vector<vector<int>> tmp;
            volatile int ans = levenshteinDP(a, b, tmp);
            (void)ans;
            });

        cout << left << setw(8) << n
            << setw(20) << tRec
            << setw(20) << tDp << "\n";

        foutLev << n << "," << tRec << "," << tDp << "\n";
    }
    foutLev.close();

    // -------------------- Задание 4 --------------------
    cout << "\nЗАДАНИЕ 4\n";
    string word1 = "гора";
    string word2 = "вор";

    vector<vector<int>> levVariantTable;
    int levVariant = levenshteinDP(word1, word2, levVariantTable);

    cout << "Слова варианта 9: " << word1 << " и " << word2 << "\n";
    cout << "Расстояние Левенштейна = " << levVariant << "\n";
    printLevenshteinTable(word1, word2, levVariantTable);

    // -------------------- Задание 5 --------------------
    cout << "\nЗАДАНИЕ 5\n";
    string X = "ABHCSUV";
    string Y = "KIBOSV";

    int lcsRec = lcsRecursive(X, Y, (int)X.size(), (int)Y.size());

    vector<vector<int>> C;
    vector<vector<Direction>> B;
    int lcsLen = lcsDP(X, Y, C, B);

    string lcsString;
    buildLCS(X, B, (int)X.size(), (int)Y.size(), lcsString);

    cout << "X = " << X << "\n";
    cout << "Y = " << Y << "\n";
    cout << "LCS рекурсивно = " << lcsRec << "\n";
    cout << "LCS динамически = " << lcsLen << "\n";
    cout << "Одна из LCS = " << lcsString << "\n";

    printLCSTables(X, Y, C, B);

    cout << "\nСравнение времени LCS:\n";
    cout << left << setw(8) << "n"
        << setw(20) << "Рекурсия, мс"
        << setw(20) << "ДП, мс" << "\n";

    ofstream foutLCS("lcs_times.csv");
    foutLCS << "n,recursion_ms,dp_ms\n";

    int maxN = min((int)X.size(), (int)Y.size());
    for (int n = 1; n <= maxN; n++)
    {
        string a = X.substr(0, n);
        string b = Y.substr(0, n);

        double tRec = measureMs([&]() {
            volatile int ans = lcsRecursive(a, b, (int)a.size(), (int)b.size());
            (void)ans;
            });

        double tDp = measureMs([&]() {
            vector<vector<int>> CC;
            vector<vector<Direction>> BB;
            volatile int ans = lcsDP(a, b, CC, BB);
            (void)ans;
            });

        cout << left << setw(8) << n
            << setw(20) << tRec
            << setw(20) << tDp << "\n";

        foutLCS << n << "," << tRec << "," << tDp << "\n";
    }
    foutLCS.close();

    return 0;
}