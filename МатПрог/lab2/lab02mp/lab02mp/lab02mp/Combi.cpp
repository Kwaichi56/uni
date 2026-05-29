//#include "Combi.h"
//#include <algorithm>
//
//namespace combi
//{
//    // подмножества
//    subset::subset(short n)
//    {
//        this->n = n;
//        this->sset = new short[n];
//        this->reset();
//    }
//
//    void subset::reset()
//    {
//        this->sn = 0;
//        this->mask = 0;
//    }
//
//    short subset::getfirst()
//    {
//        __int64 buf = this->mask;
//        this->sn = 0;
//        for (short i = 0; i < n; i++)
//        {
//            if (buf & 0x1) this->sset[this->sn++] = i;
//            buf >>= 1;
//        }
//        return this->sn;
//    }
//
//    short subset::getnext()
//    {
//        int rc = -1;
//        this->sn = 0;
//        if (++this->mask < this->count()) rc = getfirst();
//        return rc;
//    }
//
//    short subset::ntx(short i)
//    {
//        return this->sset[i];
//    }
//
//    unsigned __int64 subset::count()
//    {
//        return (unsigned __int64)(1 << this->n);
//    }
//
//    // сочетания 
//    xcombination::xcombination(short n, short m)
//    {
//        this->n = n;
//        this->m = m;
//        this->sset = new short[m + 2];
//        this->reset();
//    }
//
//    void xcombination::reset()
//    {
//        this->nc = 0;
//        for (int i = 0; i < this->m; i++) this->sset[i] = i; //!!!!!!!!!
//        this->sset[m] = this->n; //!!!!!!!!
//        this->sset[m + 1] = 0;   //!!!!!!!!
//    }
//
//    short xcombination::getfirst()
//    {
//        return (this->n >= this->m) ? this->m : -1;
//    }
//
//    short xcombination::getnext()
//    {
//        short rc = getfirst();
//        if (rc > 0)
//        {
//            short j;
//            for (j = 0; this->sset[j] + 1 == this->sset[j + 1]; ++j)
//                this->sset[j] = j;
//            if (j >= this->m) rc = -1;
//            else {
//                this->sset[j]++;
//                this->nc++;
//            }
//        }
//        return rc;
//    }
//
//    short xcombination::ntx(short i)
//    {
//        return this->sset[i];
//    }
//
//    unsigned __int64 fact(unsigned __int64 x)
//    {
//        return (x == 0) ? 1 : (x * fact(x - 1));
//    }
//
//    unsigned __int64 xcombination::count() const
//    {
//        return (this->n >= this->m) ?
//            fact(this->n) / (fact(this->n - this->m) * fact(this->m)) : 0;
//    }
//
//    // перестановки
//    permutation::permutation(short n)
//    {
//        this->n = n;
//        this->sset = new short[n];
//        this->dart = new bool[n];
//        this->reset();
//    }
//
//    void permutation::reset()
//    {
//        this->getfirst();
//    }
//
//    __int64 permutation::getfirst()
//    {
//        this->np = 0;
//        for (int i = 0; i < this->n; i++)
//        {
//            this->sset[i] = i;
//            this->dart[i] = L;
//        }
//        return (this->n > 0) ? this->np : -1;
//    }
//
//    __int64 permutation::getnext()
//    {
//        __int64 rc = -1;
//        short maxm = 0x8000, idx = -1;
//
//        for (int i = 0; i < this->n; i++)
//        {
//            if (i > 0 && this->dart[i] == L &&
//                this->sset[i] > this->sset[i - 1] &&
//                maxm < this->sset[i])
//                maxm = this->sset[idx = i];
//
//            if (i < (this->n - 1) && this->dart[i] == R &&
//                this->sset[i] > this->sset[i + 1] &&
//                maxm < this->sset[i])
//                maxm = this->sset[idx = i];
//        }
//
//        if (idx >= 0)
//        {
//            std::swap(this->sset[idx],
//                this->sset[idx + (this->dart[idx] == L ? -1 : 1)]);
//            std::swap(this->dart[idx],
//                this->dart[idx + (this->dart[idx] == L ? -1 : 1)]);
//
//            for (int i = 0; i < this->n; i++)
//                if (this->sset[i] > maxm)
//                    this->dart[i] = !this->dart[i];
//
//            rc = ++this->np;
//        }
//        return rc;
//    }
//
//    short permutation::ntx(short i)
//    {
//        return this->sset[i];
//    }
//
//    unsigned __int64 permutation::count() const
//    {
//        return fact(this->n);
//    }
//
//    // размещения
//    accomodation::accomodation(short n, short m)
//    {
//        this->n = n;
//        this->m = m;
//        this->cgen = new xcombination(n, m);
//        this->pgen = new permutation(m);
//        this->sset = new short[m];
//        this->reset();
//    }
//
//    void accomodation::reset()
//    {
//        this->na = 0;
//        this->cgen->reset();
//        this->pgen->reset();
//        this->cgen->getfirst();
//    }
//
//    short accomodation::getfirst()
//    {
//        short rc = (this->n >= this->m) ? this->m : -1;
//        if (rc > 0)
//        {
//            for (int i = 0; i < this->m; i++)
//                this->sset[i] = this->cgen->sset[this->pgen->ntx(i)];
//        }
//        return rc;
//    }
//
//    short accomodation::getnext()
//    {
//        short rc;
//        this->na++;
//        if ((this->pgen->getnext()) > 0)
//            rc = this->getfirst();
//        else if ((rc = this->cgen->getnext()) > 0)
//        {
//            this->pgen->reset();
//            rc = this->getfirst();
//        }
//        return rc;
//    }
//
//    short accomodation::ntx(short i)
//    {
//        return this->sset[i];
//    }
//
//    unsigned __int64 accomodation::count() const
//    {
//        return (this->n >= this->m) ?
//            fact(this->n) / fact(this->n - this->m) : 0;
//    }
//}
//






