# Base Calculator

This is an implementation of a really basic calculator that allows you to use operations such as `+`, `-`, `*`, and `/` on numbers in any base, not just `10`. 

## Usage

`cd` into the project directory and run `dotnet run` for the default base of `12` or `dotnet run -- --radix 8` for a base of your choosing. This will enter you into a repl that will ask for an expression in your chosen base and give you the result both in your chosen base and in base 10. 

Examples:
```
dotnet run -- --radix 12

Enter an expression in base 12
> 1 + 20
Result (base12): 21
Result (base10): 25

Enter an expression in base 12
>
```

Note that the calculator is very basic and does not respect the normal order of operations; expressions are evaluated strictly left-to-right.
