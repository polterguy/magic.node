
# Magic Node

Magic Node is a simple name/value/children graph object, in addition to a _"Hyperlambda"_ parser, allowing you to
create a textual string representations of graph objects easily transformed to its relational graph object syntax
and vice versa. This allows you to easily declaratively create execution trees using a format similar to YAML, for 
then to access every individual node, its value, name and children, from your C#, CLR code, or Hyperlambda.
For the record, Hyperlambda is _much_ easier to understand than YAML.

Hyperlambda is perfect for creating a highly humanly readable relational configuration format, or smaller
DSL engines, especially when combined with Magic Signals and Magic Lambda. Below is a small
example of Hyperlambda to give you an idea of how it looks like.

```
foo:bar
foo:int:5
foo
   child1:its-value
   child2:its-value
```

3 spaces (SP) opens up the children collection of a node, and allows you to create children associated with
some other node. In the example above, the **[child1]** and the **[child2]** nodes, have **[foo]** as their
parent. A colon `:` separates the name and the value of the node - The name is to the left of the colon, and
the value to the right.

You can optionally supply a type between a node's name and its value, which you can see above where we add
the `:int:` parts between one of our **[foo]** nodes' name and value. If you don't explicitly declare a type
then `string` will be assumed.

## Parsing Hyperlambda from C#

To traverse the nodes later in for instance C#, you could do something such as the following.

```csharp
var root = var result = HyperlambdaParser.Parse(hyperlambda);

foreach (var idxChild in root.Children)
{
   var name = idxChild.Name;
   var value = idxChild.Value;
   /* ... etc ... */
}
```

**Notice** - When you parse Hyperlambda a _"root node"_ is explicitly created wrapping all your nodes, and
this is the node that's returned to you after parsing. All nodes you declare in your Hyperlambda will be
returned to you as children of this root node.

## Supported types

Although the node structure itself can hold any value type you need inside of its `Value` property,
Hyperlambda only supports serialising the following types by default.

* `string` = System.String
* `short` = System.Int16
* `ushort` = System.UInt16
* `int` = System.Int32
* `uint` = System.UInt32
* `long` = System.Int64
* `ulong` = System.UInt64
* `decimal` = System.Decimal
* `double` = System.Double
* `single` = System.Float
* `float` = System.Float - Alias for above
* `bool` = System.Boolean
* `date` = System.DateTime - _Always_ interpreted and serialized as UTC time!
* `time` = System.TimeSpan
* `guid` = System.Guid
* `char` = System.Char
* `byte` = System.Byte
* `x` = magic.node.extensions.Expression
* `node` = magic.node.Node

The type declaration should be declared in your Hyperlambda in between the name and its value, separated by colon (:).
The default type if ommitted is `string` and strings do not need quotes or double quotes for this reason. An example
of declaring a couple of types associated with a node's value can be found below.

```
.foo1:int:5
.foo2:bool:true
.foo3:string:foo
.foo4:bar
```

### Extending the type system

The type system is extendible, and you can easily create support for serializing your own types, by using
the `Converter.AddConverter` method, that can be found in the `magic.node.extensions` namespace.
Below is an example of how to extend the typing system, to allow for serializing and de-serializing instances
of a `Foo` class into Hyperlambda.

```csharp
/*
 * Class you want to serialize into Hyperlambda.
 */
class Foo
{
    public int Value1 { get; set; }
    public decimal Value2 { get; set; }
}

/*
 * Adding our converter functions, and associating them
 * with a type, and a Hyperlambda type name.
 */
Converter.AddConverter(
    typeof(Foo),
    "foo",
    (obj) => {
        var foo = obj as Foo;
        return ("foo", $"{foo.Value1}-{foo.Value2}");
    }, (obj) => {
        var str = (obj as string).Split('-');
        return new Foo
        {
            Value1 = int.Parse(str[0]),
            Value2 = decimal.Parse(str[1]),
        };
    });
```

The above will allow you to serialize instances of `Foo` into your Hyperlambda, and
de-serialize these instances once needed. An example of adding a `Foo` instance into
your Hyperlambda can be found below.

```
.foo:foo:5-7
```

