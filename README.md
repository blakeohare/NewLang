# NewLang

I needed a simple throwaway toy language for an experimental project so I created this.

Most of my established languages are domain-specific or tightly integrated with
some weird toolset or built using transpilers or other wacky shenanigans.

NewLang is a new language that is:

- A simple canonical curly brace language
- Parser and runtime are all in one single assembly. There is no intermediate
  emitted file format that is serialized and re-parsed.
- Built using a single standard language (C#) that isn't generated code or
  transpiled or any other source code indirection.
- For codebase simplicity, it is not really optimized for performance. Most
  internal implementations of things are fairly naive.
- Able to compile to a native executable (using .NET publish)

## Running

**helloworld.newlang**:
```
// A simple Hello World app
function main(args) {
    print("Hello, World!");
}
```

In the command line:
```bash
$ newlang helloworld.newlang
Hello, World!
```
