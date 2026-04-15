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