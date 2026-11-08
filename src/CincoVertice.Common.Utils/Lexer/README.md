# Lexer
A Lexer, also known as a tokenizer or scanner, takes the source text and breaks
it into the reserved words, constants, identifiers and symbols that are defined
in the language.

The goal of the lexer (tokenizer) is to recognize different tokens and pass this
information to the parsing algorithm.

The tokens are subsequently passed to the actual parser which analyzes the
series of tokens and then determines when one of the language's syntax rules is
complete.

## Notation

The notation we are using is based on Back-Noir notation (or BNF).

It is a way to express the structure of a language, and helps the language
designer to clarify and communicate the structure (rules).

It also makes it easier to implement a compiler or interpreter.

### Terminals.

The base tokens of the language. For a programming language that may be:
keywords, operators and other symbols, characters that can be used in
identifiers, numbers, etc.

### Non-terminals.

Used to represent pieces of the structure of the language. These are enclosed in
angle brackets: <statements>

```
    <sentence> ::= <noun phrase><verb phrase>
```

### Production rules.

Rules that make up the grammar. They translate a nonterminal to a sequence of
one or more non-terminals or terminals.

```
    {Integer}  ::= [-]? {Digit}+
```

#### Assignation

Assignation is indicated with the ::= characters.

```
    {Integer}  ::= [-]? {Digit}+
```

#### Grouping and alternatives

Use parenthesis to group rules.

We can use | to indicate alternatives (it represents OR).

```
    {Decimal}  ::= ([-]? [.] {Digit}+) | ({Integer} [.]? {Digit}*)
    {Integer}  ::= [-]? {Digit}+
```

### Allowable characters

The following indicates the allowable characters. It can be seen as a character
array.

```
   {Digit} ::= [0123456789]
```

### An optional element

The following is used to indicate an optional element. It may occur or not.

```
   [-]?
   {Digit}?
```

### Repetition

To indicate only one occurrence:

```
    {Digit}
    [.]
```

To indicate N number of occurrences:

```
    {Digit}(2)   // 12 would be valid
    [.](3)       // Expects ...
```

To indicate an optional occurrence. It may not occur, but if it occurs it may
be as many occurrences

```
    {Digit}*    // abc would be valid.
                // 12345678987654321 would be valid.
```

To indicate at least one or more occurrences:

```
    {Digit}+    // 1 would be valid.
                // 12345678987654321 would be valid.
                // abc would not be valid.
```
