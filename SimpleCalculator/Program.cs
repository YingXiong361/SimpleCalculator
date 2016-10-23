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
            var testSample1 = "(1+2)";
            var testSample2 = "(1+((2+3)*(4*5)))";

            Console.WriteLine("{0} = {1}",testSample1,SimpleCalculatorWithOneStack.Calculate(testSample1));
            Console.WriteLine("{0} = {1}", testSample2, SimpleCalculatorWithOneStack.Calculate(testSample2));

            Console.Read();
        }
    }
     
    public static class SimpleCalculatorWithOneStack
    {
        private static Dictionary<string,Func<double,double,double>> 
            ArithmeticTwoOperandHandler=new Dictionary<string,Func<double,double,double>>{
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
        private static readonly string  LeftParenthesis = "(";
        private static readonly string  RightParenthesis = ")";

        public static double Calculate(string arithmeticExpression)
        {
            var calculatorCore = new Stack<string>();
            foreach(char c in arithmeticExpression.ToList<char>())
            {
                if(c.ToString()!=RightParenthesis)
                {
                    calculatorCore.Push(c.ToString());
                }
                else
                {
                    double result=double.NegativeInfinity;    
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
           return  ArithmeticTwoOperandHandler.Keys.Contains(oper);
        }

    }
}
