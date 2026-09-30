# Lexer

Grammars provide rules that specify the structure of languages, independently
from the actual meaning of the content. Grammars are classified according to the
complexity of the structure they describe.

Back-Naur Form (BNF) is a notation used to describe context free grammars.

The file is parsed into the following tokens:

```
    Token        ::=  {Space}* (<Comment> | <Config>)

    <Comment>    ::= # {Printable} {EOL}
    <Config>     ::= <Key> <Equals> <Value>

    <Key>        ::= {Letter} {Letter_}*
    <Equals>     ::= {Space}* = {Space}*
    <Value>      ::= {Space}* ( <Value> {EOL} | <String> )
    {Letter}     ::= [A-Za-z] [0xC0-0xD6] [0xD8-0xF6] [0xF8-0x2FF]
    {Letter_}    ::= {Letter} [_] {Digit}
        
    <Value>     ::= (<Hex> | <Bin> | <Number>) {SpaceOrEnd}
    <Hex>     ::= '0x' {HexDigit}+
    <Bin>     ::= '0b' {BinDigit}+
    <Number>  ::= <Integer> | <Float>
    <Integer> ::= '-'? {Digit}+
    <Float>   ::= '-'? {Digit}* '.' {Digit}+
        
    {Digit}      ::= [0123456789]
    {HexDigit}   ::= {Digit} + [abcdefABCDEF]
    {BinDigit}   ::= [01]
        
    <String> ::= <String-Quoted> | <String-Unquoted>
    <String-Quoted>   ::= '["' ({Printable}|{EOL})* '"]' {EOL}
    <String-Unquoted> ::= {Printable}* {EOL}
        
    {Printable}  ::= [0x09 0x20-0x07E 0x80-0xFFFD]
    {Space}      ::= [' ' 0x09 0x0D 0x0A]
    {EOL}        ::= 0x0D 0x0A | 0x0A 0x0D | 0x0A | 0x0D | {EOF}
    {EOF}        ::= 0x00
```
