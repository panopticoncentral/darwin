# Darwin Language Specification

## Version 1.0 (Draft 1)
## Paul Vick

<br/>

## Table of Contents

* [1 Introduction](#1)
  * [1.1 Grammar Notation](#1.1)
* [2 Lexical Grammar](#2)
  * [2.1 Whitespace](#2.1)
  * [2.2 Comments](#2.2)
  * [2.3 Identifiers](#2.3)
  * [2.4 Literals](#2.4)
    * [2.4.1 Integer Literals](#2.4.1)
    * [2.4.2 Floating-Point Literals](#2.4.2)
    * [2.4.3 String Literals](#2.4.3)
    * [2.4.4 Domain-Specific Literals](#2.4.4)
  * [2.5 Punctuators](#2.5)
  * [2.6 Operators](#2.6)

<br/>

# <a name="1"></a>1 Introduction

Darwin is a general purpose programming language that targets the .NET platform. 

## <a name="1.1"></a>1.1 Grammar Notation

This specification specifies a lexical and a syntatic grammar. Each grammar is defined using a set of *rules*. Each rule starts with a name followed by a colon and then a set of *productions*, one per line. The productions of a rule define the different possible ways to match the rule. For example the following: 

```
expression
  : literal
  | expression operator expression
  ;
```

defines a rule *expression* that either matches a single *literal* or an *expression* followed by an *operator* and then another *expression*. The rule named *start* is the start rule for a grammar. 

A production contains a set of sequential *terms*. A term can be one of the following: 

* **A character.** Unicode characters are specified in single quotes (`'a'`). A sequence of Unicode characters can be specified in double quotes and matches each character in sequence (i.e. `'i' 'f'` can also be written as `"if"`). The following escape sequences are allowed:

  * `\x` followed by four hexadecimal digits in curly braces specifies the Unicode character point (i.e. `'\x{0061}'` is equvalent to `'a'`).

  * `\c` followed by a Unicode category name in curly braces matches any Unicode character in that category (i.e. `\c{Lu}`).

  * `\p` followed by a Unicode property name in curly braces matches any Unicode character with that property (i.e. `\p{White_Space}`).

  * `\\` specifies the backslash character.

  * `\'` specifies the single quote character.

  * `\"` specifies the double quote character.

* **A rule name.** A name matches the rule named. 

* **A group.** A list of terms can be contained by parenthesis ("()") to allow grouping multiple terms. 

* **A range.** A range of character values can be specified by separating them with two dots (`..`). 

Terms can be modified with the following operators:

* The postfix operator `?` indicates a term is optional and can appear zero or one times.

* The postfix operator `+` indicates a term must appear one or more times.

* The postfix operator `*` indicates a term may appear zero or more times.

* The prefix operator `!` matches anything except the following term.

> **Note:** The grammars in this specification are not intended to be formal grammars (that is, usable by any particular parser or lexer generator). 

<br/>

# <a name="2"></a>2 Lexical Grammar

The first step in processing Darwin code is to translate a stream of Unicode characters into an ordered set of lexical tokens. 

> **Note:** All references to "Unicode" in this specification refer to Unicode version 8.0. 

```
start
  : token*
  ;

token
  : whitespace
  | comment
  | identifier
  | literal
  | punctuator
  | operator
  ;
```

## <a name="2.1"></a>2.1 Whitespace

*Whitespace* serves to separate tokens but has no other significance in the language. Whitespace is defined as any character with the Unicode property `White_Space`. A *line terminator* is whitespace that marks the lexical end of a line. 

```
whitespace
  : '\p{White_Space}'
  ;

line-terminator
  : '\u{000d}' '\u{000a}'?
  | '\u{000a}'
  ;
```

## <a name="2.2"></a>2.2 Comments

*Comments* are text that serve as comments on the source code and are treated as whitespace. A comment extends only until the next line terminator. 

```
comment
  : '#' comment-element*
  ;

comment-element
  : !line-terminator
  ;
```

## <a name="2.3"></a>2.3 Identifiers

An *identifier* is a name. Darwin identifiers conform to the Unicode Standard Annex 31. The language includes the following requirements from that specification: 

* **UAX31-R1**: Darwin specifies a profile that includes the underscore (`_`) character in the `Start` set. It also removes the `Other_ID_Start` characters from `Start` and `Other_ID_Continue` characters from `Continue`, as backwards compatibility with previous Unicode specifications is not a concern. 

* **UAX31-R4**: Darwin normalizes identifiers using normalization form NFC. 

Two things are worth noting regarding Darwin identifiers. First, unlike some languages, Unicode formatting characters are not allowed in identifiers to reduce the possibility of confusion between identical looking identifiers. Second, Darwin defines no reserved keywords.

The identifier `_` is reserved by the language and can only be used in specific contexts.

```
identifier
  : identifier-start identifier-character*
  ;

identifier-start
  : letter-character
  | '_'
  ;

identifier-character
  : connector-character
  | letter-character
  | decimal-character
  | combining-character
  ;

letter-character
  : '\c{Lu}'
  | '\c{Ll}'
  | '\c{Lt}'
  | '\c{Lm}'
  | '\c{Lo}'
  | '\c{Nl}'
  ;

decimal-character
  : '\c{Nd}'
  ;

combining-character
  : '\c{Mn}'
  | '\c{Mc}'
  ;

connector-character
  : '\c{Pc}'
  ;
```

## <a name="2.4"></a>2.4 Literals

A *literal* is a textual representation of a value. 

```
literal
  : integer-literal
  | floating-point-literal
  | string-literal
  | domain-specific-literal
  ;
```

### <a name="2.4.1"></a>2.4.1 Integer Literals

An *integer literal* is a textual representation of an integral numeric value. Integer literals can either be specified in decimal (base 10) or hexadecimal (base 16) notation. Hexadecimal notation is prefixed by `0x` and uses the letters `a` through `f` to represent the additional digit values.

Decimal notation may be followed by a *unit suffix* that specifies the units of the decimal value. No meaning is assigned to unit suffixes at the lexical level.

```
integer-literal
  : decimal-literal
  | hexadecimal-literal
  ;

decimal-literal
  : digit+ unit-suffix?
  ;

digit
  : '0'..'9'
  ;

unit-suffix
  : unit-suffix-character+
  ;

unit-suffix-character
  : 'a'..'z'
  | 'A'..'Z'
  ;

hexadecimal-literal
  : '0' ('x' | 'X') hexadecimal-digit+
  ;

hexadecimal-digit
  : '0'..'9'
  | 'a'..'f'
  | 'A'..'F'
  ;
```

### <a name="2.4.2"></a>2.4.2 Floating-Point Literals

A *floating-point literal* is a textual representation of a real numeric value. Floating point literals may also have unit suffixes.

```
floating-point-literal
  : digit+ '.' digit+ exponent? unit-suffix?
  | digit+ exponent unit-suffix?
  ;

exponent
  : ('e' | 'E') ('+' | '-')? digit+
  ;
```

### <a name="2.4.3"></a>2.4.3 String Literals

A *string literal* is a textual representation of a string value. String literals are delimited by double quotes (`"`), although double quotes can be represented in the string literal by two double quotes in a row (i.e. the string literal `"""Hello, world!"" she said."` represents the string `"Hello, world!" she said.`). String literals can contain any Unicode character, including line terminators.

String literals support *expression holes*. An expression hole is delimited by a dollar sign followed by curly braces (`${}`) and contains a Darwin expression. The behavior of expression holes is described later in this specification. 

```
string-literal
  : '"' string-literal-element* '"'
  ;

string-literal-element
  : !('"' | "${")
  | '"' '"'
  | "${" token+ '}'
  ;
```

### <a name="2.4.4"></a>2.4.4 Domain-Specific Literals

A *domain-specific literal* is a literal whose language is not part of the Darwin language. For example, an XML literal or a JSON literal could be expressed using a domain-specific literal. Domain specific literals are delimited by backticks (`) and can contain any Unicode character, including line terminators. Inside of a domain-specific literal, a backtick can be represented by two backticks (``). 

Like string literals, domain-specific literals support expression holes. An expression hole is delimited by a dollar sign followed by curly braces (`${}`) and contains a Darwin expression. The behavior of expression holes is described later in this specification.

```
domain-specific-literal
  : '`' domain-specific-literal-element* '`'
  ;

domain-specific-literal-element
  : !('`' | "${")
  | '`' '`'
  | "${" token+ '}'
  ;
```

## <a name="2.5"></a>2.5 Punctuators

A *punctuator* serves to delimit syntatic structures. 

```
punctuator
  : '('
  | ')'
  | '{'
  | '}'
  | '['
  | ']'
  | ','
  | ';'
  | ':'
  ;
```

## <a name="2.6"></a>2.6 Operators

An *operator* specifies an operation in the language. 

```
operator
  : operators+
  ;

operators
  : '~'
  | '!'
  | '@'
  | '$'
  | '%'
  | '^'
  | '&'
  | '*'
  | '-'
  | '+'
  | '='
  | '\'
  | '|'
  | '<'
  | '>'
  | '.'
  | '?'
  | '/'
  ;
```

<br/>