Later you can retrieve your `Foo` instances in your slots, using something
resembling the following, and all parsing and conversion will be automatically
taken care of.

```csharp
var foo = node.Get<Foo>();
```

## String literals

Hyperlambda also support strings the same way C# supports string, using any of the following string representations.

```
// Single quotes
foo:'howdy world this is a string'

// Double quotes
foo:"Howdy world, another string"

// Multiline strings
foo:@"Notice how the new line doesn't end the string
    here!"
```

Escape characters are supported for both single quote and double quote strings the same way they
are supported in C#, allowing you to use e.g. `\r\n` etc. If you use a multi line string literal
such as `@"qwerty"` then any CR/LF characters in it will become _"normalised"_ to CRLF. This is
to avoid hard to track down bugs related to differences in operating systems' handling of
CR/LF sequences. However, in single line string literals, CR and LF is preserved exactly as they
are specified.

## Lambda expressions

Lambda expressions are kind of like XPath expressions, except they will references nodes
in your Node graph object instead of XML nodes. Below is an example to give you an idea.

```
.foo:hello world
get-value:x:@.foo

// After invocation of the above **[get-value]**, its value will be "hello world".
```

Most slots in Magic can accept expressions to reference nodes, values of nodes, and children of
nodes somehow. This allows you to modify the lambda graph object, as it is currently being executed,
and hence allows you to modify _"anything"_ from _"anywhere"_. This resembles XPath expressions
from XML.

### Iterators

An expression is constructed from one or more _"iterator"_. This makes an expression
become _"dynamically chained Linq statements"_, where each iterator reacts upon
the results of its previous iterator. Each iterator takes as input an `IEnumerable`,
and returns as its result another `IEnumerable`, where the content of the iterator
somehow changes its given input, according to whatever the particular iterator's
implementation does. This approach just so happens to be perfect for retrieving
sub-sections of graph objects.

Each iterator ends with a _"/"_ or a CR/LF
sequence, and before its end, its value defines what it does. For instance the above iterator in
the __[get-value]__ invocation, starts out with a _"@"_. This implies that the iterator will find the
first node having a name of whatever follows its _"@"_. For the above this implies looking for the first
node who's name is _".foo"_. To see a slightly more advanced example, imagine the following.

```
.data
   item1:john
   item2:thomas
   item3:peter

get-value:x:@.data/*/item2
```

It might help to transform the above expression into humanly readable language. Its
English equivalent hence becomes as follows.

> Find the node with the name of '.data', then retrieve its children, and filter away everything not having a name of 'item2'

Of course, the result of the above becomes _"thomas"_.

Below is a list of all iterators that exists in magic. Substitute _"xxx"_ with a string,
_"n"_ with a number, and _"x"_ with an expression.