#include "Combi.h"                            // Подключаем заголовочный файл с объявлениями классов и функций.
#include <algorithm>                          // Подключаем библиотеку, где находится std::swap.

namespace combi                               // Открываем пространство имён combi.
{                                             // Начало пространства имён.

    // подмножества                           // Ниже идёт реализация генератора подмножеств.
    subset::subset(short n)                   // Конструктор класса subset, принимает количество элементов множества.
    {                                         // Начало конструктора subset.
        this->n = n;                          // Сохраняем размер исходного множества в поле объекта.
        this->sset = new short[n];            // Выделяем память под массив индексов текущего подмножества.
        this->reset();                        // Сбрасываем генератор в начальное состояние.
    }                                         // Конец конструктора subset.

    void subset::reset()                      // Метод reset() сбрасывает генератор подмножеств.
    {                                         // Начало метода reset().
        this->sn = 0;                         // Обнуляем количество элементов текущего подмножества.
        this->mask = 0;                       // Устанавливаем маску в 0, что соответствует пустому подмножеству.
    }                                         // Конец метода reset().

    short subset::getfirst()                  // Метод getfirst() формирует текущее подмножество по маске.
    {                                         // Начало метода getfirst().
        __int64 buf = this->mask;             // Копируем текущую маску в локальную переменную buf для побитовой обработки.
        this->sn = 0;                         // Обнуляем счётчик элементов текущего подмножества.
        for (short i = 0; i < n; i++)         // Проходим по всем элементам исходного множества.
        {                                     // Начало цикла по элементам множества.
            if (buf & 0x1) this->sset[this->sn++] = i; // Если младший бит маски равен 1, включаем индекс i в подмножество.
            buf >>= 1;                        // Сдвигаем маску вправо, чтобы проверить следующий бит.
        }                                     // Конец цикла по элементам множества.
        return this->sn;                      // Возвращаем количество элементов, вошедших в текущее подмножество.
    }                                         // Конец метода getfirst().

    short subset::getnext()                   // Метод getnext() переходит к следующему подмножеству.
    {                                         // Начало метода getnext().
        int rc = -1;                          // По умолчанию считаем, что следующего подмножества нет.
        this->sn = 0;                         // Обнуляем размер текущего подмножества перед построением нового.
        if (++this->mask < this->count()) rc = getfirst(); // Увеличиваем маску; если она ещё допустима, строим новое подмножество.
        return rc;                            // Возвращаем размер нового подмножества или -1, если подмножества закончились.
    }                                         // Конец метода getnext().

