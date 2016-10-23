using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleCalculator
{
    class Program
    {
        static void Main(string[] args)
        {
            var testSample1 = "( 1 + 2 )";
            var testSample2 = "( 1 + ( ( 2 + 3 ) * ( 4 * 5 ) ) )";

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.ResetColor();
            Console.WriteLine("{0} = {1}", testSample1, SimpleCalculatorWithOneStack.Calculate(testSample1));
            Console.WriteLine("{0} = {1}", testSample2, SimpleCalculatorWithOneStack.Calculate(testSample2));


            var testSample3 = "( 6 * 7 + 8 + 10 + 4 * 5 )";
            var testSample4 = "( ( 1 + square 2 ) * ( 2 * 3 * 7 + 8 + 10 + 4 * 5 ) )";
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.ResetColor();
            Console.WriteLine("{0} = {1}", testSample1, SimplecalculatorWithTwoStack.Calculate(testSample1));
            Console.WriteLine("{0} = {1}", testSample2, SimplecalculatorWithTwoStack.Calculate(testSample2));
            Console.WriteLine("{0} = {1}", testSample3, SimplecalculatorWithTwoStack.Calculate(testSample3));
            Console.WriteLine("{0} = {1}", testSample4, SimplecalculatorWithTwoStack.Calculate(testSample4));

            Console.Read();
        }
    }

    public static class SimpleCalculatorWithOneStack
    {
        private static Dictionary<string, Func<double, double, double>>
            ArithmeticTwoOperandHandler = new Dictionary<string, Func<double, double, double>>{
                {"+",(x,y)=>x+y},
                {"-",(x,y)=>(x-y)},
                {"*",(x,y)=>(x*y)},
                {"/",(x,y)=>(x/y)}
            };
        private static Dictionary<string, Func<double, double>>
            ArithmeticOneOperandHandler = new Dictionary<string, Func<double, double>>{
                {"square",x=>Math.Pow(x,2)},
                {"squareRoot",x=>Math.Sqrt(x)},
            };
        private static readonly string RightParenthesis = ")";

        public static double Calculate(string arithmeticExpression)
        {
            var calculatorCore = new Stack<string>();
            List<string> symbols = arithmeticExpression.Split(' ').ToList();
            foreach (string str in symbols)
            {
                if (str != RightParenthesis)
                {
                    calculatorCore.Push(str.ToString());
                }
                else
                {
                    double result = double.NegativeInfinity;
                    var operand1 = double.Parse(calculatorCore.Pop());
                    var operatorStr = calculatorCore.Pop();
                    if (IsATwoOperandOperator(operatorStr))
                    {
                        var operand2 = double.Parse(calculatorCore.Pop());
                        result = ArithmeticTwoOperandHandler[operatorStr](operand1, operand2);
                    }
                    else
                    {
                        result = ArithmeticOneOperandHandler[operatorStr](operand1);
                    }
                    calculatorCore.Pop();
                    calculatorCore.Push(result.ToString());
                }
            }

            return double.Parse(calculatorCore.Pop());
        }

        private static bool IsATwoOperandOperator(string oper)
        {
            return ArithmeticTwoOperandHandler.Keys.Contains(oper);
        }

    }

    public static class SimplecalculatorWithTwoStack
    {
        private static Dictionary<string, Func<double, double, double>>
    ArithmeticTwoOperandHandler = new Dictionary<string, Func<double, double, double>>{
                {"+",(x,y)=>x+y},
                {"-",(x,y)=>(x-y)},
                {"*",(x,y)=>(x*y)},
                {"/",(x,y)=>(x/y)}
            };
        private static Dictionary<string, Func<double, double>>
            ArithmeticOneOperandHandler = new Dictionary<string, Func<double, double>>{
                {"square",x=>Math.Pow(x,2)},
                {"squareRoot",x=>Math.Sqrt(x)},
            };
        private static Dictionary<string, int>
            OperatorPrecedences = new Dictionary<string, int>{
                {"+",1},
                {"-",1},
                {"*",2},
                {"/",2},    
                {"square",3},
                {"squareRoot",3},
                {"(",0},
                {string.Empty,int.MinValue}
            };
        private static readonly string LeftParenthesis = "(";
        private static readonly string RightParenthesis = ")";

        public static double Calculate(string arithmeticExpression)
        {
            var operators = new Stack<string>();
            var operands = new Stack<string>();
            List<string> symbols = arithmeticExpression.Split(' ').ToList();
            foreach (string str in symbols)
            {
                if (str != RightParenthesis)
                {
                    if (str.IsOperator()||str==LeftParenthesis)
                    {
                        operators.Push(str.ToString());
                    }
                    else
                    {
                        operands.Push(str.ToString());
                    }
                }
                else
                {
                    bool isExpressionFinished = false;
                    while (!isExpressionFinished)
                    {
                        Compute(operators, operands);
                        var operatorStr = operators.Pop();
                        if(operatorStr==LeftParenthesis)
                        {
                            isExpressionFinished = true;
                        }
                        else
                        {
                            operators.Push(operatorStr);
                        }
                        
                    }
                }
            }

            return double.Parse(operands.Pop());
        }

        private static void Compute(Stack<string> operators, Stack<string> operands)
        {
            double result = double.NegativeInfinity;
            var operatorStr = operators.Pop();
            var operand1 = double.Parse(operands.Pop());
            var formerOperator = operators.Pop();
            operators.Push(formerOperator);


            if(operatorStr.GetPrecedence()<=formerOperator.GetPrecedence())
            {
                 Compute(operators, operands);
            }

            if (operatorStr.IsATwoOperandOperator())
            {
                var operand2 = double.Parse(operands.Pop());
                result = ArithmeticTwoOperandHandler[operatorStr](operand1, operand2);
            }
            else
            {
                result = ArithmeticOneOperandHandler[operatorStr](operand1);
            }
            operands.Push(result.ToString());
        }

        private static bool IsATwoOperandOperator(this string oper)
        {
            return ArithmeticTwoOperandHandler.ContainsKey(oper);
        }

        private static bool IsOperator(this string str)
        {
            return ArithmeticOneOperandHandler.ContainsKey(str) ||
                ArithmeticTwoOperandHandler.ContainsKey(str);
        }

        private static int GetPrecedence(this string operatorStr)
        {
            return OperatorPrecedences[operatorStr];
        }
    }
}