* `*` Retrieves all children of its previous result.
* `#` Retrieves the value of its previous result as a node by reference.
* `-` Retrieves its previous result set's _"younger sibling"_ (previous node).
* `+` Retrieves its previous result set's _"elder sibling"_ (next node).
* `.` Retrieves its previous reult set's parent node(s).
* `..` Retrieves the root node.
* `**` Retrieves its previous result set's descendant, with a _"breadth first"_ algorithm.
* `{x}` Extrapolated expression that will be evaluated assuming it yields one result, replacing itself with the value of whatever node it points to.
* `=xxx` Retrieves the node with the _"xxx"_ value, converting to string if necessary.
* `[n,n]` Retrieves a subset of its previous result set, implying _"from, to"_ meaning \[n1,n2\>.
* `@xxx` Returns the first node _"before"_ in its hierarchy that matches the given _"xxx"_ in its name.
* `n` (any number) Returns the n'th child of its previous result set.

Notice, you can escape iterators by using backslash "\\". This allows you to look for nodes who's names
are for instance _"3"_, without using the n'th child iterator, which would defeat the purpose. In addition,
you can quote iterators by using double quotes `"`, to allow for having iterators with values that are normally
not legal within an iterator, such as `/`, etc. If you quote an iterator you have to quote the entire expression.

Below is an example of a slightly more advanced expression.

```
.foo
   howdy:wo/rld
   jo:nothing
   howdy:earth
.dyn:.foo
for-each:x:@"./*/{@.dyn}/*/""=wo/rld"""
   set-value:x:@.dp/#
      :thomas was here
```

After evaluating the above Hyperlambda, the value of all nodes having _"wo/rld"_ as their value
inside of **[.foo]** will be updated to become _"thomas was here"_. Obviously, the above expression
is a ridiculous complex example, that you will probably never encounter in your own code. However,
for reference purposes, let's break it down into its individual parts.

1. Get parent node
2. Get all children
3. Filter away everything not having the name of the value of `{@.dyn}`, which resolves to the equivalent of `:x:@.dyn`, being an expression, who's result becomes _".foo"_.
4. Get its children
5. Find all nodes who's value is _"wo/rld"_.

98% of your expressions will have 1-3 iterators, no complex escaping, and no parameters.
And in fact, there are thousands of lines of Hyperlambda code in Magic's middleware, and
98% of these expressions are as simple as follows.

```
.arguments
   foo1:string
get-value:x:@.arguments/*/foo1
```

Which translates into the following English.

> Give me the value of any **[foo1]** nodes, inside of the first **[.arguments]** node you can find upwards in the hierarchy.

Expressions can also be extrapolated, which allows you to parametrise your expressions, by nesting
expressions, substituting parts of your expression dynamically as your code is executed. Imagine
the following example.

```
.arg1:foo2
.data
   foo1:john
   foo2:thomas
   foo3:peter
get-value:x:@.data/*/{@.arg1}
```

The above expression will first evaluate the `{@.arg1}` parts, which results in _"foo2"_, then evaluate the
outer expression, which now will look like this `@.data/*/foo2`.

### Extending lambda expressions/iterators

You can easily extend expressions in Magic, either with a _"static"_ iterator, implying
a direct match - Or with a dynamic parametrized iterator, allowing you to create iterators that
requires _"parameters"_. To extend the supported iterators, use any of the following two static
methods.

* `Iterator.AddStaticIterator` - Creates a _"static"_ iterator, implying a direct match.
* `Iterator.AddDynamicIterator` - Creates a _"dynamic iterator create function"_.

Below is a C# example, that creates a dynamic iterator, that will only return nodes having a value,
that once converted into a string, has _exactly_ `n` characters, not less and not more.

```csharp
Iterator.AddDynamicIterator('%', (iteratorValue) => {
    var no = int.Parse(iteratorValue.Substring(1));
    return (identity, input) => {
        return input.Where(x => x.Get<string>()?.Length == no);
    };
});

var hl = @"foo
   howdy1:XXXXX
   howdy2:XXX
   howdy3:XXXXX
";
var lambda = HyperlambdaParser.Parse(hl);

var x = new Expression("../**/%3");
var result = x.Evaluate(lambda);
```

Notice how the iterator we created above, uses the `%3` parts of the expression, to parametrize
itself. If you exchange 3 with 6, it will only return **[howdy1]** and **[howdy3]** instead,
since it will look for values with 6 characters instead. The `Iterator` class can be found
in the `magic.node.extensions` namespace.

You can using the above syntax also override the default implementation of iterators, although
I wouldn't recommend it, since it would create confusion for others using your modified version.

**Notice** - To create an extension iterator is an exercise you will rarely if _ever_ need to do,
but is included here for reference purposes.

### Parsing Hyperlambda from C#

Magic allows you to easily parse Hyperlambda from C# if you need it, which can be done as follows.

```csharp
using magic.node.extensions.hyperlambda;

...
var hl = GetHyperlambdaAsString();
var lambda = HyperlambdaParser.Parse(hl);
...
```

The `GetHyperlambdaAsString` above could for instance load Hyperlambda from a file, retrieve it
from your network, or some other way retrieve a snippet of Hyperlambda text. The `HyperlambdaParser.Parse`
parts above will return your Hyperlambda as its `Node` equivalent. The `Parser` class also have an
overloaded constructor for taking a `Stream` instead of a `string`.

**Notice** - The `Node` returned above will be a root node, wrapping all nodes found in your
Hyperlambda as children nodes. This is necessary in order to avoid having a single _"document node"_
the way XML does.

Once you have a `Node` graph object, you can easily reverse the process by using the `HyperlambdaGenerator`
class, and its `GetHyperlambda` method such as the following illustrates.

```csharp
using magic.node.extensions.hyperlambda;

...
var hl1 = GetHyperlambdaAsString();
var result = HyperlambdaParser.Parse(hl1);
var hl2 = HyperlambdaGenerator.GetHyperlambda(result.Children);
...
```

## Formal specification of Hyperlambda

Hyperlambda contains 8 possible tokens in total, however since single line comments and multi line comments are
interchangeable, we simplify the specification by combining these into one logical token type. Tokens possibly
found in Hyperlambda hence becomes as follows.

1. **IND** - Indent token consisting of _exactly_ 3 SP characters.
2. **COM** - Comment token. Either C style (`/**/`) or C++ (`//`) style comments.
3. **NAM** - Name token declaring the name of some node.
4. **SEP** - Separator token separating the name of a node from its type, and/or value.
5. **TYP** - Type token declaring the type of value preceeding it.
6. **VAL** - Value token, being the value of the node.
7. **CRLF** - CRLF character sequence, implying a CR, LF or CRLF. Except for inside string literals, Hyperlambda does not discriminate between and of these 3 possible combinations, and they all become interchangeable CRLF token types after parsing.

Notice, a **VAL** and a **NAM** token can be wrapped inside of quotes (') or double quotes ("), like a C# string type.
In addition to wrapping it inside a multiline C# type of string (@""). This allows you to declare **VAL** and **NAM** tokens
with CR/LF sequences as a part of their actual value.

The formal specification of Hyperlambda is derived from combining the above 7 tokens into the following. Notice, in the
following formal specification `->` means _"must be followed by if existing"_, `(0..n)` implies _"zero to any number of repetitions"_,
`(0..1)` implies _"zero to 1 repetition"_, `(1..1)` implies _"exactly one must exist"_, `(1..n)` implies _"at least one must exist"_,
and `|` implies _"logical or"_. The square brackets `[]` implies a logical grouping of some token type(s), and the `x` parts is
an assignable variable starting at 0, optionally incremented by one for each iteration through the loop. `(0..x)`
implies  _"zero to x repetitions"_ and `(x..x+1)` implies _"x to x+1 number of repetitions"_. The `=` character assigns the
numbers of repetitions in its RHS value to the variable `x`.

0. **Set x to 0**
1. **\[CRLF\](0..n)**
2. **\[\[x=IND(0..x)\]->COM(1..1)->CRLF(1..n)\](0..n)**
3. **\[\[x=IND(0..x)\]->NAM(1..1)->\[\[SEP(1..1)->VAL(0..1)\]|\[SEP(1..1)->TYP(1..1)->SEP(1..1)->VAL(0..1)\]\]\](0..1)->\[CRLF(0..n)\]**
4. **\[x=IND(x..x+1)\]** - But _only_ executed if point 3 had at least a name and ended with at least one CRLF.
5. **GOTO 1**

The above says basically; Any number of CRLF tokens, followed by any optional number of comments, separated by at least one
CRLF sequence, followed by any number of CRLF sequences. Then optionally one name followed
by optionally one separator, followed by optionally one value - Or followed by optionally one separator followed by exactly
one type, and optionally one separator for the to be followed by optionally one value. Then followed by any number of CRLF
sequences, followed by optionally x+1 IND token, assigning number of IND to x, and then repeat back to step 1 again.

As examples realise that all the following Hyperlambda snippets are 100% perfectly legal Hyperlambda.

**Empty string, nothing**

```
```

**Only comments**

```
// Some single line comment.


/*
 * Some multi line comment.
 */
```

**One node without type or value, and one comment**

```
foo


// Some comment.
```

**One node with a value and a child without a value**

```
foo:bar
   child1
```

**Illegal Hyperlambda**

```
foo
      bar:ILLEGAL node
```

**Illegal, indents before name node has been seen**

```
   foo:illegal
```

**Illegal, indents before name node**

```
   // Illegal.
```

**Legal, only indents after name tokens**

```
foo
   bar1:Fine
      bar2:Also fine
```

## Usage

You can include the following NuGet packages into your project to access Magic Node directly.

* `magic.node` - Core node parts.
* `magic.node.extensions` - Contains support for expressions, the Hyperlambda serializer and de-serializer, in addition to the typing system.

However, all of these packages are indirectly included when you use Magic.

## Documenting nodes, arguments to slots, etc

When referencing nodes in the documentation for Magic, it is common to reference them like __[this]__, where
_"this"_ would be the name of some node - Implying in __bold__ characters, wrapped by square [brackets].

## C# extensions

If you want to, you can easily completely exchange the underlaying file system, with your own _"virtual file system"_, since all interaction with the physical file system is done through the `IFileService` and 
`IFolderService` interfaces. This allows you to circumvent the default dependency injected service, and
binding towards some other implementation, at least in theory allowing you to (for instance) use a database
based file system, etc. If you want to do this, you'll need to supply your own bindings to the following
three interfaces, using your IoC container.

* `magic.node.contracts.IFileService`
* `magic.node.contracts.IFolderService`
* `magic.node.contracts.IStreamService`

If you want to do this, you would probably want to manually declare your own implementation for these classes,
by tapping into _"magic.library"_ somehow, or not invoking its default method that binds towards the default
implementation classes somehow.

## Project website

The source code for this repository can be found at [github.com/polterguy/magic.node](https://github.com/polterguy/magic.node), and you can provide feedback, provide bug reports, etc at the same place.

## Quality gates

- ![Build status](https://github.com/polterguy/magic.node/actions/workflows/build.yaml/badge.svg)
- [![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=polterguy_magic.node&metric=alert_status)](https://sonarcloud.io/dashboard?id=polterguy_magic.node)
- [![Bugs](https://sonarcloud.io/api/project_badges/measure?project=polterguy_magic.node&metric=bugs)](https://sonarcloud.io/dashboard?id=polterguy_magic.node)
- [![Code Smells](https://sonarcloud.io/api/project_badges/measure?project=polterguy_magic.node&metric=code_smells)](https://sonarcloud.io/dashboard?id=polterguy_magic.node)
- [![Coverage](https://sonarcloud.io/api/project_badges/measure?project=polterguy_magic.node&metric=coverage)](https://sonarcloud.io/dashboard?id=polterguy_magic.node)
- [![Duplicated Lines (%)](https://sonarcloud.io/api/project_badges/measure?project=polterguy_magic.node&metric=duplicated_lines_density)](https://sonarcloud.io/dashboard?id=polterguy_magic.node)
- [![Lines of Code](https://sonarcloud.io/api/project_badges/measure?project=polterguy_magic.node&metric=ncloc)](https://sonarcloud.io/dashboard?id=polterguy_magic.node)
- [![Maintainability Rating](https://sonarcloud.io/api/project_badges/measure?project=polterguy_magic.node&metric=sqale_rating)](https://sonarcloud.io/dashboard?id=polterguy_magic.node)
- [![Reliability Rating](https://sonarcloud.io/api/project_badges/measure?project=polterguy_magic.node&metric=reliability_rating)](https://sonarcloud.io/dashboard?id=polterguy_magic.node)
- [![Security Rating](https://sonarcloud.io/api/project_badges/measure?project=polterguy_magic.node&metric=security_rating)](https://sonarcloud.io/dashboard?id=polterguy_magic.node)
- [![Technical Debt](https://sonarcloud.io/api/project_badges/measure?project=polterguy_magic.node&metric=sqale_index)](https://sonarcloud.io/dashboard?id=polterguy_magic.node)
- [![Vulnerabilities](https://sonarcloud.io/api/project_badges/measure?project=polterguy_magic.node&metric=vulnerabilities)](https://sonarcloud.io/dashboard?id=polterguy_magic.node)

## License

This project is the copyright(c) 2020-2021 of Aista, Ltd thomas@servergardens.com, and is licensed under the terms
of the LGPL version 3, as published by the Free Software Foundation. See the enclosed LICENSE file for details.

* [Magic Documentation](https://polterguy.github.io/)