    short subset::ntx(short i)                // Метод ntx() возвращает i-й элемент текущего подмножества.
    {                                         // Начало метода ntx().
        return this->sset[i];                 // Возвращаем индекс элемента из массива текущего подмножества.
    }                                         // Конец метода ntx().

    unsigned __int64 subset::count()          // Метод count() возвращает число всех подмножеств.
    {                                         // Начало метода count().
        return (unsigned __int64)(1 << this->n); // Для множества из n элементов число подмножеств равно 2^n.
    }                                         // Конец метода count().

    // сочетания                              // Ниже идёт реализация генератора сочетаний.
    xcombination::xcombination(short n, short m) // Конструктор класса xcombination, принимает n и m.
    {                                         // Начало конструктора xcombination.
        this->n = n;                          // Сохраняем размер исходного множества.
        this->m = m;                          // Сохраняем размер сочетания.
        this->sset = new short[m + 2];        // Выделяем память под массив сочетания и два служебных элемента.
        this->reset();                        // Сбрасываем генератор в начальное состояние.
    }                                         // Конец конструктора xcombination.

    void xcombination::reset()                // Метод reset() задаёт первое сочетание.
    {                                         // Начало метода reset().
        this->nc = 0;                         // Обнуляем номер текущего сочетания.
        for (int i = 0; i < this->m; i++) this->sset[i] = i; // Записываем первое сочетание: 0,1,2,...,m-1.
        this->sset[m] = this->n;              // Записываем справа сторожевое значение n для удобства алгоритма.
        this->sset[m + 1] = 0;                // Записываем ещё один служебный элемент.
    }                                         // Конец метода reset().

    short xcombination::getfirst()            // Метод getfirst() возвращает размер сочетания, если оно возможно.
    {                                         // Начало метода getfirst().
        return (this->n >= this->m) ? this->m : -1; // Если n >= m, сочетание существует и его размер равен m, иначе -1.
    }                                         // Конец метода getfirst().

    short xcombination::getnext()             // Метод getnext() строит следующее сочетание.
    {                                         // Начало метода getnext().
        short rc = getfirst();                // Сначала предполагаем, что сочетание допустимо, и берём его размер.
        if (rc > 0)                           // Продолжаем только если сочетания вообще существуют.
        {                                     // Начало блока обработки следующего сочетания.
            short j;                          // Переменная j будет искать позицию, которую можно увеличить.
            for (j = 0; this->sset[j] + 1 == this->sset[j + 1]; ++j) // Пока текущий элемент вплотную прижат к следующему, двигаемся дальше.
                this->sset[j] = j;            // Сбрасываем такие позиции к минимально возможным значениям.
            if (j >= this->m) rc = -1;        // Если дошли за пределы сочетания, значит сочетания закончились.
            else {                            // Иначе можно построить следующее сочетание.
                this->sset[j]++;              // Увеличиваем найденную позицию на 1, получая следующее сочетание.
                this->nc++;                   // Увеличиваем счётчик номера сочетания.
            }                                 // Конец блока построения следующего сочетания.
        }                                     // Конец блока обработки следующего сочетания.
        return rc;                            // Возвращаем размер сочетания или -1, если больше сочетаний нет.
    }                                         // Конец метода getnext().

    short xcombination::ntx(short i)          // Метод ntx() возвращает i-й индекс текущего сочетания.
    {                                         // Начало метода ntx().
        return this->sset[i];                 // Возвращаем i-й элемент текущего сочетания.
    }                                         // Конец метода ntx().

    unsigned __int64 fact(unsigned __int64 x) // Функция fact() вычисляет факториал числа x.
    {                                         // Начало функции fact().
        return (x == 0) ? 1 : (x * fact(x - 1)); // Если x равно 0, возвращаем 1, иначе рекурсивно считаем x!.
    }                                         // Конец функции fact().

