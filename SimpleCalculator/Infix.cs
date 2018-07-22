using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleCalculator
{
    public static class Infix
    {
        private static Dictionary<char, Func<int, int, int>> _funcs = new Dictionary<char, Func<int, int, int>>
        {
            {'+',(int a,int b)=>{return a+b;}},
            {'-',(int a,int b)=>{return a-b;}},
            {'*',(int a,int b)=>{return a*b;}},
            {'/',(int a,int b)=>{return a/b;}},
        };

        private static Dictionary<char, int> _precendences = new Dictionary<char, int>
        {
            {'+',1},
            {'-',1},
            {'*',2},
            {'/',2},
        };

        private static bool IsOperator(char e)
        {
            return _funcs.ContainsKey(e);
        }

        //-+3*-456/84
        public static int Calculate(string expression)
        {
            var charArr = expression.ToCharArray();
            var operands = new Stack<int>();
            for (int i = charArr.Length - 1; i >= 0; i--)
            {
                var e = charArr[i];
                if (!IsOperator(e))
                {
                    operands.Push(int.Parse(e.ToString()));
                }
                else
                {
                    var operand1 = operands.Pop();
                    var operand2 = operands.Pop();
                    operands.Push(_funcs[e].Invoke(operand1, operand2));
                }
            }

            return operands.Pop();
        }

        //3+(4-5)*6-8/4 => -+3*-456/84
        public static string Translate(string expression)
        {
            var charArr = expression.ToCharArray();
            var operators = new Stack<char>();
            var result = new StringBuilder();
            for (int i = charArr.Length - 1; i >= 0; i--)
            {
                var e = charArr[i];
                if (!IsOperator(e) && e != '(' && e != ')')
                {
                    result.Insert(0, e);
                    continue;
                }
                if (e == ')')
                {
                    operators.Push(e); 
                    continue;
                }
                if (e == '(')
                {
                    while (operators.Any() && operators.First() != ')')
                    {
                        result.Insert(0, operators.Pop());
                    }
                    operators.Pop();
                    continue;
                }
                if (!operators.Any() || operators.First() == ')')
                {
                    operators.Push(e);
                    continue;
                }
                if (e != '(' && _precendences[e] >= _precendences[operators.First()])
                {
                    operators.Push(e);
                    continue;
                }
                if (_precendences[e] < _precendences[operators.First()])
                {
                    while (operators.Any() && _precendences[e] <= _precendences[operators.First()])
                    {
                        result.Insert(0, operators.Pop());
                    }
                    operators.Push(e);
                }
            }


            while (operators.Any())
            {
                result.Insert(0, operators.Pop());
            }

            return result.ToString();
        }
    }
}
