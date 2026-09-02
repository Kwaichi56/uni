#include <iostream>
#include <vector>
#include <queue>
#include <algorithm>
using namespace std;

vector<vector<int>> g = {
    {1,2},
    {4},
    {3,5},
    {1,5,6},
    {6},
    {6},
    {}
};

void BFS(int s) {
    vector<int> used(7, 0);
    queue<int> q;
    q.push(s);
    used[s] = 1;
    while (!q.empty()) {
        int v = q.front();
        q.pop();
        cout << v << " ";
        for (int to : g[v])
            if (!used[to]) {
                used[to] = 1;
                q.push(to);
            }
    }
}

void DFS(int v, vector<int>& used, vector<int>& topo) {
    used[v] = 1;
    for (int to : g[v])
        if (!used[to])
            DFS(to, used, topo);
    topo.push_back(v);
}

int main() {

    cout << "BFS: ";
    BFS(0);
    cout << endl;

    vector<int> used(7, 0), topo;

    for (int i = 0; i < 7; i++) {
        if (!used[i]) {
            DFS(i, used, topo);
        }
    }

    reverse(topo.begin(), topo.end());

    cout << "DFS: ";
    for (int v : topo) {
        cout << v << " ";
    }
    cout << endl;

    return 0;
}