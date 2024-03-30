// See https://aka.ms/new-console-template for more information
using System.Globalization;
using System.Security.AccessControl;

Console.WriteLine("Hello, World!");

//string expr = "123         + 2 +3/     4 + 5";
string expr = Console.ReadLine();
Console.WriteLine(expr);
string exprws = RemoveWSpaces(expr);
Console.WriteLine(exprws);

GetToken2(exprws);



string RemoveWSpaces(string expression) {
    string ret = "";
    foreach (char c in expression) {
        if (!Char.IsWhiteSpace(c)) {
            ret += c;
        }
    }
    return ret;
}

bool IsDelimiter(char ch) {
    //char[] operations = { "+", "-", "*", "/", "(", ")"};
    string operations = "+-*/()";
    foreach (char operation in operations)
    {
        if (operation == ch)
        {
            return true;
        }
    }
    return false;
}

bool IsOperator(char ch) {
    string operations = "+-*/";
    foreach (char operation in operations) {
        if (operation == ch) {
            return true;
        }
    }
    return false;
}

bool IsIntNumber(string token) {
    foreach (char c in token)
    {
        if (!char.IsDigit(c))
        {
            return false;
        }
    }
    return true;
}

bool IsRealNumber(string token) {
    bool hasDecimal = false;
    foreach (char c in token) {
        if (!char.IsDigit(c)) {
            return false;
        }
        if (c == '.') { 
            hasDecimal = true;
        }


    }
    return hasDecimal;
}


void GetTokens1(string expression) {
    int i = 0, j = 0;
    while (i<=j && j<expression.Length) {
        if (!IsDelimiter(expression[j]))
            ++j;

        if (IsDelimiter(expression[j]) && i == j) {
            if (IsOperator(expression[j])) {
                Console.WriteLine($"{expression[j]} is an operator");
            } else {
                Console.WriteLine($"{expression[j]} is a delimiter");
            }
            j++;
            i = j;
        } else if ((IsDelimiter(expression[j]) || j == expression.Length) && i != j) {
            string substr = expression[i..j];
            if (IsIntNumber(substr)) {
                Console.WriteLine($"{substr} is an int number");
            } else if (IsRealNumber(substr)) {
                Console.WriteLine($"{substr} is a real number");
            } else {
                Console.WriteLine($"{substr} is an invalid token");
            }
            i = j;
        }
        

        
    }
}


List<string> GetTokens(string expression) { 
    List<string> tokens = new List<string>();
    string[] operations = { "+", "-", "**", "/", "(", ")" };
    for (int j = 0; j < expression.Length;) {
        string number = "";
        while (j < expression.Length && (Char.IsDigit(expression[j]) || expression[j] == '.')) { 
            number += expression[j];
            ++j;
        }

        if (number != "") { 
            tokens.Add(number);
        }
        
        string oper = "";
        //oper.Contains
        foreach (var o in operations) {
            
            int d = Math.Min(o.Length, expression.Length - j);
            if (string.Equals(o, expression.Substring(j, d))) {
                j += d;
                oper += o;
                
            }
        }

        if (oper != "")
        {
            tokens.Add(oper);
        }
        else {
            j++;
        }

    }
    return tokens;
}

void step(ref State s, char c) {
    switch (s) {
        case State.IntNumber:
            if (!char.IsDigit(c)) { 
            
            }
            break;
        case State.Operator:
            if (IsOperator(c)) { 
            }
            break;
    }
}

void GetToken2(string expression) {
    State s = State.IntNumber;
    int i = 0, j = 0;
    string token = "";
    foreach (char c in expression) {
        switch(s) {
            case State.IntNumber:
                if (char.IsDigit(c)) {
                    token += c;
                } else {
                    if (token != "") {
                        Console.WriteLine($"Int number: {token}");
                    } else {
                        Console.WriteLine("Error. Expression is incorrect");
                        return;
                    }
                    
                    if(IsOperator(c))
                        Console.WriteLine($"Operator: {c}");
                    s = State.Operator;
                    token = "";
                }
                break;
            case State.Operator:
                if (char.IsDigit(c)) {
                    token += c;
                    s = State.IntNumber;
                } else {
                    Console.WriteLine("Error. Expression is incorrect");
                    return;
                }
                break;
        }
        
    }
    if (token != "") {
        Console.WriteLine($"Int number: {token}");
    }

}

enum State { IntNumber, Operator }
