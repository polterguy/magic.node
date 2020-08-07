
# Magic Node for .Net

[![Build status](https://travis-ci.org/polterguy/magic.node.svg?master)](https://travis-ci.org/polterguy/magic.node)

Magic Node is a simple name/value/children graph object, in addition to a _"Hyperlambda"_ parser, allowing you to
create a textual string representations of graph objects easily transformed to its relational graph object syntax,
and vice versa. This allows you to easily declaratively create execution trees using a format similar to YAML, for 
then to access every individual node, its value, name and children, from your C#, CLR code or Hyperlambda code.

It is perfect for creating a highly humanly readable relational configuration format, or smaller DSL engines,
especially when combined with [Magic Signals](https://github.com/polterguy/magic.signals) and
[Magic Lambda](https://github.com/polterguy/magic.lambda). Below is a small
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
Hyperlambda only supports serialising the following types by default.

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
* `time` = System.TimeSpan
* `guid` = System.Guid
* `char` = System.Char
* `byte` = System.Byte
* `x` = magic.node.expressions.Expression
* `node` = magic.node.Node

The type declaration should be declared in your Hyperlambda in between the name and its value, separated by colon (:).
The default type if ommitted is `string`. An example of declaring a couple of types associated with a node's value
can be found below.

```
.integer:int:5
.boolean:bool:true
.explicit-string:string:foo
.inplicit-string:bar
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

Escape characters are supported for both single quote strings, and double quote strings, the same way they
are supported in C#, allowing you to use e.g. `\r\n` etc.

## Lambda expressions

Lambda expressions are kind of like XPath expressions, except (of course), they will references nodes
in your Node graph object, instead of XML nodes. Below is an example to give you an idea.

```
/*
 * Some node with some value.
 */
.foo:hello world

/*
 * Referencing the above node's value.
 */
get-value:x:@.foo

// After invocation of the above slot, its value will be "hello world".
```

Most slots in Magic can accept expressions to reference nodes, values of nodes, and children of
nodes somehow. This allows you to modify the lambda graph object, as it is currently being executed,
and hence allows you to modify _"anything"_ from _"anywhere"_.

An expression is constructed from one or more _"iterators"_. Each iterator ends with a _"/"_, or EOF,
and before its end, its value defines what it does. For instance the above iterator in the __[get-value]__
invocation, starts out with a _"@"_. This implies that the iterator will find the first node having a name
of whatever follows its _"@"_. For the above this means looking for the first node who's name is _".foo"_.
Below is a list of all iterators that exists in magic. Substitute _"xxx"_ with any string, and _"n"_ with
any number.

* `*` Retrieves all children of its previous result.
* `#` Retrieves the value of its previous result as a node by reference.
* `-` Retrieves its previous result set's _"younger sibling"_ (previous node).
* `+` Retrieves its previous result set's _"elder sibling"_ (next node).
* `.` Retrieves its previous reult set's parent node(s).
* `..` Retrieves the root node.
* `**` Retrieves its previous result set's descendant, with a _"breadth first"_ algorithm.
* `{n}` Substitutes itself with the results of its n'th child, possibly evaluating expressions found in its child node, before evaluating the result of the expression. This works similarly to `string.Format` from C#, except it
allows you to dynamically build your expression,by parametrising it with the result of a constant,
or the results of another expression.
* `=xxx` Retrieves the node with the _"xxx"_ value, converting to string if necessary.
* `[n,n]` Retrieves a subset of its previous result set, implying _"from, to"_ meaning \[n1,n2\>.
* `@xxx` Returns the first node _"before"_ in its hierarchy that matches the given _"xxx"_ in its name.
* `n` (any number) Returns the n'th child of its previous result set.

Notice, you can escape iterators by using backslash "\\". This allows you to look for nodes who's names
are for instance _"3"_, without using the n'th child iterator, which would defeat the purpose. Below
is an example of a slightly more advanced expression.

```
.foo
   howdy:world
   jo:nothing
   howdy:earth

/*
 * Loops through all children of [.foo] who's values
 * are "world".
 *
 * Notice!
 * This expression is probably more complex than anything
 * you'd normally need in your own code, but included for
 * reference purposes.
 */
.dyn:.foo
for-each:x:./*/{0}/*/=world
   .:x:@.dyn
   set-value:x:@.dp/#
      :thomas was here
```

After evaluating the above Hyperlambda, the value of all nodes having _"world"_ as their value
inside of **[.foo]** will be _"thomas was here"_.

## Documenting nodes, arguments to slots, etc

When referencing nodes in the documentation for Magic, it is common to reference them like __[this]__, where
_"this"_ would be the name of some node - Implying in __bold__ characters, wrapped by square [brackets].

## License

Although most of Magic's source code is publicly available, Magic is _not_ Open Source or Free Software.
You have to obtain a valid license key to install it in production, and I normally charge a fee for such a
key. You can [obtain a license key here](https://servergardens.com/buy/).
Notice, 5 hours after you put Magic into production, it will stop functioning, unless you have a valid
license for it.

* [Get licensed](https://servergardens.com/buy/)
