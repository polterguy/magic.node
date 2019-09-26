
# Magic Node for .Net

[![Build status](https://travis-ci.org/polterguy/magic.node.svg?master)](https://travis-ci.org/polterguy/magic.node)

Magic Node is a simple name/value/children graph object, in addition to a _"Hyperlambda"_ parser, allowing you to
create a textual string representations of graph objects easily transformed to the relational graph node syntax,
and vice versa. This allows you to easily declaratively create syntax trees using a format similar to YAML, for 
then to access every individual node, its value, name and children, from your C# or CLR code.

It is perfect for creating a highly humanly readable relational configuration format, or smaller DSL engines,
especially when combined with [Magic Signals](https://github.com/polterguy/magic.signals). Below is a small
example of Hyperlambda to give you an idea of how it looks like.

```
/*
 * This is a multiline comment, ignored by the parser.
 * Below is a single node, with a value of 'bar' and a name of 'foo'.
 */
foo:bar

// This is a single line comment, below is another node with an integer value.
foo:int:5

/*
 * Node with two children.
 * Notice, the value is optional, and childrne are declared
 * by adding 3 spaces in front of the child's name.
 */
foo
   child1:its-value
   child2:its-value
```

To traverse the nodes later in for instance C#, you could do something such as the following.

```csharp
// Parse some piece of Hyperlambda from a string.
var root = var result = new Parser(hyperlambda).Lambda();

// Retrieving name and value from root node.
var name = root.Name;
var value = root.Value;

// Iterating children nodes of root node.
foreach (var idx in root.Children)
{
   /* ... do stuff with idx here ... */
}
```

This allows you to read Hyperlambda from files, over the network, etc, to dynamically send
relational tree structures around, and serialize these in a human readable format.

## Supported types

Although the node structure itself can hold any value type you need inside of its `Value` property,
Hyperlambda only supports the following types.

* `string` = System.String
* `int` = System.Int32
* `uint` = System.UInt32
* `long` = System.Int64
* `ulong` = System.UInt64
* `decimal` = System.Decimal
* `double` = System.Double
* `single` = System.Single
* `bool` = System.Boolean
* `date` = System.DateTime
* `guid` = System.Guid
* `char` = System.Char
* `byte` = System.Byte
* `x` = magic.node.expressions.Expression
* `node` = magic.node.Node

The type declaration should be declared in your Hyperlambda in between the name and its value, separated by colon (:).
The default type if ommitted is `string`.

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

Escape characters are supported for both single quote strings, and double quote strings, the same way they
are supported in C#, allowing you to use e.g. `\r\n` etc.