    unsigned __int64 xcombination::count() const // Метод count() возвращает число всех сочетаний.
    {                                         // Начало метода count().
        return (this->n >= this->m) ?         // Если выбрать m элементов из n возможно,
            fact(this->n) / (fact(this->n - this->m) * fact(this->m)) : 0; // Используем формулу C(n,m)=n!/((n-m)!*m!), иначе 0.
    }                                         // Конец метода count().

    // перестановки                           // Ниже идёт реализация генератора перестановок.
    permutation::permutation(short n)         // Конструктор класса permutation, принимает число элементов n.
    {                                         // Начало конструктора permutation.
        this->n = n;                          // Сохраняем размер множества.
        this->sset = new short[n];            // Выделяем память под текущую перестановку.
        this->dart = new bool[n];             // Выделяем память под направления движения элементов.
        this->reset();                        // Сбрасываем генератор в начальное состояние.
    }                                         // Конец конструктора permutation.

    void permutation::reset()                 // Метод reset() возвращает генератор к первой перестановке.
    {                                         // Начало метода reset().
        this->getfirst();                     // Просто строим самую первую перестановку.
    }                                         // Конец метода reset().

    __int64 permutation::getfirst()           // Метод getfirst() задаёт начальную перестановку.
    {                                         // Начало метода getfirst().
        this->np = 0;                         // Обнуляем номер текущей перестановки.
        for (int i = 0; i < this->n; i++)     // Проходим по всем позициям перестановки.
        {                                     // Начало цикла по позициям.
            this->sset[i] = i;                // Записываем начальный порядок 0,1,2,...,n-1.
            this->dart[i] = L;                // Всем элементам задаём направление влево.
        }                                     // Конец цикла по позициям.
        return (this->n > 0) ? this->np : -1; // Возвращаем 0, если перестановки существуют, иначе -1.
    }                                         // Конец метода getfirst().

    __int64 permutation::getnext()            // Метод getnext() строит следующую перестановку по Джонсону-Троттеру.
    {                                         // Начало метода getnext().
        __int64 rc = -1;                      // По умолчанию считаем, что следующей перестановки нет.
        short maxm = 0x8000, idx = -1;        // maxm хранит наибольший подвижный элемент, idx — его позицию.

        for (int i = 0; i < this->n; i++)     // Просматриваем все элементы текущей перестановки.
        {                                     // Начало цикла поиска подвижного элемента.
            if (i > 0 && this->dart[i] == L && // Если слева есть сосед и элемент смотрит влево,
                this->sset[i] > this->sset[i - 1] && // и он больше соседа слева,
                maxm < this->sset[i])         // и он больше текущего найденного максимального подвижного элемента,
                maxm = this->sset[idx = i];   // то запоминаем его значение и позицию.

            if (i < (this->n - 1) && this->dart[i] == R && // Если справа есть сосед и элемент смотрит вправо,
                this->sset[i] > this->sset[i + 1] && // и он больше соседа справа,
                maxm < this->sset[i])         // и он больше текущего найденного максимального подвижного элемента,
                maxm = this->sset[idx = i];   // то тоже запоминаем его как лучшего кандидата.
        }                                     // Конец цикла поиска подвижного элемента.

        if (idx >= 0)                         // Если подвижный элемент найден,
        {                                     // Начало блока построения новой перестановки.
            std::swap(this->sset[idx],        // Меняем найденный элемент местами с соседом в направлении его движения.
                this->sset[idx + (this->dart[idx] == L ? -1 : 1)]); // Выбираем соседа слева или справа в зависимости от направления.
            std::swap(this->dart[idx],        // Вместе с элементами меняем и их направления,
                this->dart[idx + (this->dart[idx] == L ? -1 : 1)]); // чтобы направление перемещалось вместе с элементом.

            for (int i = 0; i < this->n; i++) // После обмена снова проходим по всем элементам.
                if (this->sset[i] > maxm)     // Если элемент больше, чем сдвинутый максимальный подвижный,
                    this->dart[i] = !this->dart[i]; // то меняем его направление на противоположное.

            rc = ++this->np;                  // Увеличиваем номер перестановки и записываем его как результат.
        }                                     // Конец блока построения новой перестановки.
        return rc;                            // Возвращаем номер новой перестановки или -1, если перестановки закончились.
    }                                         // Конец метода getnext().

