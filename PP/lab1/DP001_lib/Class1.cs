using System;

namespace DP001_lib
{
    public interface ICalcResult
    {
        int result(int y);
    }

    public partial class Calc
    {
        public int result { get; private set; } = 0;

        public Calc(int result = 0)
        {
            this.result = result;
        }

        public int sum(int x, int y) { return x + y; }
        public int sub(int x, int y) { return x - y; }
        public int mul(int x, int y) { return x * y; }
        public int div(int x, int y) { return x / y; }
        public int mod(int x, int y) { return x % y; }

        public Calc sum(int y) { return new Calc(this.result + y); }
        public Calc sub(int y) { return new Calc(this.result - y); }
        public Calc mul(int y) { return new Calc(this.result * y); }
        public Calc div(int y) { return new Calc(this.result / y); }
        public Calc mod(int y) { return new Calc(this.result % y); }

        public static Builder getBuilder()
        {
            return new Builder();
        }

        public class Builder
        {
            private ICalcResult _current;

            public Builder()
            {
                _current = new BaseResult();
            }

            public Builder Sum(int y) { _current = new SumDecorator(_current, y); return this; }
            public Builder Sub(int y) { _current = new SubDecorator(_current, y); return this; }
            public Builder Mul(int y) { _current = new MulDecorator(_current, y); return this; }
            public Builder Div(int y) { _current = new DivDecorator(_current, y); return this; }
            public Builder Mod(int y) { _current = new ModDecorator(_current, y); return this; }

            public ICalcResult Build()
            {
                return _current;
            }

            private class BaseResult : ICalcResult
            {
                public int result(int y) { return y; }
            }

            private abstract class CalcDecorator : ICalcResult
            {
                protected ICalcResult _component;
                protected int _val;

                public CalcDecorator(ICalcResult component, int val)
                {
                    _component = component;
                    _val = val;
                }

                public abstract int result(int y);
            }

            private class SumDecorator : CalcDecorator
            {
                public SumDecorator(ICalcResult c, int v) : base(c, v) { }
                public override int result(int y) { return _component.result(y) + _val; }
            }

            private class SubDecorator : CalcDecorator
            {
                public SubDecorator(ICalcResult c, int v) : base(c, v) { }
                public override int result(int y) { return _component.result(y) - _val; }
            }

            private class MulDecorator : CalcDecorator
            {
                public MulDecorator(ICalcResult c, int v) : base(c, v) { }
                public override int result(int y) { return _component.result(y) * _val; }
            }

            private class DivDecorator : CalcDecorator
            {
                public DivDecorator(ICalcResult c, int v) : base(c, v) { }
                public override int result(int y) { return _component.result(y) / _val; }
            }

            private class ModDecorator : CalcDecorator
            {
                public ModDecorator(ICalcResult c, int v) : base(c, v) { }
                public override int result(int y) { return _component.result(y) % _val; }
            }
        }
    }
}




using System; // Подключаем базовое пространство имен .NET; здесь оно почти не используется, но обычно его оставляют.

namespace DP001_lib // Объявляем пространство имен библиотеки.
{
    public interface ICalcResult // Интерфейс для объектов, которые умеют вычислять результат по входному значению y.
    {
        int result(int y); // Метод: принимает число y и возвращает вычисленный результат.
    }

    public partial class Calc // Частичный класс Calc; partial значит, что класс можно разделить на несколько файлов.
    {
        public int result { get; private set; } = 0; // Свойство result хранит текущее значение калькулятора; менять его можно только внутри класса.

        public Calc(int result = 0) // Конструктор класса Calc; если значение не передали, по умолчанию будет 0.
        {
            this.result = result; // Сохраняем переданное значение в свойство текущего объекта.
        }

        public int sum(int x, int y) { return x + y; } // Обычный метод сложения двух чисел: x + y.
        public int sub(int x, int y) { return x - y; } // Обычный метод вычитания двух чисел: x - y.
        public int mul(int x, int y) { return x * y; } // Обычный метод умножения двух чисел: x * y.
        public int div(int x, int y) { return x / y; } // Обычный метод деления двух чисел: x / y.
        public int mod(int x, int y) { return x % y; } // Обычный метод остатка от деления: x % y.

