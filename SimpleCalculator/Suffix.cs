using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleCalculator
{
    public static class Suffix
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

        //3+(4-5)*6-8/4 => 345-6*+84/-
        public static int? Calculate(string expression)
        {
            Stack<int> operands = new Stack<int>();
            var charArr = expression.ToCharArray();
            foreach (var e in charArr)
            {
                //1step: push to stack
                if (!IsOperator(e))
                {
                    operands.Push(int.Parse(e.ToString()));
                }
                //2 step: if operantor, pop two element,do the calculation, push the result into the stack
                else
                {
                    var operand2 = int.Parse(operands.Pop().ToString());
                    var operand1 = int.Parse(operands.Pop().ToString());
                    operands.Push(_funcs[e].Invoke(operand1, operand2));
                }
            }

            //last step:the result will be the last element in the stack
            return operands.Pop();
        }

        //3+(4-5)*6-8/4 => 345-6*+84/-
        public static string TranslateInfixToSuffixExpression(string infixExpression)
        {
            var charArr = infixExpression.ToCharArray();
            var operators = new Stack<char>();
            var result = new StringBuilder();

            foreach (var e in charArr)
            {
                //1 step if numeric value, print directly
                //2 setp,if not, add to stack if 1. precedence is lower than the operand in the stack, print, continue,until the condition fail 2. left parenthences, push, right parenthences, pop,until left parenthences
                if (IsDigit(e))
                {
                    result.Append(e);
                    continue;
                }
                if (e == '(')
                {
                    operators.Push(e);
                    continue;
                }
                if (e == ')')
                {
                    while (operators.Any() && operators.First() != '(')
                    {
                        result.Append(operators.Pop());
                    }

                    operators.Pop();
                }
                if (!operators.Any() || operators.First() == '(')
                {
                    operators.Push(e);
                    continue;
                }
                if (IsOperator(e))
                {
                    while (operators.Any() && _precendences[operators.First()] > _precendences[e])
                    {
                        result.Append(operators.Pop());
                    }
                    operators.Push(e);
                }
            }


            while (operators.Any())
            {
                result.Append(operators.Pop());
            }

            return result.ToString();

        }

        private static bool IsOperator(char e)
        {
            return _funcs.ContainsKey(e);
        }

        private static bool IsDigit(char e)
        {
            return !IsOperator(e) && e != '(' && e != ')';
        }
    }
}