    short permutation::ntx(short i)           // Метод ntx() возвращает i-й элемент текущей перестановки.
    {                                         // Начало метода ntx().
        return this->sset[i];                 // Возвращаем индекс элемента на позиции i.
    }                                         // Конец метода ntx().

    unsigned __int64 permutation::count() const // Метод count() возвращает число всех перестановок.
    {                                         // Начало метода count().
        return fact(this->n);                 // Число перестановок из n элементов равно n!.
    }                                         // Конец метода count().

    // размещения                             // Ниже идёт реализация генератора размещений.
    accomodation::accomodation(short n, short m) // Конструктор класса accomodation, принимает n и m.
    {                                         // Начало конструктора accomodation.
        this->n = n;                          // Сохраняем размер исходного множества.
        this->m = m;                          // Сохраняем размер размещения.
        this->cgen = new xcombination(n, m);  // Создаём внутренний генератор сочетаний.
        this->pgen = new permutation(m);      // Создаём внутренний генератор перестановок длины m.
        this->sset = new short[m];            // Выделяем память под текущее размещение.
        this->reset();                        // Сбрасываем генератор размещений в начальное состояние.
    }                                         // Конец конструктора accomodation.

    void accomodation::reset()                // Метод reset() возвращает генератор размещений к началу.
    {                                         // Начало метода reset().
        this->na = 0;                         // Обнуляем номер текущего размещения.
        this->cgen->reset();                  // Сбрасываем генератор сочетаний.
        this->pgen->reset();                  // Сбрасываем генератор перестановок.
        this->cgen->getfirst();               // Устанавливаем первое сочетание как текущее.
    }                                         // Конец метода reset().

    short accomodation::getfirst()            // Метод getfirst() строит первое текущее размещение.
    {                                         // Начало метода getfirst().
        short rc = (this->n >= this->m) ? this->m : -1; // Проверяем, можно ли выбрать m элементов из n.
        if (rc > 0)                           // Если размещение возможно,
        {                                     // Начало блока сборки размещения.
            for (int i = 0; i < this->m; i++) // Проходим по всем позициям размещения.
                this->sset[i] = this->cgen->sset[this->pgen->ntx(i)]; // Берём элементы сочетания в порядке, заданном перестановкой.
        }                                     // Конец блока сборки размещения.
        return rc;                            // Возвращаем размер размещения или -1, если размещение невозможно.
    }                                         // Конец метода getfirst().

    short accomodation::getnext()             // Метод getnext() строит следующее размещение.
    {                                         // Начало метода getnext().
        short rc;                             // Объявляем переменную для результата.
        this->na++;                           // Увеличиваем номер текущего размещения.
        if ((this->pgen->getnext()) > 0)      // Сначала пробуем получить следующую перестановку для текущего сочетания.
            rc = this->getfirst();            // Если получилось, собираем новое размещение на основе того же сочетания.
        else if ((rc = this->cgen->getnext()) > 0) // Иначе пробуем перейти к следующему сочетанию.
        {                                     // Начало блока перехода к новому сочетанию.
            this->pgen->reset();              // Для нового сочетания сбрасываем перестановки к началу.
            rc = this->getfirst();            // Собираем первое размещение для нового сочетания.
        }                                     // Конец блока перехода к новому сочетанию.
        return rc;                            // Возвращаем размер размещения или -1, если размещения закончились.
    }                                         // Конец метода getnext().

    short accomodation::ntx(short i)          // Метод ntx() возвращает i-й элемент текущего размещения.
    {                                         // Начало метода ntx().
        return this->sset[i];                 // Возвращаем индекс элемента из текущего размещения.
    }                                         // Конец метода ntx().

    unsigned __int64 accomodation::count() const // Метод count() возвращает число всех размещений.
    {                                         // Начало метода count().
        return (this->n >= this->m) ?         // Если размещения возможны,
            fact(this->n) / fact(this->n - this->m) : 0; // используем формулу A(n,m)=n!/(n-m)!, иначе возвращаем 0.
    }                                         // Конец метода count().
}                                             // Закрываем пространство имён combi.