        public Calc sum(int y) { return new Calc(this.result + y); } // Создаем новый объект Calc, где к текущему result прибавляется y.
        public Calc sub(int y) { return new Calc(this.result - y); } // Создаем новый объект Calc, где из текущего result вычитается y.
        public Calc mul(int y) { return new Calc(this.result * y); } // Создаем новый объект Calc, где текущий result умножается на y.
        public Calc div(int y) { return new Calc(this.result / y); } // Создаем новый объект Calc, где текущий result делится на y.
        public Calc mod(int y) { return new Calc(this.result % y); } // Создаем новый объект Calc, где берется остаток от деления текущего result на y.

        public static Builder getBuilder() // Статический метод для получения объекта Builder без создания Calc вручную.
        {
            return new Builder(); // Возвращаем новый экземпляр Builder.
        }

        public class Builder // Вложенный класс Builder; он будет по шагам собирать цепочку операций.
        {
            private ICalcResult _current; // Здесь хранится текущая цепочка вычислений в виде объекта, реализующего ICalcResult.

            public Builder() // Конструктор Builder.
            {
                _current = new BaseResult(); // В начале цепочка пустая, поэтому ставим базовый объект, который просто возвращает входное значение.
            }

            public Builder Sum(int y) { _current = new SumDecorator(_current, y); return this; } // Оборачиваем текущую цепочку декоратором сложения и возвращаем этот же Builder для цепочки вызовов.
            public Builder Sub(int y) { _current = new SubDecorator(_current, y); return this; } // Оборачиваем текущую цепочку декоратором вычитания.
            public Builder Mul(int y) { _current = new MulDecorator(_current, y); return this; } // Оборачиваем текущую цепочку декоратором умножения.
            public Builder Div(int y) { _current = new DivDecorator(_current, y); return this; } // Оборачиваем текущую цепочку декоратором деления.
            public Builder Mod(int y) { _current = new ModDecorator(_current, y); return this; } // Оборачиваем текущую цепочку декоратором остатка от деления.

            public ICalcResult Build() // Метод завершает сборку цепочки.
            {
                return _current; // Возвращаем готовый объект, который умеет считать результат.
            }

            private class BaseResult : ICalcResult // Базовая реализация интерфейса; это старт цепочки.
            {
                public int result(int y) { return y; } // Ничего не делает, просто возвращает исходное значение y.
            }

            private abstract class CalcDecorator : ICalcResult // Абстрактный базовый класс для всех декораторов.
            {
                protected ICalcResult _component; // Ссылка на предыдущий объект в цепочке.
                protected int _val; // Значение, которое использует конкретная операция, например +5 или *3.

                public CalcDecorator(ICalcResult component, int val) // Конструктор базового декоратора.
                {
                    _component = component; // Сохраняем объект, который декорируем.
                    _val = val; // Сохраняем число для операции.
                }

                public abstract int result(int y); // Каждый конкретный декоратор сам решает, как изменить результат.
            }

            private class SumDecorator : CalcDecorator // Декоратор для операции сложения.
            {
                public SumDecorator(ICalcResult c, int v) : base(c, v) { } // Передаем предыдущий компонент и значение в базовый конструктор.
                public override int result(int y) { return _component.result(y) + _val; } // Сначала считаем предыдущую цепочку, потом прибавляем _val.
            }

            private class SubDecorator : CalcDecorator // Декоратор для операции вычитания.
            {
                public SubDecorator(ICalcResult c, int v) : base(c, v) { } // Передаем предыдущий компонент и значение в базовый конструктор.
                public override int result(int y) { return _component.result(y) - _val; } // Сначала считаем предыдущую цепочку, потом вычитаем _val.
            }

            private class MulDecorator : CalcDecorator // Декоратор для операции умножения.
            {
                public MulDecorator(ICalcResult c, int v) : base(c, v) { } // Передаем предыдущий компонент и значение в базовый конструктор.
                public override int result(int y) { return _component.result(y) * _val; } // Сначала считаем предыдущую цепочку, потом умножаем на _val.
            }

            private class DivDecorator : CalcDecorator // Декоратор для операции деления.
            {
                public DivDecorator(ICalcResult c, int v) : base(c, v) { } // Передаем предыдущий компонент и значение в базовый конструктор.
                public override int result(int y) { return _component.result(y) / _val; } // Сначала считаем предыдущую цепочку, потом делим на _val.
            }

            private class ModDecorator : CalcDecorator // Декоратор для операции остатка от деления.
            {
                public ModDecorator(ICalcResult c, int v) : base(c, v) { } // Передаем предыдущий компонент и значение в базовый конструктор.
                public override int result(int y) { return _component.result(y) % _val; } // Сначала считаем предыдущую цепочку, потом берем остаток от деления на _val.
            }
        }
    }
}