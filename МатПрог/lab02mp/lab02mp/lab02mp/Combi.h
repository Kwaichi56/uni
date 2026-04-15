#pragma once
namespace combi
{
    // Генератор множества всех подмножеств
    struct subset
    {
        short n;                // количество элементов исходного множества < 64
        short sn;               // количество элементов текущего подмножества
        short* sset;            // массив индексов текущего подмножества
        unsigned __int64 mask;   // битовая маска

        subset(short n = 1);     // конструктор
        short getfirst();        // сформировать первый массив индексов
        short getnext();         // ++маска и сформировать массив индексов
        short ntx(short i);      // получить i-й элемент массива индексов
        unsigned __int64 count(); // общее количество подмножеств
        void reset();            // сбросить генератор
    };

    // Генератор сочетаний
    struct xcombination
    {
        short n;                 // количество элементов исходного множества
        short m;                 // количество элементов в сочетаниях
        short* sset;             // массив индексов текущего сочетания
        unsigned __int64 nc;      // номер сочетания

        xcombination(short n = 1, short m = 1);
        void reset();
        short getfirst();
        short getnext();
        short ntx(short i);
        unsigned __int64 count() const;
    };

    // Генератор перестановок (алгоритм Джонсона-Троттера)
    struct permutation
    {
        const static bool L = true;   // левая стрелка
        const static bool R = false;  // правая стрелка

        short n;                       // количество элементов
        short* sset;                   // массив индексов текущей перестановки
        bool* dart;                     // массив стрелок
        unsigned __int64 np;            // номер перестановки

        permutation(short n = 1);
        void reset();
        __int64 getfirst();
        __int64 getnext();
        short ntx(short i);
        unsigned __int64 count() const;
    };

    // Генератор размещений
    struct accomodation
    {
        short n;                       // количество элементов исходного множества
        short m;                       // количество элементов в размещении
        short* sset;                    // массив индексов текущего размещения
        xcombination* cgen;             // указатель на генератор сочетаний
        permutation* pgen;              // указатель на генератор перестановок
        unsigned __int64 na;             // номер размещения

        accomodation(short n = 1, short m = 1);
        void reset();
        short getfirst();
        short getnext();
        short ntx(short i);
        unsigned __int64 count() const;
    };
}