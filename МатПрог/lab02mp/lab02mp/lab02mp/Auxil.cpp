
#include "Auxil.h"
#include <stdlib.h>

namespace auxil
{
    void start()
    {
        srand((unsigned)time(NULL));
    }

    int iget(int a, int b)
    {
        return rand() % (b - a + 1) + a;
    }

    double dget(double a, double b)
    {
        return (double)rand() / RAND_MAX * (b - a) + a;
    }
}